using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadManager : MonoBehaviour
{
    public static string sceneToLoad;
    public static int nextIndex;
    public static bool needTime;
    public static bool isLoadByIndex;

    AsyncOperation asyncOperation;
    // Start is called before the first frame update
    void Start()
    {
        if (isLoadByIndex)
        {
            asyncOperation = SceneManager.LoadSceneAsync(nextIndex);
        }
        else
        {
            asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);
        }
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        asyncOperation.allowSceneActivation = false;

        if (needTime)
        {
            yield return new WaitForSeconds(3f);
            asyncOperation.allowSceneActivation = true;
        }
        else
        {
            asyncOperation.allowSceneActivation = true; 
        }

    }

    public static void LoadNewScene(string nextScene, bool needToWait)
    {
        sceneToLoad = nextScene;
        needTime = needToWait;
        isLoadByIndex = false;
        SceneManager.LoadScene("Loader");
    }

    public static void LoadNewScene(int nextScene, bool needToWait)
    {
        nextIndex = nextScene;
        needTime = needToWait;
        isLoadByIndex = true;
        SceneManager.LoadScene("Loader");
    }
}
