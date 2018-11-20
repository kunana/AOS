using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBehaviour : Photon.MonoBehaviour
{
    public AIDestinationSetter TheAIDest;
    public MonsterAtk monAtk;
    public AIPath TheAIPath;
    public StatClass.Stat stat;
    public string monsterJsonName;
    public FogOfWarEntity TheFogEntity;
    public MonsterRespawn myCenter;
    public List<GameObject> friendsList = new List<GameObject>();
    public BigJungleHP bigJungleHP;
    public SmallJungleHP smallJungleHP;
    private bool firstload = false;
    private AOSMouseCursor cursor;
    bool mouseChanged = false;
    public bool isDead = false;

    private void Awake()
    {
        bigJungleHP = transform.GetComponent<BigJungleHP>();
        smallJungleHP = transform.GetComponent<SmallJungleHP>();
    }
    private void OnEnable()
    {
        isDead = false;
        if (firstload)
        {
            if (bigJungleHP != null)
                bigJungleHP.BasicSetting();
            if (smallJungleHP != null)
                smallJungleHP.BasicSetting();
            isDead = true;
        }

        if (!cursor)
            cursor = GameObject.FindGameObjectWithTag("MouseCursor").GetComponent<AOSMouseCursor>();
    }
    private void Start()
    {
        firstload = true;
    }
    public void InitJungleStatus()
    {
        stat.Hp = stat.MaxHp;
        if (bigJungleHP != null)
            bigJungleHP.InitProgressBar();
        if (smallJungleHP != null)
            smallJungleHP.InitProgressBar();
    }
    public void InitValue()
    {
        monAtk.InitValue();
    }
    public void LateInit()
    {
        TheAIDest = GetComponent<AIDestinationSetter>();
        TheAIPath = GetComponent<AIPath>();
        monAtk = GetComponentInChildren<MonsterAtk>();
        TheFogEntity = GetComponent<FogOfWarEntity>();
        monAtk.LateInit();
    }

    public void SetStat(int i)
    {
        if (monsterJsonName.Contains("Dragon") || monsterJsonName.Contains("Golem"))
            i = 0;
        switch (i)
        {
            case 0:
                stat = StatClass.instance.characterData[monsterJsonName].ClassCopy();
                break;
            case 1:
                stat = StatClass.instance.characterData[monsterJsonName + "2"].ClassCopy();
                break;
        }
    }

    public void IamDead(float time = 0)
    {
        myCenter.SetPosition();
        Invoke("Dead", time);
    }

    private void Dead()
    {
        InitJungleStatus();
        if (monAtk == null)
            monAtk = transform.GetComponentInChildren<MonsterAtk>();
        if (TheAIDest == null)
            TheAIDest = gameObject.GetComponent<AIDestinationSetter>();
        if (TheAIPath == null)
            TheAIPath = gameObject.GetComponent<AIPath>();
        monAtk.TheAIPath = null;
        monAtk.nowTarget = null;
        monAtk.StopAllCoroutines();
        TheAIPath.canMove = true;
        TheAIPath.canSearch = true;
        gameObject.GetComponent<AIDestinationSetter>().target = null;
        myCenter.StartCoroutine("Respawn");
        TheAIDest.target = myCenter.transform;
        monAtk.nowTarget = null;
        gameObject.SetActive(false);

        // 죽을때 마우스바뀐상태면 원래대로
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
        }
    }

    public bool HitMe(float damage = 0, string atkType = "AD", GameObject atker = null) // AD, AP, FD(고정 데미지 = Fixed damage)
    {
        if (!PhotonNetwork.isMasterClient)
            return false;

        bool isDead = false;
        if (atkType.Equals("AD") || atkType.Equals("ad"))
        {
            damage = (damage * 100f) / (100f + stat.Attack_Def);
        }
        else if (atkType.Equals("AP") || atkType.Equals("ap"))
        {
            damage = (damage * 100f) / (100f + stat.Ability_Def);
        }

        if (stat.Hp <1)
            return false;
        if (PhotonNetwork.isMasterClient)
            if (!monAtk.isReturn)
            {
                if (!monAtk.isAtking)
                {
                    monAtk.isAtking = true;
                    monAtk.nowTarget = atker;
                    TheAIDest.target = atker.transform;
                    for (int i = 0; i < friendsList.Count; ++i)
                    {
                        if (friendsList[i] != null)
                            if (friendsList[i].activeInHierarchy)
                                friendsList[i].GetComponent<MonsterBehaviour>().monAtk.isAtking = true;
                    }
                }
            }
        stat.Hp -= damage;
        if (stat.Hp < 1)
        {
            isDead = true;
            bool isDragon = false;
            string team = "";
            if (gameObject.name.Contains("Dragon"))
            {
                isDragon = true;
                if (atker.GetComponent<PhotonView>().owner.GetTeam().Equals("red"))
                {
                    GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>().blueTeamDragonKill++;
                    team = "blue";
                }
                else
                {
                    GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>().redTeamDragonKill++;
                    team = "red";
                }
            }
            bool isChamp = true;
            if (atker == null)
                isChamp = false;
            else if (!atker.layer.Equals(LayerMask.NameToLayer("Champion")))
                isChamp = false;
            int id;
            if (isChamp)
                id = atker.GetPhotonView().viewID;
            else
                id = -1;
            KillManager.instance.SomebodyKillMonsterRPC(this.photonView.viewID, id, isChamp, isDragon, team);
        }
        else
        {
            KillManager.instance.ChangeMonsterHPRPC(this.photonView.viewID, stat.Hp);
        }
        return isDead;
    }

    public void CallDead(float time, bool isDragon, string team)
    {
        isDead = true;
        stat.Hp = 0;
        IamDead(time);
        if (monAtk == null)
            monAtk = transform.GetComponentInChildren<MonsterAtk>();
        monAtk.enemiesList.Clear();

        if (PhotonNetwork.isMasterClient)
            return;

        if (isDragon)
        {
            if (team.Equals("red"))
            {
                GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>().blueTeamDragonKill++;
            }
            else
            {
                GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>().redTeamDragonKill++;
            }
        }
    }

    [PunRPC]
    public void HitSync(int viewID)
    {
        GameObject g = PhotonView.Find(viewID).gameObject;
        if (g != null)
        {
            if (g.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                g.GetComponent<ChampionBehavior>().HitMe(stat.Attack_Damage, "AD", gameObject, gameObject.name);
            }
        }
    }

    [PunRPC]
    public void HitSyncKey(string key)
    {
        if (TowersManager.towers[key] != null)
        {
            TowersManager.towers[key].GetComponent<TowerBehaviour>().HitMe(stat.Attack_Damage);
        }
    }

    public void HitRPC(int viewID)
    {
        if (this.photonView == null)//나중에포톤뷰널이라고터지면무용지물인거니까지워라
            return;//이것도
        this.photonView.RPC("HitSync", PhotonTargets.Others, viewID);
    }

    [PunRPC]
    public void ReturnOtherClientsSync(bool _return)
    {
        monAtk.isReturn = _return;
    }

    public void ReturnOtherClients(bool _return)
    {
        this.photonView.RPC("ReturnOtherClientsSync", PhotonTargets.Others, _return);
    }

    private void OnMouseOver()
    {
        if (!mouseChanged)
        {
            cursor.SetCursor(2, Vector2.zero);
            mouseChanged = true;
        }
    }

    private void OnMouseExit()
    {
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
            mouseChanged = false;
        }
    }
}