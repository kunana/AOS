using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignName : MonoBehaviour {

    Text ChampName;

    private void Awake()
    {
        //ChampName = this.GetComponent<Text>();
        //ChampName.text = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>().ChampionName;
    }
    public void AssignText(string name)
    {
        ChampName.text = name;
    }
}
