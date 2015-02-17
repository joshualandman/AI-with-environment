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
	public Vector3 forces;
	public float maxForce = 3.0f;
    public Transform target;
    public float mass = 1.0f;
    public float maxSpeed = 6.0f;
    public float closeDistance;

    public float wanderWt;
    public float seekWt = 0.0f;
    public float arrivalWt = 0.0f;
    public float fleeWt = 0.0f;

    public float wanderAng = 0.0f;
    public float wanderMaxAng = 6.0f;
    public float wanderRad = 1.0f;
    public float wanderDist = 15.0f;

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

		acceleration = calculateSteeringForces();

        velocity += acceleration * Time.deltaTime;
        velocity.z = 0;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (velocity != Vector3.zero)
            transform.forward = velocity.normalized;

        transform.position += velocity;
        acceleration = Vector3.zero;

        //Wander();
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

    public Vector3 Seek(Vector3 myTarget)
    {
        Vector3 desired = Vector3.Normalize(myTarget - transform.position);
        desired = Vector3.ClampMagnitude(desired, maxSpeed);
        Vector3 steer = desired - velocity;
        steer.z = 0;
        return steer;
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

    public Vector3 Wander()
    {        
        //float velX = Mathf.PerlinNoise(Time.time, 0) - .5f;
        //float velY = Mathf.PerlinNoise(0, Time.time) - .5f;

        //return new Vector3(velX, velY, 0);

        wanderAng += Random.Range(-wanderRad, wanderRad);

        if (wanderAng > wanderMaxAng)
        {
            wanderAng = wanderMaxAng;
        }

        if (wanderAng < -wanderMaxAng)
        {
            wanderAng = -wanderMaxAng;
        }
        
        Quaternion rotate = Quaternion.Euler(0, 0, wanderAng);
        Vector3 direction = rotate * this.transform.forward;
        direction = direction.normalized * wanderRad;

        Vector3 wanderTo = this.transform.position + this.transform.forward * wanderDist + direction;

        return Seek(wanderTo);
    }

    public Vector3 Flee(Vector3 myTarget)
    {
        Vector3 desired = Vector3.Normalize(myTarget - transform.position);
        desired = Vector3.ClampMagnitude(desired, maxSpeed);
        Vector3 steer = desired - velocity;
        steer.z = 0;
        return -steer;
    }

	public Vector3 calculateSteeringForces()
	{
		forces = Vector3.zero;

        if (wanderWt != 0)
            forces += wanderWt * Wander();
        if (seekWt != 0)
            forces += seekWt * Seek(target.transform.position);
        if (arrivalWt != 0)
            Arrival(target);
        if (fleeWt != 0)
            forces += fleeWt * Flee(target.transform.position);

        forces.z = 0;
        return forces;
	}

    protected void ApplyForce(Vector3 steeringForce)
    {
        acceleration += steeringForce / mass;
    }

    void OnGUI()
    {
        if(GUI.Button( new Rect(10, 10, 100, 20), new GUIContent("Wander")))
        {
            wanderWt = 1.0f;
            seekWt = 0.0f;
            arrivalWt = 0.0f;
            fleeWt = 0.0f;
            Debug.Log("WANDER");
        }
        if (GUI.Button(new Rect(10, 35, 100, 20), new GUIContent("Seek")))
        {
            wanderWt = 0.0f;
            seekWt = 1.0f;
            arrivalWt = 0.0f;
            fleeWt = 0.0f;
            Debug.Log("Seek");
        }
        if (GUI.Button(new Rect(10, 60, 100, 20), new GUIContent("Arrival")))
        {
            wanderWt = 0.0f;
            seekWt = 0.0f;
            arrivalWt = 1.0f;
            fleeWt = 0.0f;
            Debug.Log("Arrival");
        }
        if (GUI.Button(new Rect(10, 85, 100, 20), new GUIContent("Flee")))
        {
            wanderWt = 0.0f;
            seekWt = 0.0f;
            arrivalWt = 0.0f;
            fleeWt = 1.0f;
            Debug.Log("Flee");
        }
    }

}
