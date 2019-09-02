using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameManager manager;

    [SerializeField]Text timeText;
    [SerializeField] Text livesAmountText;

    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<GameManager>();
        manager.Initilization(this);
        livesAmountText.text = $"X {manager.data.currentGame.currentLives}";
    }

    private void Update()
    {
        timeText.text = manager.data.currentGame.currentTotalTime.ToString("00");
    }
}
