package fr.xahla.musicx.api.model.enums;

/** <b>(API) Role list for an artist.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public enum Role {

    ALL("Made everything"),

    ARRANGER("Adapts the song for different instruments or musical genres"),

    ARTISTIC_DIRECTOR("Responsible for the overall artistic vision of the music project, including song selection, arrangements and artists"),

    BACKING_VOCALIST("Provide vocal harmonies or background vocals"),

    BEAT_PROGRAMMER("Create electronic rhythms and loops used in the music"),

    CONDUCTOR("Leads an orchestra or musical ensemble during recording or live performance"),

    INTERPRETER("Sang or played an instrument in the song"),

    LYRICIST("Wrote the lyrics of the song"),

    MASTERING_ENGINEER("Optimize the final sound of the music for distribution"),

    MUSIC_EDITOR("Select, arrange and edit music tracks for specific projects"),

    MUSIC_PRODUCER("Individual responsible for overseeing the recording, production, and overall creative process of the music"),

    REMIXER("Modify or rearrange an existing song to create a new version"),

    SESSION_MUSICIAN("Records or perform live for other artists or projects without being permanent member of the group"),

    SONGWRITER("Wrote the song, including melodies, harmonies and musical structure"),

    SOUND_ENGINEER("Professional responsible for recording, mixing and mastering the music");

    private String description;

    Role(final String description) {
        this.description = description;
    }

    public String getDescription() {
        return description;
    }

}
