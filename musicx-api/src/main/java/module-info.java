/**
 * Module management for API layer.
 * @author Cochetooo
 * @since 0.1.0
 */
module fr.xahla.musicx.api {

    requires lombok;

    exports fr.xahla.musicx.api.model;
    exports fr.xahla.musicx.api.model.enums;
    exports fr.xahla.musicx.api.repository;
    exports fr.xahla.musicx.api.repository.searchCriterias;

}