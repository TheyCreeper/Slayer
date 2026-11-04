use crate::controllers::library_controller::LibraryController;
use crate::models::artist::Artist;

pub struct ArtistController {
    library: LibraryController,
}

impl ArtistController {
    pub fn new(library: LibraryController) -> Self {
        Self { library }
    }

    pub fn get_artists(&self) -> &Vec<Artist> {
        self.library.get_artists()
    }

    pub fn add_artist(&mut self, artist: Artist) {
        self.library.add_artist(artist.name, artist.description, artist.image_path);
    }

    pub fn delete_artist(&mut self, artist_id: u32) {
        self.library.remove_artist(artist_id);
    }
}