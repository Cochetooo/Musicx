package fr.xahla.musicx.domain.helper;

import fr.xahla.musicx.domain.model.enums.CustomFieldKey;
import org.jaudiotagger.tag.FieldKey;
import org.jaudiotagger.tag.Tag;
import org.jaudiotagger.tag.TagField;
import org.jaudiotagger.tag.id3.*;
import org.jaudiotagger.tag.id3.framebody.FrameBodyTXXX;
import org.jaudiotagger.tag.mp4.Mp4Tag;
import org.jaudiotagger.tag.mp4.field.Mp4TagReverseDnsField;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

public final class AudioTaggerHelper {

    public static List<TagField> getCustomTags(final Tag tag) {
        final var customTags = new ArrayList<TagField>();

        switch (tag) {
            case AbstractID3v2Tag mp3Tag
                -> customTags.addAll(mp3Tag.getFrame("TXXX"));

            case Mp4Tag mp4Tag -> {
                for (final var customField : CustomFieldKey.values()) {
                    if (mp4Tag.hasField("----:com.apple.iTunes:" + customField.getKey())) {
                        customTags.addAll(mp4Tag.getFields("----:com.apple.iTunes:" + customField.getKey()));
                    }
                }
            }

            default -> {}
        }

        return customTags;
    }

    /**
     * @param customTags
     * @param fieldKey
     * @return The value of the custom field key, or "" if not found
     */
    public static String getCustomTag(final List<TagField> customTags, final CustomFieldKey fieldKey) {
        try {
            for (final var tagField : customTags) {
                switch (tagField) {
                    case AbstractID3v2Frame mp3TagField -> {
                        if (mp3TagField.getBody().getObjectValue("Description").equals(fieldKey.getKey())) {
                            return mp3TagField.getBody().getObjectValue("Text").toString();
                        }
                    }
                    case Mp4TagReverseDnsField mp4TagField -> {
                        if (mp4TagField.getDescriptor().equalsIgnoreCase(fieldKey.getKey())) {
                            return mp4TagField.getContent();
                        }
                    }
                    default -> {}
                }
            }

            return "";
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "Error while getting custom tag " + fieldKey.getKey(), exception);
            return "";
        }
    }

    public static void writeCustomTag(final Tag tag, final String customKey, final String value) {
        if (null == value) {
            logger().finer("Null value for custom key " + customKey);
            return;
        }

        try {
            switch (tag) {
                case AbstractID3v2Tag mp3Tag -> writeMP3Tag(mp3Tag, customKey, value);
                default -> logger().warning("Could not set custom tag " + customKey + " because tag format is not supported.");
            }
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "Could not set custom tag " + customKey + " with value " + value, exception);
        }
    }

    private static void writeMP3Tag(final AbstractID3v2Tag mp3Tag, final String customKey, final String value) {
        try {
            final var txxxBody = new FrameBodyTXXX();
            txxxBody.setDescription(customKey);
            txxxBody.setText(value);

            AbstractID3v2Frame frame = null;

            if (mp3Tag instanceof ID3v23Tag) {
                frame = new ID3v23Frame("TXXX");
            } else if (mp3Tag instanceof ID3v24Tag) {
                frame = new ID3v24Frame("TXXX");
            } else {
                logger().warning("Could not set custom MP3 Tag " + customKey + " because tag format is not supported.");
                return;
            }

            frame.setBody(txxxBody);

            mp3Tag.addField(frame);
        } catch (final Exception exception) {
            logger().log(Level.SEVERE, "Could not set MP3 Tag " + customKey + " with value " + value, exception);
        }
    }

}
