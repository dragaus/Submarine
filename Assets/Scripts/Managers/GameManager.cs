using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    UIManager uiManager;

    UserData userData;
    LevelData levelData;

    GameObject submarine;

    Transform starManager;

    Vector3 finalPos;

    bool submarineIsAlive;
    bool submarineIsTrascending;
    bool submarineIsInPosition;

    int levelNumber;

    List<int> starsThatPlayerGet = new List<int>();

    float parkingSpeed = 2f;

    // Start is called before the first frame update
    void Awake()
    {
        //We add the ui into the scene scine its easier to handle in just one separete scene for all the levels
        SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);

        //The names of the levels by convention are level + _+ number of the level
        //Thats how we can determine that the split will always work 
        levelNumber = int.Parse(SceneManager.GetActiveScene().name.Split('_')[1]);

        submarine = FindObjectOfType<Submarine>().gameObject;

        starManager = GameObject.Find("Star Manager").transform;
    }

    public async void Initilization(UIManager manager)
    {
        uiManager = manager;

        await LoadData();

        //Every try we add a try into the game but the data is only storage when a player dies or beats the level
        levelData.trys[levelNumber]++;

        //We check if the user data tell us that the player repeat the level or not
        if (!userData.currentGame.isRepeatLevel)
        {
            userData.currentGame.currentTotalTime = 0;
        }

        for (int i = 0; i < starManager.childCount; i++)
        {
            starManager.GetChild(i).gameObject.SetActive(levelData.starsInLevels[levelNumber].levelStars[i] == 0);
        }
    }

    public async Task<bool> LoadData() 
    {
        var dataUser = Addressables.LoadAssetAsync<UserData>(GameInfo.userDataPath);

        while (!dataUser.IsDone)
        {
            await Task.Yield();
        }

        userData = dataUser.Result;

        var dataLevel = Addressables.LoadAssetAsync<LevelData>(GameInfo.levelDataPath);

        while (!dataLevel.IsDone)
        {
            await Task.Yield();
        }

        levelData = dataLevel.Result;

        return true;
    }

    public void StartMovement()
    {
        submarineIsAlive = true;
    }

    void Update()
    {
        if (submarineIsAlive)
        {
            userData.currentGame.currentTotalTime += Time.deltaTime;    
        }
        if (submarineIsTrascending)
        {
            submarine.transform.eulerAngles = new Vector3(0, 0, 0);

            var position = submarine.transform.position;

            if (Vector3.Distance(position, finalPos) > 0.3f)
            {
                submarine.transform.Translate((finalPos - submarine.transform.position).normalized * Time.deltaTime * parkingSpeed);
            }
            else
            {
                submarine.transform.position = finalPos;
                submarine.GetComponent<Rigidbody>().Sleep();
            }

            if (submarine.transform.position == finalPos)
            {
                submarineIsInPosition = true;
            }
        }
    }

    #region InfoSetter
    public void SetStar(int starNumber)
    {
        //We add the stars into a list
        starsThatPlayerGet.Add(starNumber);

        //We generate a sound to make clear to player that he grab an star
        var sound = new GameObject().AddComponent<AudioSource>();
        sound.clip = Resources.Load<AudioClip>("Audio/SFX/Submarine/Star");
        StartCoroutine(DestroydSound(sound));
    }

    //Use to destroy sounds that will be only genarate for a while
    IEnumerator DestroydSound(AudioSource audioSource)
    {
        audioSource.Play();
        yield return new WaitUntil(() => !audioSource.isPlaying);
        Destroy(audioSource.gameObject);
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
        yield return new WaitUntil(() => submarine.GetComponent<Rigidbody>().IsSleeping());
        DestroySubamrine(out ParticleSystem explosionEffect);
        yield return new WaitUntil(() => !explosionEffect.isPlaying);
        DeadResult();
    }

    //We take one life of the player
    void TakeOneLife()
    {
        submarineIsAlive = false;
        userData.currentGame.currentLives--;
        userData.currentGame.isRepeatLevel = true;
        levelData.deads[levelNumber]++;
        Storage.SaveGameInfo(userData, levelData);

        //This is set if the player wants to accelerate the proces of the destruction
        uiManager.SetNextButton(()=> 
        {
            submarine.GetComponent<Rigidbody>().Sleep();
        });
    }

    //We create the effect of destruction
    void DestroySubamrine(out ParticleSystem explosionEffect)
    {
        //Creation of the explosion effect and set in the correct position
        explosionEffect = Instantiate(Resources.Load<GameObject>("Effects/Explosion")).GetComponentInChildren<ParticleSystem>();
        explosionEffect.transform.position = submarine.transform.position;

        //Disable the subamrine to avoid the explosion effect hit the submarine
        submarine.gameObject.SetActive(false);

        //Disable next button
        uiManager.HideNextButton();
    }

    void DeadResult()
    {
        if (userData.currentGame.currentLives >= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            LoadManager.LoadNewScene("EndMenu", true);
        }
    }

    #endregion

    #region Subamarine Win

    public void WinLevel(GameObject finishPad)
    {
        finalPos = finishPad.transform.position;
        finalPos.y += 1.5f;
        submarine.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;

        //This storages the data of the player
        submarineIsAlive = false;
        submarineIsTrascending = true;
        levelData.wins[levelNumber]++;
        SetStarsCollected();
        Storage.SaveGameInfo(userData, levelData);

        userData.currentGame.isRepeatLevel = false;
        userData.generalData.totalTime += userData.currentGame.currentTotalTime;

        StartCoroutine(WinRoutine(finishPad));
    }

    IEnumerator WinRoutine(GameObject finishPad)
    {
        yield return new WaitUntil(() => submarineIsInPosition);

        var particles = finishPad.GetComponentInChildren<ParticleSystem>();
        particles.Play();

        submarine.transform.eulerAngles = finishPad.transform.eulerAngles;

        yield return new WaitForSeconds(5f);
        NextScene();
    }

    public void NextScene()
    {
        LoadManager.LoadNewScene(SceneManager.GetActiveScene().buildIndex + 1, false);
    }
    #endregion
}
