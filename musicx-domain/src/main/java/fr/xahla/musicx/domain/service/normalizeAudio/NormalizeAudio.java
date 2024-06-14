package fr.xahla.musicx.domain.service.normalizeAudio;

import javax.sound.sampled.*;
import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.IOException;

import static fr.xahla.musicx.domain.application.AbstractContext.logger;

/**
 * Normalize an audio source for a given volume.
 * @author Cochetooo
 * @since 0.3.3
 */
public class NormalizeAudio {

    /**
     * @return The difference of decibels given during the normalization.
     * @since 0.3.3
     */
    public double execute(
        final String filepath,
        final double decibel
    ) {
        try {
            final var audioFile = new File(filepath);
            final var audioData = this.getAudioData(audioFile);

            final var currentDb = calculateAverageDecibels(audioData);

            final var normalizedAudioData = normalizeAudio(audioData, decibel);
            saveNormalizedAudio(normalizedAudioData, audioFile);

            return decibel - currentDb;
        } catch (final UnsupportedAudioFileException exception) {
            logger().error(exception, "IO_AUDIO_NOT_SUPPORTED", filepath);
        } catch (final IOException exception) {
            logger().error(exception, "IO_URI_ERROR", filepath);
        }

        return 0.0;
    }

    private void saveNormalizedAudio(final byte[] audioData, final File file) throws UnsupportedAudioFileException, IOException {
        AudioFormat format;

        try (final var originalStream = AudioSystem.getAudioInputStream(file)) {
            format = originalStream.getFormat();
        }

        final var byteArrayInputStream = new ByteArrayInputStream(audioData);
        try (final var audioInputStream = new AudioInputStream(byteArrayInputStream, format, audioData.length / format.getFrameSize())) {
            AudioSystem.write(audioInputStream, AudioFileFormat.Type.WAVE, file);
        }
    }

    private byte[] getAudioData(final File audioFile) throws UnsupportedAudioFileException, IOException {
        try (final var audioInputStream = AudioSystem.getAudioInputStream(audioFile)) {
            return audioInputStream.readAllBytes();
        }
    }

    private double calculateAverageDecibels(final byte[] audioData) {
        var sumLevel = 0.0;
        final var sampleCount = audioData.length / 2; // Assuming 16-bit audio

        for (var i = 0; i < sampleCount; i++) {
            final var sample = (audioData[i + 1] << 8) | (audioData[i] & 0xFF);
            final var normalizedSample = sample / 32768.0;
            final var spl = 20 * Math.log10(Math.abs(normalizedSample) + 1e-10); // Avoid Math.log10(0) by adding abs(value) + 1e-10
            sumLevel += spl;
        }

        return sumLevel / sampleCount;
    }

    private byte[] normalizeAudio(final byte[] audioData, final double targetDb) {
        final var currentDb = calculateAverageDecibels(audioData);
        final var gainDb = targetDb - currentDb;
        final var gainLinear = Math.pow(10, gainDb / 20);

        for (var i = 0; i < audioData.length; i += 2) {
            final var sample = (audioData[i + 1] << 8) | (audioData[i] & 0xFF);
            var normalizedSample = (int) (sample * gainLinear);

            // Clamp to avoid overflow
            normalizedSample = Math.min(32767, Math.max(-32768, normalizedSample));

            audioData[i] = (byte) (normalizedSample & 0xFF);
            audioData[i + 1] = (byte) ((normalizedSample >> 8) & 0xFF);
        }

        return audioData;
    }

}
