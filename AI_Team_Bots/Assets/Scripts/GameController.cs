using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameController : MonoBehaviour {

    public GameObject bullet;
    public GameObject bluBot;
    public GameObject grnBot;

    public Queue<GameObject> bQueue;

    void Awake()
    {
        bQueue = new Queue<GameObject>();
        for (int i = 0; i <15; i++)
        {
            var newBullet = GameObject.Instantiate(bullet, new Vector3(13, -15, -15), Quaternion.Euler(90, 0, 0)) as GameObject;
            bQueue.Enqueue(newBullet);

        }
        System.Random rnd = new System.Random();
        int rand = rnd.Next(0, 50);
        for (int i = 0; i<=1; i++)
        {
            rand = rnd.Next(0, 75);
            Instantiate(bluBot, GameObject.FindGameObjectWithTag("BluSpawn").transform.position + new Vector3(rand,0,0), Quaternion.Euler(0,0,0));
        }
        for (int i = 0; i <= 1; i++)
        {
            rand = rnd.Next(0, 75);
            Instantiate(grnBot, GameObject.FindGameObjectWithTag("GrnSpawn").transform.position + new Vector3(rand, 0, 0), Quaternion.Euler(0,0,0));
        }
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
