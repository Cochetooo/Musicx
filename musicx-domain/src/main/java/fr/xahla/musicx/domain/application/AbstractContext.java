package fr.xahla.musicx.domain.application;

import fr.xahla.musicx.domain.manager.AudioPlayerManagerInterface;
import fr.xahla.musicx.domain.manager.LibraryManagerInterface;

public abstract class AbstractContext {

    protected final AbstractLogger logger;
    protected final SettingsInterface settings;

    protected final AudioPlayerManagerInterface audioPlayer;
    protected final LibraryManagerInterface library;

    protected AbstractContext(
        final AbstractLogger logger,
        final SettingsInterface settings,
        final AudioPlayerManagerInterface audioPlayer,
        final LibraryManagerInterface library
    ) {
        this.logger = logger;
        this.settings = settings;
        this.audioPlayer = audioPlayer;
        this.library = library;
    }

}
