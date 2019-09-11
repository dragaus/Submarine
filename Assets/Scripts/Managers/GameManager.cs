using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    UIManager uiManager;
    public UserData data;

    GameObject subamrine;

    bool submarineIsAlive;

    // Start is called before the first frame update
    void Awake()
    {
        SceneManager.LoadScene("GameUI", LoadSceneMode.Additive);
        subamrine = FindObjectOfType<Submarine>().gameObject;
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


    void Update()
    {
        if (submarineIsAlive)
        {
            data.currentGame.currentTotalTime += Time.deltaTime;    
        }
    }

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
        data.currentGame.currentLives--;

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
