package fr.xahla.musicx.domain.helper;

import fr.xahla.musicx.domain.logging.LogMessage;
import fr.xahla.musicx.domain.model.enums.CustomFieldKey;
import org.jaudiotagger.tag.Tag;
import org.jaudiotagger.tag.TagField;
import org.jaudiotagger.tag.id3.*;
import org.jaudiotagger.tag.id3.framebody.FrameBodyTXXX;
import org.jaudiotagger.tag.mp4.Mp4Tag;
import org.jaudiotagger.tag.mp4.field.Mp4TagReverseDnsField;

import java.util.ArrayList;
import java.util.List;
import java.util.logging.Level;

import static fr.xahla.musicx.domain.application.AbstractContext.*;

/**
 * Utility class for JAudioTagger
 * @author Cochetooo
 * @since 0.3.1
 */
public final class AudioTaggerHelper {

    /**
     * @return The list of custom tags used in the application, otherwise an empty list if the format is not supported
     * @since 0.3.1
     */
    public static List<TagField> audiotagger_get_custom_tags(final Tag tag) {
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

            default -> log(LogMessage.WARNING_AUDIO_TAGGER_CUSTOM_TAG_FORMAT_NOT_SUPPORTED, "*");
        }

        return customTags;
    }

    /**
     * @return The value of the custom field key, or "" if not found
     * @since 0.3.1
     */
    public static String audiotagger_get_custom_tag(final List<TagField> customTags, final CustomFieldKey fieldKey) {
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
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_CUSTOM_TAG_FETCH, fieldKey.getKey());

            return "";
        }
    }

    /**
     * @since 0.3.1
     */
    public static void audiotagger_write_custom_tag(final Tag tag, final String customKey, final String value) {
        if (null == value) {
            log(LogMessage.FINER_AUDIO_TAGGER_NULL_CUSTOM_KEY, customKey);
            return;
        }

        try {
            switch (tag) {
                case AbstractID3v2Tag mp3Tag -> writeMP3Tag(mp3Tag, customKey, value);
                default -> log(LogMessage.WARNING_AUDIO_TAGGER_CUSTOM_TAG_FORMAT_NOT_SUPPORTED, customKey);
            }
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_CUSTOM_TAG_WRITE, customKey, value);
        }
    }

    private static void writeMP3Tag(final AbstractID3v2Tag mp3Tag, final String customKey, final String value) {
        try {
            final var txxxBody = new FrameBodyTXXX();
            txxxBody.setDescription(customKey);
            txxxBody.setText(value);

            AbstractID3v2Frame frame;

            if (mp3Tag instanceof ID3v23Tag) {
                frame = new ID3v23Frame("TXXX");
            } else if (mp3Tag instanceof ID3v24Tag) {
                frame = new ID3v24Frame("TXXX");
            } else {
                log(LogMessage.WARNING_AUDIO_TAGGER_CUSTOM_TAG_FORMAT_NOT_SUPPORTED, "(MP3) " + customKey);
                return;
            }

            frame.setBody(txxxBody);

            mp3Tag.setField(frame);
        } catch (final Exception exception) {
            error(exception, LogMessage.ERROR_AUDIO_TAGGER_CUSTOM_TAG_WRITE, "(MP3)" + customKey, value);
        }
    }

}
