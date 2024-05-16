package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.model.TaskProgress;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.value.ChangeListener;

/**
 * Handles task progress for JavaFX.
 * @author Cochetooo
 * @since 0.2.0
 */
public class TaskProgressManager {

    private final ObjectProperty<TaskProgress> taskProgress;

    public TaskProgressManager() {
        this.taskProgress = new SimpleObjectProperty<>();
    }

    /**
     * @since 0.2.0
     */
    public TaskProgress getTaskProgress() {
        return taskProgress.get();
    }

    /**
     * @since 0.2.0
     */
    public TaskProgressManager setTaskProgress(final TaskProgress taskProgress) {
        this.taskProgress.set(taskProgress);
        return this;
    }

    /**
     * @since 0.2.0
     */
    public void onTaskProgressChange(final ChangeListener<TaskProgress> change) {
        this.taskProgress.addListener(change);
    }
}
