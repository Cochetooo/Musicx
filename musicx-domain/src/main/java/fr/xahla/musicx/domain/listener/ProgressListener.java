package fr.xahla.musicx.domain.listener;

/**
 * Listener for a progress task.
 * @author Cochetooo
 */
public interface ProgressListener {
    void updateProgress(int progress, int total);
}
