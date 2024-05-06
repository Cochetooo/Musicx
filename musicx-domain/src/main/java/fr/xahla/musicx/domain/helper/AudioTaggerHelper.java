package fr.xahla.musicx.domain.helper;

import org.jaudiotagger.tag.Tag;
import org.jaudiotagger.tag.TagField;
import org.jaudiotagger.tag.id3.AbstractID3v2Tag;
import org.jaudiotagger.tag.mp4.Mp4Tag;

import java.util.ArrayList;
import java.util.List;

public final class AudioTaggerHelper {

    public static List<TagField> getCustomTags(final Tag tag) {
        final var customTags = new ArrayList<TagField>();

        switch (tag) {
            case AbstractID3v2Tag mp3Tag -> customTags.addAll(mp3Tag.getFrame("TXXX"));
            case Mp4Tag mp4Tag -> {
                if (mp4Tag.hasField("----:com.apple.iTunes:Primary Track Genres")) {
                    customTags.addAll(mp4Tag.getFields("----:com.apple.iTunes:Primary Track Genres"));
                }

                if (mp4Tag.hasField("----:com.apple.iTunes:Secondary Track Genres")) {
                    customTags.addAll(mp4Tag.getFields("----:com.apple.iTunes:Secondary Track Genres"));
                }
            }
            default -> {}
        }

        return customTags;
    }

}
