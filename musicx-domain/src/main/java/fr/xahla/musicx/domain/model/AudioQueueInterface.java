package fr.xahla.musicx.domain.model;

import fr.xahla.musicx.api.model.SongDto;

import java.util.List;

public interface AudioQueueInterface {

    /* @var List<SongInterface> songs */

    List<SongDto> getSongs();
    AudioQueueInterface setSongs(final List<SongDto> songs);

    /* @var Integer position */

    Integer getPosition();
    AudioQueueInterface setPosition(final Integer position);

}
