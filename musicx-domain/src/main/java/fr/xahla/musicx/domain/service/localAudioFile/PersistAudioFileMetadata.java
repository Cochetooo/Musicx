package fr.xahla.musicx.domain.service.localAudioFile;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.model.enums.AudioFormat;
import fr.xahla.musicx.api.repository.*;
import fr.xahla.musicx.api.repository.searchCriterias.*;
import fr.xahla.musicx.domain.helper.AudioTaggerHelper;
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

public final class PersistAudioFileMetadata {

    private SongRepositoryInterface songRepository;
    private AlbumRepositoryInterface albumRepository;
    private ArtistRepositoryInterface artistRepository;
    private GenreRepositoryInterface genreRepository;
    private LabelRepositoryInterface labelRepository;

    private Tag tag;
    private List<TagField> customTags;
    private AudioHeader audioHeader;
    private String filepath;

    public void execute(
        final AudioFile audioFile,
        final SongRepositoryInterface songRepository,
        final AlbumRepositoryInterface albumRepository,
        final GenreRepositoryInterface genreRepository,
        final ArtistRepositoryInterface artistRepository,
        final LabelRepositoryInterface labelRepository
    ) {
        this.songRepository = songRepository;
        this.albumRepository = albumRepository;
        this.artistRepository = artistRepository;
        this.genreRepository = genreRepository;
        this.labelRepository = labelRepository;

        this.tag = audioFile.getTag();
        this.audioHeader = audioFile.getAudioHeader();
        this.filepath = audioFile.getFile().getAbsolutePath();

        if (null == tag || null == audioHeader) {
            logger().warning("Skipping potentially corrupted song: " + audioFile.getFile().getAbsolutePath());
            return;
        }

        this.customTags = AudioTaggerHelper.getCustomTags(tag);

        final var duration = audioHeader.getTrackLength() * 1_000L;
        final var title = tag.getFirst(FieldKey.TITLE);
        final var trackNumber = this.getShort(tag.getFirst(FieldKey.TRACK));

        final var existingSong = songRepository.findByCriteria(Map.of(
            SongSearchCriterias.DURATION, duration,
            SongSearchCriterias.TITLE, title,
            SongSearchCriterias.TRACK_NUMBER, trackNumber
        ));

        SongDto song;

        if (!existingSong.isEmpty()) {
            song = existingSong.getFirst();
        } else {
            song = new SongDto();
        }

        song.setAlbumId(this.setAlbum());
        song.setArtistId(this.setArtist());
        song.setPrimaryGenreIds(this.setPrimaryGenres());
        song.setSecondaryGenreIds(this.setSecondaryGenres());

        song
            .setBitRate((int) audioHeader.getBitRateAsNumber())
            .setDiscNumber(this.getShort(tag.getFirst(FieldKey.DISC_NO)))
            .setDuration(duration)
            .setFormat(AudioFormat.valueOf(audioHeader.getFormat().toUpperCase()))
            .setLyrics(tag.getFirst(FieldKey.LYRICS))
            .setSampleRate(audioHeader.getSampleRateAsNumber())
            .setTitle(title)
            .setTrackNumber(trackNumber);

        songRepository.save(song);
    }

    private Long setAlbum() {
        AlbumDto album;

        final var catalogNo = tag.getFirst(FieldKey.CATALOG_NO);
        final var name = tag.getFirst(FieldKey.ALBUM);

        if (name.isBlank()) {
            return null;
        }

        final var existingAlbum = albumRepository.findByCriteria(Map.of(
            AlbumSearchCriterias.NAME, name
        ));

        if (!existingAlbum.isEmpty()) {
            album = existingAlbum.getFirst();
        } else {
            album = new AlbumDto()
                .setArtistId(this.setAlbumArtist())
                .setArtworkUrl(tag.getFirst(FieldKey.COVER_ART))
                .setCatalogNo(catalogNo)
                .setDiscTotal(this.getShort(tag.getFirst(FieldKey.DISC_TOTAL)))
                .setLabelId(this.setAlbumLabel())
                .setName(name)
                .setReleaseDate(this.getLocalDate(tag.getFirst(FieldKey.YEAR)))
                .setTrackTotal(this.getShort(tag.getFirst(FieldKey.TRACK_TOTAL)));
        }

        albumRepository.save(album);

        return album.getId();
    }

    private Long setAlbumArtist() {
        ArtistDto albumArtist;

        final var name = tag.getFirst(FieldKey.ALBUM_ARTIST);

        if (name.isBlank()) {
            return setArtist();
        }

        final var country = this.getLocale(tag.getFirst(FieldKey.COUNTRY));

        final var existingArtist = artistRepository.findByCriteria(Map.of(
            ArtistSearchCriterias.NAME, name
        ));

        if (!existingArtist.isEmpty()) {
            albumArtist = existingArtist.getFirst();
        } else {
            albumArtist = new ArtistDto()
                .setCountry(country)
                .setName(name);
        }

        artistRepository.save(albumArtist);

        return albumArtist.getId();
    }

    private Long setAlbumLabel() {
        LabelDto albumLabel;

        final var name = tag.getFirst(FieldKey.RECORD_LABEL);

        if (name.isBlank()) {
            return null;
        }

        final var existingLabel = labelRepository.findByCriteria(Map.of(
            LabelSearchCriterias.NAME, name
        ));

        if (!existingLabel.isEmpty()) {
            albumLabel = existingLabel.getFirst();
        } else {
            albumLabel = new LabelDto()
                .setName(name);
        }

        labelRepository.save(albumLabel);

        return albumLabel.getId();
    }

    private Long setArtist() {
        ArtistDto artist;

        final var country = this.getLocale(tag.getFirst(FieldKey.COUNTRY));
        final var name = tag.getFirst(FieldKey.ARTIST);

        if (name.isBlank()) {
            return null;
        }

        final var existingArtist = artistRepository.findByCriteria(Map.of(
            ArtistSearchCriterias.NAME, name
        ));

        if (!existingArtist.isEmpty()) {
            artist = existingArtist.getFirst();
        } else {
            artist = new ArtistDto()
                .setCountry(country)
                .setName(name);
        }

        artistRepository.save(artist);

        return artist.getId();
    }

    private List<Long> setPrimaryGenres() {
        final var primaryGenreNames = this.getGenresName("PRIMARY TRACK GENRES");
        return this.getGenres(primaryGenreNames);
    }

    private List<Long> setSecondaryGenres() {
        final var secondaryGenreNames = this.getGenresName("SECONDARY TRACK GENRES");
        return this.getGenres(secondaryGenreNames);
    }

    // Genres

    private List<Long> getGenres(final List<String> genreNames) {
        final var genres = new ArrayList<GenreDto>();

        genreNames.forEach(primaryGenreName -> {
            final var existGenre = genreRepository.findByCriteria(Map.of(
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
        return Integer.parseInt(fieldValue);
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
        return new Locale.Builder()
            .setLanguage(fieldValue.toLowerCase())
            .build();
    }

    private short getShort(final String fieldValue) {
        return Short.parseShort(fieldValue);
    }
}
