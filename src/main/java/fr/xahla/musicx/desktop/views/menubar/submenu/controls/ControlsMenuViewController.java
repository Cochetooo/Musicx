package fr.xahla.musicx.desktop.views.menubar.submenu.controls;

import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.config.internationalization.Translator;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.desktop.views.menubar.MenuBarViewController;
import javafx.beans.property.SimpleStringProperty;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.Menu;
import javafx.scene.control.MenuItem;
import javafx.scene.input.KeyCode;
import javafx.scene.input.KeyCodeCombination;
import javafx.scene.input.KeyCombination;

import java.net.URL;
import java.util.ResourceBundle;

public class ControlsMenuViewController implements ViewControllerInterface {

    @FXML public Menu controlsMenu;
    @FXML public MenuItem playPause;

    private Translator translator;

    private MenuBarViewController parent;

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {
        this.parent = (MenuBarViewController) viewController;

        this.translator = new Translator("menubar");
        this.makeTranslations();

        this.playPause.setAccelerator(new KeyCodeCombination(KeyCode.P, KeyCombination.CONTROL_DOWN));
    }

    public void makeTranslations() {
        this.makeMenuTranslations(this.controlsMenu);
    }

    private void makeMenuTranslations(MenuItem menuItem) {
        menuItem.setText(translator.translate(menuItem.getText()));

        if (menuItem instanceof Menu menu) {
            for (var subItem : menu.getItems()) {
                this.makeMenuTranslations(subItem);
            }
        }
    }

    public void playPause(ActionEvent actionEvent) {
        this.getParent().getParent().getAudioPlayerViewController().getTrackControlsViewController().togglePlaying();
    }

    @Override
    public MenuBarViewController getParent() {
        return this.parent;
    }
}
