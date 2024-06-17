package fr.xahla.musicx.domain.service.deleteLocalLibrary;

import org.hibernate.Session;
import org.hibernate.Transaction;

import java.util.HashMap;
import java.util.Map;

import static fr.xahla.musicx.domain.application.AbstractContext.openSession;

/**
 * Delete the whole local library, but keeps the genre list in the database.
 * @author Cochetooo
 * @since 0.5.1
 */
public final class DeleteLocalLibrary {

    public Map<String, Integer> execute() {
        Transaction transaction = null;

        final var response = new HashMap<String, Integer>();

        try (final var session = openSession()) {
            transaction = session.beginTransaction();

            response.put("album_credit_artists",    deleteTable("album_credit_artists", session));
            response.put("album_primary_genres",    deleteTable("album_primary_genres", session));
            response.put("album_secondary_genres",  deleteTable("album_secondary_genres", session));
            response.put("albums",                  deleteTable("albums", session));
            response.put("artist",                  deleteTable("artist", session));
            response.put("band_members",            deleteTable("band_members", session));
            response.put("label",                   deleteTable("label", session));
            response.put("label_genres",            deleteTable("label_genres", session));
            response.put("song",                    deleteTable("song", session));
            response.put("song_lyrics",             deleteTable("song_lyrics", session));
            response.put("song_primary_genres",     deleteTable("song_primary_genres", session));
            response.put("song_secondary_genres",   deleteTable("song_secondary_genres", session));

            transaction.commit();
        } catch (final Exception exception) {
            if (null != transaction) {
                transaction.rollback();
            }
        }

        return response;
    }

    private int deleteTable(final String tableName, final Session session) {
        final var hql = "DELETE FROM " + tableName;
        return session.createNativeMutationQuery(hql).executeUpdate();
    }

}
