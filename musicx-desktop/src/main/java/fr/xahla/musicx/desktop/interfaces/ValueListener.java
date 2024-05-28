package fr.xahla.musicx.desktop.interfaces;

import javafx.beans.value.ObservableValue;

/**
 * Custom listeners call for JavaFX Beans.
 * @author Cochetooo
 * @since 0.2.3
 */
public interface ValueListener<T> {

    void changed(ObservableValue<? extends T> observable, T oldValue, T newValue);

}
