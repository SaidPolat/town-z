using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelBorder : MonoBehaviour
{
    bool isOutOfMap = false;
    private float timeTillStart = 0;
    private bool startTime = false;
    private bool stopTimer;
    private bool restartTime = false;
    public TextMeshProUGUI timeText;
    public GameObject leavingAreaPanel;

    private void Update()
    {
        if (startTime == true)
        {
            //Debug.Log("if içi stop timer " + stopTimer);
            timeTillStart += Time.deltaTime;
            double time = 10.0 - timeTillStart;
            //Debug.Log("if içi time " + time);
            if (time <= 0)
            {
                stopTimer = true;
                StartCoroutine(FindObjectOfType<GameControl>().PlayerDead("lose"));
            }
            if (!stopTimer)
            {
                timeText.text = "Time Left: " + time.ToString("F2");
            }        
        }
        if (restartTime)
        {
            timeTillStart = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isOutOfMap)
            {
                isOutOfMap = true;
                leavingAreaPanel.SetActive(true);
                restartTime = false;
                stopTimer = false;
                startTime = true;
            }
            else
            {
                isOutOfMap = false;
                leavingAreaPanel.SetActive(false);
                startTime = false;
                restartTime = true;                
            }
            
        }
    }
}
