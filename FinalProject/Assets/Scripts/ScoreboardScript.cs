using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreboardScript : MonoBehaviour
{
    public TextMeshProUGUI scoreNames;
    public TextMeshProUGUI scores;

    List<int> scoresSorted = new List<int>();
    List<PlayerScript> playersList = new List<PlayerScript>();

    void Start()
    {
        if (MainMenuScript.namesList.Count > 0)
        {
            for (int i = 0; i < MainMenuScript.namesList.Count; i++)
            {
                playersList.Add(new PlayerScript(MainMenuScript.namesList[i], GameControl.scoreList[i]));
            }

            foreach (int item in GameControl.scoreList)
            {
                scoresSorted.Add(item);
            }

            scoresSorted.Sort();
            scoresSorted.Reverse();

            for (int i = 0; i < 5; i++)
            {
                if (i < scoresSorted.Count)
                {
                    for (int n = 0; n < playersList.Count; n++)
                    {
                        if (scoresSorted[i] == playersList[n].playerScore)
                        {
                            scoreNames.text += (i + 1) + ") " + playersList[n].playerName + "\n";
                            scores.text += playersList[n].playerScore + "\n";
                            break;
                        }

                    }

                }

            }
        }
    }
}
