package fr.xahla.musicx.desktop.views.menubar.submenu.file;

import fr.xahla.musicx.desktop.model.LibraryViewModel;
import fr.xahla.musicx.desktop.views.ViewControllerInterface;
import fr.xahla.musicx.Musicx;
import fr.xahla.musicx.config.internationalization.Translator;
import fr.xahla.musicx.desktop.views.ViewControllerProps;
import fr.xahla.musicx.desktop.views.content.trackList.dialog.ScanLibraryFolderChooserViewController;
import fr.xahla.musicx.desktop.views.menubar.MenuBarViewController;
import fr.xahla.musicx.domain.service.library.ScanFolderAsLibraryService;
import fr.xahla.musicx.infrastructure.persistence.repository.LibraryRepository;
import javafx.application.Platform;
import javafx.beans.property.SimpleStringProperty;
import javafx.concurrent.Task;
import javafx.event.ActionEvent;
import javafx.fxml.FXML;
import javafx.scene.control.*;
import javafx.scene.input.KeyCode;
import javafx.scene.input.KeyCodeCombination;
import javafx.scene.input.KeyCombination;

import java.util.logging.Logger;

public class FileMenuViewController implements ViewControllerInterface {
    public record Props(
        Logger logger,
        LibraryViewModel library
    ) implements ViewControllerProps {}

    @FXML private Menu fileMenu;
    @FXML public MenuItem importFromFolder;

    @FXML private MenuItem scanFolderForNewFileItem;
    @FXML private MenuItem showConsole;

    private Translator translator;

    private MenuBarViewController parent;
    private FileMenuViewController.Props props;

    @Override
    public void initialize(ViewControllerInterface viewController, ViewControllerProps viewControllerProps) {
        this.parent = (MenuBarViewController) viewController;
        this.props = (FileMenuViewController.Props) viewControllerProps;

        this.importFromFolder.setAccelerator(
            new KeyCodeCombination(KeyCode.O, KeyCombination.CONTROL_DOWN, KeyCombination.SHIFT_DOWN)
        );

        this.showConsole.setAccelerator(
            new KeyCodeCombination(KeyCode.L, KeyCombination.ALT_DOWN)
        );

        this.translator = new Translator("menubar");
        this.makeTranslations();
    }

    @Override public void makeTranslations() {
        this.makeMenuTranslations(fileMenu);
    }

    private void makeMenuTranslations(MenuItem menuItem) {
        menuItem.textProperty().bind(
            new SimpleStringProperty(translator.translate(menuItem.getText()))
        );

        if (menuItem instanceof Menu menu) {
            for (var subItem : menu.getItems()) {
                this.makeMenuTranslations(subItem);
            }
        }
    }

    public void importFromFolder(ActionEvent actionEvent) {
        var folderPath = new ScanLibraryFolderChooserViewController().promptLibraryFolderChooser();

        if (null != folderPath) {
            this.props.library().setFolderPaths(folderPath);
        }

        this.scanFolderForNewFile(null);
    }

    public void scanFolderForNewFile(ActionEvent actionEvent) {
        var scanFolderTask = new Task<>() {
            @Override public Void call() {
                var scanLibraryFolder = new ScanFolderAsLibraryService(
                    props.logger(),
                    new LibraryRepository(Musicx.getInstance().getApp().getSessionFactory()),
                    this::updateProgress
                );

                var songs = scanLibraryFolder.execute(
                    new ScanFolderAsLibraryService.Request(props.library)
                );

                props.library().setSongs(songs);

                parent.getParent().getTrackListViewController().saveLibrary();

                Platform.runLater(() -> {
                    scanFolderForNewFileItem.setDisable(false);
                });

                return null;
            }
        };

        var footerView = this.parent.getParent().getAppInfoViewController();

        footerView.invokeProgressBar(scanFolderTask,
            this.translator.translate("file.library.scanFolderDialog.label"));

        scanFolderTask.progressProperty().addListener(((observableValue, oldValue, newValue) -> {
            if (newValue.doubleValue() >= 1.0) {
                footerView.revokeProgressBar();
            }
        }));
    }

    public void showConsole() {
        this.parent.getParent().getConsoleViewController().show();
    }

    public void exit() {
        Musicx.getInstance().getApp().dispose();
        Platform.exit();
    }

    @Override
    public MenuBarViewController getParent() {
        return this.parent;
    }
}
