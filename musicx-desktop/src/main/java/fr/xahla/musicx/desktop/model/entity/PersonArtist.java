package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.PersonArtistDto;
import javafx.beans.property.*;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.time.LocalDate;
import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.artistRepository;

/**
 * Defines the behaviour of a person artist for a JavaFX context.
 * @author Cochetooo
 * @since 0.3.2
 */
public class PersonArtist extends Artist {

    private final StringProperty firstName;
    private final ObjectProperty<LocalDate> birthDate;
    private final ObjectProperty<LocalDate> deathDate;

    private ListProperty<BandArtist> bands;

    public PersonArtist(final PersonArtistDto artist) {
        super(artist);

        this.firstName = new SimpleStringProperty(artist.getFirstName());
        this.birthDate = new SimpleObjectProperty<>(artist.getBirthDate());
        this.deathDate = new SimpleObjectProperty<>(artist.getDeathDate());

        this.getDto().setFirstName(this.getFirstName());
        this.getDto().setBirthDate(this.getBirthDate());
        this.getDto().setDeathDate(this.getDeathDate());
    }

    /**
     * @since 0.3.0
     */
    public PersonArtistDto getDto() {
        return (PersonArtistDto) dto;
    }

    // Getters - Setters

    public String getFirstName() {
        return firstName.get();
    }

    public StringProperty firstNameProperty() {
        return firstName;
    }

    public PersonArtist setFirstName(final String firstName) {
        this.getDto().setFirstName(firstName);
        this.firstName.set(firstName);
        return this;
    }

    public LocalDate getBirthDate() {
        return birthDate.get();
    }

    public ObjectProperty<LocalDate> birthDateProperty() {
        return birthDate;
    }

    public PersonArtist setBirthDate(final LocalDate birthDate) {
        this.getDto().setBirthDate(birthDate);
        this.birthDate.set(birthDate);
        return this;
    }

    public LocalDate getDeathDate() {
        return deathDate.get();
    }

    public ObjectProperty<LocalDate> deathDateProperty() {
        return deathDate;
    }

    public PersonArtist setDeathDate(final LocalDate deathDate) {
        this.getDto().setDeathDate(deathDate);
        this.deathDate.set(deathDate);
        return this;
    }

    public ObservableList<BandArtist> getBands() {
        if (null == bands) {
            bands = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var bandsDto = artistRepository().getBands(this.getDto());
            bandsDto.forEach(band -> bands.add(new BandArtist(band)));
        }

        return bands.get();
    }

    public ListProperty<BandArtist> bandsProperty() {
        return bands;
    }

    public PersonArtist setBands(final List<BandArtist> bands) {
        if (null == this.bands) {
            this.bands = new SimpleListProperty<>(FXCollections.observableList(bands));
        } else {
            this.bands.setAll(bands);
        }

        return this;
    }
}
