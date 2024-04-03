package fr.xahla.musicx.infrastructure.action;

import fr.xahla.musicx.infrastructure.presentation.ViewModelInterface;

public interface ActionInterface {

    /**
     * Execute action and get the response as a view model.
     */
    ViewModelInterface invoke(ActionRequestInterface request);

    /**
     * Validate input data to avoid errors.
     */
    boolean validateRequest(ActionRequestInterface request);

}
