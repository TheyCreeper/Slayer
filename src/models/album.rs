use std::ffi::CString;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct Album {
    pub id: u32,
    pub name: CString,
    pub image_path: CString,
    pub songs: Vec<u32>,
    pub artist: Vec<u32>,
}