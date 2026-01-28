using System.Collections.Generic;

public class PlaylistModel
{
    public string Name { get; set; } = string.Empty;
    public int SongCount { get; set; }

    public List<Music> trackList {get;set;}

}