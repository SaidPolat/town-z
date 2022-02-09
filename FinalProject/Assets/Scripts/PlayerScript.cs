using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript
{
    public string playerName;
    public int playerScore;

    public PlayerScript(string playerName, int playerScore)
    {
        this.playerName = playerName;
        this.playerScore = playerScore;
    }

    public override string ToString()
    {
        return "isim: " + this.playerName + "   skor: " + this.playerScore;
    }
}
