using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class AI : MonoBehaviour
{

    public Transform goal;
    public List<GameObject> friendlyTeam;

    //Memory//
    public Vector3 teamLKP;


    void Awake()
    {
        GetTeam();
    }
    void Start()
    {
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //agent.destination = goal.position;
   
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(CoRoMethod(CommunicatePosition,2f));
    }
    void FixedUpdate()
    {

    }
    private void GetTeam()
    {
        friendlyTeam = new List<GameObject>();
        var t = GameObject.FindGameObjectsWithTag(gameObject.tag.ToString());

        foreach (GameObject gob in t)
        {
            friendlyTeam.Add(gob);
        }
    }

    private void CommunicatePosition()
    {
        foreach (GameObject ob in friendlyTeam)
        {
            if (ob != gameObject)
            {
                ob.GetComponent<AI>().teamLKP = gameObject.transform.position;
                Debug.Log("Fired from " + gameObject.name + " " + Time.realtimeSinceStartup);
            }
        }

    }

   IEnumerator CoRoMethod(Action Method,float waitTime)
    {

        yield return new WaitForSeconds(waitTime);
        Method();
    }


}
