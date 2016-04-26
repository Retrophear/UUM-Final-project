using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
//using FullInspector;

public class AI : MonoBehaviour
{
    public Transform goal;
    public NavMeshAgent agent;
    public Dictionary<GameObject, Vector3> friendlyTeam; //Holds all members of the friendly team and their last communicated postion
    public Dictionary<string, GameObject> squadRoles; //Holds all the squad roles and who currently is assigned to them
    public States currentState; //Current state
    public ImportantEvents currentKeyEvent; //Current key event
    public GameObject personalGoal; //Bots personal goal dictated by their squad role
    public GameObject supportTarget; //Target if the bot is asked to assist another teammate under attack
    public Transform importantTarget; //Last known location of friendly flag
    public GameObject attackTarget; //Target for the AI to attack
    public bool objStolen;
    public float targetUpdated; 

    void Awake()
    {
        squadRoles = new Dictionary<string, GameObject>();
        squadRoles.Add("ObjectiveCarrier", null);
        squadRoles.Add("Sniper", null);
        squadRoles.Add("Attacker", null);
        squadRoles.Add("Defender1", null);
        squadRoles.Add("Defender2", null);
        if (gameObject.name.Contains("blu"))
        {
            GameObject.Find("UI").GetComponent<UI>().botList.Add(gameObject);

        }
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
        InvokeRepeating("ExecuteBehaviour", 0.8f, 0.4f);
        InvokeRepeating("CheckSquad", 4f, 0.5f);
        currentState = States.Squad;
        currentKeyEvent = ImportantEvents.None;
    }
    void OnDisable()
    {
        CancelInvoke();
        foreach (KeyValuePair<string, GameObject> sm in squadRoles) //Cycle through the squad roles
        {
            if (sm.Key == "ObjectiveCarrier" && sm.Value == gameObject) //If this object is the objective carrier
            {
                foreach (KeyValuePair<GameObject, Vector3> ft in friendlyTeam) //Loop through the friendly team
                {
                    if (ft.Key.activeInHierarchy) //Check if their active
                    {
                        ft.Key.GetComponent<AI>().currentKeyEvent = ImportantEvents.CarrierDead; //Inform them that OC is dead.
                    }
                }
            }
        }
    }


