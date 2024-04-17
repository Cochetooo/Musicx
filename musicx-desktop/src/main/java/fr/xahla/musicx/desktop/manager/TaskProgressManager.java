package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.desktop.model.TaskProgress;
import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;
import javafx.beans.value.ChangeListener;

public class TaskProgressManager {

    private final ObjectProperty<TaskProgress> taskProgress;

    public TaskProgressManager() {
        this.taskProgress = new SimpleObjectProperty<>();
    }

    public TaskProgress getTaskProgress() {
        return taskProgress.get();
    }

    public TaskProgressManager setTaskProgress(final TaskProgress taskProgress) {
        this.taskProgress.set(taskProgress);
        return this;
    }

    public void onTaskProgressChange(final ChangeListener<TaskProgress> change) {
        this.taskProgress.addListener(change);
    }
}
