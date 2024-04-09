package fr.xahla.musicx.desktop.model.data;

import fr.xahla.musicx.core.model.data.LocalSongInterface;
import org.jaudiotagger.audio.AudioFile;
import org.jaudiotagger.audio.AudioHeader;
import org.jaudiotagger.tag.FieldKey;
import org.jaudiotagger.tag.KeyNotFoundException;
import org.jaudiotagger.tag.Tag;

import static fr.xahla.musicx.core.logging.SimpleLogger.logger;

public class LocalSong implements LocalSongInterface {

    private final AudioFile audioFile;
    private final Tag tag;
    private final AudioHeader header;

    private String albumName;
    private String albumArtist;
    private Integer albumYear;
    private String artistName;
    private String amazonId;
    private Integer bitRate;
    private Short bpm;
    private String catalogNo;
    private String classicalCatalog;
    private String classicalNickname;
    private String choirName;
    private String comment;
    private String composerName;
    private String conductorName;
    private String copyright;
    private String country;
    private String coverArt;
    private String custom1, custom2, custom3, custom4, custom5;
    private Short discNumber;
    private Short discTotal;
    private String djMixerName;
    private Integer duration;
    private String encoder;
    private String engineerName;
    private String ensemble;
    private String format;
    private String genreName;
    private String key;
    private String language;
    private String lyricist;
    private String lyrics;
    private String mixerName;
    private String mood;
    private String movementName;
    private Short movementNumber;
    private Short movementTotal;
    private String occasion;
    private String opus;
    private String orchestraName;
    private String partName;
    private Short partNumber;
    private String partType;
    private String performerName;
    private String period;
    private String producerName;
    private String quality;
    private String ranking;
    private Short rating;
    private String recordLabel;
    private String recordingDate;
    private String recordingStartDate;
    private String recordingEndDate;
    private String recordingLocation;
    private String remixerName;
    private Integer sampleRate;
    private String section;
    private String script;
    private String timbre;
    private String title;
    private String titleMovement;
    private String tonality;
    private Short trackNumber;
    private Short trackTotal;
    private String workName;
    private String workType;
    private Integer year;
    private String version;

    private boolean hasFailed;

    public LocalSong(
        final AudioFile audioFile
    ) {
        this.audioFile = audioFile;
        this.tag = audioFile.getTag();
        this.header = audioFile.getAudioHeader();

        if (null == this.tag || null == this.header) {
            this.title = audioFile.getFile().getAbsolutePath();
            this.hasFailed = true;
            return;
        }

        this
            .setAlbumName()
            .setAlbumArtist()
            .setAlbumYear()
            .setAmazonId()
            .setArtistName()
            .setBitRate()
            .setBpm()
            .setCatalogNo()
            .setClassicalCatalog()
            .setClassicalNickname()
            .setChoirName()
            .setComment()
            .setComposerName()
            .setConductorName()
            .setCopyright()
            .setCountry()
            .setCoverArt()
            .setCustoms()
            .setDiscNumber()
            .setDiscTotal()
            .setDjMixerName()
            .setDuration()
            .setEncoder()
            .setEngineerName()
            .setEnsemble()
            .setFormat()
            .setGenreName()
            .setKey()
            .setLanguage()
            .setLyricist()
            .setLyrics()
            .setMixerName()
            .setMood()
            .setMovementName()
            .setMovementNumber()
            .setMovementTotal()
            .setOccasion()
            .setOpus()
            .setOrchestraName()
            .setPartName()
            .setPartNumber()
            .setPartType()
            .setPerformerName()
            .setPeriod()
            .setProducerName()
            .setQuality()
            .setRanking()
            .setRating()
            .setRecordLabel()
            .setRecordingDate()
            .setRecordingStartDate()
            .setRecordingEndDate()
            .setRecordingLocation()
            .setRemixerName()
            .setSampleRate()
            .setSection()
            .setScript()
            .setTimbre()
            .setTitle()
            .setTitleMovement()
            .setTonality()
            .setTrackNumber()
            .setTrackTotal()
            .setWorkName()
            .setWorkType()
            .setYear()
            .setVersion();
    }

