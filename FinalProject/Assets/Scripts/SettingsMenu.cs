using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public AudioManager audioManager;

    public AudioMixer audioMixer;
    float sliderValue;

    public static float musicVolume;
    public static float sfxVolume;

    Resolution[] resolutions;

    void Start()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        sfxVolume = 20f;
    }

    public void SetMusicVolume(float volume)
    {
        for (int i = 42; i < 44; i++)
        {
            audioManager.sounds[i].volume = volume;
        }
        musicVolume = volume;
    }

    public void SetSfxVolume(float volume)
    {

        if(volume >= -50f && volume <= -40f)
        {
            audioMixer.SetFloat("volume", -50f);
        }
        else
        {
            sliderValue = volume / 5;
            audioMixer.SetFloat("volume", sliderValue);
        }

        for (int i = 0; i < 27; i++)
        {
            audioManager.sounds[i].volume = (volume + 40f) / 100f;
            audioManager.sounds[i].volume = Mathf.Clamp(audioManager.sounds[i].volume, 0f, 1f);
        }
        for (int i = 27; i < 33; i++)
        {
            audioManager.sounds[i].volume = (volume + 40f) / 100f;
            audioManager.sounds[i].volume = Mathf.Clamp(audioManager.sounds[i].volume, 0f, 1f);
        }
        for (int i = 33; i < 42; i++)
        {
            audioManager.sounds[i].volume = (volume + 120f) / 100f;
            audioManager.sounds[i].volume = Mathf.Clamp(audioManager.sounds[i].volume, 0f, 1f);
        }
        sfxVolume = volume;
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen changed");
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];

        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
