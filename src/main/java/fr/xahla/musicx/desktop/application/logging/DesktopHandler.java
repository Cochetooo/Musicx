package fr.xahla.musicx.desktop.application.logging;

import java.io.BufferedOutputStream;
import java.util.logging.LogRecord;
import java.util.logging.StreamHandler;

public class DesktopHandler extends StreamHandler {

    private BufferedOutputStream outputStream;

    @Override public void publish(LogRecord record) {
        super.publish(record);
    }

}
