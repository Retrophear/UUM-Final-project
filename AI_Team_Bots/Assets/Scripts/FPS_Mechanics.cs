using UnityEngine;
using System.Collections;

public class FPS_Mechanics : MonoBehaviour {

    public int health;
    public GameObject enemy;
    public float reFireRate;
    public float bulletSpeed;
    public Animator anim;
    private GameController gc;
    private float lastShot;
    private int lastHP;


    void Awake()
    {
        lastShot = 0;
        health = 200;
        gc = GameObject.FindGameObjectWithTag("gc").GetComponent<GameController>();

        InvokeRepeating("CheckHP", 0, 0.4f);

    }

	void Start () 
    {
         lastHP = health;   
	}

     
    public void Shoot(Vector3 direction)
    {
        if (lastShot + reFireRate > Time.realtimeSinceStartup)
            return;

        GameObject tBullet = gc.bQueue.Dequeue();
        tBullet.SetActive(true);

        Physics.IgnoreCollision(tBullet.GetComponent<Collider>(), GetComponent<Collider>());
        Physics.IgnoreCollision(tBullet.GetComponent<Collider>(), tBullet.GetComponent<Collider>());

        
        Rigidbody rb = tBullet.GetComponent<Rigidbody>();
        tBullet.transform.position = (gameObject.transform.position + transform.up + (transform.forward * 2.5f));
        tBullet.transform.rotation = Quaternion.Euler(direction);
        
    
        rb.AddForce(direction * bulletSpeed);

        lastShot = Time.realtimeSinceStartup;
    }
    
    void CheckHP()
    {
        if(health <= 0)
        {
            CancelInvoke();

            try
            {
                var child = gameObject.transform.FindChild("BLUEGOAL");
                if(child != null)
                {
                    child.transform.parent = null;
                    child.transform.position -= new Vector3(0, 5, 0);
                }
                child = gameObject.transform.FindChild("GREENGOAL");
                if(child != null)
                {
                    child.transform.parent = null;
                    child.transform.position -= new Vector3(0, 5, 0);
                }
            }
            catch
            {

            }
            gameObject.SetActive(false);
        }


        else if(health < lastHP)
        {
            gameObject.GetComponent<AI>().CallHelp(enemy);
            lastHP = health;
        }
    }

    public void OnReEnable()
    {
        health = 200;
        InvokeRepeating("CheckHP", 0, 0.4f);
    }
}