    private LocalSong setAlbumName() {
        try {
            this.albumName = tag.getFirst(FieldKey.ALBUM);
        } catch (KeyNotFoundException exception) {
            this.notFound("ALBUM");
        }

        return this;
    }

    private LocalSong setAlbumArtist() {
        try {
            this.albumArtist = tag.getFirst(FieldKey.ALBUM_ARTIST);
        } catch (KeyNotFoundException exception) {
            this.notFound("ALBUM_ARTIST");
        }

        return this;
    }

    private LocalSong setAlbumYear() {
        try {
            var albumYearTag = tag.getFirst(FieldKey.ALBUM_YEAR);

            if (albumYearTag.isEmpty()) {
                return this;
            }

            if (4 < albumYearTag.length()) {
                this.albumYear = Integer.parseInt(albumYearTag.substring(0, 4));
            } else {
                this.albumYear = Integer.parseInt(albumYearTag);
            }
        } catch (KeyNotFoundException exception) {
            this.notFound("ALBUM_YEAR");
        } catch (IndexOutOfBoundsException exception) {
            logger().info("ALBUM_YEAR is not formatted correctly.");
        } catch (NumberFormatException exception) {
            logger().info("ALBUM_YEAR is not a valid number.");
        }

        return this;
    }

    private LocalSong setAmazonId() {
        try {
            this.amazonId = tag.getFirst(FieldKey.AMAZON_ID);
        } catch (KeyNotFoundException exception) {
            this.notFound("AMAZON_ID");
        }

        return this;
    }

    private LocalSong setArtistName() {
        try {
            this.artistName = tag.getFirst(FieldKey.ARTIST);
        } catch (KeyNotFoundException exception) {
            this.notFound("ARTIST");
        }

        return this;
    }

    private LocalSong setBitRate() {
        try {
            this.bitRate = (int) this.header.getBitRateAsNumber();
        } catch (ClassCastException exception) {
            this.notFound("BITRATE can't be cast to integer.");
        }

        return this;
    }

    private LocalSong setBpm() {
        try {
            var bpmTag = tag.getFirst(FieldKey.BPM);

            if (bpmTag.isEmpty()) {
                return this;
            }

            this.bpm = Short.parseShort(bpmTag);
        } catch (KeyNotFoundException exception) {
            this.notFound("BPM");
        } catch (NumberFormatException exception) {
            logger().info("ALBUM_YEAR is not a valid number.");
        }

        return this;
    }

    private LocalSong setCatalogNo() {
        try {
            this.catalogNo = tag.getFirst(FieldKey.CATALOG_NO);
        } catch (KeyNotFoundException exception) {
            this.notFound("CATALOG_NO");
        }

        return this;
    }

    private LocalSong setClassicalCatalog() {
        try {
            this.classicalCatalog = tag.getFirst(FieldKey.CLASSICAL_CATALOG);
        } catch (KeyNotFoundException exception) {
            this.notFound("CLASSICAL_CATALOG");
        }

        return this;
    }

    private LocalSong setClassicalNickname() {
        try {
            this.classicalNickname = tag.getFirst(FieldKey.CLASSICAL_NICKNAME);
        } catch (KeyNotFoundException exception) {
            this.notFound("CLASSICAL_NICKNAME");
        }

        return this;
    }

    private LocalSong setChoirName() {
        try {
            this.choirName = tag.getFirst(FieldKey.CHOIR);
        } catch (KeyNotFoundException exception) {
            this.notFound("CLASSICAL_NICKNAME");
        }

        return this;
    }

    private LocalSong setComment() {
        try {
            this.comment = tag.getFirst(FieldKey.COMMENT);
        } catch (KeyNotFoundException exception) {
            this.notFound("COMMENT");
        }

        return this;
    }

