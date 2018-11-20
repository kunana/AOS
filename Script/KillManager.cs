using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillManager : Photon.PunBehaviour
{
    private SystemMessage sysmsg;
    public static List<MinionBehavior> photonMinionList = new List<MinionBehavior>();
    public Dictionary<int, ChampionData> photonChampDic = new Dictionary<int, ChampionData>();
    public static Dictionary<int, MonsterBehaviour> photonMonsterDic = new Dictionary<int, MonsterBehaviour>();
    public Dictionary<int, ChampionBehavior> photonChampBehavDic = new Dictionary<int, ChampionBehavior>();
    private static KillManager _instance = null;

    public static KillManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (KillManager)FindObjectOfType(typeof(KillManager));
            }
            return _instance;
        }
    }

    void Awake()
    {
        sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
    }

    [PunRPC]
    public void ChangeMonsterHP(int monViewID, float hp)
    {
        if (this != null)
        {
            if (!photonMonsterDic.ContainsKey(monViewID))
            {
                MonsterBehaviour monBehav = PhotonView.Find(monViewID).GetComponent<MonsterBehaviour>();
                if (monBehav != null)
                    photonMonsterDic.Add(monViewID, monBehav);
            }
            if (photonMonsterDic[monViewID] != null)
            {
                if (photonMonsterDic[monViewID].gameObject.activeInHierarchy)
                {
                    photonMonsterDic[monViewID].stat.Hp = hp;
                }
            }
        }
    }

    public void ChangeMonsterHPRPC(int monViewID, float hp)
    {
        photonView.RPC("ChangeMonsterHP", PhotonTargets.Others, monViewID, hp);
    }

    [PunRPC]
    public void ChangeMinionHP(int minKey, float hp)
    {
        if (this != null)
        {
            if (photonMinionList.Count > minKey)
            {
                if (photonMinionList[minKey] != null)
                {
                    if (photonMinionList[minKey].gameObject.activeInHierarchy)
                    {
                        photonMinionList[minKey].stat.Hp = hp;
                    }
                }
            }
        }
    }

    public void ChangeMinionHPRPC(int minKey, float hp)
    {
        photonView.RPC("ChangeMinionHP", PhotonTargets.Others, minKey, hp);
    }

    [PunRPC]
    public void SomebodyKillMinion(int minKey, int champViewID, bool isChamp)
    {
        if (this != null)
        {
            if (isChamp)
            {
                if (!photonChampDic.ContainsKey(champViewID))
                {
                    ChampionData ChampData = PhotonView.Find(champViewID).GetComponent<ChampionData>();
                    if (ChampData != null)
                        photonChampDic.Add(champViewID, ChampData);
                }
                if (photonChampDic[champViewID].GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                {
                    photonChampDic[champViewID].Kill_CS_Gold_Exp(photonMinionList[minKey].name, 1, photonMinionList[minKey].transform.position);
                }
            }
            photonMinionList[minKey].CallDead(0.2f);
        }
    }

    public void SomebodyKillMinionRPC(int minKey, int champViewID, bool isChamp)
    {
        this.photonView.RPC("SomebodyKillMinion", PhotonTargets.AllViaServer, minKey, champViewID, isChamp);
    }

    [PunRPC]
    public void SomebodyKillMonster(int monViewID, int champViewID, bool isChamp, bool isDragon, string team = "")
    {
        if (this != null)
        {
            if (!photonMonsterDic.ContainsKey(monViewID))
            {
                MonsterBehaviour monBehav = PhotonView.Find(monViewID).GetComponent<MonsterBehaviour>();
                if (monBehav != null)
                    photonMonsterDic.Add(monViewID, monBehav);
            }
            if (isChamp)
            {
                if (!photonChampDic.ContainsKey(champViewID))
                {
                    ChampionData ChampData = PhotonView.Find(champViewID).GetComponent<ChampionData>();
                    if (ChampData != null)
                        photonChampDic.Add(champViewID, ChampData);
                }
                if (photonChampDic[champViewID].GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                {
                    photonChampDic[champViewID].Kill_CS_Gold_Exp(photonMonsterDic[monViewID].name, 3, photonMonsterDic[monViewID].transform.position);
                }
            }
            photonMonsterDic[monViewID].CallDead(0.05f, isDragon, team);
        }
    }

    public void SomebodyKillMonsterRPC(int monViewID, int champViewID, bool isChamp, bool isDragon, string team = "")
    {
        photonView.RPC("SomebodyKillMonster", PhotonTargets.AllViaServer, monViewID, champViewID, isChamp, isDragon, team);
    }

    [PunRPC]
    public void SomebodyKillChampion(int dieViewID, int atkViewID, bool atkIsChamp, string killerName)
    {
        ChampionBehavior dieChampBehav;
        ChampionData atkChampData;
        if (!photonChampBehavDic.ContainsKey(dieViewID))
        {
            dieChampBehav = PhotonView.Find(dieViewID).GetComponent<ChampionBehavior>();
            if (dieChampBehav != null)
                photonChampBehavDic.Add(dieViewID, dieChampBehav);
        }
        if (atkIsChamp)
        {

            if (!photonChampDic.ContainsKey(atkViewID))
            {
                atkChampData = PhotonView.Find(atkViewID).GetComponent<ChampionData>();
                if (atkChampData != null)
                    photonChampDic.Add(atkViewID, atkChampData);
            }
            if (photonChampDic[atkViewID].GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
            {
                if (atkViewID == dieViewID) // 근우 추가.
                    return;
                photonChampDic[atkViewID].GetComponent<ChampionBehavior>().myChampAtk.IKillChamp();
                photonChampDic[atkViewID].Kill_CS_Gold_Exp(photonChampBehavDic[dieViewID].name, 0, photonChampBehavDic[dieViewID].transform.position);
            }
        }
        //시스템 메세지
        if(killerName.Contains("tower")|| killerName.Contains("Tower"))
        {
            sysmsg.sendKillmsg("tower", photonChampBehavDic[dieViewID].name.ToString(), "ex");
        }
        else if (killerName.Contains("Minion") || killerName.Contains("minion"))
        {
            if (photonChampBehavDic[dieViewID].Team.ToLower().Equals("red"))
                sysmsg.sendKillmsg("minion", photonChampBehavDic[dieViewID].name.ToString(), "blue");
            else if (photonChampBehavDic[dieViewID].Team.ToLower().Equals("blue"))
                sysmsg.sendKillmsg("minion", photonChampBehavDic[dieViewID].name.ToString(), "red");
        }
        else if(killerName.Contains("Obj") || killerName.Contains("obj"))
        {
            sysmsg.sendKillmsg("monster", photonChampBehavDic[dieViewID].name.ToString(), "ex");
        }
        else
        {
            if (photonChampBehavDic[dieViewID].Team.ToLower().Equals("red"))
            sysmsg.sendKillmsg(killerName, photonChampBehavDic[dieViewID].name.ToString(), "blue");
            else if (photonChampBehavDic[dieViewID].Team.ToLower().Equals("blue"))
            sysmsg.sendKillmsg(killerName, photonChampBehavDic[dieViewID].name.ToString(), "red");
        }
        
        photonChampBehavDic[dieViewID].CallDead(0.2f, atkViewID, atkIsChamp);
    }

    public void SomebodyKillChampionRPC(int dieViewID, int atkViewID, bool atkIsChamp, string killerName)
    {
        photonView.RPC("SomebodyKillChampion", PhotonTargets.AllViaServer, dieViewID, atkViewID, atkIsChamp, killerName);
    }

    private void OnDestroy()
    {
        photonMinionList.Clear();
        photonChampDic.Clear();
        photonMonsterDic.Clear();
        photonChampBehavDic.Clear();
        _instance = null;
    }
}