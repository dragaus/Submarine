using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    GameManager manager;

    //All time UI
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI livesAmountText;
    [SerializeField] Button pauseButton;

    //Temporal Game UI
    [SerializeField] Button nextButton;
    [SerializeField] GameObject finishMenu;
    [SerializeField] Transform starManager;
    [SerializeField] Button homeButton;
    [SerializeField] Button repeatButton;
    [SerializeField] Button nextLevelButton;
    [SerializeField] TextMeshProUGUI finishText;
    [SerializeField] TextMeshProUGUI currentTimeText;
    [SerializeField] TextMeshProUGUI bestTimeText;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        manager.Initilization(this);
        livesAmountText.text = $"X {manager.GetLives()}";
        HideNextButton();
        HideFinishPanel();
    }

    private void Update()
    {
        timeText.text = manager.GetTimeAsString("00");
    }

    #region ButtonCalls
    public void SetNextButton(UnityAction functionOfButton)
    {
        nextButton.onClick.AddListener(() =>
        {
            functionOfButton();
            HideNextButton();
        });
        nextButton.gameObject.SetActive(true);
    }

    public void ShowFinishMenu(bool isLastLevel, bool isNewBestTime, int[] levelStars)
    {
        finishMenu.SetActive(true);
        nextLevelButton.gameObject.SetActive(isLastLevel);
        if (isNewBestTime)
        {
            currentTimeText.text = "New High Score!";
            var separteTime = manager.GetTimeAsString("00.00").Split('.');
            bestTimeText.text = $"{separteTime[0]}:{separteTime[1]}";
            bestTimeText.fontSize = 55;
        }
        else
        {
            var separteTime = manager.GetTimeAsString("00.00").Split('.');
            currentTimeText.text = $"{separteTime[0]}:{separteTime[1]}";
            bestTimeText.text = $"Best time  ";
            bestTimeText.fontSize = 36;
        }

        for (int i = 0; i < levelStars.Length; i++)
        {
            if (levelStars[i] == 1)
            {
                starManager.GetChild(i).GetComponent<Image>().color = Color.yellow;
            }
        }
    }

    public void HideFinishPanel()
    {
        finishMenu.SetActive(false);
    }

    public void HideNextButton()
    {
        nextButton.gameObject.SetActive(false);
    }

    #endregion
}
