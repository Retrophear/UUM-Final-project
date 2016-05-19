using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public Text botText;
    public List<GameObject> botList = new List<GameObject>();
    public Image menu;
    public Image AIOrders;
    public GameObject confirm;
    public GameObject help;
    public Text score;
    public Text kills;
    public Text scoreTime;
    public int bluScore;
    public int grnScore;
    public int bluDeaths;
    public int grnDeaths;
    public float lastScored;

    private string confirmButton;

    void Start()
    {
        bluScore = 0;
        grnScore = 0;
        bluDeaths = 0;
        grnDeaths = 0;
        lastScored = 0;
        menu.gameObject.SetActive(false);
        confirm.SetActive(false);
        help.SetActive(false);
        InvokeRepeating("BotStatus", 1f, 0.4f);
        InvokeRepeating("UpdateScore", 10f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenu();
        }
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UnPauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Fast();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Faster();
        }

        
    }
    public void BotStatusButton()
    {
        if(AIOrders.gameObject.activeInHierarchy == true)
        {
            AIOrders.gameObject.SetActive(false);
        }
        else if (AIOrders.gameObject.activeInHierarchy == false)
        {
            AIOrders.gameObject.SetActive(true);
        }
    }
    public void BotStatus()
    {
        string text = "";
        if (AIOrders.gameObject.activeInHierarchy == true)
        {
            foreach (GameObject go in botList)
            {
                text += go.name + " Current Role: " + go.GetComponent<AI>().squadRoles.FirstOrDefault(x => x.Value == go).Key +
                                " State: " + go.GetComponent<AI>().currentState + " Event: " + go.GetComponent<AI>().currentKeyEvent +
                                 "\n";
            }
            botText.text = text;
        }
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void UnPauseGame()
    {
        Time.timeScale = 1;
    }
    public void OpenHelp()
    {
        help.SetActive(true);
    }
    public void CloseHelp()
    {
        help.SetActive(false);
    }
    public void Fast()
    {
        Time.timeScale = 1.5f;
    }
    public void Faster()
    {
        Time.timeScale = 2;
    }
    public void BackToGame()
    {
        menu.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void OpenMenu()
    {
        menu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }
    public void OpenConfirm (string callingButton)
    {
        confirmButton = callingButton;
        menu.gameObject.SetActive(false);
        confirm.SetActive(true);


    }
    public void Confirm(string button)
    {
        switch(button)
        {
            case "Yes":
                switch(confirmButton)
                {
                    case "Reset":
                        Reset();
                        break;
                    case "Quit":
                        Quit();
                        break;
                }
                break;
            case "No":
                confirmButton = "";
                confirm.SetActive(false);
                menu.gameObject.SetActive(true);
                break;
        }
    }
    private void Quit()
    {
        Application.Quit();
    }
    private void Reset()
    {
        Time.timeScale = 1;
        Application.LoadLevel(Application.loadedLevelName);
    }
    private void UpdateScore()
    {
        score.text = "BTeam: " + bluScore + " GTeam: " + grnScore;
        kills.text = "BKills: " + grnDeaths + " GKills: " + bluDeaths;
        //scoreTime.text = "Last Score: " + lastScored;
    }
}
