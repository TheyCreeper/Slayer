use std::ffi::CString;
pub struct Playlist {
    pub id: u32,
    pub name: CString,
    pub image_path: CString,
    pub songs: Vec<u32>,
}