    private LocalSong setComposerName() {
        try {
            this.composerName = tag.getFirst(FieldKey.COMPOSER);
        } catch (KeyNotFoundException exception) {
            this.notFound("COMPOSER");
        }

        return this;
    }

    private LocalSong setConductorName() {
        try {
            this.conductorName = tag.getFirst(FieldKey.CONDUCTOR);
        } catch (KeyNotFoundException exception) {
            this.notFound("CONDUCTOR");
        }

        return this;
    }

    private LocalSong setCopyright() {
        try {
            this.copyright = tag.getFirst(FieldKey.COPYRIGHT);
        } catch (KeyNotFoundException exception) {
            this.notFound("COPYRIGHT");
        }

        return this;
    }

    private LocalSong setCountry() {
        try {
            this.country = tag.getFirst(FieldKey.COUNTRY);
        } catch (KeyNotFoundException exception) {
            this.notFound("COUNTRY");
        }

        return this;
    }

    private LocalSong setCoverArt() {
        try {
            this.coverArt = tag.getFirst(FieldKey.COVER_ART);
        } catch (KeyNotFoundException exception) {
            this.notFound("COVER_ART");
        }

        return this;
    }

    private LocalSong setCustoms() {
        for (var i = 1; i <= 5; i++) {
            try {
                switch(i) {
                    case 1 -> this.custom1 = tag.getFirst(FieldKey.CUSTOM1);
                    case 2 -> this.custom2 = tag.getFirst(FieldKey.CUSTOM2);
                    case 3 -> this.custom3 = tag.getFirst(FieldKey.CUSTOM3);
                    case 4 -> this.custom4 = tag.getFirst(FieldKey.CUSTOM4);
                    case 5 -> this.custom5 = tag.getFirst(FieldKey.CUSTOM5);
                }
            } catch (KeyNotFoundException exception) {
                this.notFound("CUSTOM1");
            }
        }

        return this;
    }

    private LocalSong setDiscNumber() {
        try {
            var discNumberTag = tag.getFirst(FieldKey.DISC_NO);

            if (discNumberTag.isEmpty()) {
                return this;
            }

            this.discNumber = Short.parseShort(discNumberTag);
        } catch (KeyNotFoundException exception) {
            this.notFound("DISC_NO");
        } catch (NumberFormatException exception) {
            logger().info("DISC_NO is not a valid number.");
        }

        return this;
    }

    private LocalSong setDiscTotal() {
        try {
            var discTotalTag = tag.getFirst(FieldKey.DISC_TOTAL);

            if (discTotalTag.isEmpty()) {
                return this;
            }

            this.discTotal = Short.parseShort(discTotalTag);

            if (this.discNumber > discTotal) {
                logger().config("DISC_TOTAL is less than DISC_NO, is it an error?");
            }
        } catch (KeyNotFoundException exception) {
            this.notFound("DISC_NO");
        } catch (NumberFormatException exception) {
            logger().info("DISC_NO is not a valid number.");
        }

        return this;
    }

    private LocalSong setDjMixerName() {
        try {
            this.djMixerName = tag.getFirst(FieldKey.DJMIXER);
        } catch (KeyNotFoundException exception) {
            this.notFound("DJMIXER");
        }

        return this;
    }

    private LocalSong setDuration() {
        this.duration = this.header.getTrackLength();

        return this;
    }

    private LocalSong setEncoder() {
        try {
            this.encoder = tag.getFirst(FieldKey.ENCODER);
        } catch (KeyNotFoundException exception) {
            this.notFound("ENCODER");
        }

        return this;
    }

    private LocalSong setEngineerName() {
        try {
            this.engineerName = tag.getFirst(FieldKey.ENGINEER);
        } catch (KeyNotFoundException exception) {
            this.notFound("ENGINEER");
        }

        return this;
    }

    private LocalSong setEnsemble() {
        try {
            this.ensemble = tag.getFirst(FieldKey.ENSEMBLE);
        } catch (KeyNotFoundException exception) {
            this.notFound("ENSEMBLE");
        }

        return this;
    }

