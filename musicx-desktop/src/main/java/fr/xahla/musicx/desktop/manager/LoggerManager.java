package fr.xahla.musicx.desktop.manager;

import javafx.beans.property.ObjectProperty;
import javafx.beans.property.SimpleObjectProperty;

import java.io.ByteArrayOutputStream;
import java.io.PrintStream;
import java.util.logging.SimpleFormatter;
import java.util.logging.StreamHandler;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public class LoggerManager {

    private final ObjectProperty<ByteArrayOutputStream> outputStream;

    public LoggerManager() {
        this.outputStream = new SimpleObjectProperty<>(new ByteArrayOutputStream());
        final var printStream = new PrintStream(outputStream.get());

        //System.setOut(printStream);
        //System.setErr(printStream);

        logger().addHandler(new StreamHandler(printStream, new SimpleFormatter()));
    }

    public ByteArrayOutputStream getOutputStream() {
        return outputStream.get();
    }

    public ObjectProperty<ByteArrayOutputStream> outputStreamProperty() {
        return outputStream;
    }

    public LoggerManager setOutputStream(final ByteArrayOutputStream outputStream) {
        this.outputStream.set(outputStream);
        return this;
    }
}
