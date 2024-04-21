package fr.xahla.musicx.core.repository;

import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.api.repository.SongRepositoryInterface;
import fr.xahla.musicx.core.logging.ErrorMessage;
import fr.xahla.musicx.core.model.data.LocalSongInterface;
import fr.xahla.musicx.core.model.entity.Album;
import fr.xahla.musicx.core.model.entity.Artist;
import fr.xahla.musicx.core.model.entity.Song;
import org.hibernate.SessionFactory;
import org.hibernate.Transaction;

import java.util.logging.Logger;

import static fr.xahla.musicx.core.config.HibernateLoader.openSession;
import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

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
     * @see LocalSongInterface
     */
    public SongInterface getFromLocalSong(final LocalSongInterface localSong) {
        if (localSong.hasFailed()) {
            logger().warning(ErrorMessage.LOCAL_SONG_HAS_FAILED.getMsg(localSong.getTitle()));
        }

        var artist = new Artist()
            .setId(null)
            .setCountry(localSong.getArtistCountry())
            .setName(localSong.getArtistName());

        var album = new Album()
            .setId(null)
            .setName(localSong.getAlbumName())
            .setReleaseYear(localSong.getYear())
            .setDiscTotal(localSong.getDiscTotal())
            .setTrackTotal(localSong.getTrackTotal());

        if (localSong.getAlbumArtist().equals(artist.getName())) {
            album.setArtist(artist);
        } else {
            var albumArtist = new Artist()
                .setId(null)
                .setName(localSong.getAlbumArtist());

            album.setArtist(albumArtist);
        }

        final var song = new Song()
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

    @Override public void save(final SongInterface songInterface) {
        this.artistRepository.save(songInterface.getArtist());
        this.albumRepository.save(songInterface.getAlbum());

        Transaction transaction = null;

        try (var session = openSession()) {
            transaction = session.beginTransaction();

            Song song;

            if (null != songInterface.getId() && null != (song = session.get(Song.class, songInterface.getId()))) {
                song.set(songInterface);
                session.merge(song);
            } else {
                song = new Song().set(songInterface);
                session.persist(song);
                songInterface.setId(song.getId());
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
