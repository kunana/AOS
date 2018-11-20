﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIStat : MonoBehaviour
{

    [HideInInspector]
    public StatClass.Stat stat = null;
    public StatClass.Stat originstat = null;

    public Text AttackDamage;
    public Text AbilityPower;
    public Text Defence;
    public Text MagicResist;
    public Text AttackSpeed;
    public Text CooldownReduce;
    public Text Critical;
    public Text MoveSpeed;

    // Use this for initialization
    void Start()
    {
        stat = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>().totalstat;
        originstat = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>().mystat;
        Refresh();
    }

    //// Update is called once per frame
    //void Update () {

    //}

    public void Refresh()
    {
        AttackDamage.text = Mathf.RoundToInt(stat.Attack_Damage).ToString();
        AbilityPower.text = Mathf.RoundToInt(stat.Ability_Power).ToString();
        Defence.text = Mathf.RoundToInt(stat.Attack_Def).ToString();
        MagicResist.text = Mathf.RoundToInt(stat.Ability_Def).ToString();
        float AS = originstat.Attack_Speed * (1 + (stat.UP_AttackSpeed * (stat.Level - 1) + (stat.Attack_Speed - originstat.Attack_Speed)) / 100);
        AttackSpeed.text = System.Math.Round(AS, 2).ToString();
        CooldownReduce.text = Mathf.RoundToInt(stat.CoolTime_Decrease).ToString();
        Critical.text = Mathf.RoundToInt(stat.Critical_Percentage).ToString();
        MoveSpeed.text = Mathf.RoundToInt(stat.Move_Speed * 50f).ToString();
    }
}
