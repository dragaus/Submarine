using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "Data/User Data", order = 1)]
public class UserData : ScriptableObject
{
    [Tooltip("This are the values usesd in the current game to give info to the player")]
    public CurrentGame currentGame;
    [Tooltip("This are the values usesd in the game to mantain the player information")]
    public GeneralData generalData;
}

[System.Serializable]
public class CurrentGame
{
    public int currentLives;
    public float currentTotalTime;
    public float currentScore;
}

[System.Serializable]
public class GeneralData
{
    public int trys;
    public int deads;
    public int wins;
    public float totalTime;
    public float maxScore;
}
