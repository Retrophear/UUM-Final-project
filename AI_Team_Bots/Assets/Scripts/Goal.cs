using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    private string thisTag;
    public Vector3 startPosition;
    private Transform childObj;


    void Start()
    {
        thisTag = gameObject.name;
        startPosition = gameObject.transform.position;
        childObj = null;
    }

    void ResetPostion()
    {
        if (childObj != null)
        {
            childObj.parent = null;
            childObj.transform.position = childObj.GetComponent<Goal>().startPosition;
            childObj = null;
        }
        gameObject.transform.parent = null;
        gameObject.transform.position = startPosition;
        if (gameObject.name == "BLUEGOAL")
        {
            InformTeam("gTeam", false);
        }
        else if (gameObject.name == "GREENGOAL")
        {
            InformTeam("bTeam", false);
        }
    }
    void OnTriggerEnter(Collider other)
    {
        switch (thisTag)
        {
            case "BLUEGOAL": //Bluegoal is the object placed on the green side
                {
                    if (other.tag == "bTeam")
                    {
                        gameObject.transform.parent = other.gameObject.transform;
                        gameObject.transform.position = other.transform.position + new Vector3(0, 15, 0);
                        InformTeam("gTeam", true);
                    }
                    else if (other.tag == "gTeam" && other.transform.FindChild("GREENGOAL") != null)
                    {
                        GameObject.Find("UI").GetComponent<UI>().grnScore++;
                        GameObject.Find("UI").GetComponent<UI>().lastScored = Time.realtimeSinceStartup;
                        childObj = other.transform.FindChild("GREENGOAL");
                        ResetPostion();
                    }
                    else if (other.tag == "gTeam" && gameObject.transform.parent == null)
                    {
                        ResetPostion();
                    }
                    break;
                }
            case "GREENGOAL": //Green goal is placed on the blue side
                {
                    if (other.tag == "gTeam") //Check if tag is of green team
                    {
                        gameObject.transform.parent = other.gameObject.transform; //Add it as a child object to the bot
                        gameObject.transform.position = other.transform.position + new Vector3(0, 15, 0);
                        InformTeam("bTeam", true);
                    }
                    else if (other.tag == "bTeam" && other.transform.FindChild("BLUEGOAL") != null) //If the bot has made it to the goal with opposing teams flag
                    {
                        GameObject.Find("UI").GetComponent<UI>().bluScore++;
                        GameObject.Find("UI").GetComponent<UI>().lastScored = Time.realtimeSinceStartup;
                        childObj = other.transform.FindChild("BLUEGOAL");
                        ResetPostion();
                    }
                    else if (other.tag == "bTeam" && gameObject.transform.parent == null)
                    {
                        ResetPostion();
                    }

                    break;
                }
        }





    }

    void InformTeam(string team, bool stolen)
    {
        var teamMembers = GameObject.FindGameObjectsWithTag(team);
        foreach (GameObject go in teamMembers)
        {
            if (stolen == true)
            {
                if (go.GetComponent<TestAI>())
                {
                    go.GetComponent<TestAI>().objStolen = true;
                    go.GetComponent<TestAI>().agentState = TestAI.State.Retrieving; 
                }
                else if (go.GetComponent<AI>())
                {
                    go.GetComponent<AI>().currentKeyEvent = AI.ImportantEvents.FriendlyObjStolen;
                    go.GetComponent<AI>().objStolen = true;
                }
            }
            else if (stolen == false)
            {
                if (go.GetComponent<TestAI>())
                {
                    go.GetComponent<TestAI>().objStolen = false;
                    if(go.GetComponent<TestAI>().agentState == TestAI.State.Retrieving)
                    {
                        go.GetComponent<TestAI>().agentState = TestAI.State.Squad;
                    }
                }
                else if (go.GetComponent<AI>())
                {
                    go.GetComponent<AI>().objStolen = false;
                    if (go.GetComponent<AI>().currentKeyEvent == AI.ImportantEvents.FriendlyObjStolen)
                    {
                        go.GetComponent<AI>().currentKeyEvent = AI.ImportantEvents.None;
                    }
                    if (go.GetComponent<AI>().currentState == AI.States.Retrieving)
                    {
                        go.GetComponent<AI>().currentState = AI.States.Squad;
                    }
                }
            }
        }
    }
}
