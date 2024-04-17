package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.desktop.listener.ProgressListener;
import javafx.beans.property.*;
import javafx.concurrent.Task;

public class TaskProgress {

    private final DoubleProperty total;
    private final DoubleProperty currentProgress;
    private final StringProperty name;

    private final Task<?> task;

    public TaskProgress(
        final Task<?> task,
        final String name
    ) {
        this.total = new SimpleDoubleProperty(0);
        this.currentProgress = new SimpleDoubleProperty(-1);
        this.name = new SimpleStringProperty(name);
        this.task = task;

        this.currentProgress.bind(task.progressProperty());
        this.total.bind(task.totalWorkProperty());
    }

    public Task<?> getTask() {
        return task;
    }

    public double getTotal() {
        return total.get();
    }

    public DoubleProperty totalProperty() {
        return total;
    }

    public double getCurrentProgress() {
        return currentProgress.get();
    }

    public DoubleProperty currentProgressProperty() {
        return currentProgress;
    }

    public String getName() {
        return this.name.get();
    }

}