    private LocalSong setFormat() {
        this.format = this.header.getFormat();

        return this;
    }

    private LocalSong setGenreName() {
        try {
            this.genreName = tag.getFirst(FieldKey.GENRE);
        } catch (KeyNotFoundException exception) {
            this.notFound("GENRE");
        }

        return this;
    }

    private LocalSong setKey() {
        try {
            this.key = tag.getFirst(FieldKey.KEY);
        } catch (KeyNotFoundException exception) {
            this.notFound("KEY");
        }

        return this;
    }

    private LocalSong setLanguage() {
        try {
            this.language = tag.getFirst(FieldKey.LANGUAGE);
        } catch (KeyNotFoundException exception) {
            this.notFound("LANGUAGE");
        }

        return this;
    }

    private LocalSong setLyricist() {
        try {
            this.lyricist = tag.getFirst(FieldKey.LYRICIST);
        } catch (KeyNotFoundException exception) {
            this.notFound("LYRICIST");
        }

        return this;
    }

    private LocalSong setLyrics() {
        try {
            this.lyrics = tag.getFirst(FieldKey.LYRICS);
        } catch (KeyNotFoundException exception) {
            this.notFound("LYRICS");
        }

        return this;
    }

    private LocalSong setMixerName() {
        try {
            this.mixerName = tag.getFirst(FieldKey.MIXER);
        } catch (KeyNotFoundException exception) {
            this.notFound("MIXER");
        }

        return this;
    }

    private LocalSong setMood() {
        try {
            this.mood = tag.getFirst(FieldKey.MOOD);
        } catch (KeyNotFoundException exception) {
            this.notFound("MOOD");
        }

        return this;
    }

    private LocalSong setMovementName() {
        try {
            this.movementName = tag.getFirst(FieldKey.MOVEMENT);
        } catch (KeyNotFoundException exception) {
            this.notFound("MOVEMENT");
        }

        return this;
    }

    private LocalSong setMovementNumber() {
        try {
            var movementNumberTag = tag.getFirst(FieldKey.MOVEMENT_NO);

            if (movementNumberTag.isEmpty()) {
                return this;
            }

            this.movementNumber = Short.parseShort(movementNumberTag);
        } catch (KeyNotFoundException exception) {
            this.notFound("MOVEMENT_NO");
        } catch (NumberFormatException exception) {
            logger().info("MOVEMENT_NO is not a valid number.");
        }

        return this;
    }

    private LocalSong setMovementTotal() {
        try {
            var movementTotalTag = tag.getFirst(FieldKey.MOVEMENT_TOTAL);

            if (movementTotalTag.isEmpty()) {
                return this;
            }

            this.movementTotal = Short.parseShort(movementTotalTag);

            if (this.movementNumber > movementTotal) {
                logger().config("MOVEMENT_TOTAL is less than MOVEMENT_NO, is it an error?");
            }
        } catch (KeyNotFoundException exception) {
            this.notFound("MOVEMENT_NO");
        } catch (NumberFormatException exception) {
            logger().info("MOVEMENT_NO is not a valid number.");
        }

        return this;
    }

    private LocalSong setOccasion() {
        try {
            this.occasion = tag.getFirst(FieldKey.OCCASION);
        } catch (KeyNotFoundException exception) {
            this.notFound("OCCASION");
        }

        return this;
    }

    private LocalSong setOpus() {
        try {
            this.opus = tag.getFirst(FieldKey.OPUS);
        } catch (KeyNotFoundException exception) {
            this.notFound("OPUS");
        }

        return this;
    }

    private LocalSong setOrchestraName() {
        try {
            this.orchestraName = tag.getFirst(FieldKey.ORCHESTRA);
        } catch (KeyNotFoundException exception) {
            this.notFound("ORCHESTRA");
        }

        return this;
    }

    private LocalSong setPartName() {
        try {
            this.partName = tag.getFirst(FieldKey.PART);
        } catch (KeyNotFoundException exception) {
            this.notFound("PART");
        }

        return this;
    }

