package fr.xahla.musicx.infrastructure.action.library;

import fr.xahla.musicx.Musicx;
import fr.xahla.musicx.infrastructure.action.ActionInterface;
import fr.xahla.musicx.infrastructure.action.ActionRequestInterface;
import fr.xahla.musicx.domain.usecase.library.get.GetLibraryByName;
import fr.xahla.musicx.infrastructure.presentation.PresenterInterface;
import fr.xahla.musicx.infrastructure.persistence.repository.LibraryRepository;
import fr.xahla.musicx.infrastructure.presentation.ViewModelInterface;

public class ShowLibrary implements ActionInterface {

    public record Request(
        String name
    ) implements ActionRequestInterface {}

    private final GetLibraryByName useCase;
    private final PresenterInterface presenter;

    public ShowLibrary(PresenterInterface presenter) {
        var sessionFactory = Musicx.getInstance().getApp().getSessionFactory();

        this.useCase = new GetLibraryByName(
            new LibraryRepository(sessionFactory)
        );

        this.presenter = presenter;
    }

    @Override public ViewModelInterface invoke(ActionRequestInterface requestInterface) {
        var request = (ShowLibrary.Request) requestInterface;

        this.validateRequest(request);

        var response = this.useCase.execute(
            new GetLibraryByName.Request(request.name())
        );

        this.presenter.present(response);

        return this.presenter.getViewModel();
    }

    @Override public boolean validateRequest(ActionRequestInterface request) {
        return true;
    }
}
