using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Button playButton;
    LevelData levelData;
    UserData userData;
    // Start is called before the first frame update
    void Start()
    {
        levelData = Resources.Load<LevelData>(GameInfo.levelDataPath);
        userData = Resources.Load<UserData>(GameInfo.userDataPath);

        if (Storage.CanLoadData())
        {
            Debug.Log("I'm loading data");
            var gameData = JsonUtility.FromJson<GameData>(Storage.LoadJsonData(GameInfo.savingData));

            //Recovering level data values 
            levelData.bestTime = gameData.bestTime;
            levelData.deads = gameData.deads;
            levelData.starsInLevels = gameData.starsInLevels;
            levelData.trys = gameData.trys;
            levelData.wins = gameData.wins;

            //Recovering user data values
            userData.currentGame.currentLives = gameData.currentLives;
            userData.currentGame.currentScore = gameData.currentScore;
            userData.currentGame.currentTotalTime = gameData.currentTotalTime;
            userData.currentGame.isRepeatLevel = gameData.isRepeatLevel;
            userData.generalData.totalTime = gameData.totalTime;
        }

        Debug.Log(JsonUtility.ToJson(levelData));

        levelData.Initialize();

        Storage.SaveGameInfo();

        playButton.onClick.AddListener(GoToLevel0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoToLevel0()
    {
        SceneManager.LoadScene("Level_0");
    }
}

[System.Serializable]
public struct DataToSerialize
{
    public UserData userData;
    public LevelData levelData;
}