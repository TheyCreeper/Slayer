using LibVLCSharp.Shared;
using LiteDB;
using SlayerApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlayerApp.Store.DataContext
{
    public class PlaylistDC
    {
        private readonly LiteDatabase database;
        public PlaylistDC(LiteDatabase dataBase) { database = dataBase; }

        /// <summary>
        /// Gets the ILiteCollection collection of Playlists. Simplifies stuff ig
        /// </summary>
        /// <returns></returns>
        private ILiteCollection<Playlist> GetPlaylistLiteCollection() => database.GetCollection<Model.Playlist>("playlists");


        public List<Playlist> GetAllPlaylists() => GetPlaylistLiteCollection().FindAll().ToList();
        public void AddOrUpdatePlaylist(Playlist playlist) => GetPlaylistLiteCollection().Upsert(playlist);
        public void AddPlaylist(Playlist playlist) => GetPlaylistLiteCollection().Insert(playlist);
        public void RemovePlaylist(Playlist playlist) => GetPlaylistLiteCollection().Delete(playlist.Id);
        public void UpdatePlaylist(Playlist playlist) => GetPlaylistLiteCollection().Update(playlist);
    }
}
