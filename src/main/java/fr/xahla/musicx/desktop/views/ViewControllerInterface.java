package fr.xahla.musicx.desktop.views;

public interface ViewControllerInterface {

    ViewControllerInterface getParent();
    void initialize(ViewControllerInterface parentController, ViewControllerProps props);

    void makeTranslations();

}
