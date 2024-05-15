package fr.xahla.musicx.api.model;

import fr.xahla.musicx.api.model.enums.AudioFormat;
import lombok.Builder;
import lombok.Data;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

/**
 * Transferring song data throughout application layers.
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
    private String filepath;
    private AudioFormat format;
    private Map<Long, String> lyrics;
    private int sampleRate;
    private String title;
    private short trackNumber;

    public void setRawLyrics(final String lyrics) {
        this.lyrics = Map.of(0L, lyrics);
    }

    public String getRawLyrics() {
        return String.join("\n", lyrics.values());
    }

}
