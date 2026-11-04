use serde::{Serialize, de::DeserializeOwned};
use std::error::Error;
use std::fs;

pub struct JsonSerializer;

impl JsonSerializer {
    pub fn save<T: Serialize>(path: &str, data: &T) -> Result<(), Box<dyn Error>> {
        let json = serde_json::to_string_pretty(data)?;
        fs::write(path, json)?;
        Ok(())
    }

    pub fn load<T: DeserializeOwned>(path: &str) -> Result<T, Box<dyn Error>> {
        let json = fs::read_to_string(path)?;
        let data = serde_json::from_str(&json)?;
        Ok(data)
    }
}