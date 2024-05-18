package fr.xahla.musicx.domain.logging;

import lombok.Getter;

import java.time.ZoneId;
import java.time.format.DateTimeFormatter;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.ConsoleHandler;
import java.util.logging.Level;
import java.util.logging.LogRecord;

/**
 * Handles logs persistence for each log call.
 * @author Cochetooo
 * @since 0.3.1
 */
@Getter
public class SplitConsoleHandler extends ConsoleHandler {

    public interface MessageListener {
        void onMessage(final MessageData message);
    }

    /**
     * Carries the console message data for its listeners.
     * @since 0.3.1
     */
    public record MessageData(
        String message,
        Level level,
        ConsoleLogType type
    ) {}

    public static final int MAX_MESSAGE = 2_000;

    private final List<MessageData> hibernateSqlLogs;
    private final List<MessageData> otherLogs;

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

        if (Level.INFO == record.getLevel() && record.getMessage().contains("[Hibernate:Query]")) {
            if (hibernateSqlLogs.size() > MAX_MESSAGE) {
                hibernateSqlLogs.removeFirst();
            }

            final var messageData = new MessageData(
                outputMessage.replace("[Hibernate SQL]", ""), record.getLevel(), ConsoleLogType.HIBERNATE
            );

            hibernateSqlLogs.add(messageData);
            listeners.forEach(listener -> listener.onMessage(messageData));
        } else {
            if (otherLogs.size() > MAX_MESSAGE) {
                otherLogs.removeFirst();
            }

            final var messageData = new MessageData(
                outputMessage, record.getLevel(), ConsoleLogType.OTHER
            );

            otherLogs.add(messageData);
            listeners.forEach(listener -> listener.onMessage(messageData));
        }
    }

    /**
     * @since 0.3.1
     */
    public void addListener(final MessageListener listener) {
        listeners.add(listener);
    }

}
