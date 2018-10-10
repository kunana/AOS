using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridWaypointData : MonoBehaviour {

    public int GridX, GridZ;
    private void Awake()
    {
        GridX = (int)(transform.position.x / 2);
        GridZ = (int)(transform.position.z / 2);
    }
}