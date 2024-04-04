package fr.xahla.musicx.desktop.manager;

import fr.xahla.musicx.api.data.LibraryInterface;
import fr.xahla.musicx.desktop.model.LibraryViewModel;
import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;
import fr.xahla.musicx.infrastructure.persistence.repository.LibraryRepository;

public class LibraryViewManager {

    private LibraryRepository libraryRepository;
    private LibraryViewModel library;

    public LibraryViewManager(
        final LibraryRepository libraryRepository,
        final LibraryViewModel library
    ) {
        this.libraryRepository = libraryRepository;
        this.library = library;
    }

    public LibraryViewModel getLibrary() {
        return this.library;
    }

    public void findOneByName(String name) {
        this.library.set(libraryRepository.findOneByName(name));
    }

    public void save() {
        this.libraryRepository.save(library);
    }

}
