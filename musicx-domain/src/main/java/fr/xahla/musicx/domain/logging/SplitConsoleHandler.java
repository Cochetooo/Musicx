package fr.xahla.musicx.domain.logging;

import lombok.Getter;

import java.time.ZoneId;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.ConsoleHandler;
import java.util.logging.Level;
import java.util.logging.LogRecord;

@Getter
public class SplitConsoleHandler extends ConsoleHandler {

    public static interface MessageListener {
        void onMessage(final String message, final Level level);
    }

    public static final int MAX_MESSAGE = 2_000;

    private final List<String> hibernateSqlLogs;
    private final List<String> otherLogs;

    private final List<MessageListener> listeners;

    public SplitConsoleHandler() {
        super();

        this.hibernateSqlLogs = new ArrayList<>();
        this.otherLogs = new ArrayList<>();

        this.listeners = new ArrayList<>();
    }

    @Override public void publish(final LogRecord record) {
        final var sourceClassName = record.getSourceClassName();
        final var dateFormatted = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss,SSS")
            .format(record.getInstant().atZone(ZoneId.of("UTC")));

        final var outputMessage =
            "[" + dateFormatted + "] " +
            "- " + record.getLevel() + " - " +
            "[" + sourceClassName.substring(sourceClassName.lastIndexOf('.') + 1) + "." + record.getSourceMethodName() + "] " +
            record.getMessage();

        if (Level.INFO == record.getLevel() && record.getMessage().contains("[Hibernate SQL]")) {
            if (hibernateSqlLogs.size() > MAX_MESSAGE) {
                hibernateSqlLogs.removeFirst();
            }

            hibernateSqlLogs.add(outputMessage);
        } else {
            if (otherLogs.size() > MAX_MESSAGE) {
                otherLogs.removeFirst();
            }

            otherLogs.add(outputMessage);
        }

        listeners.forEach(listener -> listener.onMessage(outputMessage, record.getLevel()));
    }

    public void addListener(final MessageListener listener) {
        listeners.add(listener);
    }

}