    private LocalSong setPartNumber() {
        try {
            var partNumberTag = tag.getFirst(FieldKey.PART_NUMBER);

            if (partNumberTag.isEmpty()) {
                return this;
            }

            this.partNumber = Short.parseShort(partNumberTag);
        } catch (KeyNotFoundException exception) {
            this.notFound("PART_NUMBER");
        } catch (NumberFormatException exception) {
            logger().info("PART_NUMBER is not a valid number.");
        }

        return this;
    }

    private LocalSong setPartType() {
        try {
            this.partType = tag.getFirst(FieldKey.PART_TYPE);
        } catch (KeyNotFoundException exception) {
            this.notFound("PART_TYPE");
        }

        return this;
    }

    private LocalSong setPerformerName() {
        try {
            this.performerName = tag.getFirst(FieldKey.PERFORMER);
        } catch (KeyNotFoundException exception) {
            this.notFound("PERFORMER");
        }

        return this;
    }

    private LocalSong setPeriod() {
        try {
            this.period = tag.getFirst(FieldKey.PERIOD);
        } catch (KeyNotFoundException exception) {
            this.notFound("PERIOD");
        }

        return this;
    }

    private LocalSong setProducerName() {
        try {
            this.producerName = tag.getFirst(FieldKey.PRODUCER);
        } catch (KeyNotFoundException exception) {
            this.notFound("PRODUCER");
        }

        return this;
    }

    private LocalSong setQuality() {
        try {
            this.quality = tag.getFirst(FieldKey.QUALITY);
        } catch (KeyNotFoundException exception) {
            this.notFound("QUALITY");
        }

        return this;
    }

    private LocalSong setRanking() {
        try {
            this.ranking = tag.getFirst(FieldKey.RANKING);
        } catch (KeyNotFoundException exception) {
            this.notFound("RANKING");
        }

        return this;
    }

    private LocalSong setRating() {
        try {
            var ratingTag = tag.getFirst(FieldKey.RATING);

            if (ratingTag.isEmpty()) {
                return this;
            }

            this.rating = Short.parseShort(ratingTag);
        } catch (KeyNotFoundException exception) {
            this.notFound("RATING");
        } catch (NumberFormatException exception) {
            logger().info("RATING is not a valid number.");
        }

        return this;
    }

    private LocalSong setRecordLabel() {
        try {
            this.recordLabel = tag.getFirst(FieldKey.RECORD_LABEL);
        } catch (KeyNotFoundException exception) {
            this.notFound("RECORD_LABEL");
        }

        return this;
    }

    private LocalSong setRecordingDate() {
        try {
            this.recordingDate = tag.getFirst(FieldKey.RECORDINGDATE);
        } catch (KeyNotFoundException exception) {
            this.notFound("RECORDINGDATE");
        }

        return this;
    }

    private LocalSong setRecordingStartDate() {
        try {
            this.recordingStartDate = tag.getFirst(FieldKey.RECORDINGSTARTDATE);
        } catch (KeyNotFoundException exception) {
            this.notFound("RECORDINGSTARTDATE");
        }

        return this;
    }

    private LocalSong setRecordingEndDate() {
        try {
            this.recordingEndDate = tag.getFirst(FieldKey.RECORDINGENDDATE);
        } catch (KeyNotFoundException exception) {
            this.notFound("RECORDINGENDDATE");
        }

        return this;
    }

    private LocalSong setRecordingLocation() {
        try {
            this.recordingLocation = tag.getFirst(FieldKey.RECORDINGLOCATION);
        } catch (KeyNotFoundException exception) {
            this.notFound("RECORDINGLOCATION");
        }

        return this;
    }

    private LocalSong setRemixerName() {
        try {
            this.remixerName = tag.getFirst(FieldKey.REMIXER);
        } catch (KeyNotFoundException exception) {
            this.notFound("REMIXER");
        }

        return this;
    }

