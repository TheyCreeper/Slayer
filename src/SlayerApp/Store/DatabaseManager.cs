using SlayerApp.Model;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SlayerApp.Store
{
    /// <summary>
    /// Is used primarly to sync the database with the data
    /// so like if you add stuff in the folder, you'd hit a button in the settings and 
    /// it would run this big thing
    /// </summary>
    public class DatabaseManager
    {
        private Database _db { get => App.Database; }

        private static readonly string[] AudioExtensions = [".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a"];
        private static readonly HashSet<string> AudioExtensionSet = new(AudioExtensions, StringComparer.OrdinalIgnoreCase);
        
        // Chunk size for partial file hashing (64KB from start and end)
        private const int HashChunkSize = 64 * 1024;

        public DatabaseManager() { }

        /// <summary>
        /// Adds a new library path and triggers a sync.
        /// </summary>
        public SyncResult AddLibraryPathAndSync(string path)
        {
            if (!Directory.Exists(path) && !File.Exists(path))
                throw new DirectoryNotFoundException($"Path not found: {path}");

            _db.files.AddLibraryPath(path);
            return SyncLibrary();
        }

        /// <summary>
        /// Removes a library path (does not remove songs from DB).
        /// </summary>
        public void RemoveLibraryPath(string path)
        {
            _db.files.RemoveLibraryPath(path);
        }

        /// <summary>
        /// Gets all configured library paths.
        /// </summary>
        public List<string> GetLibraryPaths() => _db.files.GetAllLibraryPaths();

        /// <summary>
        /// Synchronizes the database with the file system.
        /// Uses partial file hashing and bulk operations for performance.
        /// </summary>
        public SyncResult SyncLibrary()
        {
            var result = new SyncResult();
            var libraryPaths = _db.files.GetAllLibraryPaths();

            if (libraryPaths.Count == 0)
            {
                return result; // Nothing to sync
            }

            // Get all existing songs from the db by checksum (unique to files and not to the db)
            var existingSongsByChecksum = _db.song.GetAllSongs()
                .ToDictionary(s => s.Checksum, s => s);

            // Get existing album names, avoids duplication
            var existingAlbumNames = _db.album.GetAllAlbums()
                .Select(a => a.Name)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Discover all audio files in library paths
            var discoveredFiles = DiscoverAudioFiles(libraryPaths);
            var discoveredChecksums = new ConcurrentDictionary<string, string>();

            var newSongs = new ConcurrentBag<Song>();
            var newAlbums = new ConcurrentBag<Album>();
            var updatedSongs = new ConcurrentBag<Song>();

            // Configure parallelism based on CPU cores
            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount - 1)
            };

            // Process files in parallel with optimized settings
            Parallel.ForEach(discoveredFiles, parallelOptions, filePath =>
            {
                try
                {
                    var checksum = ComputeFastChecksum(filePath);
                    discoveredChecksums[checksum] = filePath;

                    if (existingSongsByChecksum.TryGetValue(checksum, out var existingSong))
                    {
                        // Song exists - check if it moved
                        if (existingSong.Path != filePath)
                        {
                            existingSong.Path = filePath;
                            updatedSongs.Add(existingSong);
                            Interlocked.Increment(ref result._movedCount);
                        }
                    }
                    else
                    {
                        // New song
                        var song = Song.FromFile(filePath);
                        song.Checksum = checksum;
                        newSongs.Add(song);
                        Interlocked.Increment(ref result._addedCount);

                        // Check if we need a new album
                        lock (existingAlbumNames)
                        {
                            if (!string.IsNullOrEmpty(song.Album) && !existingAlbumNames.Contains(song.Album))
                            {
                                var album = new Album(
                                    song.Album,
                                    song.Artists.Length > 0 ? song.Artists[0] : string.Empty,
                                    song.Path);
                                newAlbums.Add(album);
                                existingAlbumNames.Add(song.Album);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // Skip files that can't be read (corrupted, locked, etc.)
                }
            });

            // Find removed songs (in DB but file no longer exists)
            var removedSongs = existingSongsByChecksum.Values
                .Where(s => !discoveredChecksums.ContainsKey(s.Checksum))
                .ToList();
            result.RemovedCount = removedSongs.Count;

            // Bulk persist changes to database (much faster than individual operations)
            if (newSongs.Count > 0)
                _db.song.AddSongs(newSongs);

            if (updatedSongs.Count > 0)
                _db.song.UpdateSongs(updatedSongs);

            if (removedSongs.Count > 0)
                _db.song.RemoveSongs(removedSongs);

            foreach (var album in newAlbums)
                _db.album.AddAlbum(album);

            return result;
        }

        /// <summary>
        /// Discovers all audio files in the given library paths.
        /// Uses EnumerationOptions for better performance.
        /// </summary>
        private List<string> DiscoverAudioFiles(List<string> libraryPaths)
        {
            var options = new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            };

            return libraryPaths
                .SelectMany(path =>
                {
                    if (Directory.Exists(path))
                        return Directory.EnumerateFiles(path, "*.*", options);
                    else if (File.Exists(path))
                        return [path];
                    return [];
                })
                .Where(IsAudioFile)
                .ToList();
        }

        /// <summary>
        /// Computes a fast checksum using partial file hashing.
        /// Hashes: file size + first 64KB + last 64KB
        /// This is ~10-100x faster than full file hashing while still being unique enough for audio files.
        /// </summary>
        private string ComputeFastChecksum(string filePath)
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: HashChunkSize);
            var fileLength = stream.Length;

            // For small files, just hash the whole thing
            if (fileLength <= HashChunkSize * 2)
            {
                using var md5 = MD5.Create();
                var hash = md5.ComputeHash(stream);
                return $"{fileLength:X}-{Convert.ToHexString(hash)}";
            }

            // For larger files, hash first chunk + last chunk + file size
            var buffer = ArrayPool<byte>.Shared.Rent(HashChunkSize * 2);
            try
            {
                // Read first chunk
                var firstBytesRead = stream.Read(buffer, 0, HashChunkSize);

                // Seek to last chunk
                stream.Seek(-HashChunkSize, SeekOrigin.End);
                var lastBytesRead = stream.Read(buffer, HashChunkSize, HashChunkSize);

                using var md5 = MD5.Create();
                var hash = md5.ComputeHash(buffer, 0, firstBytesRead + lastBytesRead);
                return $"{fileLength:X}-{Convert.ToHexString(hash)}";
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private bool IsAudioFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return AudioExtensionSet.Contains(extension);
        }
    }

    /// <summary>
    /// Result of a library sync operation.
    /// </summary>
    public class SyncResult
    {
        internal int _addedCount;
        internal int _movedCount;

        public int AddedCount { get => _addedCount; set => _addedCount = value; }
        public int RemovedCount { get; set; }
        public int MovedCount { get => _movedCount; set => _movedCount = value; }

        public override string ToString() =>
            $"Sync complete: {AddedCount} added, {RemovedCount} removed, {MovedCount} moved";
    }
}