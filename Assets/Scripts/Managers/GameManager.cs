using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    UIManager uiManager;

    UserData userData;
    LevelData levelData;

    GameObject subamrine;

    Transform starManager;

    bool submarineIsAlive;

    int levelNumber;

    List<int> starsThatPlayerGet = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        //We add the ui into the scene scine its easier to handle in just one separete scene for all the levels
        SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);

        userData = Resources.Load<UserData>("Data/User/CurrentUserData");
        levelData = Resources.Load<LevelData>("Data/Level/CurrentLevelData");

        //We check if the user data tell us that the player repeat the level or not
        if (!userData.currentGame.isRepeatLevel)
        {
            userData.currentGame.currentTotalTime = 0;
        }

        //The names of the levels by convention are level + _+ number of the level
        //Thats how we can determine that the split will always work 
        levelNumber = int.Parse(SceneManager.GetActiveScene().name.Split('_')[1]);

        //Every try we add a try into the game but the data is only storage when a player dies or beats the level
        levelData.trys[levelNumber]++;

        subamrine = FindObjectOfType<Submarine>().gameObject;

        starManager = GameObject.Find("Star Manager").transform;
        for (int i = 0; i < starManager.childCount; i++)
        {
            starManager.GetChild(i).gameObject.SetActive(levelData.starsInLevels[levelNumber].levelStars[i] == 0);
        }
    }

    public void Initilization(UIManager manager)
    {
        uiManager = manager;
    }

    public void StartMovement()
    {
        submarineIsAlive = true;
    }

    public void NextScene()
    {
        //Save the data
        levelData.wins[levelNumber]++;
        SetStarsCollected();
        Storage.SaveGameInfo();

        userData.currentGame.isRepeatLevel = false;
        userData.generalData.totalTime += userData.currentGame.currentTotalTime;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }


    void Update()
    {
        if (submarineIsAlive)
        {
            userData.currentGame.currentTotalTime += Time.deltaTime;    
        }
    }

    #region InfoSetter
    public void SetStar(int starNumber)
    {
        //We add the stars into a list
        starsThatPlayerGet.Add(starNumber);
    }

    //TThis is used only in case the player wins the level and storage all the stars that the player collect
    public void SetStarsCollected()
    {
        foreach (int i in starsThatPlayerGet)
        {
            levelData.starsInLevels[levelNumber].levelStars[i] = 1;
        }
    }

    #endregion

    #region InfoGetter

    //Pass the amount of lives
    public int GetLives()
    {
        return userData.currentGame.currentLives;
    }

    //Pass the time as string to show it in the manager
    public string GetTimeAsString(string format)
    {
        return userData.currentGame.currentTotalTime.ToString(format);
    }

    #endregion

    #region Submarine Dead

    //This is the public function call in the submarine to start the dead routine
    public void EndGame()
    {
        StartCoroutine(DeadRoutine());
    }

    //This is the corutine call when the submarine hits something
    public IEnumerator DeadRoutine()
    {
        TakeOneLife();
        yield return new WaitUntil(() => subamrine.GetComponent<Rigidbody>().IsSleeping());
        DestroySubamrine(out ParticleSystem explosionEffect);
        yield return new WaitUntil(() => !explosionEffect.isPlaying);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    //We take one life of the player
    void TakeOneLife()
    {
        submarineIsAlive = false;
        userData.currentGame.currentLives--;
        userData.currentGame.isRepeatLevel = true;
        levelData.deads[levelNumber]++;
        Storage.SaveGameInfo();

        //This is set if the player wants to accelerate the proces of the destruction
        uiManager.SetNextButton(()=> 
        {
            subamrine.GetComponent<Rigidbody>().Sleep();
        });
    }

    //We create the effect of destruction
    void DestroySubamrine(out ParticleSystem explosionEffect)
    {
        //Creation of the explosion effect and set in the correct position
        explosionEffect = Instantiate(Resources.Load<GameObject>("Effects/Explosion/Explosion")).GetComponentInChildren<ParticleSystem>();
        explosionEffect.transform.position = subamrine.transform.position;

        //Disable the subamrine to avoid the explosion effect hit the submarine
        subamrine.gameObject.SetActive(false);

        //Disable next button
        uiManager.HideNextButton();
    }

    #endregion
}
