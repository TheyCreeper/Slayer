using LiteDB;
using SlayerApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlayerApp.Store.DataContext
{
    public class SongDC
    {
        private readonly LiteDatabase database;
        public SongDC(LiteDatabase dataBase) { database = dataBase; }

        /// <summary>
        /// Gets the ILiteCollection collection of songs. Simplifies stuff ig
        /// </summary>
        /// <returns></returns>
        private ILiteCollection<Song> GetSongLiteCollection() => database.GetCollection<Model.Song>("songs");


        public List<Song> GetAllSongs() => GetSongLiteCollection().FindAll().ToList();
        public void AddOrUpdateSong(Song song) => GetSongLiteCollection().Upsert(song);
        public void AddSong(Song song) => GetSongLiteCollection().Insert(song);
        public void RemoveSong(Song song) => GetSongLiteCollection().Delete(song.Id);
        public void UpdateSong(Song song) => GetSongLiteCollection().Update(song);
    }
}
