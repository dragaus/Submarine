using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    UIManager uiManager;
    public UserData data;

    bool submarineIsAlive;

    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void EndGame()
    {
        StartCoroutine(DeadRoutine());
    }

    public IEnumerator DeadRoutine()
    {
        submarineIsAlive = false;
        data.currentGame.currentLives--;
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void Update()
    {
        if (submarineIsAlive)
        {
            data.currentGame.currentTotalTime += Time.deltaTime;    
        }
    }
}
