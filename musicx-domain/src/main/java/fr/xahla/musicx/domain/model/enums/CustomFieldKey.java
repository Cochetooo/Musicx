package fr.xahla.musicx.domain.model.enums;

import lombok.Getter;

/**
 * List of custom tags used by the application.
 * @author Cochetooo
 */
@Getter
public enum CustomFieldKey {

    ALBUM_PRIMARY_GENRES("Track Primary Genres"),
    ALBUM_SECONDARY_GENRES("Album Secondary Genres"),
    ALBUM_TYPE("Album Type"),
    ARTIST_ARTWORK_URL("Artist Artwork Url"),
    ARTWORK_URL("Album Artwork Url"),
    NOTE_KEY("Note Key"),
    SONG_SECONDARY_GENRES("Song Secondary Genres");

    private final String key;

    CustomFieldKey(String key) {
        this.key = key;
    }

}
