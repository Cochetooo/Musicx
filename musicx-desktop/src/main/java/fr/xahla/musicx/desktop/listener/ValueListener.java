package fr.xahla.musicx.desktop.listener;

import javafx.beans.value.ObservableValue;

public interface ValueListener<T> {

    void changed(ObservableValue<? extends T> observable, T oldValue, T newValue);

}
