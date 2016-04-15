using UnityEngine;
using System.Collections;

public class UI : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
}
