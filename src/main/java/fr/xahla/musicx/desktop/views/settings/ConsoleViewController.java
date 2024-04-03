package fr.xahla.musicx.desktop.views.settings;

import fr.xahla.musicx.Musicx;
import fr.xahla.musicx.config.internationalization.Translator;
import javafx.beans.property.SimpleStringProperty;
import javafx.scene.Scene;
import javafx.scene.control.Label;
import javafx.scene.control.TextArea;
import javafx.scene.layout.VBox;
import javafx.stage.Modality;
import javafx.stage.Stage;

public class ConsoleViewController {

    private final Stage consoleStage;
    private final Scene consoleScene;

    private final VBox contentBox;
    private final Label consoleLabel;
    private final TextArea logText;

    public ConsoleViewController() {
        var translator = new Translator("console");

        var app = Musicx.getInstance().getApp();

        this.consoleStage = new Stage();
        this.consoleStage.initModality(Modality.APPLICATION_MODAL);
        this.consoleStage.initOwner(app.getStage());
        this.consoleStage.setTitle(translator.translate("windowTitle"));

        this.contentBox = new VBox();
        this.contentBox.setPrefWidth(800);
        this.contentBox.setPrefHeight(800);

        this.consoleLabel = new Label();
        this.consoleLabel.setText(translator.translate("windowLabel"));

        this.logText = new TextArea();
        this.logText.setEditable(false);
        this.logText.setPrefRowCount(20);
        this.logText.setPrefColumnCount(80);
        this.logText.textProperty().bind(new SimpleStringProperty("Lol"));

        this.contentBox.getChildren().add(this.consoleLabel);
        this.contentBox.getChildren().add(this.logText);

        this.consoleScene = new Scene(contentBox, 840, 840);
        this.consoleStage.setScene(this.consoleScene);
    }

    public void show() {
        this.consoleStage.show();
    }

}
