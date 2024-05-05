package fr.xahla.musicx.api.model.data;

import fr.xahla.musicx.api.model.PersonArtistDto;

import java.time.LocalDate;
import java.util.List;

public interface PersonArtistInterface extends ArtistInterface {

    /* --- First Name --- */

    String getFirstName();
    PersonArtistInterface setFirstName(final String firstName);

    /* --- Birth Date --- */

    LocalDate getBirthDate();
    PersonArtistInterface setBirthDate(final LocalDate birthDate);

    /* --- Death Date --- */

    LocalDate getDeathDate();
    PersonArtistInterface setDeathDate(final LocalDate deathDate);

    /* -- Bands --- */

    List<? extends BandArtistInterface> getBands();
    PersonArtistInterface setBands(final List<? extends BandArtistInterface> bands);

}
