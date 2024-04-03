package fr.xahla.musicx.desktop.views.content.trackList.dialog;

import fr.xahla.musicx.config.internationalization.Translator;
import javafx.stage.DirectoryChooser;

public class ScanLibraryFolderChooserViewController {

    private final DirectoryChooser directoryChooser;

    public ScanLibraryFolderChooserViewController() {
        this.directoryChooser = new DirectoryChooser();

        this.makeTranslations();
    }

    /**
     * Prompt a folder chooser dialog to the user and returns the folder path.
     */
    public String promptLibraryFolderChooser() {
        var selectedDirectory = this.directoryChooser.showDialog(null);

        if (null == selectedDirectory) {
            return null;
        }

        return selectedDirectory.getAbsolutePath();
    }

    private void makeTranslations() {
        var translator = new Translator("library");

        this.directoryChooser.setTitle(
            translator.translate("import.folderChooserTitle")
        );
    }

}
