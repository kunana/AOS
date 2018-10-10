using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

    private void Update()
    {
        transform.Rotate(0, Time.deltaTime * 15,0);
    }

}
