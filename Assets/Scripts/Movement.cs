using UnityEngine;
using System.Collections;

public enum MovementState{
	Seek,
	Arrive,
	Wander
}

public class Movement : MonoBehaviour {

	private MovementState myState;
	public MovementState MyState { get { return myState;}}

	private Transform myTarget;

	// Use this for initialization
	void Start () {
		myState = MovementState.Wander;
		myTarget = null;
	
	}
	
	// Update is called once per frame
	void Update () {
		ChangeState (MovementState.Seek);
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
}
