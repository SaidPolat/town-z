using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuScript : MonoBehaviour
{
    public AudioManager audioManager;
    public Animator canvasAnimator;
    public Animator cameraAnimator;
    public GameObject pressAnyKeyText;
    public Animator warningAnimator;
    public TMP_InputField iField;
    public static string myName;
    public static List<string> namesList = new List<string>();
    private bool opening = true;

    void Start()
    {
        audioManager.Play("Menu Theme");
        SettingsMenu.musicVolume = 0.405f;
        SettingsMenu.sfxVolume = 0.485f;
    }

    
    void Update()
    {
        if (Input.anyKeyDown && opening)
        {
            audioManager.Play("Menu4");
            canvasAnimator.SetTrigger("PressAnyKey");
            cameraAnimator.SetTrigger("PressAnyKey");
            StartCoroutine(PressAnyKeyClose());
            opening = false;
        }
    }
    
    public void MenuButtons(int index)
    {
        switch (index)
        {
            case 1:
                audioManager.Play("Menu3");
                canvasAnimator.SetTrigger("StartNewGameOpenCanvas");
                cameraAnimator.SetTrigger("StartNewGameOpenCamera");
                break;
            case 2:
                audioManager.Play("Menu5");
                canvasAnimator.SetTrigger("StartNewGameCloseCanvas");
                cameraAnimator.SetTrigger("StartNewGameCloseCamera");
                break;
            case 3:
                audioManager.Play("Menu3");
                canvasAnimator.SetTrigger("ScoreboardOpenCanvas");
                cameraAnimator.SetTrigger("ScoreboardOpenCamera");
                break;
            case 4:
                audioManager.Play("Menu5");
                canvasAnimator.SetTrigger("ScoreboardCloseCanvas");
                cameraAnimator.SetTrigger("ScoreboardCloseCamera");
                break;
            case 5:
                audioManager.Play("Menu3");
                canvasAnimator.SetTrigger("SettingsOpenCanvas");
                cameraAnimator.SetTrigger("SettingsOpenCamera");
                break;
            case 6:
                audioManager.Play("Menu5");
                canvasAnimator.SetTrigger("SettingsCloseCanvas");
                cameraAnimator.SetTrigger("SettingsCloseCamera");
                break;
            case 7:
                myName = iField.text;
                if (iField.text == "")
                {
                    warningAnimator.SetTrigger("Start");
                }
                else
                {
                    namesList.Add(myName);
                    audioManager.Play("Menu3");
                    StartCoroutine(LoadLevel());
                }
                break;
            case 8:
                audioManager.Play("Menu3");
                Application.Quit();
                break;
        }
    }

    IEnumerator PressAnyKeyClose()
    {
        yield return new WaitForSeconds(1f);
        pressAnyKeyText.SetActive(false);
    }

    public void OnPointerForButtons()
    {
        audioManager.Play("OnPointerSFX");
    }
   
    IEnumerator LoadLevel()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cameraAnimator.SetTrigger("LoadLevel");
        canvasAnimator.SetTrigger("LoadLevel");
        yield return new WaitForSeconds(2.4f);
        SceneManager.LoadScene(1);
    }
}
