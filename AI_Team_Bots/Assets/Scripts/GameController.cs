using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
public class GameController : MonoBehaviour
{

    #region Public Vars
    /* Public Variables*/
    public GameObject bullet; //Reference to the Bullet GO
    public GameObject bluBot; //Reference to the Blue Bot GO
    public GameObject grnBot; //Reference to the Green Bot GO
    public Queue<GameObject> bQueue; //Queue to hold all bullets
    public int bulletAmount; //Int to decide how many bullets are spawned
    public int botsPerTeam; //Int to decide how many bots are spawned
    public GameObject blueSpawn; //Spawnpoint for team blue
    public GameObject greenSpawn; //Spawnpoint for team green
    public Text fpsText;
    #endregion

    #region Private vars
    /*Private Variables */
    private System.Random rnd;  //Random for spawning the bots 
    private int rand;
    private List<GameObject> bluBotPool; //Pool for blue bots
    private List<GameObject> grnBotPool; //Pool for green bots
    private float dTime;
    #endregion

    void Awake()
    {
        #region Bullet Spawn
        bQueue = new Queue<GameObject>(); //Intialize the bullet queue
        for (int i = 0; i < bulletAmount; i++) //Loop through and spawn bullets 
        {
            var newBullet = GameObject.Instantiate(bullet) as GameObject; //Instantiate the bullet
            bQueue.Enqueue(newBullet); //Add it to the queue
            newBullet.SetActive(false); //Deactivate it
        }
        #endregion

        #region Bot Spawn
        rnd = new System.Random();
        GameObject obj; //Temp GO to hold bot spawns
        bluBotPool = new List<GameObject>(); //Intitialize the blue bot pool
        grnBotPool = new List<GameObject>(); //Intitalize the green bot pool
        rand = rnd.Next(0, 50); //Get next random number

        for (int i = 0; i<botsPerTeam; i++)// Blue bot spawn
        {
            rand = rnd.Next(0, 50);
            obj =  Instantiate(bluBot, blueSpawn.transform.position + new Vector3(rand,0,0), Quaternion.Euler(0,0,0)) as GameObject;
            obj.name = "BlueBot " + i;
            bluBotPool.Add(obj);
        }
        for (int i = 0; i < botsPerTeam; i++)// Green bot spawn
        {
            rand = rnd.Next(0, 50);
            obj = Instantiate(grnBot, greenSpawn.transform.position + new Vector3(rand, 0, 0), Quaternion.Euler(0,180,0)) as GameObject;
            obj.name = "GreenBot " + i;
            grnBotPool.Add(obj);
        }
        #endregion

        dTime = 0.0f;
        
    }
	void Start () {
        InvokeRepeating("Spawn", 10f, 10f);
      
	}
	
	// Update is called once per frame
	void Update () {
        dTime += (Time.deltaTime - dTime) * 0.1f;
        SystemStats();
	}

    public GameObject RequestBullet()
    {
        return (bQueue.Dequeue() as GameObject);
    }

    private void Spawn()
    {
        foreach(GameObject go in bluBotPool) //Respawn all blue bots
        {
            if(go.activeInHierarchy == false)
            {
                go.SetActive(true);
                rand = rnd.Next(0, 50);
                go.transform.position = blueSpawn.transform.position + new Vector3(rand, 0, 0);
                go.transform.rotation = Quaternion.Euler(0,0,0);

            }
        }
        foreach (GameObject go in grnBotPool) //Respawn all green bots
        {
            if (go.activeInHierarchy == false)
            {
                go.SetActive(true);
                rand = rnd.Next(0, 50);
                go.transform.position = greenSpawn.transform.position +  new Vector3(rand, 0, 0);
                go.transform.rotation = Quaternion.Euler(0, 180, 0);

            }
        }
    }

    private void SystemStats()
    {
        float msec = dTime * 1000.0f;
        float fps = (1.0f / dTime);
        fpsText.text = "FPS: " + Math.Round(fps,0).ToString();
    }

}   