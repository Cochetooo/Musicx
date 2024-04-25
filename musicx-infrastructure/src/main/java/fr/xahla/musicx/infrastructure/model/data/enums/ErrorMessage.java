package fr.xahla.musicx.infrastructure.model.data.enums;

import java.text.MessageFormat;

/** <b>Enum containing common error messages.</b>
 * <p>
 * Copyright (C) Xahla - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Alexis Cochet <alexiscochet.pro@gmail.com>, April 2024
 * </p>
 *
 * @author Cochetooo
 */
public enum ErrorMessage {

    LOCAL_SONG_HAS_FAILED("Local Song {0} has failed while importing.");

    String msg;

    private ErrorMessage(String msg) {
        this.msg = msg;
    }

    public String getMsg() {
        return this.msg;
    }

    public String getMsg(Object... args) {
        return MessageFormat.format(this.getMsg(), args);
    }

}