    private LocalSong setSampleRate() {
        this.sampleRate = this.header.getSampleRateAsNumber();

        return this;
    }

    private LocalSong setSection() {
        try {
            this.section = tag.getFirst(FieldKey.SECTION);
        } catch (KeyNotFoundException exception) {
            this.notFound("SECTION");
        }

        return this;
    }

    private LocalSong setScript() {
        try {
            this.script = tag.getFirst(FieldKey.SCRIPT);
        } catch (KeyNotFoundException exception) {
            this.notFound("SCRIPT");
        }

        return this;
    }

    private LocalSong setTimbre() {
        try {
            this.timbre = tag.getFirst(FieldKey.TIMBRE);
        } catch (KeyNotFoundException exception) {
            this.notFound("TIMBRE");
        }

        return this;
    }

    private LocalSong setTitle() {
        try {
            this.title = tag.getFirst(FieldKey.TITLE);
        } catch (KeyNotFoundException exception) {
            this.notFound("TITLE");
        }

        return this;
    }

    private LocalSong setTitleMovement() {
        try {
            this.titleMovement = tag.getFirst(FieldKey.TITLE_MOVEMENT);
        } catch (KeyNotFoundException exception) {
            this.notFound("TITLE_MOVEMENT");
        }

        return this;
    }

    private LocalSong setTonality() {
        try {
            this.tonality = tag.getFirst(FieldKey.TONALITY);
        } catch (KeyNotFoundException exception) {
            this.notFound("TONALITY");
        }

        return this;
    }

    private LocalSong setTrackNumber() {
        try {
            var trackNumberTag = tag.getFirst(FieldKey.TRACK);

            if (trackNumberTag.isEmpty()) {
                return this;
            }

            this.trackNumber = Short.parseShort(trackNumberTag);
        } catch (KeyNotFoundException exception) {
            this.notFound("TRACK");
        } catch (NumberFormatException exception) {
            logger().info("TRACK is not a valid number.");
        }

        return this;
    }

    private LocalSong setTrackTotal() {
        try {
            var trackTotalTag = tag.getFirst(FieldKey.TRACK_TOTAL);

            if (trackTotalTag.isEmpty()) {
                return this;
            }

            this.trackTotal = Short.parseShort(trackTotalTag);
        } catch (KeyNotFoundException exception) {
            this.notFound("TRACK_TOTAL");
        } catch (NumberFormatException exception) {
            logger().info("TRACK_TOTAL is not a valid number.");
        }

        return this;
    }

    private LocalSong setWorkName() {
        try {
            this.workName = tag.getFirst(FieldKey.WORK);
        } catch (KeyNotFoundException exception) {
            this.notFound("WORK");
        }

        return this;
    }

    private LocalSong setWorkType() {
        try {
            this.workType = tag.getFirst(FieldKey.WORK_TYPE);
        } catch (KeyNotFoundException exception) {
            this.notFound("WORK_TYPE");
        }

        return this;
    }

    private LocalSong setYear() {
        try {
            var yearTag = tag.getFirst(FieldKey.YEAR);

            if (yearTag.isEmpty()) {
                return this;
            }

            if (4 < yearTag.length()) {
                this.year = Integer.parseInt(yearTag.substring(0, 4));
            } else {
                this.year = Integer.parseInt(yearTag);
            }
        } catch (KeyNotFoundException exception) {
            this.notFound("YEAR");
        } catch (IndexOutOfBoundsException exception) {
            logger().info("YEAR is not formatted correctly.");
        } catch (NumberFormatException exception) {
            logger().info("YEAR is not a valid number.");
        }

        return this;
    }

    private LocalSong setVersion() {
        try {
            this.version = tag.getFirst(FieldKey.VERSION);
        } catch (KeyNotFoundException exception) {
            this.notFound("VERSION");
        }

        return this;
    }

    public String getAlbumName() {
        return albumName;
    }

    public String getAlbumArtist() {
        return albumArtist;
    }

