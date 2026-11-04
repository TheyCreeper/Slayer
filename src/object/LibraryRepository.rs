use std::ffi::CString;

pub struct Artist {
    pub id: u32,
    pub name: CString,
    pub description: CString,
    pub image_path: CString,
    pub albums: Vec<u32>,
    pub songs: Vec<u32>
}

pub struct Album {
    pub id: u32,
    pub name: CString,
    pub image_path: CString,
    pub songs: Vec<u32>
}

pub struct Song {
    pub id: u32,
    pub name: CString,
    pub album: u32,
    pub song_file_path: CString,
}

pub struct Playlist {
    pub id: u32,
    pub name: CString,
    pub image_path: CString,
    pub songs: Vec<u32>
}

pub struct LibraryRepository {
    pub artists: Vec<Artist>,
    pub albums: Vec<Album>,
    pub songs: Vec<Song>,
    pub playlists: Vec<Playlist>
}
