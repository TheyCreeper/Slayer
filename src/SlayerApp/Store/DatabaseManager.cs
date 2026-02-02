using SlayerApp.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        private readonly Database _db;

        private static readonly string[] AudioExtensions = [".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a"];
        private static readonly HashSet<string> AudioExtensionSet = new(AudioExtensions, StringComparer.OrdinalIgnoreCase);

        public DatabaseManager(Database db)
        {
            _db = db;
        }

        /// <summary>
        /// Synchronizes the database with the file system.
        /// </summary>
        public SyncResult SyncLibrary()
        {
            var result = new SyncResult();
            var libraryPaths = _db.files.GetAllPlaylists();

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

            // Process files in parallel
            Parallel.ForEach(discoveredFiles, filePath =>
            {
                var checksum = ComputeFileChecksum(filePath);
                discoveredChecksums[checksum] = filePath;

                if (existingSongsByChecksum.TryGetValue(checksum, out var existingSong))
                {
                    // Song exists - check if it moved
                    if (existingSong.Path != filePath)
                    {
                        existingSong.Path = filePath;
                        updatedSongs.Add(existingSong);
                        result.MovedCount++;
                    }
                }
                else
                {
                    // New song
                    var song = Song.FromFile(filePath);
                    song.Checksum = checksum;
                    newSongs.Add(song);
                    result.AddedCount++;

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
            });

            // Find removed songs (in DB but file no longer exists)
            var removedSongs = existingSongsByChecksum.Values
                .Where(s => !discoveredChecksums.ContainsKey(s.Checksum))
                .ToList();
            result.RemovedCount = removedSongs.Count;

            // Persist changes to database
            foreach (var song in newSongs)
                _db.song.AddSong(song);

            foreach (var song in updatedSongs)
                _db.song.UpdateSong(song);

            foreach (var song in removedSongs)
                _db.song.RemoveSong(song);

            foreach (var album in newAlbums)
                _db.album.AddAlbum(album);

            return result;
        }

        /// <summary>
        /// Discovers all audio files in the given library paths.
        /// </summary>
        private List<string> DiscoverAudioFiles(List<string> libraryPaths)
        {
            return libraryPaths
                .SelectMany(path =>
                {
                    if (Directory.Exists(path))
                        return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories);
                    else if (File.Exists(path))
                        return [path];
                    return [];
                })
                .Where(IsAudioFile)
                .ToList();
        }

        private string ComputeFileChecksum(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1024 * 1024);
            var hash = md5.ComputeHash(stream);
            return Convert.ToHexString(hash);
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
        public int AddedCount { get; set; }
        public int RemovedCount { get; set; }
        public int MovedCount { get; set; }

        public override string ToString() =>
            $"Sync complete: {AddedCount} added, {RemovedCount} removed, {MovedCount} moved";
    }
}