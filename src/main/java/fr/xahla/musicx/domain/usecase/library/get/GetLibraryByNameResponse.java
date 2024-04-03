package fr.xahla.musicx.domain.usecase.library.get;

import fr.xahla.musicx.domain.ResponseInterface;
import fr.xahla.musicx.api.data.LibraryInterface;

public class GetLibraryByNameResponse implements ResponseInterface {

    private LibraryInterface library;

    public LibraryInterface getLibrary() {
        return library;
    }

    public GetLibraryByNameResponse setLibrary(LibraryInterface library) {
        this.library = library;
        return this;
    }

}
