using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pool_HP : MonoBehaviour
{

    public static Pool_HP current; //모든 클래스에서 직접 호출 가능

    //풀링할 오브젝트
    public GameObject MinionHPBar;
    public GameObject TowerHPBar;
    public GameObject NexusHPBar;
    public GameObject SmallJungleHPBar;
    public GameObject BigJungleHPBar;
    public GameObject ChampionHPBar;

    public GameObject Storage;

    public int PoolAmount_MinionHPBar = 300;
    public int PoolAmount_TowerHPBar = 30;
    public int PoolAmount_NexusHPBar = 4;
    public int PoolAmount_SmallJungleHPBar = 20;
    public int PoolAmount_BigJungleHPBar = 10;
    public int PoolAmount_ChampionHPBar = 12;

    private List<GameObject> MinionHPBar_List = new List<GameObject>();
    private List<GameObject> TowerHPBar_List = new List<GameObject>();
    private List<GameObject> NexusHPBar_List = new List<GameObject>();
    private List<GameObject> SmallJungleHPBar_List = new List<GameObject>();
    private List<GameObject> BigJungleHPBar_List = new List<GameObject>();
    private List<GameObject> ChampionHPBar_List = new List<GameObject>();

    Dictionary<string, List<GameObject>> hpbarlist = new Dictionary<string, List<GameObject>>();

    private int entityRedCount;
    private int entityBlueCount;
    public int curRedCount = 0;
    public int curBlueCount = 0;

    private void Awake()
    {
        //entityRedCount = PoolAmount_MinionHPBar + PoolAmount_TowerHPBar + PoolAmount_NexusHPBar + PoolAmount_ChampionHPBar / 2;
        //entityBlueCount = PoolAmount_MinionHPBar + PoolAmount_TowerHPBar + PoolAmount_NexusHPBar + PoolAmount_ChampionHPBar / 2;

        //TheFogEntityRed.gameObjects = new GameObject[entityRedCount];
        //TheFogEntityBlue.gameObjects = new GameObject[entityBlueCount];
        //static으로 선언한 Minion_ObjectPool current에 접근
        current = this;

        HpBarPooling(PoolAmount_MinionHPBar, MinionHPBar, "MinionHPBar");
        HpBarPooling(PoolAmount_TowerHPBar, TowerHPBar, "TowerHPBar");
        HpBarPooling(PoolAmount_NexusHPBar, NexusHPBar, "NexusHPBar");
        HpBarPooling(PoolAmount_SmallJungleHPBar, SmallJungleHPBar, "SmallJungleHPBar");
        HpBarPooling(PoolAmount_BigJungleHPBar, BigJungleHPBar, "BigJungleHPBar");
        HpBarPooling(PoolAmount_ChampionHPBar, ChampionHPBar, "ChampionHPBar");


    }

    private void HpBarPooling(int amount, GameObject hpbar, string listname)
    {
        if (!hpbarlist.ContainsKey(listname))
        {
            List<GameObject> list = new List<GameObject>();
            hpbarlist.Add(listname, list);
        }
        List<GameObject> tempList = new List<GameObject>();
        for (int i = 0; i < amount; ++i)
        {
            GameObject obj = Instantiate(hpbar, Storage.transform);
            obj.SetActive(false);
            tempList.Add(obj);
        }
        hpbarlist[listname].InsertRange(0, tempList);
    }

    public GameObject GetPooledHPBar(string listName)
    {
        GameObject obj = hpbarlist[listName][0];
        if (!obj.activeInHierarchy)
        {
            if (hpbarlist[listName].Count == 0)
            {
                HpBarPooling(10, obj, listName);
            }
            hpbarlist[listName].RemoveAt(0);
            hpbarlist[listName].Add(obj);
            obj.SetActive(true);
        }
        return obj;
    }

    public GameObject GetPooledHPBard()
    {
        for (int i = 0; i < MinionHPBar_List.Count; ++i)
        {
            GameObject g = MinionHPBar_List[i];
            if (!g.activeInHierarchy)
            {
                MinionHPBar_List.RemoveAt(i);
                MinionHPBar_List.Add(g);
                g.SetActive(true);
                return g;
            }
        }
        return null;
    }

    //public GameObject GetPooledTowerHPBar()
    //{
    //    for (int i = 0; i < TowerHPBar_List.Count; ++i)
    //    {
    //        GameObject g = TowerHPBar_List[i];
    //        if (!g.activeInHierarchy)
    //        {
    //            TowerHPBar_List.RemoveAt(i);
    //            TowerHPBar_List.Add(g);
    //            g.SetActive(true);
    //            return g;
    //        }
    //    }
    //    return null;
    //}

    //public GameObject GetPooledNexusHPBar()
    //{
    //    for (int i = 0; i < NexusHPBar_List.Count; ++i)
    //    {
    //        GameObject g = NexusHPBar_List[i];
    //        if (!g.activeInHierarchy)
    //        {
    //            NexusHPBar_List.RemoveAt(i);
    //            NexusHPBar_List.Add(g);
    //            g.SetActive(true);
    //            return g;
    //        }
    //    }
    //    return null;
    //}

    //public GameObject GetPooledSmallJungleHPBar()
    //{
    //    for (int i = 0; i < SmallJungleHPBar_List.Count; ++i)
    //    {
    //        GameObject g = SmallJungleHPBar_List[i];
    //        if (!g.activeInHierarchy)
    //        {
    //            SmallJungleHPBar_List.RemoveAt(i);
    //            SmallJungleHPBar_List.Add(g);
    //            g.SetActive(true);
    //            return g;
    //        }
    //    }
    //    return null;
    //}

    //public GameObject GetPooledBigJungleHPBar()
    //{
    //    for (int i = 0; i < BigJungleHPBar_List.Count; ++i)
    //    {
    //        GameObject g = BigJungleHPBar_List[i];
    //        if (!g.activeInHierarchy)
    //        {
    //            BigJungleHPBar_List.RemoveAt(i);
    //            BigJungleHPBar_List.Add(g);
    //            g.SetActive(true);
    //            return g;
    //        }
    //    }
    //    return null;
    //}

    //public GameObject GetPooledChampionHPBar()
    //{
    //    for (int i = 0; i < ChampionHPBar_List.Count; ++i)
    //    {
    //        GameObject g = ChampionHPBar_List[i];
    //        if (!g.activeInHierarchy)
    //        {
    //            ChampionHPBar_List.RemoveAt(i);
    //            ChampionHPBar_List.Add(g);
    //            g.SetActive(true);
    //            return g;
    //        }
    //    }
    //    return null;
    //}
}
