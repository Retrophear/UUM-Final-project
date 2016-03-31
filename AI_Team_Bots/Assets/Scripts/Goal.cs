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
        if(childObj != null)
        {
            childObj.parent = null;
            childObj.transform.position = childObj.GetComponent<Goal>().startPosition;
            childObj = null;
        }
        gameObject.transform.parent = null;
        gameObject.transform.position = startPosition;
    }
    void OnTriggerEnter(Collider other)
    {
        switch(thisTag)
        {
            case "BLUEGOAL": //Bluegoal is the object placed on the green side
                {
                    if (other.tag == "bTeam")
                    {
                        gameObject.transform.parent = other.gameObject.transform;
                        gameObject.transform.position = other.transform.position + new Vector3(0, 5, 0);
                    }
                    else if(other.tag == "gTeam" && other.transform.FindChild("GREENGOAL") != null)
                    {
                        Debug.Log("SCORE");
                        childObj = other.transform.FindChild("GREENGOAL");
                        ResetPostion();
                    }
                    break;
                }
            case "GREENGOAL": //Green goal is placed on the blue side
                {
                    if(other.tag == "gTeam") //Check if tag is of green team
                    {
                        gameObject.transform.parent = other.gameObject.transform; //Add it as a child object to the bot
                        gameObject.transform.position = other.transform.position + new Vector3(0, 5, 0);
                    }
                    else if (other.tag == "bTeam" && other.transform.FindChild("BLUEGOAL") != null) //If the bot has made it to the goal with opposing teams flag
                    {
                        Debug.Log("SCORE");
                        childObj = other.transform.FindChild("BLUEGOAL");
                        ResetPostion();
                    }
                    break;
                }
        }

        
     


    }
}
