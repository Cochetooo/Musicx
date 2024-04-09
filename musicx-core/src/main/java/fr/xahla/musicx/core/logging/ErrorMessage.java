package fr.xahla.musicx.core.logging;

import java.text.MessageFormat;

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
