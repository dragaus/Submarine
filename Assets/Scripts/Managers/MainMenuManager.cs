using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        playButton.gameObject.SetActive(false);

        if (Storage.CanLoadData())
        {
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
            playButton.gameObject.SetActive(true);
            Debug.Log($"trys in level 1 is {levelData.trys[0]}");
        }
        else
        {
            playButton.gameObject.SetActive(true);
        }

        levelData.Initialize();

        Storage.SaveGameInfo();

        playButton.onClick.AddListener(GoToLevel0);
    }

    void GoToLevel0()
    {
        userData.currentGame.currentLives = GameInfo.lives;
        userData.currentGame.isRepeatLevel = false;
        LoadManager.LoadNewScene("Level_0", true);
    }
}

[System.Serializable]
public struct DataToSerialize
{
    public UserData userData;
    public LevelData levelData;
}