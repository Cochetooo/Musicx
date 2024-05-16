package fr.xahla.musicx.desktop.model.entity;

import fr.xahla.musicx.api.model.BandArtistDto;
import javafx.beans.property.ListProperty;
import javafx.beans.property.SimpleListProperty;
import javafx.collections.FXCollections;
import javafx.collections.ObservableList;

import java.util.ArrayList;
import java.util.List;

import static fr.xahla.musicx.domain.application.AbstractContext.artistRepository;

/**
 * Defines the behaviour of a band artist for a JavaFX context.
 * @author Cochetooo
 * @since 0.3.0
 */
public class BandArtist extends Artist {

    private ListProperty<PersonArtist> members;

    public BandArtist(final BandArtistDto artist) {
        super(artist);
    }

    /**
     * @since 0.3.0
     */
    public BandArtistDto getDto() {
        return (BandArtistDto) dto;
    }

    public ObservableList<PersonArtist> getMembers() {
        if (null == members) {
            members = new SimpleListProperty<>(FXCollections.observableList(new ArrayList<>()));

            final var membersDto = artistRepository().getMembers(this.getDto());
            membersDto.forEach(member -> members.add(new PersonArtist(member)));
        }

        return members.get();
    }

    public ListProperty<PersonArtist> membersProperty() {
        return members;
    }

    public BandArtist setMembers(final List<PersonArtist> members) {
        if (null == this.members) {
            this.members = new SimpleListProperty<>(FXCollections.observableList(members));
        } else {
            this.members.setAll(members);
        }

        return this;
    }
}
