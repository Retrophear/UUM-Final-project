﻿using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {
    public float deadZone = 5f;             // The number of degrees for which the rotation isn't controlled by Mecanim.
    
    private NavMeshAgent agent;
    private Animator anim;
    private AnimatorSetup animSetup;
 
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        animSetup = new AnimatorSetup(anim);

   
        agent = GetComponent<NavMeshAgent>();

        //agent.updateRotation = false;
        deadZone *= Mathf.Deg2Rad;
	}
	
	// Update is called once per frame
	void Update () {
        NavAnimSetup();
	}
    void NavAnimSetup()
    {
        // Create the parameters to pass to the helper function.
        float speed;
        float angle;

  
            // Otherwise the speed is a projection of desired velocity on to the forward vector...
            speed = Vector3.Project(agent.desiredVelocity, transform.forward).magnitude;

            // ... and the angle is the angle between forward and the desired velocity.
            angle = FindAngle(transform.forward, agent.desiredVelocity, transform.up);

            // If the angle is within the deadZone...
            if (Mathf.Abs(angle) < deadZone)
            {
                // ... set the direction to be along the desired direction and set the angle to be zero.
                transform.LookAt(transform.position + agent.desiredVelocity);
                angle = 0f;
            }
        

        // Call the Setup function of the helper class with the given parameters.
        animSetup.Setup(speed, angle);
    }


    float FindAngle(Vector3 fromVector, Vector3 toVector, Vector3 upVector)
    {
        // If the vector the angle is being calculated to is 0...
        if (toVector == Vector3.zero)
            // ... the angle between them is 0.
            return 0f;

        // Create a float to store the angle between the facing of the enemy and the direction it's travelling.
        float angle = Vector3.Angle(fromVector, toVector);

        // Find the cross product of the two vectors (this will point up if the velocity is to the right of forward).
        Vector3 normal = Vector3.Cross(fromVector, toVector);

        // The dot product of the normal with the upVector will be positive if they point in the same direction.
        angle *= Mathf.Sign(Vector3.Dot(normal, upVector));

        // We need to convert the angle we've found from degrees to radians.
        angle *= Mathf.Deg2Rad;

        return angle;
    }
}