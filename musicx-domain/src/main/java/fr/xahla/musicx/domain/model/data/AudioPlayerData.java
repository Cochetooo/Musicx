package fr.xahla.musicx.domain.model.data;

import fr.xahla.musicx.domain.model.enums.RepeatMode;
import fr.xahla.musicx.domain.model.enums.ShuffleMode;

public record AudioPlayerData(
    RepeatMode repeatMode,
    ShuffleMode shuffleMode
) {}
