using LiteDB;
using SlayerApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlayerApp.Store.DataContext
{
    public class AlbumDC
    {
        private readonly LiteDatabase database;
        public AlbumDC(LiteDatabase dataBase) { database = dataBase; }

        /// <summary>
        /// Gets the ILiteCollection collection of Albums. Simplifies stuff ig
        /// </summary>
        /// <returns></returns>
        private ILiteCollection<Album> GetAlbumLiteCollection() => database.GetCollection<Album>("albums");

        public List<Album> GetAllAlbums() => GetAlbumLiteCollection().FindAll().ToList();
        public void AddOrUpdateAlbum(Album Album) => GetAlbumLiteCollection().Upsert(Album);
        public void AddAlbum(Album Album) => GetAlbumLiteCollection().Insert(Album);
        public void RemoveAlbum(Album Album) => GetAlbumLiteCollection().Delete(Album.Id);
        public void UpdateAlbum(Album Album) => GetAlbumLiteCollection().Update(Album);
    }
}
