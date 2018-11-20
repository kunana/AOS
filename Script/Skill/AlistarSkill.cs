using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DG.Tweening;
public class AlistarSkill : Skills
{   //R 스킬은 방어력 감소이므로, 나중에 챔피언이 공격받은 후를 만들고 완성시킨다.
    //그리고 E 스킬에서 유닛 무시 이동은 구현하지 못했다. 
    //하려면 하겠지만 현재의 A* 등을 보면 꼬이는 부분이 많이 생길 것 같아서 우선 넘어감.
    public GameObject mySkills;
    public GameObject QSkillprefab = null;
    public GameObject WSkillprefab = null;
    public GameObject ESkillprefab = null;
    public GameObject RSkillprefab = null;
    public GameObject PlayerAStarTarget = null;
    private AIPath TheAIPath = null;
    public enum SSelect { none, Q, W, E, R };
    public SSelect skillselect = SSelect.none;
    private float rSkillTempVal = 0;
    string team = "";
    private SystemMessage sysmsg;
    private Vector3 adjust = new Vector3(0, 1f, 0f);
    void OnLevelWasLoaded(int level)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Contains("InGame"))
        {
            if (!sysmsg)
                sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
            if (!TheUIStat)
                FindUICanvas();
        }
    }
    public override void InitInstance()
    {
        team = GetComponent<PhotonView>().owner.GetTeam().ToString();
        base.InitInstance();
        TheChampionData.playerSkill = this;
        mySkills = new GameObject("AlistarSkills");
        mySkills.transform.SetParent(SkillParticleManager.transform);
        PlayerAStarTarget = GetComponent<PlayerMouse>().myTarget;
        TheAIPath = GetComponent<AIPath>();
        skillData = TheSkillClass.skillData["Alistar"];

    }

    private void Awake()
    {
        InitInstance();
        AllPooling();
    }

    private void AllPooling()
    {
        Pooling(QSkillprefab, "Q");
        Pooling(WSkillprefab, "W");
        Pooling(ESkillprefab, "E");
        Pooling(RSkillprefab, "R");
    }

    public void Pooling(GameObject prefab, string type, int amount = 10)
    {
        if (!SkillObj.ContainsKey(type))
        {
            List<GameObject> list = new List<GameObject>();
            SkillObj.Add(type, list);
        }
        List<GameObject> tempList = new List<GameObject>();
        for (int i = 0; i < amount; ++i)
        {
            GameObject obj = Instantiate(prefab, mySkills.transform);
            obj.GetComponent<SkillFactioner>().ChampFogEntity = TheChampionBehaviour.GetComponent<FogOfWarEntity>();
            if (team == "")
                team = GetComponent<PhotonView>().owner.GetTeam().ToString();
            if (team.Equals("red"))
                obj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
            else if (team.Equals("blue"))
                obj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
            obj.SetActive(false);
            tempList.Add(obj);
            switch (type)
            {
                case "Q":
                    obj.GetComponent<AlistarQ>().mySkill = this;
                    break;
                case "E":
                    obj.GetComponent<AlistarE>().mySkill = this;
                    break;
            }
        }
        SkillObj[type].InsertRange(0, tempList);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {//스킬선택해제
            TheSplatManager.Cancel();
            InitTempValue();
            skillselect = SSelect.none;
            isSkillIng = false;
        }
        if (skillselect.Equals(SSelect.W))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = (-1) - ((1 << LayerMask.NameToLayer("WallCollider")));
                RaycastHit[] r = Physics.RaycastAll(ray, 280, layerMask);
                bool isEnemyClick = false;
                foreach (RaycastHit rc in r)
                {//미니언일때부터 분류. 챔피언은 나중에 추가
                    //if (rc.transform.tag.Equals("Minion") || rc.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                    if (rc.transform.tag.Equals("Minion"))
                    {
                        if (!rc.transform.name.Contains(TheChampionBehaviour.Team))
                            if (rc.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                                isEnemyClick = true;
                    }
                    else if (rc.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                    {
                        if (!rc.transform.GetComponent<ChampionBehavior>().Team.Equals(TheChampionBehaviour.Team))
                            if (rc.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                                isEnemyClick = true;
                    }
                    else if (rc.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
                    {
                        if (rc.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                            isEnemyClick = true;
                    }
                    if (isEnemyClick)
                    {
                        if (Vector3.Distance(transform.position, rc.transform.position) <= TheSplatManager.Point.Range)
                        {
                            isSkillIng = true;
                            TheSplatManager.Cancel();
                            TheAIPath.isStopped = true;
                            TempObject1 = rc.transform.gameObject;
                            TempVector1 = transform.position;
                            TempVector2 = rc.transform.position;
                            TheChampionData.UsedW();
                            transform.DOLookAt(TempVector2, 0.1f);
                            transform.DOMove(TempVector2, 0.1f);
                            Invoke("W", 0.1f);

                            SkillEnd(0.1f);
                            break;
                        }
                    }
                    else
                    {
                        TheSplatManager.Cancel();
                        InitTempValue();
                        skillselect = SSelect.none;
                        isSkillIng = false;
                    }
                }
            }
        }
    }

    public override void QCasting()
    {
        isSkillIng = true;
        skillselect = SSelect.none;
        TheSplatManager.Cancel();
        HitEffectRPC("Alistar", "Q");
        Q();
        TheChampionData.UsedQ();
        SkillEnd(1f);
        championAnimation.AnimationApply("Q", true);
        championAnimation.AnimationApply("Q", false, 0.7f);
    }

    public override void WCasting()
    {
        if (skillselect != SSelect.W)
        {
            skillselect = SSelect.W;
            TheSplatManager.Point.Select();
            TheSplatManager.Point.Scale = 14f;
            TheSplatManager.Point.Range = 14f;
        }
    }

    public override void ECasting()
    {
        isSkillIng = true;
        skillselect = SSelect.E;
        HitEffectRPC("Alistar", "E", 10, 0.5f);
        for (int i = 0; i < 10; ++i)
            Invoke("E", 0.5f * i);
        SkillEnd(5);
        TheChampionData.UsedE();
    }

    public override void RCasting()
    {
        //HitEffectRPC("Alistar", "R", 14, 0.5f);
        //for (int i = 0; i < 14; ++i)
        //    Invoke("R", 0.5f * i);
        //한개로 나오게 바꿈
        HitEffectRPC("Alistar", "R", 1, 0.5f);
        Invoke("R", 0.5f);
        TheChampionData.UsedR();
        switch (TheChampionData.skill_R)
        {
            case 1: rSkillTempVal = 122; break; //55% -> 45 = 10000/(100+x) -> x = 1100/9 = 122.2222222222
            case 2: rSkillTempVal = 186; break; //65% -> 35 = 10000/(100+x) -> x = 1300/7 = 185.7142857142
            case 3: rSkillTempVal = 300; break; //75% -> 25 = 10000/(100+x) -> x = 300
        }
        TheChampionData.totalstat.Ability_Def += rSkillTempVal;
        TheChampionData.totalstat.Attack_Def += rSkillTempVal;
        if (!TheUIStat)
            FindUICanvas();
        TheUIStat.Refresh();
        Invoke("DownDefence", 7f);
    }

    public override void Q()
    {
        GameObject obj = SkillObj["Q"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(QSkillprefab, "Q", 10);
            obj = SkillObj["Q"][0];
        }
        SkillObj["Q"].RemoveAt(0);
        SkillObj["Q"].Add(obj);
        PauseMove(0.7f);
        obj.transform.position = transform.position;
        obj.SetActive(true);
    }
    public override void W()
    {
        InitTempValue();
        OnMove();
        HitEffectRPC("Alistar", "W");
        GameObject obj = SkillObj["W"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(QSkillprefab, "W", 10);
            obj = SkillObj["W"][0];
        }
        SkillObj["W"].RemoveAt(0);
        SkillObj["W"].Add(obj);
        obj.transform.position = transform.position;
        obj.SetActive(true);
        if (TempObject1.tag.Equals("Minion"))
        {
            MinionBehavior mB = TempObject1.GetComponent<MinionBehavior>();
            if (!TempObject1.gameObject.name.Contains(TheChampionBehaviour.Team))
            {
                MinionAtk mA = mB.minAtk;
                //mA.PushMe(Vector3.up * 3 + TempObject1.transform.position
                //    + (((TempObject1.transform.position - TempVector1).normalized) * 5), 0.5f);
                Vector3 direction = (TempObject1.transform.position - TempVector1).normalized;
                Vector3 v = TempObject1.transform.position
                    + (direction * 10); ;
                RaycastHit hit;
                if (Physics.Raycast(mA.transform.position, direction, out hit, 12, 1 << LayerMask.NameToLayer("WallCollider")))
                {
                    float dis = Vector3.Distance(hit.point, TempObject1.transform.position);
                    v = TempObject1.transform.position
                    + (direction * (dis - 1f));
                }

                v.y = 0;
                mA.PushMe(v, 0.5f);
                mA.PauseAtk(1f, true);
                float damage = skillData.wDamage[TheChampionData.skill_W - 1]
                    + Acalculate(skillData.wAstat, skillData.wAvalue);

                if (mB != null)
                {
                    int viewID = mB.GetComponent<PhotonView>().viewID;
                    HitRPC(viewID, damage, "AP", "Push");
                    if (mB.HitMe(damage, "AP", gameObject))
                    {
                        //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                        TheChampionAtk.ResetTarget();

                        // 스킬쏜애 주인이 나면 킬올리자
                        if (GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        {
                            TheChampionData.Kill_CS_Gold_Exp(TempObject1.gameObject.name, 1, TempObject1.transform.position);
                        }
                    }
                }
            }
        }
        //else if (TempObject1.tag.Equals("Player"))
        else if (TempObject1.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            ChampionBehavior cB = TempObject1.GetComponent<ChampionBehavior>();
            if (cB.Team != TheChampionBehaviour.Team)
            {
                ChampionAtk cA = cB.myChampAtk;
                Vector3 direction = (TempObject1.transform.position - TempVector1).normalized;
                Vector3 v = TempObject1.transform.position
                    + (direction * 5); ;
                RaycastHit hit;
                if (Physics.Raycast(cA.transform.position, direction, out hit, 6, 1 << LayerMask.NameToLayer("WallCollider")))
                {
                    float dis = Vector3.Distance(hit.point, TempObject1.transform.position);
                    v = TempObject1.transform.position
                    + (direction * (dis - 1f));
                }

                v.y = 0.5f;
                cA.PushMe(v, 0.5f);
                cA.PauseAtk(1f, true);
                float damage = skillData.wDamage[TheChampionData.skill_W - 1]
        + Acalculate(skillData.wAstat, skillData.wAvalue);

                if (cB != null)
                {
                    int viewID = cB.GetComponent<PhotonView>().viewID;
                    HitRPC(viewID, damage, "AP", "Push");
                    if (cB.HitMe(damage, "AP", gameObject, gameObject.name))
                    {
                        TheChampionAtk.ResetTarget();
                        if (!sysmsg)
                            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
                        //sysmsg.sendKillmsg("alistar", TempObject1.GetComponent<ChampionData>().ChampionName, TheChampionBehaviour.Team.ToString());
                        // 스킬쏜애 주인이 나면 킬올리자
                        //if (GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        //{

                        //    TheChampionData.Kill_CS_Gold_Exp(TempObject1.gameObject.name, 0, TempObject1.transform.position);
                        //}
                    }
                }
            }
        }
        else if (TempObject1.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            MonsterBehaviour mB = TempObject1.GetComponent<MonsterBehaviour>();
            MonsterAtk mA = mB.monAtk;
            Vector3 direction = (TempObject1.transform.position - TempVector1).normalized;
            Vector3 v = TempObject1.transform.position
                + (direction * 5); ;
            RaycastHit hit;
            if (Physics.Raycast(mA.transform.position, direction, out hit, 6, 1 << LayerMask.NameToLayer("WallCollider")))
            {
                float dis = Vector3.Distance(hit.point, TempObject1.transform.position);
                v = TempObject1.transform.position
                + (direction * (dis - 1f));
            }

            v.y = 0;
            mA.PushMe(v, 0.5f);
            mA.PauseAtk(1f, true);
            float damage = skillData.wDamage[TheChampionData.skill_W - 1]
    + Acalculate(skillData.wAstat, skillData.wAvalue);

            if (mB != null)
            {
                int viewID = mB.GetComponent<PhotonView>().viewID;
                HitRPC(viewID, damage, "AP", "Push");
                if (mB.HitMe(damage, "AP", gameObject))
                {
                    TheChampionAtk.ResetTarget();

                    //// 스킬쏜애 주인이 나면 킬올리자
                    //if (GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                    //{
                    //    TheChampionData.Kill_CS_Gold_Exp(TempObject1.gameObject.name, 3, TempObject1.transform.position);
                    //}
                }
            }

        }
        skillselect = SSelect.none;
    }
    public override void E()
    {
        GameObject obj = SkillObj["E"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(ESkillprefab, "E", 10);
            obj = SkillObj["E"][0];
        }
        SkillObj["E"].RemoveAt(0);
        SkillObj["E"].Add(obj);
        obj.transform.position = transform.position;
        obj.SetActive(true);
    }
    public override void R() //부모따라다니게 바꿈
    {
        GameObject obj = SkillObj["R"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(RSkillprefab, "R", 10);
            obj = SkillObj["R"][0];
        }
        SkillObj["R"].RemoveAt(0);
        SkillObj["R"].Add(obj);
        obj.transform.position = transform.position;
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }

    public override void QEffect()
    {
        GameObject obj = SkillObj["Q"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(QSkillprefab, "Q", 10);
            obj = SkillObj["Q"][0];
        }
        SkillObj["Q"].RemoveAt(0);
        SkillObj["Q"].Add(obj);
        obj.transform.position = transform.position + adjust;
        obj.SetActive(true);
    }

    public override void WEffect()
    {
        GameObject obj = SkillObj["W"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(QSkillprefab, "W", 10);
            obj = SkillObj["W"][0];
        }
        SkillObj["W"].RemoveAt(0);
        SkillObj["W"].Add(obj);
        obj.transform.position = transform.position + adjust;
        obj.SetActive(true);
    }

    public override void EEffect()
    {
        GameObject obj = SkillObj["E"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(ESkillprefab, "E", 10);
            obj = SkillObj["E"][0];
        }
        SkillObj["E"].RemoveAt(0);
        SkillObj["E"].Add(obj);
        obj.transform.position = transform.position + adjust;
        obj.SetActive(true);
    }

    public override void REffect() //부모따라다니게 바꿈
    {
        GameObject obj = SkillObj["R"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(RSkillprefab, "R", 10);
            obj = SkillObj["R"][0];
        }
        SkillObj["R"].RemoveAt(0);
        SkillObj["R"].Add(obj);
        obj.transform.position = transform.position + adjust;
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }

    public void PauseMove(float f)
    {
        TheAIPath.isStopped = true;
        Invoke("OnMove", f);
    }

    private void OnMove()
    {
        Vector3 tempV = transform.position;
        tempV.y = 1;
        PlayerAStarTarget.transform.position = tempV;
        TheAIPath.isStopped = false;
    }

    public void SkillEnd(float f, string next = "", float nextF = 0)
    {
        Invoke("OffIsSkillIng", f);
        if (next != "")
        {
            Invoke(next, f + nextF);
        }
    }

    private void OffIsSkillIng()
    {
        skillselect = SSelect.none;
        isSkillIng = false;
    }

    private void DownDefence()
    {
        TheChampionData.totalstat.Ability_Def -= rSkillTempVal;
        TheChampionData.totalstat.Attack_Def -= rSkillTempVal;
        rSkillTempVal = 0;
        if (!TheUIStat)
            FindUICanvas();
        TheUIStat.Refresh();
    }

    public override void InitTempValue()
    {
        base.InitTempValue();
    }

    // 알고보니 w 타겟팅만 맞음. 지나가는 애들 안맞음 ㅠㅠ
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (skillselect.Equals(SSelect.W) && isSkillIng)
    //    {
    //        if (collision.gameObject != TempObject1)
    //        {
    //            if (collision.transform.tag.Equals("Minion"))
    //            {
    //                MinionBehavior mB = collision.gameObject.GetComponent<MinionBehavior>();
    //                if (!collision.gameObject.name.Contains(TheChampionBehaviour.Team))
    //                {
    //                    MinionAtk mA = mB.minAtk;
    //                    mA.PushMe(Vector3.up * 3 + collision.transform.position
    //                        + (((collision.transform.position - TempVector1).normalized) * 5), 0.5f);
    //                    mA.PauseAtk(1f, true);
    //                    float damage = skillData.wDamage[TheChampionData.skill_W - 1]
    //                        + Acalculate(skillData.wAstat, skillData.wAvalue);
    //                    if (collision.gameObject.GetComponent<MinionBehavior>().HitMe(damage, "AP"))
    //                    {
    //                        //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
    //                        TheChampionAtk.ResetTarget();
    //                    }
    //                }
    //            }
    //            //else if (collision.transform.tag.Equals("Player"))
    //            else if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
    //            {
    //                ChampionBehavior cB = collision.gameObject.GetComponent<ChampionBehavior>();
    //                if (cB.Team != TheChampionBehaviour.Team)
    //                {
    //                    ChampionAtk cA = cB.myChampAtk;
    //                    cA.PushMe(Vector3.up * 3 + collision.transform.position
    //                        + (((collision.transform.position - TempVector1).normalized) * 5), 0.5f);
    //                    cA.PauseAtk(1f, true);
    //                    float damage = skillData.wDamage[TheChampionData.skill_W - 1]
    //                        + Acalculate(skillData.wAstat, skillData.wAvalue);
    //                    if (collision.gameObject.GetComponent<ChampionBehavior>().HitMe(damage, "AP"))
    //                    {
    //                        //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
    //                        TheChampionAtk.ResetTarget();
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    //[PunRPC]
    //public void HitSync(int viewID)
    //{
    //    GameObject g = PhotonView.Find(viewID).gameObject;
    //    if (g != null)
    //    {
    //        if (g.tag.Equals("Minion"))
    //            g.GetComponent<MinionBehavior>().HitMe(TheChampionData.totalstat.Attack_Damage);
    //        else if (g.layer.Equals(LayerMask.NameToLayer("Champion")))
    //            g.GetComponent<ChampionBehavior>().HitMe(TheChampionData.totalstat.Attack_Damage);
    //    }
    //}

    public void HitRPC(int viewID, float damage, string atktype, string cc = null)
    {
        int myViewID = GetComponent<PhotonView>().viewID;
        this.photonView.RPC("HitSyncSkill", PhotonTargets.Others, viewID, damage, atktype, cc, myViewID);
    }

    public void HitEffectRPC(string name, string key, int number = 1, float term = 0)
    {
        int myViewID = GetComponent<PhotonView>().viewID;
        this.photonView.RPC("HitSyncEffect", PhotonTargets.Others, myViewID, name, key, number, term);
    }

    public void InvokeEffect(string methodName, int number, float term)
    {
        for (int i = 0; i < number; ++i)
        {
            Invoke(methodName, term * i);
        }
    }
}