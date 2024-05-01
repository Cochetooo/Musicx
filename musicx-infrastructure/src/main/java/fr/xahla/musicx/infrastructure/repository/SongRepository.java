package fr.xahla.musicx.infrastructure.repository;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.api.repository.SongRepositoryInterface;
import fr.xahla.musicx.infrastructure.model.data.enums.InfrastructureErrorMessage;
import fr.xahla.musicx.infrastructure.model.data.AudioDataInterface;
import fr.xahla.musicx.infrastructure.model.entity.AlbumDto;
import fr.xahla.musicx.infrastructure.model.entity.ArtistDto;
import org.hibernate.Transaction;

import static fr.xahla.musicx.infrastructure.config.HibernateLoader.openSession;
import static fr.xahla.musicx.infrastructure.model.SimpleLogger.logger;

/** <b>Class that defines the repository for the Song model.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class SongRepository implements SongRepositoryInterface {

    private final ArtistRepository artistRepository;
    private final AlbumRepository albumRepository;

    public SongRepository() {
        this.albumRepository = new AlbumRepository();
        this.artistRepository = new ArtistRepository();
    }

    /**
     * Hydrate a song and all its members from a raw local song imported by an external library,
     * following the <i>LocalSongInterface</i> scheme.
     * @see AudioDataInterface
     */
    public SongDto getFromLocalSong(final AudioDataInterface localSong) {
        if (localSong.hasFailed()) {
            logger().warning(InfrastructureErrorMessage.LOCAL_SONG_HAS_FAILED.getMsg(localSong.getTitle()));
        }

        var artist = new ArtistDto()
            .setId(null)
            .setCountry(localSong.getArtistCountry())
            .setName(localSong.getArtistName());

        var album = new AlbumDto()
            .setId(null)
            .setName(localSong.getAlbumName())
            .setReleaseYear(localSong.getYear())
            .setDiscTotal(localSong.getDiscTotal())
            .setTrackTotal(localSong.getTrackTotal());

        if (localSong.getAlbumArtist().equals(artist.getName())) {
            album.setArtist(artist);
        } else {
            var albumArtist = new ArtistDto()
                .setId(null)
                .setName(localSong.getAlbumArtist());

            album.setArtist(albumArtist);
        }

        final var song = new fr.xahla.musicx.infrastructure.model.entity.SongDto()
            .setId(null)
            .setAvailable(true)
            .setAlbum(album)
            .setArtist(artist)
            .setBitRate(localSong.getBitRate())
            .setDuration(localSong.getDuration())
            .setSampleRate(localSong.getSampleRate())
            .setTrackNumber(localSong.getTrackNumber())
            .setDiscNumber(localSong.getDiscNumber())
            .setPrimaryGenres(localSong.getGenresPrimary())
            .setSecondaryGenres(localSong.getGenresSecondary())
            .setTitle(localSong.getTitle());

        return song;
    }

    @Override public void save(final SongDto songDto) {
        this.artistRepository.save(songDto.getArtist());
        this.albumRepository.save(songDto.getAlbum());

        Transaction transaction = null;

        try (var session = openSession()) {
            transaction = session.beginTransaction();

            fr.xahla.musicx.infrastructure.model.entity.SongDto song;

            if (null != songDto.getId() && null != (song = session.get(fr.xahla.musicx.infrastructure.model.entity.SongDto.class, songDto.getId()))) {
                song.set(songDto);
                session.merge(song);
            } else {
                song = new fr.xahla.musicx.infrastructure.model.entity.SongDto().set(songDto);
                session.persist(song);
                songDto.setId(song.getId());
            }

            transaction.commit();
        } catch (Exception e) {
            if (null != transaction) {
                transaction.rollback();
            }

            logger().severe(e.getLocalizedMessage());
        }
    }
}
