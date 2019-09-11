using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager manager;

    [SerializeField] Text timeText;
    [SerializeField] Text livesAmountText;
    [SerializeField] Button nextButton;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        manager.Initilization(this);
        livesAmountText.text = $"X {manager.data.currentGame.currentLives}";
        HideNextButton();
    }

    private void Update()
    {
        timeText.text = manager.data.currentGame.currentTotalTime.ToString("00");
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

    public void HideNextButton()
    {
        nextButton.gameObject.SetActive(false);
    }

    #endregion
}
