using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionIngameList : MonoBehaviour
{
    public static List<GameObject> RedChampionList = new List<GameObject>();
    public static List<GameObject> BlueChampionList = new List<GameObject>();
    private void Start()
    {
        ChampionBehavior[] pvArray = FindObjectsOfType(typeof(ChampionBehavior)) as ChampionBehavior[];
        for (int i = 0; i < pvArray.Length; ++i)
        {
            if (pvArray[i].gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                if (pvArray[i].Team.Equals("Red"))
                    RedChampionList.Add(pvArray[i].gameObject);
                else
                    BlueChampionList.Add(pvArray[i].gameObject);
            }
        }
        SetFogEntityFaction(RedChampionList, 0);
        SetFogEntityFaction(BlueChampionList, 1);
    }

    //private void TurnOnTheFogEntity(List<GameObject> list)
    //{
    //    for (int i = 0; i < list.Count; ++i)
    //    {
    //        list[i].GetComponent<FogOfWarEntity>().enabled = false;
    //        list[i].GetComponent<FogOfWarEntity>().enabled = true;
    //    }
    //}

    private void SetFogEntityFaction(List<GameObject> list, int factionNum)
    {
        //0123 => 1248
        int factionInt = 1;
        for (int i = -1; ++i < factionNum; factionInt *= 2) ;
        FogOfWar.Players faction = (FogOfWar.Players)factionInt;
        for (int i = 0; i < list.Count; ++i)
        {
            FogOfWarEntity f = list[i].GetComponent<FogOfWarEntity>();
            if (f != null)
            {
                f.enabled = false;
                f.faction = faction;
                f.enabled = true;
            }
        }
    }
}
