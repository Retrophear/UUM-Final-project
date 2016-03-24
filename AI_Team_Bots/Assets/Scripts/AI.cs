using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using FullInspector;

public class AI : BaseBehavior
{

    public Transform goal;

    //Memory//
    public Dictionary<GameObject, Vector3> friendlyTeam;
    public Dictionary<string, GameObject> squadRoles;
    public NavMeshAgent agent;
    
    public GameObject personalGoal;
    void Awake()
    {
        squadRoles.Add("ObjectiveCarrier", null);
        squadRoles.Add("Sniper", null);
        squadRoles.Add("Attacker", null);
        squadRoles.Add("Defender1", null);
        squadRoles.Add("Defender2", null);
    }
    void Start()
    {
        switch (gameObject.tag)
        {
            case "bTeam":
                goal = GameObject.Find("BLUEGOAL").transform;
                break;
            case "gTeam":
                goal = GameObject.Find("GREENGOAL").transform;
                break;
        }
        GetTeam();
        InvokeRepeating("CommunicatePosition", 0.4f, 1f);

         agent = GetComponent<NavMeshAgent>();

        if (personalGoal)
        {
            agent.destination = personalGoal.transform.position;
        }
        InvokeRepeating("ExecuteBehaviour", 0.8f, 0.5f);
    }
    void OnEnable()
    {
       
    }
    // Update is called once per frame
    void Update()
    {

    }

    private void GetTeam()
    {
        friendlyTeam = new Dictionary<GameObject, Vector3>(); // Intialize GO List
        var t = GameObject.FindGameObjectsWithTag(gameObject.tag); //Find all friendly bots

        foreach (GameObject gob in t) //Loop and add each bot tto the list
        {
            if (gob != gameObject)
            {
                friendlyTeam.Add(gob, gob.transform.position);
            }
        }

        AssignSquad();
        // teamLKP = new Vector3[friendlyTeam.Count]; //Intialize the teammate postion memory to the number of friendly bots
    }

    private void CommunicatePosition()
    {
        //int squadPostion;

        //squadPostion = Int32.Parse(Regex.Match(gameObject.name, @"\d+").Value);
        foreach (KeyValuePair<GameObject, Vector3> ob in friendlyTeam)
        {
            if (ob.Key != gameObject)
            {

                ob.Key.GetComponent<AI>().friendlyTeam[gameObject] = gameObject.transform.position;

            }
        }

    }

    private void CommmunicateSquad(string sqRole)
    {
        squadRoles[sqRole] = gameObject;
        foreach (KeyValuePair<GameObject, Vector3> ob in friendlyTeam)
        {
            if (ob.Key != gameObject)
            {
                ob.Key.GetComponent<AI>().squadRoles[sqRole] = gameObject;
            }
        }
    }
    private void AssignSquad()
    {
        GameObject sqObjc = null;
        #region Assign Objective Carrier
        //Determine whos the Objective Carrier
        float myPos = Vector3.Distance(transform.position, goal.transform.position);
        float tPos = 1000f;
        foreach (KeyValuePair<GameObject, Vector3> dis in friendlyTeam)
        {
            if (tPos > (Vector3.Distance(dis.Value, goal.position)))
            {
                tPos = Vector3.Distance(dis.Value, goal.position);
                sqObjc = dis.Key;
            }
        }
        if (tPos >= myPos)
        {            
            CommmunicateSquad("ObjectiveCarrier");
            personalGoal = goal.gameObject;
        }
        #endregion

        #region Get Sniper
        //Determine whos the sniper
        if (!squadRoles.ContainsValue(gameObject) && squadRoles["Sniper"] == null)//Check if this object has already been assigned to a role
        {
            Vector3 hPoint = GameObject.Find("HighPoint").transform.position;

            myPos = Vector3.Distance(transform.position, hPoint);
            tPos = 1000f;
            foreach (KeyValuePair<GameObject, Vector3> dis in friendlyTeam)
            {
                if (!squadRoles.ContainsValue(dis.Key)) //Skip any already assigned squad members
                {
                    if (tPos > (Vector3.Distance(dis.Value, hPoint)))
                    {
                        tPos = Vector3.Distance(dis.Value, hPoint);
                    }
                }
            }
            if (tPos >= myPos)
            {
                CommmunicateSquad("Sniper");
                personalGoal = GameObject.Find("HighPoint");
            }
        }
        #endregion

        #region Get Attacker
        if (!squadRoles.ContainsValue(gameObject) && squadRoles["Attacker"] == null)//Check if this object has already been assigned to a role
        {
            myPos = Vector3.Distance(transform.position, sqObjc.transform.position);
            tPos = 1000f;
            foreach (KeyValuePair<GameObject, Vector3> dis in friendlyTeam)
            {
                if (!squadRoles.ContainsValue(dis.Key) && dis.Key != sqObjc) //Skip assigned AND skip the to-be-assigned OC
                {
                    if (tPos > (Vector3.Distance(dis.Value, sqObjc.transform.position)))
                    {
                        tPos = Vector3.Distance(dis.Value, sqObjc.transform.position);
                    }
                }
            }
            if (tPos >= myPos)
            {
                CommmunicateSquad("Attacker");
                personalGoal = sqObjc;
            }

        }
        #endregion

        #region AssignDefenders
        if (!squadRoles.ContainsValue(gameObject))
        {
            if (!squadRoles["Defender1"])
            {
                CommmunicateSquad("Defender1");
                switch (gameObject.tag)
                {
                    case "bTeam":
                        personalGoal = GameObject.Find("GREENGOAL");
                        break;
                    case "gTeam":
                        personalGoal = GameObject.Find("BLUEGOAL");
                        break;
                }
            }
            else
            {
                CommmunicateSquad("Defender2");
                switch (gameObject.tag)
                {
                    case "bTeam":
                        personalGoal = GameObject.Find("GREENGOAL");
                        break;
                    case "gTeam":
                        personalGoal = GameObject.Find("BLUEGOAL");
                        break;
                }
            }
        }
        #endregion
    }
    private void ExecuteBehaviour()
    {
        string sqRole ="";
        foreach(KeyValuePair<string, GameObject> go in squadRoles)
        {
            if(go.Value == gameObject)
            {
                sqRole = go.Key;
            }
        }
        switch(sqRole)
        {
            case "ObjectiveCarrier":
                break;
            case "Sniper":
                break;
            case "Attacker":
                agent.SetDestination(goal.transform.position);
                break;
            case "Defender1":
                if (goal.name == "GREENGOAL" || goal.name == "BLUEGOAL")
                {
                    Vector3 faceDirection = new Vector3(0, 0, 0);
                    float dist = agent.remainingDistance;
                    if (agent.remainingDistance < 5)
                    {
                        agent.Stop();
                        gameObject.transform.LookAt(faceDirection);

                    }
                }
                break;
            case "Defender2":

                break;
        }
    }
}


