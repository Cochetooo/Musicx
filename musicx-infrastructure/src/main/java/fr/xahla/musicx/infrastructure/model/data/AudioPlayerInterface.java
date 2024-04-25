package fr.xahla.musicx.infrastructure.model.data;

import fr.xahla.musicx.api.model.SongInterface;
import fr.xahla.musicx.infrastructure.model.data.enums.RepeatMode;
import fr.xahla.musicx.infrastructure.model.data.enums.ShuffleMode;

public interface AudioPlayerInterface {

    /* @var SongInterface playingSong */

    SongInterface getPlayingSong();
    AudioPlayerInterface setPlayingSong(final SongInterface song);

    /* @var RepeatMode repeatMode */

    RepeatMode getRepeatMode();
    AudioPlayerInterface setRepeatMode(final RepeatMode repeatMode);

    /* @var ShuffleMode shuffleMode */

    ShuffleMode getShuffleMode();
    AudioPlayerInterface setShuffleMode(final ShuffleMode shuffleMode);

    /* @var QueueInterface queue */

    QueueInterface getQueue();
    AudioPlayerInterface setQueue(final QueueInterface queue);

}
