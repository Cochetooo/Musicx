package fr.xahla.musicx.desktop.presentation.library;

import fr.xahla.musicx.domain.ResponseInterface;
import fr.xahla.musicx.domain.usecase.library.get.GetLibraryByNameResponse;
import fr.xahla.musicx.desktop.model.LibraryViewModel;
import fr.xahla.musicx.infrastructure.presentation.PresenterInterface;
import javafx.collections.FXCollections;

import java.util.ArrayList;

public class ShowLibraryPresenter implements PresenterInterface {

    private LibraryViewModel viewModel;

    @Override public void present(ResponseInterface responseInterface) {
        this.viewModel = new LibraryViewModel();
        var response = (GetLibraryByNameResponse) responseInterface;

        if (null != response.getLibrary()) {
            this.viewModel.set(response.getLibrary());
        } else {
            this.viewModel
                .setSongs(FXCollections.observableList(new ArrayList<>()))
                .setName("MyLibrary");
        }
    }

    @Override public LibraryViewModel getViewModel() {
        return this.viewModel;
    }

}
