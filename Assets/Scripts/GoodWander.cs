﻿using UnityEngine;
using System.Collections;

public class GoodWander : MonoBehaviour {


    NavMeshAgent agent;

    //attributes
    public Transform target;

    //movement vectors
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 forces;

    //math forces
    public float maxForce = 3.0f;
    public float mass = 1.0f;
    public float maxSpeed = 6.0f;
    public float closeDistance;

    //weights
    public float wanderWt;
    public float seekWt = 0.0f;
    public float arrivalWt = 0.0f;
    public float fleeWt = 0.0f;

    public float wanderAng = 0.0f;
    public float wanderMaxAng = 6.0f;
    public float wanderRad = 100.0f;
    public float wanderDist = 4.0f;

    private MovementState myState;
    private Transform myTarget;

    public MovementState MyState { get { return myState; } }



    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myState = MovementState.Wander;
        myTarget = null;
        acceleration = new Vector3(0, 0);
        velocity = new Vector3(.12f * transform.localScale.x, 0, 0);
    }

    // Update is called once per frame
    /// <summary>
    /// Update acceleration based off of forces and then add that to the velocity
    /// then add velocity to position for mvoement
    /// reset acceleration at end
    /// </summary>
    void Update()
    {
        if ((transform.position - target.position).magnitude < 5.0f)
        {
            wanderWt = 0;
            maxSpeed = .08f;
        }
        else
        {
            wanderWt = 1;
            maxSpeed = .02f;
        }

        acceleration = calculateSteeringForces();

        calculateSteeringForces();

        velocity += acceleration * Time.deltaTime;
        //agent.velocity.y = 0;
        velocity = Vector3.ClampMagnitude(agent.velocity, maxSpeed);

        if (velocity != Vector3.zero)
            transform.forward = velocity.normalized;

        transform.position += velocity;
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Changes the state of the object, if you do not specify a target it will go to wander.
    /// </summary>
    /// <param name="newState">State changing to.</param>
    /// <param name="myTarget">Target transform to follow in case of seek or arrive.</param>
    public void ChangeState(MovementState newState, Transform myTarget = null)
    {
        myState = newState;

        if (myTarget == null)
        {
            myState = MovementState.Wander;
            return;
        }

        this.myTarget = myTarget;
    }

    /// <summary>
    /// Create and normalise a desired vector
    /// create a steer vector by subtracting the velocity from the desired
    /// return the desire vector
    /// </summary>
    /// <param name="myTarget"> vector position of target </param>
    /// <returns></returns>
    public void Seek(Vector3 myTarget)
    {
        /*Vector3 desired = Vector3.Normalize(myTarget - agent.transform.position);
        desired *= maxSpeed;
        Vector3 steer = desired - velocity;
        steer.z = 0;
        return steer;*/
        agent.SetDestination(myTarget);
    }

    /// <summary>
    /// calculate displacesment of target and current, retrieve the madnitude for distance to slow down
    /// create a desired vector by normalizing the distance and multiplying by maxspeed
    /// if you are within a lerp range of distance+1 and distance *2, slow down
    /// if you are less then or equal to the set close distance, stop moving
    /// return desired vector - velocity
    /// </summary>
    /// <param name="myTarget"> vector position of target</param>
    /// <returns></returns>
    public void Arrival(Transform myTarget)
    {
        agent.SetDestination(myTarget.position);
        Vector3 distance = myTarget.transform.position - agent.transform.position;
        float d = distance.magnitude;

        Vector3 desiredVelocity = distance.normalized;
        desiredVelocity *= maxSpeed;

        if (d <= (closeDistance * 2) && d >= (closeDistance + 1))
        {
            agent.speed *= Mathf.InverseLerp(0, closeDistance * 2, d);
        }
        else if (d <= closeDistance)
        {
            agent.SetDestination(Vector3.zero);
        }


        Vector3 steer = desiredVelocity - agent.velocity;
        steer.y = 0;
        //return steer;
    }

    /// <summary>
    /// set wander angle to a range between -radius and positive wonder radius
    /// if the angle is larget than the max, set to max, if smaller than the min, set to min
    /// create a quarternion based off the wander angle
    /// create a direction vector based off the quarternion times the forward
    /// multiply the normalized direction vector by the radius
    /// set a vector to a position that is the current position + the forward * the distance + the direction
    /// return a seek to the vector position
    /// </summary>
    /// <returns></returns>
    public void Wander()
    {
        wanderAng += Random.Range(-wanderRad, wanderRad);

        if (wanderAng > wanderMaxAng)
        {
            wanderAng = wanderMaxAng;
        }

        if (wanderAng < -wanderMaxAng)
        {
            wanderAng = -wanderMaxAng;
        }

        Quaternion rotate = Quaternion.Euler(0, wanderAng, 0);
        Vector3 direction = rotate * this.transform.forward;
        direction = direction.normalized * wanderRad;
        Debug.Log(direction);

        Vector3 wanderTo = this.transform.position + this.transform.forward * wanderDist + direction;

        agent.SetDestination(wanderTo);
    }

    /// <summary>
    /// return -seek
    /// </summary>
    /// <param name="myTarget"></param>
    /// <returns></returns>
    public void Flee(Vector3 myTarget)
    {
        agent.SetDestination(2 * transform.position - myTarget);
    }

    /// <summary>
    /// set forces to zero
    /// based off of the weight of movement choices
    /// set forces to the weight and call the required movement function
    /// </summary>
    /// <returns></returns>
    public Vector3 calculateSteeringForces()
    {
        forces = Vector3.zero;
        if (wanderWt != 0)
            Wander();
        else
            Flee(target.position);

        forces.y = 0;
        return forces;
    }

    /// <summary>
    /// take in a vector 3 for movement
    /// add the vector/mass to the acceleration
    /// </summary>
    /// <param name="steeringForce"></param>
    protected void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / mass;
    }



    
}
