use std::ffi::CString;
pub struct Song {
    pub id: u32,
    pub name: CString,
    pub album: u32,
    pub song_file_path: CString,
}
