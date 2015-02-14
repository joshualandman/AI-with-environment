using UnityEngine;
using System.Collections;

public enum MovementState{
	Seek,
	Arrive,
	Wander
}

public class Movement : MonoBehaviour {

    public Vector3 velocity;
    public Vector3 acceleration;
    public Transform target;
    public float maxSpeed;
    public float closeDistance;

	private MovementState myState;
	public MovementState MyState { get { return myState;}}

	private Transform myTarget;

	// Use this for initialization
	void Start () {
		myState = MovementState.Wander;
		myTarget = null;
        acceleration = new Vector3(0, 0);
        velocity = new Vector3(.12f * transform.localScale.x, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

        velocity += acceleration;
        transform.position += velocity;
        Wander();
        //Seek(target);
        //Arrival(target);
		//ChangeState (MovementState.Seek);
	}

	/// <summary>
	/// Changes the state of the object, if you do not specify a target it will go to wander.
	/// </summary>
	/// <param name="newState">State changing to.</param>
	/// <param name="myTarget">Target transform to follow in case of seek or arrive.</param>
	public void ChangeState(MovementState newState, Transform myTarget = null)
	{
		myState = newState;

		if(myTarget == null)
		{
			myState = MovementState.Wander;
			return;
		}

		this.myTarget = myTarget;
	}

    public void Seek(Transform myTarget)
    {
        Vector3 desired = Vector3.Normalize(myTarget.transform.position - transform.position);
        Vector3 steer = desired - velocity;
        velocity += steer;
    }

    public void Arrival(Transform myTarget)
    {
        Vector3 desiredVelocity = myTarget.transform.position - transform.position;
        float d = desiredVelocity.magnitude;

        desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, maxSpeed);
        Vector3 steer = desiredVelocity - velocity;
        velocity += steer;

        if (d <= (closeDistance * 2) && d >= (closeDistance + 1))
        {
            desiredVelocity = Vector3.ClampMagnitude(desiredVelocity, .5f);
        }
        else if (d <= closeDistance)
        {
            velocity = new Vector3(0,0,0);
        }
    }

    public void Wander()
    {        
        
    }
}
