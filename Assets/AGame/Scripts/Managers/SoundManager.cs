using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Image musicOnIcon;
    [SerializeField] Image musicOffIcon;
    [SerializeField] Image soundOnIcon;
    [SerializeField] Image soundOffIcon;

    private bool isMusicMuted;
    private bool isSoundMuted;

    void Start()
    {
        // Load the saved settings
        LoadSettings();

        // Apply the settings
        ApplySettings();
    }

    public void OnMusicButtonPress()
    {
        isMusicMuted = !isMusicMuted;
        AudioManager.Instance.ToggleMusic();
        SaveMusicSettings();
        UpdateMusicButtonIcon();
    }

    public void OnSoundButtonPress()
    {
        isSoundMuted = !isSoundMuted;
        AudioManager.Instance.ToggleSFX();
        SaveSoundSettings();
        UpdateSoundButtonIcon();
    }

    private void UpdateMusicButtonIcon()
    {
        musicOnIcon.enabled = !isMusicMuted;
        musicOffIcon.enabled = isMusicMuted;
    }

    private void UpdateSoundButtonIcon()
    {
        soundOnIcon.enabled = !isSoundMuted;
        soundOffIcon.enabled = isSoundMuted;
    }

    private void LoadSettings()
    {
        isMusicMuted = PlayerPrefs.GetInt("musicMuted", 0) == 1;
        isSoundMuted = PlayerPrefs.GetInt("soundMuted", 0) == 1;
    }

    private void ApplySettings()
    {
        AudioManager.Instance.musicSource.mute = isMusicMuted;
        AudioManager.Instance.sfxSource.mute = isSoundMuted;
        UpdateMusicButtonIcon();
        UpdateSoundButtonIcon();
    }

    private void SaveMusicSettings()
    {
        PlayerPrefs.SetInt("musicMuted", isMusicMuted ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SaveSoundSettings()
    {
        PlayerPrefs.SetInt("soundMuted", isSoundMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
