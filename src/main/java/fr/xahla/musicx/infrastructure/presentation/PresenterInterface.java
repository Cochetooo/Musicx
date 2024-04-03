package fr.xahla.musicx.infrastructure.presentation;

import fr.xahla.musicx.domain.ResponseInterface;

public interface PresenterInterface {
    void present(ResponseInterface responseInterface);
    ViewModelInterface getViewModel();
}
