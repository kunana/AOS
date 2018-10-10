using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSk : MonoBehaviour {

    public Skills s;
	// Use this for initialization
	void Start () {
        s = gameObject.AddComponent<AlistarSkill>();
        //print();
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
