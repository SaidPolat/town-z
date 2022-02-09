using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeagleForSound : MonoBehaviour
{
    public AudioManager audioManager;


    public void ReloadSound1()
    {
        audioManager.Play("Deagle Reload1");
    }

    public void ReloadSound2()
    {
        audioManager.Play("Deagle Reload2");
    }

    public void ReloadSound3()
    {
        audioManager.Play("Deagle Reload3");
    }
}
