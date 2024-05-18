package fr.xahla.musicx.domain.helper;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Simple utility class to make benchmark in milliseconds by returning elapsed time.
 * @author Cochetooo
 * @since 0.3.2
 */
public final class Benchmark {

    private long start;

    public Benchmark() {
        reset();
    }

    /**
     * @return Elapsed time since the instantiation of the benchmark in milliseconds.
     * @since 0.3.2
     */
    public long getElapsedTime() {
        return System.currentTimeMillis() - start;
    }

    /**
     * Put the counter to current time to simulate a reset.
     * @since 0.3.2
     */
    public void reset() {
        start = System.currentTimeMillis();
    }

    /**
     * @since 0.3.2
     */
    public void print(final String action) {
        logger().info("BENCHMARK", action, this.getElapsedTime());
    }
}
