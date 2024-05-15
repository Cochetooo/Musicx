package fr.xahla.musicx.api.repository.searchCriterias;

import lombok.Getter;

/**
 * Available criteria for filtering artist search in database.
 * @author Cochetooo
 */
@Getter
public enum ArtistSearchCriteria {

    COUNTRY("country"),
    ID("id"),
    NAME("name"),
    TYPE("artistType");

    private final String column;

    ArtistSearchCriteria(final String column) {
        this.column = column;
    }

}
