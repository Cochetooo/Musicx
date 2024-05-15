package fr.xahla.musicx.api.repository.searchCriterias;

import lombok.Getter;

/**
 * Available criteria for filtering album search in database.
 * @author Cochetooo
 */
@Getter
public enum AlbumSearchCriteria {

    ARTIST("artistId"),
    CATALOG_NO("catalogNo"),
    ID("id"),
    LABEL("labelId"),
    NAME("name"),
    RELEASE_DATE("releaseDate");

    private final String column;

    AlbumSearchCriteria(final String column) {
        this.column = column;
    }

}
