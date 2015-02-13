using UnityEngine;
using System.Collections;

public class ObjectManager : MonoBehaviour {

    /// <summary>
    /// Map containing all of the objects in the environment.
    /// </summary>
    GameObject[][] map;

    /// <summary>
    /// Distance a single grid space is by X.
    /// </summary>
    public int xSeparation;

    /// <summary>
    /// Distance a single grid space is by Y.
    /// </summary>
    public int ySeparation;

    /// <summary>
    /// Number of grid spaces along the x axis.
    /// </summary>
    public int width;

    /// <summary>
    /// Number of grid spaces along the y axis.
    /// </summary>
    public int height;

	// Use this for initialization
    public void Initialize()
    {
        // Initialize the jagged array.
        map = new GameObject[width][];
        for (int i = 0; i < width; i++)
        {
            map[i] = new GameObject[height];

            // Making sure everything is explicitly null.
            for (int j = 0; j < height; j++)
            {
                map[i][j] = null;
            }
        }
    }

    /// <summary>
    /// Places an object on the map.
    /// </summary>
    /// <param name="x">X position on the map.</param>
    /// <param name="y">Y position on the map.</param>
    /// <param name="terrainPiece">Terrain piece to place on the map.</param>
    public void PlaceObject(int x, int y, GameObject terrainPiece)
    {
        map[x][y] = terrainPiece;

        // Moves the empty to the right location.
        terrainPiece.transform.position = new Vector3(x * xSeparation, y * ySeparation, terrainPiece.transform.position.z);
    }
}
