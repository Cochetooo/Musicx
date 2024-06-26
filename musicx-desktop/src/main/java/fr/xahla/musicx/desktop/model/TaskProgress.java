package fr.xahla.musicx.desktop.model;

import javafx.beans.property.DoubleProperty;
import javafx.beans.property.SimpleDoubleProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;
import javafx.concurrent.Task;

/**
 * Handles task progress for desktop application.
 * @author Cochetooo
 * @since 0.2.0
 */
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
