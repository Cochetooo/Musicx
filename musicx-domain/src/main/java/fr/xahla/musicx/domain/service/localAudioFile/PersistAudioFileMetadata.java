package fr.xahla.musicx.domain.service.localAudioFile;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.AlbumType;
import fr.xahla.musicx.api.model.enums.AudioFormat;
import fr.xahla.musicx.api.repository.searchCriterias.*;
import fr.xahla.musicx.domain.helper.AudioTaggerHelper;
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
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.repository.AlbumRepository.albumRepository;
import static fr.xahla.musicx.domain.repository.ArtistRepository.artistRepository;
import static fr.xahla.musicx.domain.repository.GenreRepository.genreRepository;
import static fr.xahla.musicx.domain.repository.LabelRepository.labelRepository;
import static fr.xahla.musicx.domain.repository.SongRepository.songRepository;

public final class PersistAudioFileMetadata {

    private Tag tag;
    private List<TagField> customTags;
    private AudioHeader audioHeader;
    private String filepath;

    public void execute(
        final AudioFile audioFile
    ) {
        this.tag = audioFile.getTag();
        this.audioHeader = audioFile.getAudioHeader();
        this.filepath = audioFile.getFile().getAbsolutePath();

        if (null == tag || null == audioHeader) {
            logger().warning("Skipping potentially corrupted song: " + audioFile.getFile().getAbsolutePath());
            return;
        }

        this.customTags = AudioTaggerHelper.getCustomTags(tag);

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
            final var discNumber = this.getShort(tag.getFirst(FieldKey.DISC_NO));
            final var duration = audioHeader.getTrackLength() * 1_000L;
            final var format = AudioFormat.valueOf(audioHeader.getFormat().toUpperCase());
            final var rawLyrics = tag.getFirst(FieldKey.LYRICS);
            final var sampleRate = audioHeader.getSampleRateAsNumber();
            final var title = tag.getFirst(FieldKey.TITLE);
            final var trackNumber = this.getShort(tag.getFirst(FieldKey.TRACK));

            final var existingSong = songRepository().findByCriteria(Map.of(
                SongSearchCriterias.DURATION, duration,
                SongSearchCriterias.TITLE, title,
                SongSearchCriterias.TRACK_NUMBER, trackNumber
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
            logger().log(Level.SEVERE, "Failed to read song for filepath: " + filepath, exception);

            return SongDto.builder().title(filepath).build();
        }
    }

    private Long readAlbum() {
        try {
            AlbumDto album;

            final var artworkUrl = AudioTaggerHelper.getCustomTag(customTags, CustomFieldKey.ARTWORK_URL);
            final var catalogNo = tag.getFirst(FieldKey.CATALOG_NO);
            final var discTotal = this.getShort(tag.getFirst(FieldKey.DISC_TOTAL));
            final var name = tag.getFirst(FieldKey.ALBUM);
            final var releaseDate = this.getLocalDate(tag.getFirst(FieldKey.YEAR));
            final var trackTotal = this.getShort(tag.getFirst(FieldKey.TRACK_TOTAL));
            final var type = AlbumType.valueOf(
                AudioTaggerHelper.getCustomTag(customTags, CustomFieldKey.ALBUM_TYPE).toUpperCase()
            );

            if (name.isBlank()) {
                return null;
            }

            final var existingAlbum = albumRepository().findByCriteria(Map.of(
                AlbumSearchCriterias.NAME, name
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
            logger().log(Level.SEVERE, "Failed to read album for song: " + filepath, exception);
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
                ArtistSearchCriterias.NAME, name
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
            logger().log(Level.SEVERE, "Failed to read album artist for song: " + filepath, exception);
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
                LabelSearchCriterias.NAME, name
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
            logger().log(Level.SEVERE, "Failed to read album label for song: " + filepath, exception);
            return null;
        }
    }

    private Long readArtist() {
        try {
            ArtistDto artist;

            final var artistArtworkUrl = AudioTaggerHelper.getCustomTag(customTags, CustomFieldKey.ARTIST_ARTWORK_URL);
            final var country = this.getLocale(tag.getFirst(FieldKey.COUNTRY));
            final var name = tag.getFirst(FieldKey.ARTIST);

            if (name.isBlank()) {
                return null;
            }

            final var existingArtist = artistRepository().findByCriteria(Map.of(
                ArtistSearchCriterias.NAME, name
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
            logger().log(Level.SEVERE, "Failed to read artist for song: " + filepath, exception);
            return null;
        }
    }

    private List<Long> readSongPrimaryGenres() {
        try {
            final var primaryGenreNames = this.getGenresName("Primary Track Genres");
            return this.getGenres(primaryGenreNames);
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "Failed to read primary genres for song: " + filepath, exception);
            return new ArrayList<>();
        }
    }

    private List<Long> readSongSecondaryGenres() {
        try {
            final var secondaryGenreNames = this.getGenresName(CustomFieldKey.SONG_SECONDARY_GENRES.getKey());
            return this.getGenres(secondaryGenreNames);
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "Failed to read secondary genres for song: " + filepath, exception);
            return new ArrayList<>();
        }
    }

    // Genres

    private List<Long> getGenres(final List<String> genreNames) {
        final var genres = new ArrayList<GenreDto>();

        genreNames.forEach(primaryGenreName -> {
            final var existGenre = genreRepository().findByCriteria(Map.of(
                GenreSearchCriterias.NAME, primaryGenreName
            ));

            if (!existGenre.isEmpty()) {
                genres.add(existGenre.getFirst());
            } else {
                logger().warning("Given genre does not exist: " + primaryGenreName);
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
            logger().log(Level.SEVERE, "Error while setting up " + fieldKey + " for song: " + filepath, exception);
        }

        return genresName;
    }

    // Getters

    private int getInteger(final String fieldValue) {
        try {
            return Integer.parseInt(fieldValue);
        } catch (final NumberFormatException exception) {
            return 0;
        }
    }

    private LocalDate getLocalDate(final String fieldValue) {
        try {
            if (fieldValue.length() == 4) {
                return LocalDate.of(Integer.parseInt(fieldValue), Month.JANUARY, 1);
            }

            return LocalDate.parse(fieldValue);
        } catch (final DateTimeParseException | NumberFormatException exception) {
            logger().warning("Wrong format of date: " + fieldValue);

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

    private short getShort(final String fieldValue) {
        try {
            return Short.parseShort(fieldValue);
        } catch (final NumberFormatException exception) {
            return 0;
        }
    }
}