    /* PRIVATE METHODS*/
    private void GetTeam()
    {
        friendlyTeam = new Dictionary<GameObject, Vector3>(); // Intialize GO List
        var t = GameObject.FindGameObjectsWithTag(gameObject.tag); //Find all friendly bots

        foreach (GameObject gob in t) //Loop and add each bot to the list
        {
            if (gob != gameObject)
            {
                friendlyTeam.Add(gob, gob.transform.position);
            }
        }

        AssignSquad();
    }
    private void CommunicatePosition()
    {
        foreach (KeyValuePair<GameObject, Vector3> ob in friendlyTeam)
        {
            if (ob.Key != gameObject)
            {
                if (ob.Key.activeInHierarchy)
                {
                    ob.Key.GetComponent<AI>().friendlyTeam[gameObject] = gameObject.transform.position;
                }

            }
        }

    }
    private void CommmunicateSquad(string sqRole)
    {
        squadRoles[sqRole] = gameObject;
        var dictionaryBuffer = new List<string>(squadRoles.Keys);

        foreach (var key in dictionaryBuffer)
        {
            if (squadRoles[key] == gameObject && key != sqRole)
            {
                squadRoles[key] = null;
            }
        }
        foreach (KeyValuePair<GameObject, Vector3> ob in friendlyTeam)
        {
            if (ob.Key != gameObject)
            {
                var tmSqRole = ob.Key.GetComponent<AI>().squadRoles;
                dictionaryBuffer = new List<string>(tmSqRole.Keys);
                foreach(var key in dictionaryBuffer)
                {
                    if(tmSqRole[key] == gameObject && key != sqRole)
                    {
                        tmSqRole[key] = null;
                    }
                }
                ob.Key.GetComponent<AI>().squadRoles[sqRole] = gameObject;
                if (sqRole == "ObjectiveCarrier" && ob.Key.GetComponent<AI>().currentKeyEvent == ImportantEvents.CarrierDead)
                {
                    ob.Key.GetComponent<AI>().currentKeyEvent = ImportantEvents.None;
                }
            }
        }
    }
    private void CommunicateImportantLocation(Transform pos, float time)
    {
        foreach(KeyValuePair<GameObject,Vector3> ft in friendlyTeam)
        {
            if(ft.Key.GetComponent<AI>().targetUpdated < time)
            {
                ft.Key.GetComponent<AI>().targetUpdated = time;
                ft.Key.GetComponent<AI>().importantTarget = pos;
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
            if (dis.Key.activeInHierarchy)
            {
                if (tPos > (Vector3.Distance(dis.Value, goal.position)))
                {
                    tPos = Vector3.Distance(dis.Value, goal.position);
                    sqObjc = dis.Key;
                }
            }
        }
        if (tPos >= myPos)
        {
            CommmunicateSquad("ObjectiveCarrier");
            personalGoal = goal.gameObject;
            if (currentKeyEvent == ImportantEvents.CarrierDead)
            {
                currentKeyEvent = ImportantEvents.None;
            }
        }
        #endregion

        #region Get Sniper
        //Determine whos the sniper
        if (!squadRoles.ContainsValue(gameObject) && squadRoles["Sniper"] == null)//Check if this object has already been assigned to a role
        {
            GameObject hPoint = null; 
            switch(gameObject.tag)
            { 
                case "bTeam":
                 hPoint = GameObject.Find("HighPointBlue");
                 break;
                case "gTeam":
                 hPoint = GameObject.Find("HighPointGreen");
                 break;
            }

            myPos = Vector3.Distance(transform.position, hPoint.transform.position);
            tPos = 1000f;
            foreach (KeyValuePair<GameObject, Vector3> dis in friendlyTeam)
            {
                if (dis.Key.activeInHierarchy)
                {
                    if (!squadRoles.ContainsValue(dis.Key)) //Skip any already assigned squad members
                    {
                        if (tPos > (Vector3.Distance(dis.Value, hPoint.transform.position)))
                        {
                            tPos = Vector3.Distance(dis.Value, hPoint.transform.position);
                        }
                    }
                }
            }
            if (tPos >= myPos)
            {
                CommmunicateSquad("Sniper");
                personalGoal = hPoint;
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
                if (dis.Key.activeInHierarchy)
                {
                    if (!squadRoles.ContainsValue(dis.Key) && dis.Key != sqObjc) //Skip assigned AND skip the to-be-assigned OC
                    {
                        if (tPos > (Vector3.Distance(dis.Value, sqObjc.transform.position)))
                        {
                            tPos = Vector3.Distance(dis.Value, sqObjc.transform.position);
                        }
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
        string sqRole = "";
        SquadCleaner();
        foreach (KeyValuePair<string, GameObject> go in squadRoles)
        {
            if (go.Value == gameObject)
            {
                sqRole = go.Key;
            }
        }
        if (currentKeyEvent == ImportantEvents.CarrierDead)
        {
            AssignSquad();
        }
        if(currentKeyEvent == ImportantEvents.FriendlyObjStolen && sqRole.Contains("Defender"))
        {
            currentState = States.Retrieving;
        }
        switch (currentState)
        {
            #region Squad Behaviours
            case States.Squad:
                switch (sqRole)
                {
                    case "ObjectiveCarrier":
                        if (gameObject.transform.childCount > 1)
                        {
                            switch (gameObject.tag)
                            {
                                case "bTeam":
                                    personalGoal = GameObject.Find("GREENGOAL");
                                    agent.SetDestination(personalGoal.transform.position);
                                    break;
                                case "gTeam":
                                    personalGoal = GameObject.Find("BLUEGOAL");
                                    agent.SetDestination(personalGoal.transform.position);
                                    break;
                            }
                        }
                        else
                        {
                            switch (gameObject.tag)
                            {
                                case "bTeam":
                                    personalGoal = GameObject.Find("BLUEGOAL");
                                    agent.SetDestination(personalGoal.transform.position);
                                    break;
                                case "gTeam":
                                    personalGoal = GameObject.Find("GREENGOAL");
                                    agent.SetDestination(personalGoal.transform.position);
                                    break;
                            }
                        }
                        break;
                    case "Sniper":
                        if(personalGoal.name == "HighPointBlue" || personalGoal.name == "HighPointGreen")
                        {
                            if(agent.remainingDistance < 5)
                            {
                                agent.destination = RandomNavSphere(personalGoal.transform.position);
       
                            }
                        }
                        break;
                    case "Attacker":
                        if (squadRoles["ObjectiveCarrier"] != personalGoal)
                        {
                            personalGoal = squadRoles["ObjectiveCarrier"];
                        }
                        if (personalGoal.activeInHierarchy && personalGoal != null)
                        {
                            agent.SetDestination(personalGoal.transform.position);
                        }
                        else
                        {
                            AssignSquad();
                        }
                        break;
                    case "Defender1":
                    case "Defender2":
                        if (goal.name == "GREENGOAL" || goal.name == "BLUEGOAL")
                        {
                            Vector3 faceDirection = new Vector3(0, 0, 0);
                            float dist = agent.remainingDistance;
                            if (agent.remainingDistance < 5)
                            {
                                gameObject.transform.LookAt(faceDirection);
                                agent.destination = RandomNavSphere((faceDirection - goal.transform.position) * 0.5f);
                            }
                        }
                        break;
                }
                break;
            #endregion
            #region Attacking
            case States.Attacking:
                if (attackTarget != null)
                {
                    if (attackTarget.activeInHierarchy)
                    {
                        SupportFire(attackTarget);
                    }
                    else
                    {
                        currentState = States.Squad;
                    }
                }
                break;
            #endregion
            #region Supporting
            case States.Supporting:
                if (supportTarget != null)
                {
                    if (supportTarget.activeInHierarchy)
                    {
                        SupportFire(supportTarget);
                    }
                    else
                    {
                        currentState = States.Squad;
                    }
                }
                else
                {
                    currentState = States.Squad;
                }
                
                break;
            #endregion
            #region Retrieving
            case States.Retrieving:
                agent.destination = importantTarget.transform.position;
                break;
            #endregion

        }


    }
    private void SupportFire(GameObject target)
    {
        gameObject.transform.LookAt(target.transform.position); //Look at the support target
        if (Vector3.Distance(gameObject.transform.position, target.transform.position) > 35) //Check if their within 35 units
        {
            agent.destination = target.transform.position; //Travel towards it if it is out of range
        }
        else
        {
            agent.destination = RandomNavSphere(gameObject.transform.position); //Move around if within range
        }

    }
    private void SquadCleaner()
    {
        var dictionaryBuffer = new List<string>(squadRoles.Keys);

        foreach (var key in dictionaryBuffer)
        {
            if (squadRoles[key] != null && squadRoles[key].activeInHierarchy == false)
            {
                squadRoles[key] = null;
            }

        }
    }
    private void CheckSquad()
    {
        if(!squadRoles.ContainsValue(gameObject))
        {
            AssignSquad();
        }
    }
    /*PUBLIC METHODS*/
    public static Vector3 RandomNavSphere(Vector3 origin)
    {
        float dist = UnityEngine.Random.Range(1.0f, 15.0f);
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
    }
    public void CallHelp(GameObject enemy)
    {
        foreach (KeyValuePair<GameObject, Vector3> ft in friendlyTeam)
        {
            if (Vector3.Distance(gameObject.transform.position, ft.Value) < 50) //Find all allys within 50 units
            {
                var aiScript = ft.Key.GetComponent<AI>(); //Get script reference
                if (aiScript.currentState != States.Supporting) //Check if their already supporting
                {
                    aiScript.currentState = States.Supporting; //Tell them to support 
                    aiScript.supportTarget = enemy; //Set their support target to current enemy
                }
            }
        }
    }
    public void OnReEnable()
    {
        if(objStolen == true)
        {
            currentKeyEvent = ImportantEvents.FriendlyObjStolen;
        }
        else
        {
            currentKeyEvent = ImportantEvents.None;
        }
        currentState = States.Squad;
        SquadCleaner();
        AssignSquad();
        InvokeRepeating("CommunicatePosition", 0.4f, 1f);
        InvokeRepeating("ExecuteBehaviour", 0.8f, 0.5f);
        InvokeRepeating("CheckSquad", 4f, 0.5f);
    }
    public void UpdateLocation(Transform pos)
    {
        if(Time.realtimeSinceStartup > targetUpdated)
        {
            importantTarget = pos;
            targetUpdated = Time.realtimeSinceStartup;
            CommunicateImportantLocation(pos,Time.realtimeSinceStartup);
        }
    }

    /*ENUMS*/
    public enum States
    {
        Squad,
        Attacking,
        Supporting,
        Retrieving,
    };
    public enum ImportantEvents
    {
        None,
        CarrierDead,
        FriendlyObjStolen,
        UnderAttack

    };
}


