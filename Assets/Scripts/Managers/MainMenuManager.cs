using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class MainMenuManager : MonoBehaviour
{
    public Button playButton;
    LevelData levelData;
    UserData userData;
    // Start is called before the first frame update
    void Start()
    {
        Initialization();


    }

    async void Initialization() 
    {
        playButton.onClick.AddListener(GoToLevel0);
        playButton.gameObject.SetActive(true);

        Debug.Log("In Order 0");

        await GetData();

        Debug.Log("In Order 2");

        levelData.Initialize();

        Storage.SaveGameInfo(userData, levelData);

    }

    async Task<bool> GetData() 
    {
        var levelInAddress = Addressables.LoadAssetAsync<LevelData>(GameInfo.levelDataPath);

        while (!levelInAddress.IsDone) 
        {
            await Task.Yield();
        }

        levelData = levelInAddress.Result;

        var userInAddress = Addressables.LoadAssetAsync<UserData>(GameInfo.userDataPath);

        while (!userInAddress.IsDone)
        {
            await Task.Yield();
        }

        userData = userInAddress.Result;

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
            Debug.Log($"trys in level 1 is {levelData.trys[0]}, Order 1");
        }
        else
        {
            playButton.gameObject.SetActive(true);
        }

        return true;
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