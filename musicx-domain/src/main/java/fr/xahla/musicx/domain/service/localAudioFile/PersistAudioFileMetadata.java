package fr.xahla.musicx.domain.service.localAudioFile;

import fr.xahla.musicx.api.model.*;
import fr.xahla.musicx.api.repository.*;
import fr.xahla.musicx.api.repository.searchCriterias.AlbumSearchCriterias;
import fr.xahla.musicx.api.repository.searchCriterias.ArtistSearchCriterias;
import fr.xahla.musicx.domain.helper.AudioTaggerHelper;
import org.jaudiotagger.audio.AudioFile;
import org.jaudiotagger.audio.AudioHeader;
import org.jaudiotagger.tag.FieldKey;
import org.jaudiotagger.tag.Tag;
import org.jaudiotagger.tag.TagField;

import java.time.LocalDate;
import java.util.List;
import java.util.Locale;
import java.util.Map;

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

        if (null == tag || null == audioHeader) {
            logger().warning("Skipping potentially corrupted song: " + audioFile.getFile().getAbsolutePath());
            return;
        }

        this.customTags = AudioTaggerHelper.getCustomTags(tag);

        final var song = new SongDto();

        song.setAlbumId(this.setAlbum());
        song.setArtistId(this.setArtist());
        song.setPrimaryGenreIds(this.setPrimaryGenres());
        song.setSecondaryGenreIds(this.setSecondaryGenres());

        songRepository.save(song);
    }

    private Long setAlbum() {
        AlbumDto album;

        final var existingAlbum = albumRepository.findByCriteria(Map.of(
            AlbumSearchCriterias.NAME, FieldKey.ALBUM,
            AlbumSearchCriterias.ARTIST_NAME, FieldKey.ALBUM_ARTIST
        ));

        if (!existingAlbum.isEmpty()) {
            album = existingAlbum.getFirst();
        } else {
            album = new AlbumDto()
                .setArtistId(this.setAlbumArtist())
                .setArtworkUrl(tag.getFirst(FieldKey.COVER_ART))
                .setCatalogNo(tag.getFirst(FieldKey.CATALOG_NO))
                .setDiscTotal(this.getShort(tag.getFirst(FieldKey.DISC_TOTAL)))
                .setReleaseDate(this.getLocalDate(tag.getFirst(FieldKey.ALBUM_YEAR)))
                .setTrackTotal(this.getShort(tag.getFirst(FieldKey.TRACK_TOTAL)));
        }

        albumRepository.save(album);

        return album.getId();
    }

    private Long setArtist() {
        ArtistDto artist;

        final var existingArtist = artistRepository.findByCriteria(Map.of(
            ArtistSearchCriterias.NAME, FieldKey.ARTIST,
            ArtistSearchCriterias.COUNTRY, FieldKey.COUNTRY
        ));

        if (!existingArtist.isEmpty()) {
            artist = existingArtist.getFirst();
        } else {
            artist = new ArtistDto()
                .setCountry(new Locale(tag.getFirst(FieldKey.COUNTRY)))
                .setName(tag.getFirst(FieldKey.ARTIST));
        }

        artistRepository.save(artist);

        return artist.getId();
    }

    private Long setAlbumArtist() {
        ArtistDto albumArtist;

        return albumArtist.getId();
    }

    private List<Long> setPrimaryGenres() {
        List<GenreDto> primaryGenres;

        return
    }

    private List<Long> setSecondaryGenres() {
        List<GenreDto> secondaryGenres;
    }

    private Long setLabel() {
        LabelDto albumLabel;
    }

    // Getters

    private int getInteger(final String fieldValue) {
        return Integer.parseInt(fieldValue);
    }

    private LocalDate getLocalDate(final String fieldValue) {
        return LocalDate.parse(fieldValue);
    }

    private short getShort(final String fieldValue) {
        return Short.parseShort(fieldValue);
    }
}
