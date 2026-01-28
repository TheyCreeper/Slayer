using System;
using Avalonia.Controls.Primitives;

public class Music
{
    public string Name {get;set;} = string.Empty;
    public Artist Artist {get;set;}
    public Album Album {get;set;}
    public TimeSpan TrackLenght {get;set;}
    public Genre TrackGenre {get;set;}

    public string TrackPath {get;set;}

    public Music() {}

    public Music(string name, Artist artist, Album album, TimeSpan trackLenght, Genre trackGenre, string trackPath) 
    {
        Name = name;
        Artist = artist;
        Album = album;
        TrackLenght = trackLenght;
        TrackGenre = trackGenre;
        TrackPath = trackPath;
    }

    // TODO
    /* 
        
    */

    public void PlayTrack()
    {
        throw new NotImplementedException();
    }

    public Music GetCompleteTrack(string path)
    {
        // TODO Get the track info from the file path, create an object and return it.
        return new Music();
    }
}