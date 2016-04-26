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
    public Text score;

    public int bluScore;
    public int grnScore;

    // Use this for initialization
    void Start()
    {
        bluScore = 0;
        grnScore = 0;
        menu.gameObject.SetActive(false);
        InvokeRepeating("BotStatus", 1f, 0.4f);
        InvokeRepeating("UpdateScore", 10f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
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
    public void BotStatus()
    {
        string text = "";
        foreach(GameObject go in botList)
        {
            text += go.name  + " Current Role: " + go.GetComponent<AI>().squadRoles.FirstOrDefault(x=> x.Value == go).Key + 
                            " State: " + go.GetComponent<AI>().currentState + " Event: " + go.GetComponent<AI>().currentKeyEvent + 
                             "\n";
        }
        botText.text =  text;
    }
    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void UnPauseGame()
    {
        Time.timeScale = 1;
    }
    public void Fast()
    {
        Time.timeScale = 1.5f;
    }
    public void Faster()
    {
        Time.timeScale = 2;
    }

    private void UpdateScore()
    {
        score.text = "Blue Team: " + bluScore + "Green Team: " + grnScore;
    }
}
