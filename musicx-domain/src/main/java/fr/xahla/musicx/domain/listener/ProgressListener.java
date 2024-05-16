package fr.xahla.musicx.domain.listener;

/**
 * Listener for a progress task.
 * @author Cochetooo
 * @since 0.2.0
 */
public interface ProgressListener {
    void updateProgress(int progress, int total);
}
