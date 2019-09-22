using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Data/Level Data", order = 2)]
public class LevelData : ScriptableObject
{
    public List<StarsInLevel> starsInLevels = new List<StarsInLevel>();
    public List<float> bestTime = new List<float>();
    public List<int> trys = new List<int>();
    public List<int> deads = new List<int>();
    public List<int> wins = new List<int>();


    public void Initialize()
    {
        if (starsInLevels.Count < GameInfo.numberOfLevels)
        {
            Debug.Log("init");
            for (int i = starsInLevels.Count; i < GameInfo.numberOfLevels; i++)
            {
                starsInLevels.Add(new StarsInLevel());
                bestTime.Add(0);
                trys.Add(0);
                deads.Add(0);
                wins.Add(0);
            }
        }
    }

}

[System.Serializable]
public class StarsInLevel
{
    public int[] levelStars = new int[3];
}
