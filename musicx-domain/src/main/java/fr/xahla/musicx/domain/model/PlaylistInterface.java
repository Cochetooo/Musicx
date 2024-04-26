package fr.xahla.musicx.domain.model;

import fr.xahla.musicx.api.model.SongInterface;

import java.util.List;

public interface PlaylistInterface extends PlaylistNodeInterface {

    /* @var List<SongInterface> songs */

    List<SongInterface> getSongs();
    PlaylistInterface setSongs(final List<SongInterface> songs);

    /* @var String description */

    String getDescription();
    PlaylistInterface setDescription(final String description);

}
