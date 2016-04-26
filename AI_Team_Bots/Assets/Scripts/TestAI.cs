using UnityEngine;
using System.Collections;

public class TestAI : MonoBehaviour
{
    public Transform goal;
    public NavMeshAgent agent;
    public TestStates currentState;

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartBehaviour();
    }
    void OnDisable()
    {
        CancelInvoke();
    }
    public void ReEnable()
    {
        StartBehaviour();
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void StartBehaviour()
    {
        switch (currentState)
        {
            case TestStates.RushObjective:
                InvokeRepeating("RushObjective", 0.8f, 0.4f);
                break;
        }
    }
    private void RushObjective()
    {
        if (gameObject.transform.childCount > 1)
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

    public enum TestStates
    {
        RushObjective
    }
}
