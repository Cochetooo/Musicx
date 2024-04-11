package fr.xahla.musicx.desktop.listener;

/** <b>Listener invoked when a task is executed and a progress has been made.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 * @see javafx.concurrent.Task
 */
public interface ProgressListener {
    void updateProgress(int progress, int total);
}
