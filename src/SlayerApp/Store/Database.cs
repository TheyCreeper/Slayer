using LiteDB;
using SlayerApp.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using SlayerApp.Store.DataContext;

namespace SlayerApp.Store
{
    public class Database
    {
        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SlayerApp");

        private static LiteDatabase db = new LiteDatabase(Path.Combine(AppDataPath, "slayer.db"));

        public AlbumDC album = new(db);
        public PlaylistDC playlist = new(db);
        public SongDC song = new(db);
        public FileDC files = new(db);
    }
}
