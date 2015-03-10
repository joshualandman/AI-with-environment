using UnityEngine;
using System.Collections;

public enum CurrentState
{
    Rest,
    Wander,
    Pursue,
    Flee
}

public class GoodWander : MonoBehaviour {

    

    #region FIXED_CHARACTER_MOTIVES

    /// From 0 to 1.
    public float bravery;

    #endregion

    #region LUCID_CHARACTER_MOTIVES

    // From 0 to 1.
    public float confidence;

    #endregion

    #region WEAPON_ATTRIBUTES

    public int maxAmmo = 30;
    public int currentAmmo = 30;
    public int maxReload = 60;
    public int currentReload = 0;

    public float accurateDist = 1.0f;
    public float innacurateDist = 20.0f;

    public float maxAccuracy = .8f;

    private Vector3 coneLeft;
    private Vector3 coneRight;

    #endregion

    #region CHARACTER_STATES
    public int health = 2;
    #endregion

    #region STATE_HANDLING

    // State
    private CurrentState myState;
    public CurrentState MyState { get { return myState; } }

    #endregion

    public GameObject target;

    #region NAVIGATION

    NavMeshAgent agent;

    public float closeDistance;

    public float wanderAng = 0.0f;
    public float wanderMaxAng = 6.0f;
    public float wanderRad = 100.0f;
    public float wanderDist = 4.0f;

    #endregion

    // Use this for initialization
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myState = CurrentState.Pursue;
    }

    /// <summary>
    /// Handles all state calls
    /// </summary>
    void Update()
    {
        // Things seem backwards because we're dotting against these.  They're 90 degrees off of what we actually want
        coneLeft = Quaternion.Euler(0, 60, 0) * transform.forward;
        coneRight = Quaternion.Euler(0, -60, 0) * transform.forward;

        // This is the code needed to ensure reloading occurs.  You can move this code somewhere else if you only want reloading to happen in other states.
        if (currentReload > 0)
            currentReload--;

        if (currentReload == 0)
        {
            currentAmmo = maxAmmo;
            currentReload = -1;
        }

        UpdateStates();
        FindDestination();
    }

    /// <summary>
    /// Takes care of any housekeeping needed by any of the states.  This is where you'd do things like call state changes.
    /// </summary>
    public void UpdateStates()
    {
        //-----------WARNING-----------|
        // Make sure to set a target   |
        // for pursue and flee if you  |
        // plan on changing to them.   |
        //-----------WARNING-----------|

        switch(myState)
        {
            case CurrentState.Rest:
                break;

            case CurrentState.Wander:
                break;

            case CurrentState.Pursue:
                break;

            case CurrentState.Flee:
                break;
        }
    }


    /// <summary>
    /// Initializes the state chosen.
    /// </summary>
    public void InitializeState(CurrentState newState)
    {
        //-----------WARNING-----------|
        // Make sure to set a target   |
        // for pursue and flee if you  |
        // plan on changing to them.   |
        // You must do that right      |
        // before you call this method.|
        //-----------WARNING-----------|

        myState = newState;

        switch (newState)
        {
            case CurrentState.Rest:
                agent.Stop();
                break;

            case CurrentState.Wander:
                agent.Resume();
                break;

            case CurrentState.Pursue:
                agent.Resume();
                break;

            case CurrentState.Flee:
                agent.Resume();
                break;
        }
    }

    /// <summary>
    /// Set's the destination.
    /// </summary>
    /// <returns></returns>
    public void FindDestination()
    {

        switch(myState)
        {
            case CurrentState.Rest:
            break;
            
            case CurrentState.Wander:
            Wander();
            break;

            case CurrentState.Pursue:
            Pursue(target.transform.position, target.GetComponent<Movement>().velocity);
            break;

            case CurrentState.Flee:
            Flee(target.transform.position);
            break;

        }
    }

    /// <summary>
    /// Set's the destination of the character to the position of the target a few frames ahead.
    /// </summary>
    /// <param name="myTarget"> vector position of target </param>
    /// <param name="myTargetVel">Velocity of the target</param>
    public void Pursue(Vector3 myTarget, Vector3 myTargetVel)
    {
        agent.SetDestination(myTarget + myTargetVel.normalized * 3);
    }

    /// <summary>
    /// Wanders in the nearby area.
    /// </summary>
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
    /// Flees a target.
    /// </summary>
    /// <param name="myTarget"></param>
    /// <returns></returns>
    public void Flee(Vector3 myTarget)
    {
        agent.SetDestination(2 * transform.position - myTarget);
    }

    /// <summary>
    /// Shoots at the current target
    /// </summary>
    public void ShootAtTarget()
    {
        if (currentAmmo <= 0)
            return;

        // Make sure that the target is relatively in front of you.  Later we'll also check to make sure you can see the target via raycasting.
        if (Vector3.Dot(coneLeft, target.transform.position - transform.position) < 0 && Vector3.Dot(coneRight, target.transform.position - transform.position) > 0)
        {
            currentAmmo--;

            // Get some preliminary info
            float dist = (target.transform.position - transform.position).magnitude;
            float rand = Random.Range(0.0f, 1.0f);

            // If they're within max accuracy range they're within max accuracy.
            if (dist < accurateDist && maxAccuracy > rand)
            {
                target.GetComponent<GoodWander>().health--;
                return;
            }


            // Otherwise the further they get the closer to 0% chance of hitting them, linearly.  Linear should be fine for our purpose.
            if(maxAccuracy 
                * Mathf.Lerp(1, 0, (dist - accurateDist) / (innacurateDist - accurateDist)) > rand)
            {
                target.GetComponent<GoodWander>().health--;
            }
        }
    }

    /// <summary>
    /// Begins reloading the weapon
    /// All of the code that is actually used for the reloading itself occurs elsewhere.
    /// </summary>
    public void Reload()
    {
        currentReload = maxReload;

        // THE CODE BELOW IS FOR DEMONSTRATION
        // Reloading code is below.  You can place it wherever you want to go so reloading occurs

        //if (currentReload > 0)
        //    currentReload--;

        //if (currentReload == 0)
        //{
        //    currentAmmo = maxAmmo;
        //    currentReload = -1;
        //}

    }



    
}
