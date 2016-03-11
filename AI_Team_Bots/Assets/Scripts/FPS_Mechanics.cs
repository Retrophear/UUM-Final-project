using UnityEngine;
using System.Collections;

public class FPS_Mechanics : MonoBehaviour {

    int health;
    int bDamage;

    public float fieldOfViewAngle = 110f;          
    public bool enemySighted;
    private SphereCollider col;
    private GameController gc;

    private float lastShot;
    void Awake()
    {
        lastShot = 0;
        health = 200;
        bDamage = 50;
        col = gameObject.GetComponent<SphereCollider>();
        gc = GameObject.FindGameObjectWithTag("gc").GetComponent<GameController>();
    }

	void Start () 
    {
	}

	void Update () {

        Debug.DrawRay(transform.position + transform.up * 1.36f, transform.forward, Color.red,20f);
	}

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == gameObject.tag)
        {
            return;
        }

        if (other.gameObject.tag == "bTeam" || other.gameObject.tag == "gTeam")
        {
     
            enemySighted = false;
 
            // Create a vector from the enemy to the player and store the angle between it and forward.
            Vector3 direction = other.transform.position - transform.position;
            float angle = Vector3.Angle(direction, transform.forward);

            // If the angle between forward and where the player is, is less than half the angle of view...
            if (angle < fieldOfViewAngle * 0.5f)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position + transform.up *1.36f, direction, out hit, col.radius);
                Debug.Log(hit.collider);
                // ... and if a raycast towards the player hits something...
                if (Physics.Raycast(transform.position + transform.up *1.36f, direction, out hit, col.radius))
                {

                    // ... and if the raycast hits the player...
                    if (hit.collider == other.gameObject.GetComponent<CapsuleCollider>())
                    {
                        // ... the player is in sight.
                        enemySighted = true;
                        Shoot(direction);
                    }
                }
            }

        }
    }
    void Shoot(Vector3 direction)
    {
        if (lastShot + 3 > Time.realtimeSinceStartup)
            return;

        GameObject tBullet = gc.bQueue.Dequeue();
        Rigidbody rb = tBullet.GetComponent<Rigidbody>();
        tBullet.transform.position = (gameObject.transform.position + transform.up);
        rb.AddForce(direction * 50f);

        lastShot = Time.realtimeSinceStartup;
    }

    
}
