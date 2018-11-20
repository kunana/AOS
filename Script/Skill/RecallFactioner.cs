using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecallFactioner : SkillFactioner
{

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
