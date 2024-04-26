package fr.xahla.musicx.domain.model;

import fr.xahla.musicx.api.model.SongInterface;

import java.util.List;

public interface AudioQueueInterface {

    /* @var List<SongInterface> songs */

    List<SongInterface> getSongs();
    AudioQueueInterface setSongs(final List<SongInterface> songs);

    /* @var Integer position */

    Integer getPosition();
    AudioQueueInterface setPosition(final Integer position);

}
