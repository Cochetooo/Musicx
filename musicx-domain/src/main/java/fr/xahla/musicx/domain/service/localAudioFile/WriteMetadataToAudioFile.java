package fr.xahla.musicx.domain.service.localAudioFile;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.GenreType;
import fr.xahla.musicx.domain.helper.AudioTaggerHelper;
import fr.xahla.musicx.domain.model.enums.CustomFieldKey;
import org.jaudiotagger.audio.AudioFile;
import org.jaudiotagger.audio.AudioFileIO;
import org.jaudiotagger.tag.FieldDataInvalidException;
import org.jaudiotagger.tag.FieldKey;
import org.jaudiotagger.tag.Tag;

import java.io.File;
import java.util.List;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

public class WriteMetadataToAudioFile {

    private Tag tag;
    private String filepath;

    public void execute(final SongDto song) {
        try {
            this.filepath = song.getFilepath();

            final var audioFile = AudioFileIO.read(new File(filepath));
            this.tag = audioFile.getTag();

            writeSong(song);

            final var songAlbum = songRepository().getAlbum(song);
            final var songArtist = songRepository().getArtist(song);
            final var songPrimaryGenres = songRepository().getPrimaryGenres(song);
            final var songSecondaryGenres = songRepository().getSecondaryGenres(song);

            if (null != songAlbum) {
                writeAlbum(songAlbum);
            }

            if (null != songArtist) {
                writeArtist(songArtist);
            }

            if (null != songPrimaryGenres && !songPrimaryGenres.isEmpty()) {
                writeGenres(songPrimaryGenres, 's', GenreType.PRIMARY);
            }

            if (null != songSecondaryGenres && !songSecondaryGenres.isEmpty()) {
                writeGenres(songSecondaryGenres, 's', GenreType.SECONDARY);
            }

            AudioFileIO.write(audioFile);
            logger().fine("File written with success: " + filepath);
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "An error has occurred while trying to write audio file: " + song.getFilepath(), exception);
        }
    }

    public void execute(final AlbumDto album) {
        try {
            final var songs = albumRepository().getSongs(album);

            for (final var song : songs) {
                this.filepath = song.getFilepath();
                final var audioFile = AudioFileIO.read(new File(filepath));
                this.tag = audioFile.getTag();

                writeAlbum(album);

                AudioFileIO.write(audioFile);
            }

            logger().fine("Files written with success for albums: " + album.getName());
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "An error has occured while trying to write audio files for album: " + album.getName(), exception);
        }
    }

    private void writeSong(final SongDto song) {
        try {
            tag.setField(FieldKey.DISC_NO, String.valueOf(song.getDiscNumber()));
            tag.setField(FieldKey.LYRICS, song.getRawLyrics());
            tag.setField(FieldKey.TITLE, song.getTitle());
            tag.setField(FieldKey.TRACK, String.valueOf(song.getTrackNumber()));

            logger().finer("Successfully written song fields for " + filepath);
        } catch (FieldDataInvalidException exception) {
            logger().log(Level.SEVERE, "Could not set song fields for " + filepath, exception);
        }
    }

    private void writeAlbum(final AlbumDto album) {
        try {
            tag.setField(FieldKey.ALBUM, album.getName());
            tag.setField(FieldKey.CATALOG_NO, album.getCatalogNo());
            tag.setField(FieldKey.DISC_TOTAL, String.valueOf(album.getDiscTotal()));
            tag.setField(FieldKey.TRACK_TOTAL, String.valueOf(album.getTrackTotal()));
            tag.setField(FieldKey.YEAR, album.getReleaseDate().toString());

            AudioTaggerHelper.writeCustomTag(
                tag,
                CustomFieldKey.ARTWORK_URL.getKey(),
                album.getArtworkUrl()
            );

            final var albumArtist = albumRepository().getArtist(album);
            final var albumLabel = albumRepository().getLabel(album);
            final var albumPrimaryGenres = albumRepository().getPrimaryGenres(album);
            final var albumSecondaryGenres = albumRepository().getSecondaryGenres(album);

            if (null != albumArtist) {
                tag.setField(FieldKey.ALBUM_ARTIST, albumArtist.getName());
            }

            if (null != albumLabel) {
                writeLabel(albumLabel);
            }

            if (null != albumPrimaryGenres && !albumPrimaryGenres.isEmpty()) {
                writeGenres(albumPrimaryGenres, 'a', GenreType.PRIMARY);
            }

            if (null != albumSecondaryGenres && !albumSecondaryGenres.isEmpty()) {
                writeGenres(albumSecondaryGenres, 'a', GenreType.SECONDARY);
            }

            if (null != album.getType()) {
                AudioTaggerHelper.writeCustomTag(
                    tag,
                    CustomFieldKey.ALBUM_TYPE.getKey(),
                    album.getType().name()
                );
            }

            logger().finer("Successfully written album fields for " + filepath);
        } catch (FieldDataInvalidException exception) {
            logger().log(Level.SEVERE, "Could not set album fields for " + filepath, exception);
        }
    }

    private void writeArtist(final ArtistDto artist) {
        try {
            tag.setField(FieldKey.ARTIST, artist.getName());
            tag.setField(FieldKey.COUNTRY, artist.getCountry().getLanguage());

            AudioTaggerHelper.writeCustomTag(
                tag,
                CustomFieldKey.ARTIST_ARTWORK_URL.getKey(),
                artist.getArtworkUrl()
            );

            logger().finer("Successfully written artist fields for " + filepath);
        } catch (FieldDataInvalidException exception) {
            logger().log(Level.SEVERE, "Could not set artist fields for " + filepath, exception);
        }
    }

    private void writeLabel(final LabelDto label) {
        try {
            tag.setField(FieldKey.RECORD_LABEL, label.getName());

            logger().finer("Successfully written label fields for " + filepath);
        } catch (FieldDataInvalidException exception) {
            logger().log(Level.SEVERE, "Could not set label fields for " + filepath, exception);
        }
    }

    private void writeGenres(final List<GenreDto> genre, final char entity, final GenreType type) {
        try {
            final var genreString = String.join("\0", genre.stream().map(GenreDto::getName).toList());

            switch (entity) {
                // Album
                case 'a' -> {
                    if (GenreType.PRIMARY == type) {
                        AudioTaggerHelper.writeCustomTag(
                            tag,
                            CustomFieldKey.ALBUM_PRIMARY_GENRES.getKey(),
                            genreString
                        );
                    } else if (GenreType.SECONDARY == type) {
                        AudioTaggerHelper.writeCustomTag(
                            tag,
                            CustomFieldKey.ALBUM_SECONDARY_GENRES.getKey(),
                            genreString
                        );
                    }
                }

                // Song
                case 's' -> {
                    if (GenreType.PRIMARY == type) {
                        tag.setField(FieldKey.GENRE, genreString);
                    } else if (GenreType.SECONDARY == type) {
                        AudioTaggerHelper.writeCustomTag(
                            tag,
                            CustomFieldKey.SONG_SECONDARY_GENRES.getKey(),
                            genreString
                        );
                    }
                }
            }

            logger().finer("Successfully written genre fields for " + filepath);
        } catch (final FieldDataInvalidException exception) {
            logger().log(Level.SEVERE, "Could not set " + entity + " "
                + type.name() + " genre fields for " + filepath, exception);
        }
    }

}
