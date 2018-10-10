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
    
    public override void InitInstance()
    {
        base.InitInstance();
        GetComponent<ChampionData>().playerSkill = this;
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
        if (Input.GetKeyDown(KeyCode.Escape))
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
                RaycastHit[] r = Physics.RaycastAll(ray, 280);
                foreach (RaycastHit rc in r)
                {//미니언일때부터 분류. 챔피언은 나중에 추가
                    if (rc.transform.tag.Equals("Minion"))
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
                            transform.DOMove(TempVector2, 0.1f);
                            Invoke("W", 0.1f);

                            SkillEnd(0.1f);
                            break;
                        }
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
        Q();
        TheChampionData.UsedQ();
        SkillEnd(1f);
    }

    public override void WCasting()
    {
        if (skillselect != SSelect.W)
        {
            skillselect = SSelect.W;
            TheSplatManager.Point.Select();
            TheSplatManager.Point.Scale = 7f;
            TheSplatManager.Point.Range = 7f;
        }
    }

    public override void ECasting()
    {
        isSkillIng = true;
        skillselect = SSelect.E;
        for (int i = 0; i < 10; ++i)
            Invoke("E", 0.5f * i);
        SkillEnd(5);
        TheChampionData.UsedE();
    }

    public override void RCasting()
    {
        for (int i = 0; i < 14; ++i)
            Invoke("R", 0.5f * i);
        TheChampionData.UsedR();
        switch (TheChampionData.skill_R)
        {
            case 1: rSkillTempVal = 122; break; //55% -> 45 = 10000/(100+x) -> x = 1100/9 = 122.2222222222
            case 2: rSkillTempVal = 186; break; //65% -> 35 = 10000/(100+x) -> x = 1300/7 = 185.7142857142
            case 3: rSkillTempVal = 300; break; //75% -> 25 = 10000/(100+x) -> x = 300
        }
        TheChampionData.mystat.Ability_Def += rSkillTempVal;
        TheChampionData.mystat.Attack_Def += rSkillTempVal;
        TheUIStat.Refresh();
        Invoke("DownDefence", 7f);
    }

    public override void Q()
    {
        base.Q();
        GameObject obj = SkillObj["Q"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(QSkillprefab, "Q", 10);
            obj = SkillObj["Q"][0];
        }
        SkillObj["Q"].RemoveAt(0);
        SkillObj["Q"].Add(obj);
        PauseMove(0.5f);
        obj.transform.position = transform.position;
        obj.SetActive(true);
    }
    public override void W()
    {
        base.W();
        InitTempValue();
        OnMove();
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
            MinionAtk mA = TempObject1.GetComponent<MinionBehavior>().minAtk;
            mA.PushMe(Vector3.up * 3 + TempObject1.transform.position
                + (((TempObject1.transform.position - TempVector1).normalized) * 5), 0.5f);
            mA.PauseAtk(1f, true);
            float damage = skillData.wDamage[TheChampionData.skill_W - 1]
                + Acalculate(skillData.wAstat, skillData.wAvalue);
            if (TempObject1.GetComponent<MinionBehavior>().HitMe(damage, "AP"))
            {
                //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                TheChampionAtk.ResetTarget();
            }
        }
        else if (TempObject1.tag.Equals("Player"))
        {
            ChampionAtk cA = TempObject1.GetComponent<ChampionBehavior>().myChampAtk;
            cA.PushMe(Vector3.up * 3 + TempObject1.transform.position
                + (((TempObject1.transform.position - TempVector1).normalized) * 5), 0.5f);
            cA.PauseAtk(1f, true);
            float damage = skillData.wDamage[TheChampionData.skill_W - 1]
    + Acalculate(skillData.wAstat, skillData.wAvalue);
            if (TempObject1.GetComponent<ChampionBehavior>().HitMe(damage, "AP"))
            {
                //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                TheChampionAtk.ResetTarget();
            }
        }
        skillselect = SSelect.none;
    }
    public override void E()
    {
        base.E();
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
    public override void R()
    {
        base.R();
        GameObject obj = SkillObj["R"][0];
        if (obj.activeInHierarchy)
        {
            Pooling(RSkillprefab, "R", 10);
            obj = SkillObj["R"][0];
        }
        SkillObj["R"].RemoveAt(0);
        SkillObj["R"].Add(obj);
        obj.transform.position = transform.position;
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
        TheChampionData.mystat.Ability_Def -= rSkillTempVal;
        TheChampionData.mystat.Attack_Def -= rSkillTempVal;
        rSkillTempVal = 0;

        TheUIStat.Refresh();
    }

    public override void InitTempValue()
    {
        base.InitTempValue();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (skillselect.Equals(SSelect.W) && isSkillIng)
        {
            if (collision.transform.tag.Equals("Minion") && collision.gameObject != TempObject1)
            {
                MinionAtk mA = collision.gameObject.GetComponent<MinionBehavior>().minAtk;
                mA.PushMe(Vector3.up * 3 + collision.transform.position
                    + (((collision.transform.position - TempVector1).normalized) * 5), 0.5f);
                mA.PauseAtk(1f, true);
                float damage = skillData.wDamage[TheChampionData.skill_W - 1]
                    + Acalculate(skillData.wAstat, skillData.wAvalue);
                if (collision.gameObject.GetComponent<MinionBehavior>().HitMe(damage, "AP"))
                {
                    //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                    TheChampionAtk.ResetTarget();
                }
            }
        }
    }
}
