using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    // Start is called before the first frame update
    public List<AudioClip> AudioClips;
    public AudioSource BgAudioSource;
    public AudioSource MainAudioSource;
    public AudioSource ButtonaudioSource;
    public AudioSource KenoAudioSource;

    private readonly List<AudioSource> allSources = new List<AudioSource>();
    private readonly Dictionary<AudioSource, bool> preFocusMuteState = new Dictionary<AudioSource, bool>();
    private bool isForceMuted = false;

    void Awake()
    {
        allSources.Add(BgAudioSource);
        allSources.Add(MainAudioSource);
        allSources.Add(ButtonaudioSource);
        allSources.Add(KenoAudioSource);
    }

    void Start()
    {
        BgAudioSource.Play();
    }

    void Update()
    {

    }
    public void PlayMainAudio(int index)
    {
        if (index < 0 || index >= AudioClips.Count)
        {
            Debug.LogWarning("Audio index out of range: " + index);
            return;
        }
        MainAudioSource.Stop(); // Stop any currently playing audio
        MainAudioSource.clip = AudioClips[index];
        MainAudioSource.Play();
    }
    public void PlayButtonAudio()
    {
        if (ButtonaudioSource != null && AudioClips.Count > 0)
        {
            ButtonaudioSource.clip = AudioClips[0]; // Assuming the first clip is for button sounds
            ButtonaudioSource.Play();
        }
    }

    public void PlayKenoAudio(int index)
    {
        if (KenoAudioSource != null )
        {
            KenoAudioSource.clip = AudioClips[index]; // Assuming the second clip is for Keno sounds
            KenoAudioSource.Play();
        }
    }

    public void StopMainAudio()
    {
        if (MainAudioSource.isPlaying)
        {
            MainAudioSource.Stop();
        }
    }

    // public void ToggleAllSound(bool isOn)
    // {
    //     if (isOn)
    //     {
    //         if (!BgAudioSource.isPlaying && BgAudioSource.clip != null)
    //             BgAudioSource.Play();

    //         // Resume main audio if it had a clip
    //         if (MainAudioSource.clip != null && !MainAudioSource.isPlaying)
    //             MainAudioSource.Play();

    //         // Resume button audio if needed
    //         if (ButtonaudioSource.clip != null && !ButtonaudioSource.isPlaying)
    //             ButtonaudioSource.Play();
    //     }
    //     else
    //     {
    //         // Stop all sounds
    //         BgAudioSource.Stop();
    //         MainAudioSource.Stop();
    //         ButtonaudioSource.Stop();
    //     }
    // }
    public void ToggleBgSound(bool isOn)
    {
        if (isOn) BgAudioSource.Play();
        bool mute = !isOn;
        if (isForceMuted) preFocusMuteState[BgAudioSource] = mute;
        else BgAudioSource.mute = mute;
    }

    public void ToggleMainSound(bool isOn)
    {
        bool mute = !isOn;
        foreach (var src in new[] { MainAudioSource, ButtonaudioSource, KenoAudioSource })
        {
            if (isForceMuted) preFocusMuteState[src] = mute;
            else src.mute = mute;
        }
    }

    // Focus-driven — called from BOTH UIManager.OnFocusChanged (WebGL/JS path) and OnApplicationFocus below.
    internal void SetMuteAll(bool forceMute)
    {
        if (forceMute == isForceMuted) return;
        isForceMuted = forceMute;

        foreach (var source in allSources)
        {
            if (source == null) continue;
            if (forceMute)
            {
                preFocusMuteState[source] = source.mute;
                source.mute = true;
            }
            else
            {
                source.mute = preFocusMuteState.TryGetValue(source, out bool prevMuted) ? prevMuted : source.mute;
            }
        }
    }

    // Native/editor focus path — calls the SAME method the WebGL OnFocusChanged path calls.
    private void OnApplicationFocus(bool focus)
    {
        SetMuteAll(!focus);
    }
}
