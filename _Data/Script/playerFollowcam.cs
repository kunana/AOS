using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerFollowcam : MonoBehaviour {

    public GameObject player;
    new Vector3 yFourty = new Vector3(0, 40, 0);
    private void Update()
    {
        transform.position = player.transform.position;
        transform.position += yFourty;
    }
}
