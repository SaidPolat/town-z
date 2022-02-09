using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    float sliderValue;
    public AudioManager audioManager;
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject pauseMain;
    public GameObject settingsScreen;
    public GameObject audioPanel;
    public GameObject graphicsPanel;
    public GameObject sensivityPanel;
    public Animator animator;
    public Slider sliderSfx;
    public TMP_Dropdown aaDropdown;
    public AudioMixer audioMixer;

    void Start()
    {    
        sliderSfx.value = SettingsMenu.sfxVolume;
        aaDropdown.value = CameraShake.dropdownValue;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GameControl.isMarketOpen)
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        } 
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseMain.SetActive(true);
        settingsScreen.SetActive(false);
        audioPanel.SetActive(false);
        graphicsPanel.SetActive(true);
        sensivityPanel.SetActive(false);
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    void Pause()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
        Time.timeScale = 0f;
    }

    public void ReturnToMenuButton()
    {
        GameControl gameControl = FindObjectOfType<GameControl>();
        GameControl.scoreList.Add(gameControl.killCount);
        gameControl.killCount = 0;
        GameIsPaused = false;
        StartCoroutine(ReturnToMenu());
    }
    IEnumerator ReturnToMenu()
    {
        animator.Play("AfterRestartOrMenu");
        yield return new WaitForSecondsRealtime(1.1f);
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(0);
    }

    public void QuitFromGame()
    {
        Application.Quit();
    }

    IEnumerator RestartLevel()
    {
        animator.Play("AfterRestartOrMenu");
        yield return new WaitForSecondsRealtime(1.1f);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(1);

    }

    public void RestartLevelButton()
    {
        StartCoroutine(RestartLevel());
    }

    public void SettingsButton()
    {
        pauseMain.SetActive(false);
        settingsScreen.SetActive(true);

    }

    public void BackToMenuButtonFromSettings()
    {
        pauseMain.SetActive(true);
        settingsScreen.SetActive(false);
    }

    public void GoSensPanel()
    {
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(false);
        sensivityPanel.SetActive(true);
    }

    public void GoGraphsPanel()
    {
        graphicsPanel.SetActive(true);
        audioPanel.SetActive(false);
        sensivityPanel.SetActive(false);
    }

    public void GoAudioPanel()
    {
        graphicsPanel.SetActive(false);
        audioPanel.SetActive(true);
        sensivityPanel.SetActive(false);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetSfxVolume(float volume)  
    {
        if (volume >= -50f && volume <= -40f)
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
    }
}

