package fr.xahla.musicx.desktop.views;

import fr.xahla.musicx.Musicx;
import fr.xahla.musicx.desktop.manager.LibraryViewManager;
import fr.xahla.musicx.desktop.manager.PlayerViewManager;
import fr.xahla.musicx.desktop.model.LibraryViewModel;
import fr.xahla.musicx.desktop.model.QueueViewModel;
import fr.xahla.musicx.desktop.views.content.topbar.TopBarViewController;
import fr.xahla.musicx.desktop.views.content.trackList.TrackListViewController;
import fr.xahla.musicx.desktop.views.footer.appInfo.AppInfoViewController;
import fr.xahla.musicx.desktop.views.menubar.MenuBarViewController;
import fr.xahla.musicx.desktop.views.footer.audioPlayer.AudioPlayerViewController;
import fr.xahla.musicx.desktop.views.settings.ConsoleViewController;
import fr.xahla.musicx.infrastructure.persistence.repository.LibraryRepository;
import javafx.fxml.FXML;
import javafx.fxml.Initializable;

import java.net.URL;
import java.util.ResourceBundle;
import java.util.logging.Logger;

public class MusicxViewController implements ViewControllerInterface, Initializable {

    @FXML private TrackListViewController trackListViewController;
    @FXML private MenuBarViewController menuBarViewController;
    @FXML private AudioPlayerViewController audioPlayerViewController;
    @FXML private AppInfoViewController appInfoViewController;
    @FXML private TopBarViewController topBarViewController;

    private ConsoleViewController consoleViewController;

    private LibraryViewManager libraryManager;
    private PlayerViewManager playerManager;

    private Logger logger;

    @Override public void initialize(URL url, ResourceBundle resourceBundle) {
        this.libraryManager = new LibraryViewManager(
            new LibraryRepository(
                Musicx.getInstance().getApp().getSessionFactory()
            ),
            new LibraryViewModel()
        );

        this.libraryManager.findOneByName("MyLibrary");

        this.playerManager = new PlayerViewManager(
            new QueueViewModel(),
            this.audioPlayerViewController.getTrackControlsViewController()
        );

        this.logger = Musicx.getInstance().getApp().getLogger();

        this.topBarViewController.initialize(this, null);

        this.trackListViewController.initialize(this,
            new TrackListViewController.Props(
                this.libraryManager,
                this.playerManager
            ));

        this.appInfoViewController.initialize(this,
            new AppInfoViewController.Props(
                this.libraryManager
            ));

        this.menuBarViewController.initialize(this,
            new MenuBarViewController.Props(
                this.logger,
                this.libraryManager
            ));

        this.audioPlayerViewController.initialize(this,
            new AudioPlayerViewController.Props(
                this.playerManager
            ));

        this.consoleViewController = new ConsoleViewController();
    }

    public TrackListViewController getTrackListViewController() {
        return this.trackListViewController;
    }

    public MenuBarViewController getMenuBarViewController() {
        return this.menuBarViewController;
    }

    public AudioPlayerViewController getAudioPlayerViewController() {
        return this.audioPlayerViewController;
    }

    public AppInfoViewController getAppInfoViewController() {
        return this.appInfoViewController;
    }

    public ConsoleViewController getConsoleViewController() {
        return this.consoleViewController;
    }

    @Override public ViewControllerInterface getParent() {
        return null;
    }

    @Override public void initialize(ViewControllerInterface viewController, ViewControllerProps props) {

    }

    @Override public void makeTranslations() {

    }
}
