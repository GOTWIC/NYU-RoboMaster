using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The purpose of this class is to save the player's name input on the main menu screen
// This requires a seperate script becuase the room player is not created until host/join is selected
// Once host/join is selected, the room player is created, and the player name is handed off to the room player object

public class Name : MonoBehaviour
{
    [SerializeField] private string playerName = "";

    public void setPlayerName(string name)
    {
        playerName = name;
    }

    public string getPlayerName()
    {
        return playerName;
    }
}
