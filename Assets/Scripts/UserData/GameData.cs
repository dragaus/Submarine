using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int currentLives;
    public bool isRepeatLevel = false;
    public float currentTotalTime;
    public float currentScore;
    public float totalTime;
    public List<StarsInLevel> starsInLevels = new List<StarsInLevel>();
    public List<float> bestTime = new List<float>();
    public List<int> trys = new List<int>();
    public List<int> deads = new List<int>();
    public List<int> wins = new List<int>();
}
