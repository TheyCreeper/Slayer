use std::ffi::CString;
use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct Artist {
    pub id: u32,
    pub name: CString,
    pub description: CString,
    pub image_path: CString,
    pub albums: Vec<u32>,
    pub songs: Vec<u32>,
}