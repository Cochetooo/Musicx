package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.AudioFormat;
import lombok.Builder;
import lombok.Data;

import java.util.List;
import java.util.Map;

/** <b>(API) Interface for Song Model contracts.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
@Data
@Builder
public class SongDto {

    private Long id;

    private Long albumId;
    private Long artistId;
    private List<Long> primaryGenreIds;
    private List<Long> secondaryGenreIds;

    private int bitRate;
    private short discNumber;
    private long duration;
    private AudioFormat format;
    private Map<Long, String> lyrics;
    private int sampleRate;
    private String title;
    private short trackNumber;

    public void setLyrics(final String lyrics) {
        this.lyrics = Map.of(0L, lyrics);
    }

}
