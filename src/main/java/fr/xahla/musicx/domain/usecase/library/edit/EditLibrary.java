package fr.xahla.musicx.domain.usecase.library.edit;

import fr.xahla.musicx.api.data.LibraryInterface;
import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;

public class EditLibrary {

    private final LibraryRepositoryInterface libraryManager;

    public EditLibrary(final LibraryRepositoryInterface libraryManager) {
        this.libraryManager = libraryManager;
    }

    public void execute(LibraryInterface library) {
        this.libraryManager.save(library);
    }

}
