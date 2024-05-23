import fr.xahla.musicx.domain.application.AbstractContext;
import fr.xahla.musicx.domain.database.HibernateLoader;

import java.util.logging.Logger;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;
import static fr.xahla.musicx.domain.application.AbstractContext.songRepository;

public class MainTest {

    static class Context extends AbstractContext {
        protected Context() {
            super(Logger.getLogger(Context.class.getName()));
        }
    }

    public static void main(final String[] args) {
        new Context();
        new HibernateLoader();

       logger().info("Fetching all songs: ");

       final var time = System.currentTimeMillis();

       final var songs = songRepository().findAll();

       logger().info("Time loading all songs: " + (System.currentTimeMillis() - time) + " ms (" + songs.size() + " elements loaded).");

    }

}
