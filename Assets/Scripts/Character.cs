using UnityEngine;
using System.Collections;

public enum CurrentState
{
    Rest,
    Wander,
    Pursue,
    Flee
}

public class Character : MonoBehaviour {

    

    #region FIXED_CHARACTER_MOTIVES

    /// From 0 to 1.
    public float bravery = .5f;

    #endregion

    #region LUCID_CHARACTER_MOTIVES

    // From 0 to 1.
    public float confidence = .8f;

    #endregion

    public int timeSurvived = 0;
    public int kills = 0;

    public bool friends;
    public float mentalFortitude;

    #region WEAPON_ATTRIBUTES

    public int maxAmmo = 30;
    public int currentAmmo = 30;
    public int maxReload = 60;
    public int currentReload = 0;

    public float accurateDist = 1.0f;
    public float innacurateDist = 20.0f;
    public Vector3 initialPos;

    public float vision = 40.0f;

    public float maxAccuracy = .5f;

    public bool enemiesShooting = false;

    public string enemyTag = "goodguy";
    public string friendTag = "badguy";

    private Vector3 coneLeft;
    private Vector3 coneRight;

    #endregion

    #region CHARACTER_STATES
    public int health = 10;
    #endregion

    #region STATE_HANDLING

    // State
    public CurrentState myState;

    #endregion

    public GameObject target;
    public GameObject friend;

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
        agent = this.GetComponent<NavMeshAgent>();
        myState = CurrentState.Wander;
        initialPos = transform.position;
        this.transform.Rotate(new Vector3(0, 1, 0), Random.Range(0, 360));
    }

    float getDistance(Vector3 self, Vector3 desiredLocation)
    {
        return Mathf.Sqrt(Mathf.Pow((self.x - desiredLocation.x), 2) + Mathf.Pow((self.y - desiredLocation.y), 2) + Mathf.Pow((self.z - desiredLocation.z), 2));
    }

    /// <summary>
    /// Handles all state calls
    /// </summary>
    void FixedUpdate()
    {
        timeSurvived++;
        if (CompareTag("dead"))
            return;

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

        enemiesShooting = false;
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
                if (currentAmmo == maxAmmo)
                    InitializeState(CurrentState.Wander);
                break;

            case CurrentState.Wander:
                target = FindTarget();
                if (target != null)
                {
                    InitializeState(CurrentState.Pursue);
                }
                if(currentAmmo <= 0)
                {
                    Reload();
                    InitializeState(CurrentState.Rest);
                }
                break;

            case CurrentState.Pursue:

                if(target == null)
                {
                    target = FindTarget();
                    if (target == null)
                    {
                        InitializeState(CurrentState.Wander);
                        break;
                    }
                }
                float charDist = (target.transform.position - transform.position).magnitude;
                if (charDist >= vision)
                    InitializeState(CurrentState.Wander);

                if (enemiesShooting && charDist <= innacurateDist)
                {
                    if (Random.Range(0,1.0f) >= bravery)
                    {
                        InitializeState(CurrentState.Flee);
                    }
                }
                if (currentAmmo <= 0)
                {
                    InitializeState(CurrentState.Flee);
                }
                if (charDist <= innacurateDist * confidence)
                    ShootAtTarget();
                break;

            case CurrentState.Flee:
                if (!enemiesShooting && currentAmmo > 0)
                {
                    target = FindTarget();
                    if (target != null)
                    {
                        InitializeState(CurrentState.Pursue);
                    }
                    break;
                }
                if(target == null)
                {
                    friend = FindFriend();
                    InitializeState(CurrentState.Pursue);
                    break;
                }
                if ((target.transform.position - transform.position).magnitude >= vision)
                    InitializeState(CurrentState.Wander);

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
                agent.speed = 0.0f;
                agent.Stop();
                break;

            case CurrentState.Wander:
                agent.speed = 3.5f;
                agent.Resume();
                break;

            case CurrentState.Pursue:
                agent.speed = 3.5f;
                agent.Resume();
                break;

            case CurrentState.Flee:
                agent.speed = 7.0f;
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
                if(target != null)
                    Pursue(target.transform.position);
                break;

            case CurrentState.Flee:
                if (target != null)
                    Flee(target.transform.position);
                break;
        }
    }

    /// <summary>
    /// Set's the destination of the character to the position of the target a few frames ahead.
    /// </summary>
    /// <param name="myTarget"> vector position of target </param>
    /// <param name="myTargetVel">Velocity of the target</param>
    public void Pursue(Vector3 myTarget)
    {
        agent.SetDestination(myTarget);
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
        if (target == null)
            return;
        if (currentAmmo <= 0)
            return;

        // Make sure that the target is relatively in front of you.  Later we'll also check to make sure you can see the target via raycasting.
        //if (Vector3.Dot(coneLeft, target.transform.position - transform.position) < 0 && Vector3.Dot(coneRight, target.transform.position - transform.position) > 0)
        if(true)
        {
            currentAmmo--;

            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

            for (int i = 0; i < enemies.Length; i++)
            {
                Vector3 betweenVec = enemies[i].transform.position - transform.position;
                if (enemies[i].GetComponent<Character>() == null)
                    continue;
                if (betweenVec.magnitude <= vision)
                {
                    enemies[i].GetComponent<Character>().enemiesShooting = true;
                }
            }

            Debug.DrawLine(transform.position, target.transform.position);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, target.transform.position - transform.position, out hit))
            {
                if (hit.transform.CompareTag("goodguy") || hit.transform.CompareTag("badguy"))
                {
                    hit.transform.GetComponent<Character>().health--;
                    if (hit.transform.GetComponent<Character>().health <= 0)
                    {
                        hit.transform.GetComponent<Character>().tag = "dead";
                        kills++;
                    }
                }
                return;
            }

            // Get some preliminary info
            float dist = (target.transform.position - transform.position).magnitude;
            float rand = Random.Range(0.0f, 1.0f);

            // If they're within max accuracy range they're within max accuracy.
            if (dist < accurateDist && maxAccuracy > rand)
            {
                target.GetComponent<Character>().health--;
                if (hit.transform.GetComponent<Character>().health <= 0)
                {
                    hit.transform.GetComponent<Character>().tag = "dead";
                    kills++;
                }
                return;
            }


            // Otherwise the further they get the closer to 0% chance of hitting them, linearly.  Linear should be fine for our purpose.
            if(maxAccuracy * Mathf.Lerp(1, 0, (dist - accurateDist) / (innacurateDist - accurateDist)) > rand)
            {
                target.GetComponent<Character>().health--;
                if (hit.transform.GetComponent<Character>().health <= 0)
                {
                    hit.transform.GetComponent<Character>().tag = "dead";
                    kills++;
                }
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

    private GameObject FindFriend()
    {
        GameObject[] friends = GameObject.FindGameObjectsWithTag(friendTag);

        for (int i = 0; i < friends.Length; i++)
        {
            Vector3 betweenVec = friends[i].transform.position - transform.position;
            if (betweenVec.magnitude <= vision && Vector3.Dot(betweenVec, transform.forward) > 0)
            {
                return friends[i];
            }
        }

        return null;
    }    

    private GameObject FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        for (int i = 0; i < enemies.Length; i++)
        {
            Vector3 betweenVec = enemies[i].transform.position - transform.position;
            if (betweenVec.magnitude <= vision && Vector3.Dot(betweenVec, transform.forward) > 0)
            {

                
                return enemies[i];
            }
        }

        return null;        
    }    
}
