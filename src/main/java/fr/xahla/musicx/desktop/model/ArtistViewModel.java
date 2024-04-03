package fr.xahla.musicx.desktop.model;

import fr.xahla.musicx.api.data.ArtistInterface;
import fr.xahla.musicx.infrastructure.presentation.ViewModelInterface;
import javafx.beans.property.LongProperty;
import javafx.beans.property.SimpleLongProperty;
import javafx.beans.property.SimpleStringProperty;
import javafx.beans.property.StringProperty;

public class ArtistViewModel implements ViewModelInterface, ArtistInterface {

    private LongProperty id;
    private StringProperty name;

    public ArtistViewModel() {
        this.id = new SimpleLongProperty();
        this.name = new SimpleStringProperty();
    }

    public ArtistViewModel set(ArtistInterface artist) {
        if (null != artist.getId()) {
            this.setId(artist.getId());
        }

        if (null != artist.getName()) {
            this.setName(artist.getName());
        }

        return this;
    }

    @Override
    public Long getId() {
        return id.get();
    }

    public LongProperty idProperty() {
        return id;
    }

    public ArtistViewModel setId(Long id) {
        this.id.set(id);
        return this;
    }

    @Override
    public String getName() {
        return name.get();
    }

    public StringProperty nameProperty() {
        return name;
    }

    public ArtistViewModel setName(String name) {
        this.name.set(name);
        return this;
    }


}
