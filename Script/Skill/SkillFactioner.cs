﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillFactioner : MonoBehaviour
{
    public FogOfWarEntity ChampFogEntity;
    public FogOfWarEntity myFogEntity;
    //private void Awake()
    //{
    //    myFogEntity.SetSameTeam(ChampFogEntity);
    //}
    private void OnEnable()
    {
        if (ChampFogEntity != null)
            myFogEntity.SetSameTeam(ChampFogEntity);
    }
    void Update()
    {
        myFogEntity.isInTheBush = ChampFogEntity.isInTheBush;
        myFogEntity.isInTheBushMyEnemyToo = ChampFogEntity.isInTheBushMyEnemyToo;
    }
}