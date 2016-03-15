using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    private GameController gc;
    public int bDmg;

    // Use this for initialization
	void Start () {
        gc = GameObject.FindGameObjectWithTag("gc").GetComponent<GameController>();
        bDmg = 50;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter(Collision other)
    {

        if(other.gameObject.tag == "bTeam" || other.gameObject.tag=="gTeam")
        {
            other.gameObject.GetComponent<FPS_Mechanics>().health -= bDmg;
        }
        
        gc.bQueue.Enqueue(gameObject);
        gameObject.SetActive(false);
        
    }
}
