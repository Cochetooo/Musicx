package fr.xahla.musicx.desktop.views.content.console;

import fr.xahla.musicx.domain.logging.ConsoleLogType;
import fr.xahla.musicx.domain.logging.SplitConsoleHandler;
import javafx.application.Platform;
import javafx.collections.FXCollections;
import javafx.collections.transformation.FilteredList;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;
import javafx.scene.control.ComboBox;
import javafx.scene.control.ListCell;
import javafx.scene.control.ListView;
import javafx.scene.paint.Color;

import java.net.URL;
import java.util.ResourceBundle;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * View for the console output.
 * @author Cochetooo
 * @since 0.2.2
 */
public class Console implements Initializable {

    @FXML private ComboBox<ConsoleLevel> logLevelComboBox;

    @FXML private ListView<SplitConsoleHandler.MessageData> queryOutputTextArea;
    @FXML private ListView<SplitConsoleHandler.MessageData> consoleOutputTextArea;

    @Override public void initialize(final URL url, final ResourceBundle resourceBundle) {
        final var consoleHandler = (SplitConsoleHandler) logger().getLogger().getHandlers()[0];

        final var consoleList = FXCollections.observableList(consoleHandler.getOtherLogs());
        final var consoleFilterList = new FilteredList<>(consoleList);

        consoleOutputTextArea.setItems(consoleFilterList);

        logLevelComboBox.getItems().addAll(ConsoleLevel.values());
        logLevelComboBox.getSelectionModel().select(ConsoleLevel.ALL);
        logLevelComboBox.valueProperty().addListener((observable, oldValue, newValue)
            -> this.updateFilterList(consoleFilterList, newValue.value)
        );

        final var queryList = FXCollections.observableList(consoleHandler.getHibernateSqlLogs());
        queryOutputTextArea.setItems(queryList);

        consoleOutputTextArea.setCellFactory(cellFactory -> new ConsoleCellFactory());
        queryOutputTextArea.setCellFactory(cellFactory -> new ConsoleCellFactory());

        consoleHandler.addListener(message -> {
            if (message.type() == ConsoleLogType.HIBERNATE) {
                queryList.add(message);
            } else {
                Platform.runLater(()
                    -> this.updateFilterList(consoleFilterList, logLevelComboBox.getSelectionModel().getSelectedItem().value));
            }
        });
    }

    private void updateFilterList(final FilteredList<SplitConsoleHandler.MessageData> filteredList, final int newValue) {
        filteredList.setPredicate(messageData ->
            messageData.level().intValue() >= newValue
        );
    }

    /* -------- Inner classes ----------- */

    static class ConsoleCellFactory extends ListCell<SplitConsoleHandler.MessageData> {
        @Override protected void updateItem(final SplitConsoleHandler.MessageData messageData, final boolean empty) {
            if (null == messageData) {
                setText(null);
                return;
            }

            super.updateItem(messageData, empty);

            if (Level.SEVERE == messageData.level()) setTextFill(Color.SALMON);
            else if (Level.WARNING == messageData.level()) setTextFill(Color.GOLD);
            else if (Level.INFO == messageData.level()) setTextFill(Color.LIGHTCYAN);
            else if (Level.CONFIG == messageData.level()) setTextFill(Color.WHITE);
            else setTextFill(Color.LIGHTGRAY);

            setText(messageData.message());
        }
    }

    enum ConsoleLevel {
        OFF(Level.OFF, Integer.MAX_VALUE),
        SEVERE(Level.SEVERE, 1000),
        WARNING(Level.WARNING, 900),
        INFO(Level.INFO, 800),
        CONFIG(Level.CONFIG, 700),
        FINE(Level.FINE, 500),
        FINER(Level.FINER, 400),
        FINEST(Level.FINEST, 300),
        ALL(Level.ALL, Integer.MIN_VALUE);

        final Level level;
        final int value;

        ConsoleLevel(final Level level, final int value) {
            this.level = level;
            this.value = value;
        }
    }
}
