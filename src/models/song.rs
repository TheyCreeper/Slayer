use std::ffi::CString;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct Song {
    pub id: u32,
    pub name: CString,
    pub album: u32,
    pub song_file_path: CString,
}
