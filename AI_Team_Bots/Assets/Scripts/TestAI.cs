using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TestAI : MonoBehaviour
{
    public Transform goal;
    public NavMeshAgent agent;
    public TestStates currentState;
    public State agentState;
    Dictionary<GameObject, string> friendlyTeam;
    private GameObject highPoint;
    public bool objStolen;
    public Transform importantLocation;
    public GameObject attackTarget;
    float targetUpdated;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartBehaviour();
        friendlyTeam = new Dictionary<GameObject, string>();
        GetTeam();

    }
    void OnDisable()
    {
        CancelInvoke();
    }
    public void ReEnable()
    {
        StartBehaviour();
    }

    private void StartBehaviour()
    {
        switch (currentState)
        {
            case TestStates.RushObjective:
                InvokeRepeating("RushObjective", 0.8f, 0.4f);
                break;
            case TestStates.BaseTeam:
                InvokeRepeating("BaseTeam", 0.8f, 0.4f);
                break;
        }
    }
    private void RushObjective()
    {
        if (gameObject.transform.childCount > 10)
        {
            switch (gameObject.tag)
            {
                case "bTeam":
                    goal = GameObject.Find("GREENGOAL").transform;
                    agent.SetDestination(goal.transform.position);
                    break;
                case "gTeam":
                    goal = GameObject.Find("BLUEGOAL").transform;
                    agent.SetDestination(goal.transform.position);
                    break;
            }
        }
        else
        {
            switch (gameObject.tag)
            {
                case "bTeam":
                    goal = GameObject.Find("BLUEGOAL").transform;
                    agent.SetDestination(goal.transform.position);
                    break;
                case "gTeam":
                    goal = GameObject.Find("GREENGOAL").transform;
                    agent.SetDestination(goal.transform.position);
                    break;
            }
        }
    }
    private void BaseTeam()
    {
        switch (agentState)
        {
            case State.Retrieving:
                if(friendlyTeam[gameObject] == "Defender1" || friendlyTeam[gameObject] == "Defender2")
                agent.SetDestination(importantLocation.position);
                break;
            case State.Attacking:
                if (attackTarget.activeInHierarchy)
                {
                    SupportFire(attackTarget);
                }
                else
                {
                    agentState = State.Squad;
                }
                break;
            case State.Squad:

                switch (friendlyTeam[gameObject])
                {
                    case "ObjectiveCarrier":
                        RushObjective();
                        break;
                    case "Attacker":
                        agent.SetDestination(friendlyTeam.FirstOrDefault(x => x.Value == "ObjectiveCarrier").Key.transform.position);
                        break;
                    case "Sniper":
                        if (agent.remainingDistance < 5)
                        {
                            agent.destination = RandomNavSphere(highPoint.transform.position);
                        }
                        break;
                    case "Defender1":
                    case "Defender2":
                        if (goal.name == "GREENGOAL" || goal.name == "BLUEGOAL")
                        {
                           
                            Vector3 faceDirection = new Vector3(0, 0, 0);
                            float dist = agent.remainingDistance;
                            if (dist < 5)
                            {
                                gameObject.transform.LookAt(faceDirection);
                                agent.destination = RandomNavSphere((faceDirection - goal.transform.position) * -0.5f);
                            }
                        }
                        break;

                }
                break;
        }

    }
    private void GetTeam()
    {
        var team = GameObject.FindGameObjectsWithTag("gTeam");

        foreach (GameObject go in team)
        {
            friendlyTeam.Add(go, "");
        }
        foreach (GameObject go in team)
        {
            if (!friendlyTeam.ContainsValue("ObjectiveCarrier") && friendlyTeam[go] == "")
            {
                friendlyTeam[go] = "ObjectiveCarrier";
            }
            if (!friendlyTeam.ContainsValue("Attacker") && friendlyTeam[go] == "")
            {
                friendlyTeam[go] = "Attacker";
            }
            if (!friendlyTeam.ContainsValue("Sniper") && friendlyTeam[go] == "")
            {
                friendlyTeam[go] = "Sniper";
                highPoint = GameObject.Find("HighPointGreen");
                goal = highPoint.transform;
                agent.destination = goal.transform.position;
            }
            if (!friendlyTeam.ContainsValue("Defender1") && friendlyTeam[go] == "")
            {
                friendlyTeam[go] = "Defender1";
                goal = GameObject.Find("BLUEGOAL").transform;
                agent.destination = goal.transform.position;
            }
            if (!friendlyTeam.ContainsValue("Defender2") && friendlyTeam[go] == "")
            {
                friendlyTeam[go] = "Defender2";
                goal = GameObject.Find("BLUEGOAL").transform;
                agent.destination = goal.transform.position;
            }
        }

        //foreach (KeyValuePair<GameObject, string> ft in friendlyTeam)
        //{
        //    Debug.Log(ft.Key.gameObject.name + " " + ft.Value);
        //}
    }
    private static Vector3 RandomNavSphere(Vector3 origin)
    {
        float dist = UnityEngine.Random.Range(1.0f, 15.0f);
        Vector3 randDirection = UnityEngine.Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, NavMesh.AllAreas);

        return navHit.position;
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
    public void UpdateLocation(Transform pos)
    {
        if (Time.realtimeSinceStartup > targetUpdated)
        {
            importantLocation = pos;
            targetUpdated = Time.realtimeSinceStartup;
        }
    }
    public enum TestStates
    {
        RushObjective,
        BaseTeam
    }
    public enum State
    {
        Squad,
        Attacking,
        Retrieving
    }
}
