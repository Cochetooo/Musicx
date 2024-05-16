package fr.xahla.musicx.domain.service.localAudioFile;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.AudioFormat;
import fr.xahla.musicx.api.model.enums.ReleaseType;
import fr.xahla.musicx.api.repository.searchCriterias.*;
import fr.xahla.musicx.domain.helper.AudioTaggerHelper;
import fr.xahla.musicx.domain.logging.LogMessage;
import fr.xahla.musicx.domain.model.enums.CustomFieldKey;
import org.jaudiotagger.audio.AudioFile;
import org.jaudiotagger.audio.AudioHeader;
import org.jaudiotagger.tag.FieldKey;
import org.jaudiotagger.tag.Tag;
import org.jaudiotagger.tag.TagField;
import org.jaudiotagger.tag.id3.AbstractID3v2Frame;
import org.jaudiotagger.tag.mp4.field.Mp4TagReverseDnsField;

import java.time.LocalDate;
import java.time.Month;
import java.time.format.DateTimeParseException;
import java.util.*;

import static fr.xahla.musicx.domain.application.AbstractContext.*;
import static fr.xahla.musicx.domain.helper.StringHelper.str_parse_short_safe;

/**
 * Write information about songs, albums, artists, labels and genres into audio files.
 * @author Cochetooo
 * @since 0.3.1
 */
public final class PersistAudioFileMetadata {

    private Tag tag;
    private List<TagField> customTags;
    private AudioHeader audioHeader;
    private String filepath;

    /**
     * @since 0.3.1
     */
    public void execute(
        final AudioFile audioFile
    ) {
        this.tag = audioFile.getTag();
        this.audioHeader = audioFile.getAudioHeader();
        this.filepath = audioFile.getFile().getAbsolutePath();

        if (null == tag || null == audioHeader) {
            log(LogMessage.WARNING_AUDIO_TAGGER_FILE_NOT_VALID, audioFile.getFile().getAbsolutePath());
            return;
        }

        this.customTags = AudioTaggerHelper.audiotagger_get_custom_tags(tag);

        final var song = this.readSong();

        song.setAlbumId(this.readAlbum());
        song.setArtistId(this.readArtist());
        song.setPrimaryGenreIds(this.readSongPrimaryGenres());
        song.setSecondaryGenreIds(this.readSongSecondaryGenres());

        songRepository().save(song);
    }

