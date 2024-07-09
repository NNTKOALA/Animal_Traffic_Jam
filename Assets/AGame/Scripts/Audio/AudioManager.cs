using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadAudioSettings();
        PlayMusic("Theme");
    }

    public void PlayMusic(string name)
    {
        Sound sound = Array.Find(musicSounds, x => x.name == name);

        if (sound == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            musicSource.clip = sound.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string name)
    {
        Sound sound = Array.Find(sfxSounds, x => x.name == name);

        if (sound == null)
        {
            Debug.Log("Sound Not Found");
        }
        else
        {
            sfxSource.PlayOneShot(sound.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
        PlayerPrefs.SetInt("MusicMute", musicSource.mute ? 1 : 0);
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        PlayerPrefs.SetInt("SFXMute", sfxSource.mute ? 1 : 0);
    }

    private void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey("MusicMute"))
        {
            musicSource.mute = PlayerPrefs.GetInt("MusicMute") == 1;
        }
        else
        {
            PlayerPrefs.SetInt("MusicMute", 0);
        }

        if (PlayerPrefs.HasKey("SFXMute"))
        {
            sfxSource.mute = PlayerPrefs.GetInt("SFXMute") == 1;
        }
        else
        {
            PlayerPrefs.SetInt("SFXMute", 0);
        }
    }
}
