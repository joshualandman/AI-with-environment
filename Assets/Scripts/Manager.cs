using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Manager : MonoBehaviour
{

    public List<float> bestGoodConf;
    public List<float> bestGoodBrave;
    public List<float> bestBadConf;
    public List<float> bestBadBrave;

    public bool goodWaiting = false;
    public bool badWaiting = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        GameObject[] goodguys = GameObject.FindGameObjectsWithTag("goodguy");
        GameObject[] badguys = GameObject.FindGameObjectsWithTag("badguy");
        GameObject[] deadgoodguys = GameObject.FindGameObjectsWithTag("deadgoodguy");
        GameObject[] deadbadguys = GameObject.FindGameObjectsWithTag("deadbadguy");
        if (!goodWaiting)
        {
            
            bestGoodConf = new List<float>();
            bestGoodBrave = new List<float>();
            for (int i = 0; i < goodguys.Length; i++)
            {
                if (goodguys[i].GetComponent<Character>().health > 0){
                    bestGoodConf.Add(goodguys[i].GetComponent<Character>().confidence);
                    bestGoodBrave.Add(goodguys[i].GetComponent<Character>().bravery);
                }
            }
            if (bestGoodConf.Count <= 5)
            {
                goodWaiting = true;
            }
        }
         if (!badWaiting)
        {
           
            bestBadConf = new List<float>();
            bestBadBrave = new List<float>();
            for (int i = 0; i < badguys.Length; i++)
            {
                if (badguys[i].GetComponent<Character>().health > 0){
                    bestBadConf.Add(badguys[i].GetComponent<Character>().confidence);
                    bestBadBrave.Add(badguys[i].GetComponent<Character>().bravery);
                }
            }
            if (bestBadConf.Count <= 5)
            {
                badWaiting = true;
            }
        }

        if(goodguys.Length == 0 && !badWaiting)
        {
            badWaiting = true;
            while(bestBadBrave.Count >5)
            {
                bestBadBrave.RemoveAt(Random.Range(0, bestBadBrave.Count));
            }
            while (bestBadConf.Count > 5)
            {
                bestBadConf.RemoveAt(Random.Range(0, bestBadConf.Count));
            }
        }

        if (badguys.Length == 0 && !goodWaiting)
        {
            goodWaiting = true;
            while (bestGoodBrave.Count > 5)
            {
                bestGoodBrave.RemoveAt(Random.Range(0, bestGoodBrave.Count));
            }
            while (bestGoodConf.Count > 5)
            {
                bestGoodConf.RemoveAt(Random.Range(0, bestGoodConf.Count));
            }
        }

        if(goodWaiting && badWaiting)
        {
            goodWaiting = false;
            badWaiting = false;
            //restart the map
            for(int i = 0; i < goodguys.Length + deadgoodguys.Length; i++)
            {
                if (i < goodguys.Length)
                {
                    goodguys[i].transform.position = goodguys[i].GetComponent<Character>().initialPos;
                    goodguys[i].GetComponent<Character>().target = null;
                    goodguys[i].tag = "goodguy";
                    goodguys[i].GetComponent<Character>().confidence = bestGoodConf[i % bestGoodConf.Count] + Random.Range(-0.5f, 0.5f);
                    if (goodguys[i].GetComponent<Character>().confidence > 1)
                        goodguys[i].GetComponent<Character>().confidence = 1;
                    if (goodguys[i].GetComponent<Character>().confidence < 0)
                        goodguys[i].GetComponent<Character>().confidence = 0;
                    goodguys[i].GetComponent<Character>().bravery = bestGoodBrave[i % bestGoodBrave.Count] + Random.Range(-0.5f, 0.5f);
                    if (goodguys[i].GetComponent<Character>().bravery > 1)
                        goodguys[i].GetComponent<Character>().bravery = 1;
                    if (goodguys[i].GetComponent<Character>().bravery < 0)
                        goodguys[i].GetComponent<Character>().bravery = 0;
                    goodguys[i].GetComponent<Character>().currentAmmo = goodguys[i].GetComponent<Character>().maxAmmo;
                    goodguys[i].GetComponent<Character>().InitializeState(CurrentState.Wander);
                }
                else
                {
                    deadgoodguys[i - goodguys.Length].transform.position = deadgoodguys[i - goodguys.Length].GetComponent<Character>().initialPos;
                    deadgoodguys[i - goodguys.Length].tag = "goodguy";
                    deadgoodguys[i - goodguys.Length].GetComponent<Character>().target = null;
                    deadgoodguys[i - goodguys.Length].GetComponent<Character>().confidence = bestGoodConf[i % bestGoodConf.Count] + Random.Range(-0.5f, 0.5f);
                    if (deadgoodguys[i - goodguys.Length].GetComponent<Character>().confidence > 1)
                        deadgoodguys[i - goodguys.Length].GetComponent<Character>().confidence = 1;
                    if (deadgoodguys[i - goodguys.Length].GetComponent<Character>().confidence < 0)
                        deadgoodguys[i - goodguys.Length].GetComponent<Character>().confidence = 0;
                    deadgoodguys[i - goodguys.Length].GetComponent<Character>().bravery = bestGoodBrave[i % bestGoodBrave.Count] + Random.Range(-0.5f, 0.5f);
                    if (deadgoodguys[i - goodguys.Length].GetComponent<Character>().bravery > 1)
                        deadgoodguys[i - goodguys.Length].GetComponent<Character>().bravery = 1;
                    if (deadgoodguys[i - goodguys.Length].GetComponent<Character>().bravery < 0)
                        deadgoodguys[i - goodguys.Length].GetComponent<Character>().bravery = 0;
                    deadgoodguys[i - goodguys.Length].GetComponent<Character>().currentAmmo = deadgoodguys[i - goodguys.Length].GetComponent<Character>().maxAmmo;
                    deadgoodguys[i - goodguys.Length].GetComponent<Character>().InitializeState(CurrentState.Wander);
                }
            }

            for (int i = 0; i < badguys.Length + deadbadguys.Length; i++)
            {
                if (i < badguys.Length)
                {
                    badguys[i].transform.position = badguys[i].GetComponent<Character>().initialPos;
                    badguys[i].GetComponent<Character>().target = null;
                    badguys[i].tag = "badguy";
                    badguys[i].GetComponent<Character>().confidence = bestBadConf[i % bestBadConf.Count] + Random.Range(-0.5f, 0.5f);
                    if (badguys[i].GetComponent<Character>().confidence > 1)
                        badguys[i].GetComponent<Character>().confidence = 1;
                    if (badguys[i].GetComponent<Character>().confidence < 0)
                        badguys[i].GetComponent<Character>().confidence = 0;
                    badguys[i].GetComponent<Character>().bravery = bestBadBrave[i % bestBadBrave.Count] + Random.Range(-0.5f, 0.5f);
                    if (badguys[i].GetComponent<Character>().bravery > 1)
                        badguys[i].GetComponent<Character>().bravery = 1;
                    if (badguys[i].GetComponent<Character>().bravery < 0)
                        badguys[i].GetComponent<Character>().bravery = 0;
                    badguys[i].GetComponent<Character>().currentAmmo = badguys[i].GetComponent<Character>().maxAmmo;
                    badguys[i].GetComponent<Character>().InitializeState(CurrentState.Wander);
                }
                else
                {
                    deadbadguys[i - badguys.Length].transform.position = deadbadguys[i - badguys.Length].GetComponent<Character>().initialPos;
                    deadbadguys[i - badguys.Length].tag = "badguy";
                    deadbadguys[i - badguys.Length].GetComponent<Character>().target = null;
                    deadbadguys[i - badguys.Length].GetComponent<Character>().confidence = bestBadConf[i % bestBadConf.Count] + Random.Range(-0.5f, 0.5f);
                    if (deadbadguys[i - badguys.Length].GetComponent<Character>().confidence > 1)
                        deadbadguys[i - badguys.Length].GetComponent<Character>().confidence = 1;
                    if (deadbadguys[i - badguys.Length].GetComponent<Character>().confidence < 0)
                        deadbadguys[i - badguys.Length].GetComponent<Character>().confidence = 0;
                    deadbadguys[i - badguys.Length].GetComponent<Character>().bravery = bestBadBrave[i % bestBadBrave.Count] + Random.Range(-0.5f, 0.5f);
                    if (deadbadguys[i - badguys.Length].GetComponent<Character>().bravery > 1)
                        deadbadguys[i - badguys.Length].GetComponent<Character>().bravery = 1;
                    if (deadbadguys[i - badguys.Length].GetComponent<Character>().bravery < 0)
                        deadbadguys[i - badguys.Length].GetComponent<Character>().bravery = 0;
                    deadbadguys[i - badguys.Length].GetComponent<Character>().currentAmmo = deadbadguys[i - badguys.Length].GetComponent<Character>().maxAmmo;
                    deadbadguys[i - badguys.Length].GetComponent<Character>().InitializeState(CurrentState.Wander);
                }
            }
        }
    }
}
