using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsSync : MonoBehaviour {
    public AudioMixer mainMixer;
    public GameSettings settings;

    void Start() {
        RefreshVolumes();
    }

    public void RefreshVolumes() {
        // We use Log10 because human hearing is logarithmic, not linear.
        // 0.0001 to 1 becomes -80dB to 0dB.
        mainMixer.SetFloat("MusicVol", Mathf.Log10(settings.musicVolume) * 20);

    }
}