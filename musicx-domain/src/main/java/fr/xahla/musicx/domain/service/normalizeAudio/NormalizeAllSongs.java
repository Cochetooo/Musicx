package fr.xahla.musicx.domain.service.normalizeAudio;

import fr.xahla.musicx.api.model.SongDto;
import fr.xahla.musicx.domain.helper.Benchmark;
import fr.xahla.musicx.domain.listener.ProgressListener;

import java.util.List;
import java.util.concurrent.Executors;
import java.util.concurrent.atomic.AtomicInteger;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * @author Cochetooo
 * @version 0.5.0
 */
public class NormalizeAllSongs {

    public void execute(
        final List<SongDto> songs,
        final double decibel,
        final ProgressListener progressListener
    ) {
        final var benchmark = new Benchmark();

        final var progressCount = new AtomicInteger();

        try (final var threadExecutor = Executors.newFixedThreadPool(Runtime.getRuntime().availableProcessors())) {
            songs.forEach(song -> {
                threadExecutor.submit(() -> {
                    final var dbModified = new NormalizeAudio().execute(
                        song.getFilepath(),
                        decibel
                    );

                    logger().fine("SERVICE_NORMALIZE_VOLUME_DB_MODIFIED", song.getFilepath(), dbModified);

                    progressCount.incrementAndGet();
                    progressListener.updateProgress(progressCount.get(), songs.size());
                });
            });
        }

        benchmark.print("Finished normalizing " + songs.size() + " songs");
    }

}
