package fr.xahla.musicx.domain.helper;

import fr.xahla.musicx.domain.logging.LogMessage;

import static fr.xahla.musicx.domain.application.AbstractContext.log;
import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Simple utility class to make benchmark in milliseconds by returning elapsed time.
 * @author Cochetooo
 */
public final class Benchmark {

    private long start;

    public Benchmark() {
        reset();
    }

    /**
     * @return Elapsed time since the instantiation of the benchmark in milliseconds.
     */
    public long getElapsedTime() {
        return System.currentTimeMillis() - start;
    }

    /**
     * Put the counter to current time to simulate a reset.
     */
    public void reset() {
        start = System.currentTimeMillis();
    }

    public void print(final String action) {
        log(LogMessage.INFO_BENCHMARK, action, this.getElapsedTime());
    }
}
