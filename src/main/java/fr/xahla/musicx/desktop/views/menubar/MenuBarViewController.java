package fr.xahla.musicx.desktop.views.menubar;

import fr.xahla.musicx.desktop.model.LibraryViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.desktop.views.MusicxViewController;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.desktop.views.menubar.submenu.controls.ControlsMenuViewController;
import fr.xahla.musicx.desktop.views.menubar.submenu.file.FileMenuViewController;
import javafx.fxml.FXML;

import java.net.URL;
import java.util.ResourceBundle;
import java.util.logging.Logger;

public class MenuBarViewController implements ViewControllerInterface {
    public record Props(
        Logger logger,
        LibraryViewModel library
    ) implements ViewControllerProps {}

    @FXML private FileMenuViewController fileMenuViewController;
    @FXML private ControlsMenuViewController controlsMenuViewController;

    private MusicxViewController parent;
    private MenuBarViewController.Props props;

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {
        this.parent = (MusicxViewController) viewController;
        this.props = (MenuBarViewController.Props) props;

        this.fileMenuViewController.initialize(this,
            new FileMenuViewController.Props(
                this.props.logger(),
                this.props.library()
            ));
        this.controlsMenuViewController.initialize(this, null);
    }

    public FileMenuViewController getFileMenuBarViewController() {
        return this.fileMenuViewController;
    }

    @Override
    public MusicxViewController getParent() {
        return this.parent;
    }

    @Override public void makeTranslations() {

    }
}