    public Integer getAlbumYear() {
        return albumYear;
    }

    public String getArtistCountry() {
        return country;
    }

    public String getArtistName() {
        return artistName;
    }

    public String getAmazonId() {
        return amazonId;
    }

    public Short getBpm() {
        return bpm;
    }

    public String getCatalogNo() {
        return catalogNo;
    }

    public String getClassicalCatalog() {
        return classicalCatalog;
    }

    public String getClassicalNickname() {
        return classicalNickname;
    }

    public String getChoirName() {
        return choirName;
    }

    public String getComment() {
        return comment;
    }

    public String getComposerName() {
        return composerName;
    }

    public String getConductorName() {
        return conductorName;
    }

    public String getCopyright() {
        return copyright;
    }

    public String getCoverArt() {
        return coverArt;
    }

    public String getCustom1() {
        return custom1;
    }

    public String getCustom2() {
        return custom2;
    }

    public String getCustom3() {
        return custom3;
    }

    public String getCustom4() {
        return custom4;
    }

    public String getCustom5() {
        return custom5;
    }

    public Short getDiscNumber() {
        return discNumber;
    }

    public Short getDiscTotal() {
        return discTotal;
    }

    public String getDjMixerName() {
        return djMixerName;
    }

    public String getEncoder() {
        return encoder;
    }

    public String getEngineerName() {
        return engineerName;
    }

    public String getEnsemble() {
        return ensemble;
    }

    public String getGenreName() {
        return genreName;
    }

    public String getKey() {
        return key;
    }

    public String getLanguage() {
        return language;
    }

    public String getLyricist() {
        return lyricist;
    }

    public String getLyrics() {
        return lyrics;
    }

    public String getMixerName() {
        return mixerName;
    }

    public String getMood() {
        return mood;
    }

    public String getMovementName() {
        return movementName;
    }

    public Short getMovementNumber() {
        return movementNumber;
    }

    public Short getMovementTotal() {
        return movementTotal;
    }

    public String getOccasion() {
        return occasion;
    }

    public String getOpus() {
        return opus;
    }

    public String getOrchestraName() {
        return orchestraName;
    }

    public String getPartName() {
        return partName;
    }

    public Short getPartNumber() {
        return partNumber;
    }

    public String getPartType() {
        return partType;
    }

    public String getPerformerName() {
        return performerName;
    }

    public String getPeriod() {
        return period;
    }

    public String getProducerName() {
        return producerName;
    }

    public String getQuality() {
        return quality;
    }

    public String getRanking() {
        return ranking;
    }

    public Short getRating() {
        return rating;
    }

    public String getRecordLabel() {
        return recordLabel;
    }

    public String getRecordingDate() {
        return recordingDate;
    }

    public String getRecordingStartDate() {
        return recordingStartDate;
    }

    public String getRecordingEndDate() {
        return recordingEndDate;
    }

    public String getRecordingLocation() {
        return recordingLocation;
    }

    public String getRemixerName() {
        return remixerName;
    }

    public String getSection() {
        return section;
    }

    public String getScript() {
        return script;
    }

    public String getTimbre() {
        return timbre;
    }

    public String getTitle() {
        return title;
    }

    public String getTitleMovement() {
        return titleMovement;
    }

    public String getTonality() {
        return tonality;
    }

    public Short getTrackNumber() {
        return trackNumber;
    }

    public Short getTrackTotal() {
        return trackTotal;
    }

    public String getWorkName() {
        return workName;
    }

    public String getWorkType() {
        return workType;
    }

    public Integer getYear() {
        return year;
    }

    public String getVersion() {
        return version;
    }

    public Integer getBitRate() {
        return bitRate;
    }

    public Integer getDuration() {
        return duration;
    }

    public String getFormat() {
        return format;
    }

    public Integer getSampleRate() {
        return sampleRate;
    }

    public boolean hasFailed() {
        return this.hasFailed;
    }

    private void notFound(final String key) {
        logger().warning("Key not found: " + key);
    }

}
