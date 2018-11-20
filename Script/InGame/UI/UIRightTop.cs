using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRightTop : MonoBehaviour {

    // 타이머는 IngameManager - InGameTimer에서 

    public Text blueTeamKill;
    public Text redTeamKill;
    public Text csText;
    public Text kdaText;

    private ChampionData myChampionData;
    private InGameManager inGameManager;

    // Use this for initialization
    void Start () {
        myChampionData = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>();
        inGameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();
    }
	
	public void AllUpdate()
    {
        CSUpdate();
        kdaText.text = myChampionData.kill.ToString() + "/" + myChampionData.death.ToString() + "/" + myChampionData.assist.ToString();

        int redTotalKill = 0;
        foreach(GameObject redPlayer in inGameManager.redTeamPlayer)
        {
            if (redPlayer.GetComponent<ChampionData>() == null)
                continue;
            redTotalKill += redPlayer.GetComponent<ChampionData>().kill;
        }
        redTeamKill.text = redTotalKill.ToString();

        int blueTotalKill = 0;
        foreach (GameObject bluePlayer in inGameManager.blueTeamPlayer)
        {
            if (bluePlayer.GetComponent<ChampionData>() == null)
                continue;
            blueTotalKill += bluePlayer.GetComponent<ChampionData>().kill;
        }
        blueTeamKill.text = blueTotalKill.ToString();
    }

    public void CSUpdate()
    {
        csText.text = myChampionData.cs.ToString();
    }
}
