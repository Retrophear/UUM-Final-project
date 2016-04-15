using UnityEngine;
using System.Collections;

public class Vision : MonoBehaviour {
    public SphereCollider col;
    public float fov;

    private FPS_Mechanics fpsScript;
    private string enemyTag;

    void Start()
    {
        //Work out which team this object is on.
        switch (transform.parent.tag)
        {
            case "bTeam":
                enemyTag = "gTeam";
                break;
            case "gTeam":
                enemyTag = "bTeam";
                break;
        }

        fpsScript = GetComponentInParent<FPS_Mechanics>();
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == enemyTag)
        {
            CheckVision(other.gameObject);
        }
    }
    private void CheckVision(GameObject target)
    {
        RaycastHit hit;
        Vector3 direction = target.transform.position - transform.parent.position;
        float angle = Vector3.Angle(direction, transform.parent.forward);
        if (angle < fov * 0.5) //Check if the target is in field of view
        {
            if (Physics.Raycast(transform.parent.position + transform.parent.up, direction.normalized, out hit, col.radius))
            {
                if (hit.transform.gameObject.tag == enemyTag) //If enemy is seen
                {
                    fpsScript.Shoot(direction.normalized); //Call the fps script and shoot
                }
            }
        }

    }
}
