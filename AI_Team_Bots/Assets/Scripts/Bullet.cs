using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    private GameController gc;
    public int bDmg;

    public float time;

    // Use this for initialization
	void Start () {
        gc = GameObject.FindGameObjectWithTag("gc").GetComponent<GameController>();
        bDmg = 50;

	}
	void FixedUpdate()
    {
        if(time > 0)
        {
            time -= Time.deltaTime;
        }
        if(time <= 0 && gameObject.activeInHierarchy)
        {
            gc.bQueue.Enqueue(gameObject);
            gameObject.SetActive(false);
        }
    }
    void OnCollisionEnter(Collision other)
    {

        if(other.gameObject.tag == "bTeam" || other.gameObject.tag=="gTeam")
        {
            other.gameObject.GetComponent<FPS_Mechanics>().health -= bDmg;
        }
        
        gc.bQueue.Enqueue(gameObject);
        time = 0;
        gameObject.SetActive(false);
        
    }
}
