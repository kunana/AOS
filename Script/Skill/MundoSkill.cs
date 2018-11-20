using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MundoSkill : Skills
{
    public GameObject PlayerAStarTarget = null;
    private AIPath TheAIPath = null;
    public enum SSelect { none, Q, W, E, R };
    public SSelect skillselect = SSelect.none;
    public GameObject QSkillObj = null;
    public GameObject WSkillObj = null;
    public GameObject[] ESkillObj = null;
    public GameObject RSkillObj = null;
    public GameObject[] RSkillOnlyFirstEffect = null;
    public FogOfWarEntity myFogEntity;
    public Vector3 invokeVec = Vector3.zero;
    public bool isW = false;
    public bool isE = false;
    public bool isR = false;
    public float wTime = 1f;
    public float eTime = 5f;
    public float rTime = 1f;
    public int rCount = 12;
    public float rHealValue = 0;

    public override void CancelSkill()
    {
        TheSplatManager.Cancel();
        InitTempValue();
        skillselect = SSelect.none;
        isSkillIng = false;
    }

    public override void InitInstance()
    {
        base.InitInstance();
        TheChampionData.playerSkill = this;
        PlayerAStarTarget = GetComponent<PlayerMouse>().myTarget;
        TheAIPath = GetComponent<AIPath>();
        skillData = TheSkillClass.skillData["Mundo"];
        string s = GetComponent<PhotonView>().owner.GetTeam().ToString();
        if (s.Equals("red"))
        {
            QSkillObj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
            WSkillObj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
            RSkillObj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
        }
        else if (s.Equals("blue"))
        {
            QSkillObj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
            WSkillObj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
            RSkillObj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
        }
    }

    private void Awake()
    {
        InitInstance();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {//스킬선택해제
            CancelSkill();
        }
        if (skillselect.Equals(SSelect.Q))
        {
            if (Input.GetMouseButtonDown(0))
            {
                skillselect = SSelect.none;
                Vector3 h = Vector3.zero;
                Vector3 v = Input.mousePosition;
                Ray r = Camera.main.ScreenPointToRay(v);
                RaycastHit[] hits = Physics.RaycastAll(r, 50f);

                TempVector1 = Vector3.zero;
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag.Equals("Terrain"))
                    {
                        isSkillIng = true;
                        TheSplatManager.Cancel();
                        TheChampionData.UsedQ();
                        TempVector1 = hit.point;
                        TempVector1.y = 0.5f;

                        championAnimation.AnimationApply("Q", true);
                        championAnimation.AnimationApply("Q", false, 0.5f);

                        Invoke("Q", 0.2f);
                        break;
                    }
                }
            }
        }
        if (isW)
        {
            wTime -= Time.deltaTime;
            if (wTime < 0)
            {
                //if (Input.GetKeyDown(KeyCode.W))
                //{
                //    HitEffectRPC("Mundo", "W");
                //    WSkillObj.SetActive(false);
                //    SkillEnd(0f);
                //    isW = false;
                //    wTime = 1;
                //    SkillEnd(0f);
                //}
                //else
                //{
                wTime += 1;
                if (TheChampionData.totalstat.Hp - 2 < TheChampionData.mana_W)
                {
                    HitEffectRPC("Mundo", "W");
                    WSkillObj.SetActive(false);
                    isW = false;
                    wTime = 1;
                    SkillEnd(0f);
                }
                else
                {
                    TheChampionData.totalstat.Hp -= TheChampionData.mana_W;
                }
            }
        }
        if (isE)
        {
            if (photonView.isMine)
            {
                eTime -= Time.deltaTime;
                float losedHpPercent = ((TheChampionData.totalstat.MaxHp - TheChampionData.totalstat.Hp) / TheChampionData.totalstat.MaxHp);
                float minimalSkillAD = skillData.eDamage[TheChampionData.skill_E - 1] / 2f;
                TheChampionData.skillPlusAtkDam = minimalSkillAD + (minimalSkillAD * losedHpPercent);
                if (TheChampionAtk.skillKey.Equals("MundoE") && TheChampionAtk.skillKeyNum > 0)
                {
                    int e = TheChampionData.skill_E - 1;
                    TheChampionData.skillPlusAtkDam += TheChampionData.totalstat.MaxHp * (0.03f + (float)e * 0.005f);
                }
                TheChampionData.skillPlusAtkDam = Mathf.Round(TheChampionData.skillPlusAtkDam);
                if (eTime <= 0)
                {
                    HitEffectRPC("Mundo", "E");
                    E();
                }
                TheChampionData.TotalStatDamDefUpdate();
                TheChampionData.UIStat.Refresh();
            }
        }
        if (isR)
        {
            if (photonView.isMine)
            {
                if(TheChampionBehaviour.isDead)
                {
                    RSkillObj.SetActive(false);
                    TheChampionData.totalstat.Hp = 0;
                    ChampionSound.instance.TempAudio.Stop();
                    ChampionSound.instance.TempAudio.loop = false;
                    ChampionSound.instance.TempAudio.clip = null;
                    if (photonView.isMine)
                    {
                        TheChampionData.skillPlusSpeed = 0;
                        TheChampionData.TotalStatSpeedUpdate();
                        TheChampionData.UIStat.Refresh();
                    }
                    return;
                }
                rTime -= Time.deltaTime;
                if (rTime <= 0)
                {
                    rTime = 1f;
                    TheChampionData.totalstat.Hp += rHealValue;
                    if (--rCount == 0)
                    {
                        HitEffectRPC("Mundo", "R");
                        R();
                    }
                }
            }
        }
    }

    public override void QCasting()
    {
        isSkillIng = true;
        skillselect = SSelect.Q;
        TheSplatManager.Direction.Select();
        TheSplatManager.Direction.Scale = 25f;
    }
    public override void WCasting()
    {
        isSkillIng = true;
        skillselect = SSelect.none;
        TheSplatManager.Cancel();
        HitEffectRPC("Mundo", "W");
        TheChampionData.UsedW();
        W();
        SkillEnd(0f);
    }
    public override void ECasting()
    {
        isSkillIng = true;
        skillselect = SSelect.none;
        TheSplatManager.Cancel();
        TheChampionData.UsedE();
        HitEffectRPC("Mundo", "E");
        E();
        SkillEnd(0f);
    }
    public override void RCasting()
    {
        isSkillIng = true;
        skillselect = SSelect.none;
        TheSplatManager.Cancel();
        HitEffectRPC("Mundo", "R");
        TheChampionData.UsedR();
        R();
        SkillEnd(0f);
    }



    public override void Q()
    {
        Vector3 dest = TempVector1;
        TempVector1 = Vector3.zero;
        QSkillObj.SetActive(true);
        QSkillObj.transform.position = transform.position;
        transform.DOLookAt(dest, 0);
        QSkillObj.transform.DOLookAt(dest, 0);
        HitEffectVectorRPC("Mundo", "Q", dest);
        QSkillObj.GetComponent<MundoQ>().SkillOn(dest);

        PauseMove(0.5f);
        SkillEnd(0.5f);
    }
    public override void W()
    {
        isW = !isW;
        if (isW)
        {
            WSkillObj.SetActive(true);
            TheChampionData.current_Cooldown_W = -1f;
        }
        else
        {
            WSkillObj.SetActive(false);
            wTime = 1;
        }
    }
    public override void E()
    {
        isE = !isE;
        if (isE)
        {
            TheChampionAtk.skillKey = "MundoE";
            TheChampionAtk.skillKeyNum = 1;
            eTime = 5f;
            ESkillObj[0].SetActive(true);
            ESkillObj[1].SetActive(true);
        }
        else
        {
            TheChampionAtk.skillKey = "";
            TheChampionAtk.skillKeyNum = 0;
            ESkillObj[0].SetActive(false);
            ESkillObj[1].SetActive(false);
            TheChampionData.skillPlusAtkDam = 0;
        }
    }
    public override void R()
    {
        isR = !isR;
        if (isR)
        {
            rTime = 1f;
            rCount = 12;
            RSkillObj.SetActive(true);
            ChampionSound.instance.TempAudio.loop = true;
            ChampionSound.instance.TempAudio.clip = ChampionSound.instance.Mundo_RActive;
            ChampionSound.instance.TempAudio.Play();
            float hpPerc = TheChampionData.totalstat.MaxHp;
            if (TheChampionData.skill_R.Equals(2))
                hpPerc *= 0.75f;
            else if (TheChampionData.skill_R.Equals(1))
                hpPerc *= 0.5f;
            rHealValue = hpPerc / 12f;


            if (photonView.isMine)
            {
                float perc = 0.15f + ((float)(TheChampionData.skill_R - 1) * 0.1f);
                TheChampionData.skillPlusSpeed = (TheChampionData.mystat.Move_Speed + TheChampionData.itemstat.movement_speed) * perc;
                TheChampionData.TotalStatSpeedUpdate();
                TheChampionData.UIStat.Refresh();
                for (int i = 0; i < RSkillOnlyFirstEffect.Length; ++i)
                    RSkillOnlyFirstEffect[i].SetActive(true);
            }
            else
            {
                if (myFogEntity.isInTheSightRange)
                {//적이 시야 범위에 있음
                    if (myFogEntity.isInTheBush)
                    {//시야 범위 내의 부쉬 안에 있음
                        if (myFogEntity.isInTheBushMyEnemyToo)
                        {//그 부쉬에 우리 팀도 있으면 보임
                            for (int i = 0; i < RSkillOnlyFirstEffect.Length; ++i)
                                RSkillOnlyFirstEffect[i].SetActive(true);
                        }
                        else
                        {//그 부쉬에 우리 팀이 없으면 안보임
                            for (int i = 0; i < RSkillOnlyFirstEffect.Length; ++i)
                                RSkillOnlyFirstEffect[i].SetActive(false);
                        }
                    }
                    else
                    {//시야 범위 내인데 부쉬에도 없으면 보임
                        for (int i = 0; i < RSkillOnlyFirstEffect.Length; ++i)
                            RSkillOnlyFirstEffect[i].SetActive(true);
                    }
                }
                else
                {//적이 시야 범위에 없으면 안보임
                    for (int i = 0; i < RSkillOnlyFirstEffect.Length; ++i)
                        RSkillOnlyFirstEffect[i].SetActive(false);
                }
            }
        }
        else
        {
            RSkillObj.SetActive(false);
            ChampionSound.instance.TempAudio.Stop();
            ChampionSound.instance.TempAudio.loop = false;
            ChampionSound.instance.TempAudio.clip = null;
            if (photonView.isMine)
            {
                TheChampionData.skillPlusSpeed = 0;
                TheChampionData.TotalStatSpeedUpdate();
                TheChampionData.UIStat.Refresh();
            }
        }
    }
    public override void QVecEffect()
    {
        Vector3 dest = invokeVec;
        invokeVec = Vector3.zero;
        QSkillObj.SetActive(true);
        QSkillObj.transform.position = transform.position;
        QSkillObj.transform.DOLookAt(dest, 0);
        transform.DOLookAt(dest, 0);
        QSkillObj.GetComponent<MundoQ>().SkillOn(dest);
        SkillEnd(0);
    }
    public override void WEffect()
    {
        W();
    }
    public override void EEffect()
    {
        E();
    }
    public override void REffect()
    {
        R();
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

    public override void InitTempValue()
    {
        base.InitTempValue();
    }

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

    public void HitEffectVectorRPC(string name, string key, Vector3 vec, int number = 1, float term = 0)
    {
        int myViewID = GetComponent<PhotonView>().viewID;
        this.photonView.RPC("HitSyncEffectVector", PhotonTargets.Others, myViewID, name, key, vec, number, term);
    }

    public void InvokeEffect(string methodName, int number, float term)
    {
        for (int i = 0; i < number; ++i)
        {
            Invoke(methodName, term * i);
        }
    }

    public void InvokeVecEffect(string methodName, int number, float term, Vector3 vec)
    {
        invokeVec = vec;
        for (int i = 0; i < number; ++i)
        {
            Invoke(methodName, term * i);
        }
    }

    public void Heal(float value)
    {
        TheChampionData.totalstat.Hp += value;
        if (TheChampionData.totalstat.Hp > TheChampionData.totalstat.MaxHp)
            TheChampionData.totalstat.Hp = TheChampionData.totalstat.MaxHp;
    }

}