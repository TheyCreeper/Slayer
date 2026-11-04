use std::ffi::CString;
pub struct Album {
    pub id: u32,
    pub name: CString,
    pub image_path: CString,
    pub songs: Vec<u32>,
    pub artist: Vec<u32>,
}