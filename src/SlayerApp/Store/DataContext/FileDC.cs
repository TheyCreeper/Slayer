using LiteDB;
using SlayerApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlayerApp.Store.DataContext
{
    public class FileDC
    {
        private readonly LiteDatabase database;
        public FileDC(LiteDatabase dataBase) { database = dataBase; }

        /// <summary>
        /// Gets the ILiteCollection collection of Albums. Simplifies stuff ig
        /// </summary>
        /// <returns></returns>
        private ILiteCollection<string> GetLocationsCollection() => database.GetCollection<string>("locations");

        public List<string> GetAllPlaylists() => GetLocationsCollection().FindAll().ToList();
        public void AddLocation(string location) => GetLocationsCollection().Insert(location);
    }
}
