package fr.xahla.musicx.domain.helper;

import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.Mixer;

/**
 * Manager for system audio devices
 *
 * @author Cochetooo
 * @since 0.5.0
 */
public final class AudioDeviceHelper {

    /**
     * @since 0.5.0
     */
    public static Mixer.Info[] getAudioDevices() {
        return AudioSystem.getMixerInfo();
    }

}
