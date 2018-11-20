using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RecallUI : MonoBehaviour {

    public ProgressBar RecallProgressBar;
    public Text RemainTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void RecallButton()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>().Recall();
    }
}
