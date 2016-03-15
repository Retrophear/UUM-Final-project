using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameController : MonoBehaviour {

    /* Public Variables*/
    public GameObject bullet; //Reference to the Bullet GO
    public GameObject bluBot; //Reference to the Blue Bot GO
    public GameObject grnBot; //Reference to the Green Bot GO
    public Queue<GameObject> bQueue; //Queue to hold all bullets
    public int bulletAmount; //Int to decide how many bullets are spawned

    /*Private Variables */
    private Queue<GameObject> bluBotPool;
    private Queue<GameObject> grnBotPool;
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

        System.Random rnd = new System.Random(); //Random for spawning the bots 
        int rand = rnd.Next(0, 50);
        for (int i = 0; i<=1; i++)
        {
            rand = rnd.Next(0, 75);
            Instantiate(bluBot, GameObject.FindGameObjectWithTag("BluSpawn").transform.position + new Vector3(rand,0,0), Quaternion.Euler(0,0,0));
        }
        for (int i = 0; i <= 1; i++)
        {
            rand = rnd.Next(0, 75);
            Instantiate(grnBot, GameObject.FindGameObjectWithTag("GrnSpawn").transform.position + new Vector3(rand, 0, 0), Quaternion.Euler(0,180,0));
        }
        #endregion
    }
	void Start () {
        
	
	}
	
	// Update is called once per frame
	void Update () {

	}

    public GameObject RequestBullet()
    {
        return (bQueue.Dequeue() as GameObject);
    }
}