    private SongDto readSong() {
        try {
            final var bitRate = (int) audioHeader.getBitRateAsNumber();
            final var discNumber = str_parse_short_safe(tag.getFirst(FieldKey.DISC_NO));
            final var duration = audioHeader.getTrackLength() * 1_000L;
            final var format = AudioFormat.valueOf(audioHeader.getFormat().toUpperCase());
            final var rawLyrics = tag.getFirst(FieldKey.LYRICS);
            final var sampleRate = audioHeader.getSampleRateAsNumber();
            final var title = tag.getFirst(FieldKey.TITLE);
            final var trackNumber = str_parse_short_safe(tag.getFirst(FieldKey.TRACK));

            final var existingSong = songRepository().findByCriteria(Map.of(
                SongSearchCriteria.DURATION, duration,
                SongSearchCriteria.TITLE, title,
                SongSearchCriteria.TRACK_NUMBER, trackNumber
            ));

            SongDto song;

            if (!existingSong.isEmpty()) {
                song = existingSong.getFirst();
            } else {
                song = SongDto.builder().build();
            }

            song.setBitRate(bitRate);
            song.setDiscNumber(discNumber);
            song.setDuration(duration);
            song.setFilepath(filepath);
            song.setFormat(format);
            song.setRawLyrics(rawLyrics);
            song.setSampleRate(sampleRate);
            song.setTitle(title);
            song.setTrackNumber(trackNumber);

            return song;
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_READ_FILE, "[Song Partition] " + filepath);

            return SongDto.builder().title(filepath).build();
        }
    }

    private Long readAlbum() {
        try {
            AlbumDto album;

            final var artworkUrl = AudioTaggerHelper.audiotagger_get_custom_tag(customTags, CustomFieldKey.ARTWORK_URL);
            final var catalogNo = tag.getFirst(FieldKey.CATALOG_NO);
            final var discTotal = str_parse_short_safe(tag.getFirst(FieldKey.DISC_TOTAL));
            final var name = tag.getFirst(FieldKey.ALBUM);
            final var releaseDate = this.getLocalDate(tag.getFirst(FieldKey.YEAR));
            final var trackTotal = str_parse_short_safe(tag.getFirst(FieldKey.TRACK_TOTAL));

            final var typeCustomTag = AudioTaggerHelper.audiotagger_get_custom_tag(customTags, CustomFieldKey.ALBUM_TYPE);

            ReleaseType type;

            try {
                type = typeCustomTag.isBlank() ? null : ReleaseType.valueOf(typeCustomTag.toUpperCase());
            } catch (final IllegalArgumentException exception) {
                log(LogMessage.WARNING_AUDIO_TAGGER_TAG_NOT_VALID, "ReleaseType", typeCustomTag);
                type = null;
            }

            if (name.isBlank()) {
                return null;
            }

            final var existingAlbum = albumRepository().findByCriteria(Map.of(
                AlbumSearchCriteria.NAME, name
            ));

            if (!existingAlbum.isEmpty()) {
                album = existingAlbum.getFirst();
            } else {
                album = AlbumDto.builder()
                    .artistId(this.readAlbumArtist())
                    .labelId(this.readAlbumLabel())
                    .artworkUrl(artworkUrl)
                    .catalogNo(catalogNo)
                    .discTotal(discTotal)
                    .name(name)
                    .releaseDate(releaseDate)
                    .trackTotal(trackTotal)
                    .type(type)
                    .build();
            }

            albumRepository().save(album);

            return album.getId();
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_READ_FILE, "[Album Partition] " + filepath);

            return null;
        }
    }

    private Long readAlbumArtist() {
        try {
            ArtistDto albumArtist;

            final var name = tag.getFirst(FieldKey.ALBUM_ARTIST);

            if (name.isBlank()) {
                return this.readArtist();
            }

            final var country = this.getLocale(tag.getFirst(FieldKey.COUNTRY));

            final var existingArtist = artistRepository().findByCriteria(Map.of(
                ArtistSearchCriteria.NAME, name
            ));

            if (!existingArtist.isEmpty()) {
                albumArtist = existingArtist.getFirst();
            } else {
                albumArtist = ArtistDto.builder()
                    .country(country)
                    .name(name)
                    .build();
            }

            artistRepository().save(albumArtist);

            return albumArtist.getId();
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_READ_FILE, "[Album Artist Partition] " + filepath);

            return null;
        }
    }

    private Long readAlbumLabel() {
        try {
            LabelDto albumLabel;

            final var name = tag.getFirst(FieldKey.RECORD_LABEL);

            if (name.isBlank()) {
                return null;
            }

            final var existingLabel = labelRepository().findByCriteria(Map.of(
                LabelSearchCriteria.NAME, name
            ));

            if (!existingLabel.isEmpty()) {
                albumLabel = existingLabel.getFirst();
            } else {
                albumLabel = LabelDto.builder()
                    .name(name)
                    .build();
            }

            labelRepository().save(albumLabel);

            return albumLabel.getId();
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_READ_FILE, "[Label Partition] " + filepath);
            return null;
        }
    }

    private Long readArtist() {
        try {
            ArtistDto artist;

            final var artistArtworkUrl = AudioTaggerHelper.audiotagger_get_custom_tag(customTags, CustomFieldKey.ARTIST_ARTWORK_URL);
            final var country = this.getLocale(tag.getFirst(FieldKey.COUNTRY));
            final var name = tag.getFirst(FieldKey.ARTIST);

            if (name.isBlank()) {
                return null;
            }

            final var existingArtist = artistRepository().findByCriteria(Map.of(
                ArtistSearchCriteria.NAME, name
            ));

            if (!existingArtist.isEmpty()) {
                artist = existingArtist.getFirst();
            } else {
                artist = ArtistDto.builder()
                    .artworkUrl(artistArtworkUrl)
                    .country(country)
                    .name(name)
                    .build();
            }

            artistRepository().save(artist);

            return artist.getId();
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_READ_FILE, "[Artist Partition] " + filepath);
            return null;
        }
    }

    private List<Long> readSongPrimaryGenres() {
        try {
            final var primaryGenreNames = this.getGenresName("Primary Track Genres");
            return this.getGenres(primaryGenreNames);
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_READ_FILE, "[Song Primary Genres Partition] " + filepath);
            return new ArrayList<>();
        }
    }

    private List<Long> readSongSecondaryGenres() {
        try {
            final var secondaryGenreNames = this.getGenresName(CustomFieldKey.SONG_SECONDARY_GENRES.getKey());
            return this.getGenres(secondaryGenreNames);
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_READ_FILE, "[Song Secondary Genres Partition] " + filepath);
            return new ArrayList<>();
        }
    }

    // Genres

    private List<Long> getGenres(final List<String> genreNames) {
        final var genres = new ArrayList<GenreDto>();

        genreNames.forEach(primaryGenreName -> {
            final var existGenre = genreRepository().findByCriteria(Map.of(
                GenreSearchCriteria.NAME, primaryGenreName
            ));

            if (!existGenre.isEmpty()) {
                genres.add(existGenre.getFirst());
            } else {
                log(LogMessage.WARNING_REPOSITORY_ITEM_NOT_FOUND, "Genre", "name", primaryGenreName);
            }
        });

        return genres.stream()
            .map(GenreDto::getId)
            .toList();
    }

    private List<String> getGenresName(final String fieldKey) {
        final List<String> genresName = new ArrayList<>();

        try {
            this.customTags.forEach(tagField -> {
                switch (tagField) {
                    case AbstractID3v2Frame mp3TagField -> {
                        if (mp3TagField.getBody().getObjectValue("Description").equals(fieldKey)) {
                            genresName.addAll(
                                Arrays.asList(mp3TagField.getBody().getObjectValue("Text").toString().split("\0"))
                            );
                        }
                    }
                    case Mp4TagReverseDnsField mp4TagField -> {
                        if (mp4TagField.getDescriptor().equalsIgnoreCase(fieldKey)) {
                            genresName.add(mp4TagField.getContent());
                        }
                    }
                    default -> genresName.addAll(Arrays.asList(tag.getFirst(FieldKey.GENRE).split("\0")));
                }
            });
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_CUSTOM_TAG_FETCH, fieldKey + " for song: " + filepath);
        }

        return genresName;
    }

    // Getters

    private LocalDate getLocalDate(final String fieldValue) {
        try {
            if (fieldValue.length() == 4) {
                return LocalDate.of(Integer.parseInt(fieldValue), Month.JANUARY, 1);
            }

            return LocalDate.parse(fieldValue);
        } catch (final DateTimeParseException | NumberFormatException exception) {
            log(LogMessage.WARNING_DATE_FORMAT_INCORRECT, fieldValue);

            return LocalDate.of(0, 1, 1);
        }
    }

    private Locale getLocale(final String fieldValue) {
        try {
            return new Locale.Builder()
                .setLanguage(fieldValue.toLowerCase())
                .build();
        } catch (final Exception exception) {
            return new Locale.Builder()
                .setLanguage("en")
                .build();
        }
    }
}
