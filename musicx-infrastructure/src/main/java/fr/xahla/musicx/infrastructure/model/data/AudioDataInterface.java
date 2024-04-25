package fr.xahla.musicx.infrastructure.model.data;

import java.util.List;

/** <b>Interface defining the abstract raw metadata of an audio file.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public interface AudioDataInterface {

    /**
     * Should be set to true if an error has occured while creating the local song.
     */
    boolean hasFailed();

    String getAlbumName();
    String getAlbumArtist();
    Integer getAlbumYear();
    String getArtistCountry();
    String getArtistName();
    Integer getBitRate();
    Short getBpm();
    String getCatalogNo();
    String getCoverArt();
    Short getDiscNumber();
    Short getDiscTotal();
    Integer getDuration();
    String getFormat();
    List<String> getGenresPrimary();
    List<String> getGenresSecondary();
    String getLanguage();
    String getLyrics();
    Short getRating();
    String getRecordLabel();
    Integer getSampleRate();
    String getTitle();
    Short getTrackNumber();
    Short getTrackTotal();
    Integer getYear();

}
