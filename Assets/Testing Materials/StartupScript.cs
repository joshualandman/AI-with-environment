using UnityEngine;
using System.Collections;

public class StartupScript : MonoBehaviour {

    public GameObject objectManager;
    public GameObject terrainPiece;

	// Basic test.
    void Start()
    {
        objectManager.GetComponent<ObjectManager>().Initialize();
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                GameObject myObject = GameObject.Instantiate(terrainPiece) as GameObject;
                objectManager.GetComponent<ObjectManager>().PlaceObject(i, j, myObject);
            }
        }
    }
}
