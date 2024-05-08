package fr.xahla.musicx.domain.model.data;

import fr.xahla.musicx.api.model.SongDto;

import java.util.List;

public interface PlaylistInterface extends PlaylistNodeInterface {

    /* @var List<SongInterface> songs */

    List<SongDto> getSongs();
    PlaylistInterface setSongs(final List<SongDto> songs);

    /* @var String description */

    String getDescription();
    PlaylistInterface setDescription(final String description);

}
