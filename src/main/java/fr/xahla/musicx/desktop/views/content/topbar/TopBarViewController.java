package fr.xahla.musicx.desktop.views.content.topbar;

import atlantafx.base.controls.CustomTextField;
import fr.xahla.musicx.config.internationalization.Translator;
import fr.xahla.musicx.desktop.views.MusicxViewController;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import javafx.fxml.FXML;
import javafx.scene.input.InputMethodEvent;

import java.net.URL;
import java.util.ResourceBundle;

public class TopBarViewController implements ViewControllerInterface {

    /* --- FXML Components --- */
    @FXML CustomTextField searchTextField;

    /* ----------------------- */

    private MusicxViewController parent;

    @Override public ViewControllerInterface getParent() {
        return this.parent;
    }

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {
        this.parent = (MusicxViewController) viewController;

        this.makeTranslations();
    }

    @Override public void makeTranslations() {
        var translator = new Translator("topbar");

        this.searchTextField.setPromptText(
            translator.translate(this.searchTextField.getPromptText())
        );
    }

    /* --- Events --- */

    @FXML public void onInput(InputMethodEvent event) {

    }
}
