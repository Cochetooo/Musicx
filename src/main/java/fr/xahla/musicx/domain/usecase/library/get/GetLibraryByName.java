package fr.xahla.musicx.domain.usecase.library.get;

import fr.xahla.musicx.domain.repository.LibraryRepositoryInterface;

public class GetLibraryByName {

    public record Request(String name) { }

    private final LibraryRepositoryInterface libraryManager;

    public GetLibraryByName(
        final LibraryRepositoryInterface libraryManager
    ) {
        this.libraryManager = libraryManager;
    }

    public GetLibraryByNameResponse execute(GetLibraryByName.Request request) {
        var library = this.libraryManager.findOneByName(request.name());

        return new GetLibraryByNameResponse()
            .setLibrary(library);
    }

}
