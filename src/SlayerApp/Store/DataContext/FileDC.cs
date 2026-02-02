using LiteDB;
using System.Collections.Generic;
using System.Linq;

namespace SlayerApp.Store.DataContext
{
    /// <summary>
    /// Wrapper class to store library paths in LiteDB (primitives need a wrapper)
    /// </summary>
    public class LibraryPath
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
        public string Path { get; set; } = string.Empty;

        public LibraryPath() { }
        public LibraryPath(string path) => Path = path;
    }

    public class FileDC
    {
        private readonly LiteDatabase _database;

        public FileDC(LiteDatabase dataBase) => _database = dataBase;

        private ILiteCollection<LibraryPath> GetLocationsCollection() => 
            _database.GetCollection<LibraryPath>("library_paths");

        public List<string> GetAllLibraryPaths() => 
            GetLocationsCollection().FindAll().Select(x => x.Path).ToList();

        public void AddLibraryPath(string path)
        {
            var collection = GetLocationsCollection();
            // Avoid duplicates
            if (!collection.Exists(x => x.Path == path))
            {
                collection.Insert(new LibraryPath(path));
            }
        }

        public void RemoveLibraryPath(string path)
        {
            var collection = GetLocationsCollection();
            collection.DeleteMany(x => x.Path == path);
        }

        public bool HasLibraryPath(string path) => 
            GetLocationsCollection().Exists(x => x.Path == path);
    }
}
