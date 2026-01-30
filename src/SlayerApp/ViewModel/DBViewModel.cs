using SlayerApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;

namespace SlayerApp.ViewModel
{
    /// <summary>
    /// Handles common data across the app. Helps serialization and deserialization. 
    /// 
    /// It's called a viewmodel but it's much more of a """database""" to access the data
    /// across the app
    /// 
    /// This way the app stores only the paths to the user's media library, 
    /// and loads it all up when starting the app
    /// </summary>
    public class DBViewModel
    {
        private static readonly string[] AudioExtensions = [".mp3", ".wav", ".flac", ".aac", ".ogg", ".m4a"];
        private static readonly HashSet<string> AudioExtensionSet = new(AudioExtensions, StringComparer.OrdinalIgnoreCase);

        private List<Album> AlbumList = new List<Album>();
        private List<Song> SongList = new List<Song>();
        private List<Playlist> Playlists = new List<Playlist>();
        private List<Genre> GenreList = new List<Genre>();
        private List<string> LibraryPaths = new();

        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SlayerApp");

        private static readonly string LibraryPathsFile = Path.Combine(AppDataPath, "library_paths.json");
        private static readonly string SongsFile = Path.Combine(AppDataPath, "songs.json");
        private static readonly string AlbumsFile = Path.Combine(AppDataPath, "albums.json");
        private static readonly string PlaylistsFile = Path.Combine(AppDataPath, "playlists.json");
        private static readonly string GenresFile = Path.Combine(AppDataPath, "genres.json");

        private static readonly JsonSerializerOptions s_jsonOptions = new()
        {
            WriteIndented = true
        };

        public DBViewModel()
        {
            LoadLibraryPaths();
            LoadCachedData();
            LoadLibraryData();
        }

        public DBViewModel(string[] libraryPaths)
        {
            LibraryPaths = new List<string>(libraryPaths);
        }

        /// <summary>
        /// Load and reload library data
        /// </summary>
        public void LoadLibraryData()
        {
            var existingSongsByChecksum = SongList.ToDictionary(s => s.Checksum, s => s);
            var existingAlbumNames = new HashSet<string>(AlbumList.Select(a => a.Name));
            var newSongs = new System.Collections.Concurrent.ConcurrentBag<Song>();
            var newAlbums = new System.Collections.Concurrent.ConcurrentBag<Album>();

            var allFiles = LibraryPaths
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

            Parallel.ForEach(allFiles, file =>
            {
                var checksum = ComputeFileChecksum(file);

                lock (existingSongsByChecksum)
                {
                    if (existingSongsByChecksum.TryGetValue(checksum, out var existingSong))
                    {
                        existingSong.Path = file;
                    }
                    else
                    {
                        var song = Song.FromFile(file);
                        song.Checksum = checksum;

                        if (!existingAlbumNames.Contains(song.Album))
                        {
                            var album = new Album(song.Album, song.Artists.Length > 0 ? song.Artists[0] : string.Empty, song.Path);
                            newAlbums.Add(album);
                            existingAlbumNames.Add(song.Album);
                        }

                        newSongs.Add(song);
                        existingSongsByChecksum[checksum] = song;
                    }
                }
            });

            SongList.AddRange(newSongs);
            AlbumList.AddRange(newAlbums);

            foreach (var playlist in Playlists)
            {
                playlist.SongCount = playlist.trackList.Count;
            }

            // Save updated data to cache
            SaveCachedData();
        }

        private void ProcessAudioFile(string filePath, Dictionary<string, Song> existingSongsByChecksum)
        {
            var checksum = ComputeFileChecksum(filePath);

            if (existingSongsByChecksum.TryGetValue(checksum, out var existingSong))
            {
                // song already exists but is in a different dir
                existingSong.Path = filePath;
            }
            else
            {
                // new song
                var song = Song.FromFile(filePath);
                song.Checksum = checksum;
                SongList.Add(song);
                existingSongsByChecksum[checksum] = song;
            }
        }

        private string ComputeFileChecksum(string filePath)
        {
            using var md5 = MD5.Create();
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 1024 * 1024);
            var hash = md5.ComputeHash(stream);
            return Convert.ToHexString(hash);
        }

        public List<Album> GetAlbums() { return AlbumList; }
        public List<Song> GetSongs() { return SongList; }
        public List<Playlist> GetPlaylists() { return Playlists; }
        public List<Genre> GetGenres() { return GenreList; }
        public List<string> GetLibraryPaths() { return LibraryPaths; }
        public void AddLibraryPath(string pathToAdd)
        {
            LibraryPaths.Add(pathToAdd);
            SaveLibraryPaths();
        }

