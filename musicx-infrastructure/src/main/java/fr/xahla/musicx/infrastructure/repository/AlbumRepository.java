package fr.xahla.musicx.infrastructure.repository;

import fr.xahla.musicx.api.model.AlbumDto;
import fr.xahla.musicx.api.repository.AlbumRepositoryInterface;
import org.hibernate.Transaction;

import static fr.xahla.musicx.infrastructure.config.HibernateLoader.openSession;
import static fr.xahla.musicx.infrastructure.model.SimpleLogger.logger;

/** <b>Class that defines the repository for the Album model.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public class AlbumRepository implements AlbumRepositoryInterface {

    private final ArtistRepository artistRepository;

    public AlbumRepository() {
        this.artistRepository = new ArtistRepository();
    }

    @Override public void save(final AlbumDto albumDto) {
        this.artistRepository.save(albumDto.getArtist());

        Transaction transaction = null;

        try (var session = openSession()) {
            transaction = session.beginTransaction();

            fr.xahla.musicx.infrastructure.model.entity.AlbumDto album;

            if (null != albumDto.getId() && null != (album = session.get(fr.xahla.musicx.infrastructure.model.entity.AlbumDto.class, albumDto.getId()))) {
                album.set(albumDto);
                session.merge(album);
            } else {
                album = new fr.xahla.musicx.infrastructure.model.entity.AlbumDto().set(albumDto);
                session.persist(album);
                albumDto.setId(album.getId());
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
