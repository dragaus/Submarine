using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndManger : MonoBehaviour
{
    public TextMeshProUGUI text;

    public Button menuButton;
    // Start is called before the first frame update
    void Start()
    {
        text.text = "Game Over";

        menuButton.onClick.AddListener(GoMenu);

        menuButton.GetComponentInChildren<TextMeshProUGUI>().text = "Menu";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GoMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
