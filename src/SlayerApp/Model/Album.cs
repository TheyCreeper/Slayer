using iTunesSearch.Library;
using SlayerApp;
using SlayerApp.ViewModel;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;

public class Album
{
    public string Name {get;set;} = string.Empty;
    public string ReleaseDate {get;set;}
    public string AlbumArt {get;set;}
    public string Artist { get; set; }

    public Album() { }

    public Album(string name,string artist, string songPath = "")
    {
        Name = name;
        Artist = artist;
        GetAlbumInfo(songPath);
    }

    public async void GetAlbumInfo(string songPath)
    {
        if (!string.IsNullOrEmpty(songPath))
        {
            var file = TagLib.File.Create(songPath);
            var picture = file.Tag.Pictures.FirstOrDefault();
            if (picture != null)
            {
                var artFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlbumArt");
                Directory.CreateDirectory(artFolder);
                var safeFileName = string.Join("_", $"{Artist}_{Name}".Split(Path.GetInvalidFileNameChars()));
                var artPath = Path.Combine(artFolder, $"{safeFileName}.jpg");

                await System.IO.File.WriteAllBytesAsync(artPath, picture.Data.Data);
                AlbumArt = artPath;
                return;
            }
        }
        // if getting it from the file fails, get it from iTunes. If that fails, default to nothing.
        try
        {
            var query = await App.s_SearchManager.GetAlbumsAsync(Artist + "," + Name).ConfigureAwait(false);
            AlbumArt = query.Albums[0].ArtworkUrl100.Replace("100x100bb", "600x600bb");
            ReleaseDate = query.Albums[0].ReleaseDate;
        } catch
        {
            AlbumArt = null;
        }
    }
}