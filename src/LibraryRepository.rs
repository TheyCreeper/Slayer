use std::ffi::CString;
use crate::models::album::Album;
use crate::models::artist::Artist;
use crate::models::song::Song;
use crate::models::playlist::Playlist;


pub struct LibraryRepository {
    pub artists: Vec<Artist>,
    pub albums: Vec<Album>,
    pub songs: Vec<Song>,
    pub playlists: Vec<Playlist>,
}

impl LibraryRepository {
    ///
    /// Artist methods
    ///
    // region artists methods

    /// This saves the object
    pub fn set_artist(&mut self, new_artist: Artist) {
        if let Some(artist) = self.get_artist_id(new_artist.id) {
            *artist = new_artist;
        }
    }

    /// Adds an album to an artist
    pub fn add_album_to_artist(&mut self, album_id: u32, artist_id: u32) {
        // get artist
        let artist = self.get_artist_id(artist_id);
        // add album to artist
        artist.unwrap().albums.push(album_id);
    }

    /// Remove an album from an artist
    pub fn rem_album_to_artist(&mut self, album_id: u32, artist_id: u32) {
        // get artist
        let artist = self.get_artist_id(artist_id);
        // retain every album except the one with the id
        artist.unwrap().albums.retain(|album| *album != album_id);
    }

    /// Adds a song to an artist
    pub fn add_song_to_artist(&mut self, song_id: u32, artist_id: u32) {
        // get artist
        let artist = self.get_artist_id(artist_id);
        // add album to artist
        artist.unwrap().songs.push(song_id);
    }

    /// Removes a song from an artist
    pub fn rem_song_to_artist(&mut self, song_id: u32, artist_id: u32) {
        // get artist
        let artist = self.get_artist_id(artist_id);
        // retain every album except the one with the id
        artist.unwrap().songs.retain(|song| *song != song_id);
    }

    /// Add an artist to the list of artists
    pub fn add_artist(&mut self, name: CString, description: CString, image_path: CString) {
        let highest_id = self
            .artists
            .iter()
            .map(|artist| artist.id)
            .max()
            .unwrap_or(0);

        let new_artist = Artist {
            id: highest_id + 1,
            name,
            description,
            image_path,
            albums: Vec::new(),
            songs: Vec::new(),
        };

        self.artists.push(new_artist);
    }

    pub fn remove_artist(&mut self, artist_id: u32) {
        self.artists.retain(|artist| artist.id != artist_id);
    }

    // region> helper methods
    pub fn get_artist_id(&mut self, id: u32) -> Option<&mut Artist> {
        self.artists.iter_mut().find(|artist| artist.id == id)
    }
    // endregion> helper methods

    // endregion artists methods

    ///
    /// Album methods
    ///
    // region album methods
    pub fn set_album(&mut self, new_album: Album) {
        if let Some(album) = self.get_album_id(new_album.id) {
            *album = new_album;
        }
    }

    /// Adds a song to an album
    pub fn add_song_to_album(&mut self, song_id: u32, album_id: u32) {
        // get artist
        let album = self.get_album_id(album_id);
        // add album to artist
        album.unwrap().songs.push(song_id);
    }

    /// Removes a song from an album
    pub fn rem_song_to_album(&mut self, song_id: u32, album_id: u32) {
        // get artist
        let album = self.get_album_id(album_id);
        // retain every album except the one with the id
        album.unwrap().songs.retain(|song| *song != song_id);
    }

    pub fn add_album(
        &mut self,
        name: CString,
        image_path: CString,
        songs: Vec<u32>,
        artist: Vec<u32>,
    ) {
        let highest_id = self.albums.iter().map(|album| album.id).max().unwrap_or(0);

        for id in artist.iter() {
            self.add_album_to_artist(highest_id + 1, *id);
        }

        let album = Album {
            id: highest_id + 1,
            name,
            image_path,
            songs,
            artist,
        };
        self.albums.push(album);
    }

    pub fn remove_album(&mut self, album_id: u32) {
        // remove the album from the artist(s)
        if let Some(artist) = self.artists.iter_mut().find(|artist| artist.albums.contains(&album_id)) {
            artist.albums.retain(|album| *album != album_id);
        }

        self.albums.retain(|album| album.id != album_id);
    }

    // region> helper methods
    pub fn get_album_id(&mut self, id: u32) -> Option<&mut Album> {
        self.albums.iter_mut().find(|album| album.id == id)
    }
    // endregion> helper methods

    // endregion

    ///
    /// Song methods
    ///
    // region song methods
    pub fn set_song(&mut self, new_song: Song) {
        if let Some(song) = self.get_song_id(new_song.id) {
            *song = new_song;
        }
    }

    pub fn add_song(
        &mut self,
        name: CString,
        song_file_path: CString,
        album: u32,
        artist: Vec<u32>,
    ) {
        let highest_id = self.songs.iter().map(|song| song.id).max().unwrap_or(0);

        let song = Song {
            id: highest_id + 1,
            name,
            album,
            song_file_path,
        };

        for id in artist.iter() {
            self.add_song_to_album(song.id, album);
        }
        self.songs.push(song);
    }

    pub fn remove_song(&mut self, song_id: u32) {
        // remove the song from the artist(s)
        self.artists.iter_mut().for_each(|artist| {
            artist.songs.retain(|song| *song != song_id);
        });
        self.albums.iter_mut().for_each(|album| {
            album.songs.retain(|song| *song != song_id);
        });

        self.songs.retain(|song| song.id != song_id);
    }

    // region> helper methods
    pub fn get_song_id(&mut self, id: u32) -> Option<&mut Song> {
        self.songs.iter_mut().find(|song| song.id == id)
    }
    // endregion> helper methods
    // endregion song methods

    ///
    /// Playlist methods
    ///
    // region playlist methods
    pub fn set_playlist(&mut self, new_playlist: Playlist) {
        if let Some(playlist) = self.get_playlist_id(new_playlist.id) {
            *playlist = new_playlist;
        }
    }

    pub fn add_playlist(&mut self, name: CString, image_path: CString, songs: Vec<u32>) {
        let highest_id = self
            .playlists
            .iter()
            .map(|playlist| playlist.id)
            .max()
            .unwrap_or(0);

        let playlist = Playlist {
            id: highest_id + 1,
            name,
            image_path,
            songs,
        };

        self.playlists.push(playlist);
    }

    pub fn add_songs_playlist(&mut self, songs_to_add: Vec<u32>, playlist_id: u32) {
        if let Some(playlist) = self.get_playlist_id(playlist_id) {
            playlist.songs.extend(songs_to_add);
        }
    }

    pub fn remove_playlist(&mut self, playlist_id: u32) {
        self.albums.retain(|album| album.id != playlist_id);
    }

    // region> helper methods
    pub fn get_playlist_id(&mut self, id: u32) -> Option<&mut Playlist> {
        self.playlists.iter_mut().find(|playlist| playlist.id == id)
    }
    // endregion> helper methods
    // endregion song methods
}
