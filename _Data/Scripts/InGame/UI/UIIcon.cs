using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{

    [HideInInspector]
    public StatClass.Stat stat = null;

    public GameObject StatUI;
    public Image ChampionIcon;
    public Text LevelText;
    public Image ExpBar;

    private void FindPlayer()
    {   

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            StructureSetting.instance.ActiveTrue();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        ChampionData cd = Player.GetComponent<ChampionData>();
        stat = cd.mystat;
        ChampionIcon.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + cd.ChampionName);
    }

    void Start()
    {
        FindPlayer();
    }


    // Update is called once per frame
    void Update()
    {
            float ExpPercent = (float)stat.Exp / (float)stat.RequireExp;
            ExpBar.fillAmount = ExpPercent;
        
    }

    public void LevelUp()
    {
        LevelText.text = stat.Level.ToString();
    }

    public void StatButton()
    {
        if (StatUI.activeSelf)
            StatUI.SetActive(false);
        else
            StatUI.SetActive(true);
    }
}