        public void SaveLibraryPaths()
        {
            Directory.CreateDirectory(AppDataPath);
            var json = JsonSerializer.Serialize(LibraryPaths, s_jsonOptions);
            File.WriteAllText(LibraryPathsFile, json);
        }

        public void LoadLibraryPaths()
        {
            if (File.Exists(LibraryPathsFile))
            {
                FileInfo file = new FileInfo(LibraryPathsFile);
                string fullPath = file.FullName;
                var json = File.ReadAllText(LibraryPathsFile);
                var paths = JsonSerializer.Deserialize<List<string>>(json);
                if (paths != null)
                {
                    LibraryPaths = paths;
                }
            }
        }

        /// <summary>
        /// Load cached data from JSON files
        /// </summary>
        private void LoadCachedData()
        {
            try
            {
                if (File.Exists(SongsFile))
                {
                    var json = File.ReadAllText(SongsFile);
                    var songs = JsonSerializer.Deserialize<List<Song>>(json);
                    if (songs != null)
                        SongList = songs;
                }

                if (File.Exists(AlbumsFile))
                {
                    var json = File.ReadAllText(AlbumsFile);
                    var albums = JsonSerializer.Deserialize<List<Album>>(json);
                    if (albums != null)
                        AlbumList = albums;
                }

                if (File.Exists(PlaylistsFile))
                {
                    var json = File.ReadAllText(PlaylistsFile);
                    var playlists = JsonSerializer.Deserialize<List<Playlist>>(json);
                    if (playlists != null)
                        Playlists = playlists;
                }

                if (File.Exists(GenresFile))
                {
                    var json = File.ReadAllText(GenresFile);
                    var genres = JsonSerializer.Deserialize<List<Genre>>(json);
                    if (genres != null)
                        GenreList = genres;
                }
            }
            catch
            {
                // If deserialization fails, start fresh
                SongList = new List<Song>();
                AlbumList = new List<Album>();
                Playlists = new List<Playlist>();
                GenreList = new List<Genre>();
            }
        }

        /// <summary>
        /// Save all data to JSON files
        /// </summary>
        public void SaveCachedData()
        {
            try
            {
                Directory.CreateDirectory(AppDataPath);

                File.WriteAllText(SongsFile, JsonSerializer.Serialize(SongList, s_jsonOptions));
                File.WriteAllText(AlbumsFile, JsonSerializer.Serialize(AlbumList, s_jsonOptions));
                File.WriteAllText(PlaylistsFile, JsonSerializer.Serialize(Playlists, s_jsonOptions));
                File.WriteAllText(GenresFile, JsonSerializer.Serialize(GenreList, s_jsonOptions));
            }
            catch
            {
                // Silently fail if we can't save
            }
        }

        private bool IsAudioFile(string filePath)
        {
            var extension = Path.GetExtension(filePath);
            return AudioExtensionSet.Contains(extension);
        }

        public void AddData(Song song)
        {
            var existingIndex = SongList.FindIndex(s => s.Checksum == song.Checksum);
            if (existingIndex >= 0)
                SongList[existingIndex] = song;
            else
                SongList.Add(song);
            SaveCachedData();
        }

        public void AddData(Album album)
        {
            var existingIndex = AlbumList.FindIndex(a => a.Name == album.Name);
            if (existingIndex >= 0)
                AlbumList[existingIndex] = album;
            else
                AlbumList.Add(album);
        }

        public void AddData(Playlist playlist)
        {
            var existingIndex = Playlists.FindIndex(p => p.Name == playlist.Name);
            if (existingIndex >= 0)
                Playlists[existingIndex] = playlist;
            else
                Playlists.Add(playlist);
            
            SaveCachedData();
        }

        public void AddData(Genre genre)
        {
            var existingIndex = GenreList.FindIndex(g => g.Name == genre.Name);
            if (existingIndex >= 0)
                GenreList[existingIndex] = genre;
            else
                GenreList.Add(genre);
            SaveCachedData();
        }

        public void AddData(string libraryPath)
        {
            if (!LibraryPaths.Contains(libraryPath, StringComparer.OrdinalIgnoreCase))
            {
                LibraryPaths.Add(libraryPath);
                SaveLibraryPaths();
            }
            SaveCachedData();
        }
    }
}