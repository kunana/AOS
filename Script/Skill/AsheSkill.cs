using DG.Tweening;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsheSkill : Skills
{
    public GameObject mySkills;
    public GameObject QSkillObj = null;
    public GameObject WSkillprefab = null;
    public GameObject[] ESkillObj = null;
    public GameObject RSkillObj = null;
    public GameObject PlayerAStarTarget = null;
    private AIPath TheAIPath = null;
    public enum SSelect { none, Q, W, E, R };
    public SSelect skillselect = SSelect.none;
    string team = "";
    public int AsheHawkCount = 1;
    public float AsheHawkChargeTime = 90f;
    public Vector3 invokeVec = Vector3.zero;
    public int qStackCount = 0;
    public float keepQStackTime = 4f;
    public float reduceQStackTime = 0.75f;
    public bool isQ = false;
    public float qTIme = 4f;
    private AudioSource audio;
    private StackImage TheStackImage = null;
    public int beforeELv = 0;
    public bool? IAmAshe = null;
    public void qCountUp()
    {
        if (TheChampionData.skill_Q > 0)
        {
            if (TheStackImage == null)
            {
                TheStackImage = GameObject.FindGameObjectWithTag("StackImage").GetComponent<StackImage>();
            }
            if (qStackCount < 4)
            {
                ++qStackCount;
                if (IAmAshe == true)
                {
                    if (qStackCount.Equals(1))
                        TheStackImage.ImageDic["AsheQ"].gameObject.SetActive(true);
                    TheStackImage.TextDic["AsheQ"].text = qStackCount.ToString();
                }
            }
            else if (qStackCount.Equals(4))
                qStackCount = 4;
            keepQStackTime = 4f;
            reduceQStackTime = 0.75f;
        }
    }
    public override void CancelSkill()
    {
        TheSplatManager.Cancel();
        InitTempValue();
        skillselect = SSelect.none;
        isSkillIng = false;
    }

    public override void InitInstance()
    {
        team = GetComponent<PhotonView>().owner.GetTeam().ToString();
        base.InitInstance();
        TheChampionData.playerSkill = this;
        mySkills = new GameObject("AsheSkills");
        mySkills.transform.SetParent(SkillParticleManager.transform);
        PlayerAStarTarget = GetComponent<PlayerMouse>().myTarget;
        TheAIPath = GetComponent<AIPath>();
        skillData = TheSkillClass.skillData["Ashe"];
    }

    private void Awake()
    {
        InitInstance();
        AllPooling();
        audio = GetComponent<AudioSource>();
    }

    private void AllPooling()
    {
        Pooling(WSkillprefab, "W", 20);
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
                case "W":
                    obj.GetComponent<AsheW>().mySkill = this;
                    break;
            }
        }
        SkillObj[type].InsertRange(0, tempList);
    }

    private void Update()
    {
        if (IAmAshe == null)
        {
            if (gameObject.tag.Equals("Player"))
                IAmAshe = true;
            else
                IAmAshe = false;
        }
        if (IAmAshe == true)
        {
            if (beforeELv.Equals(0))
            {
                if (TheChampionData.skill_E > 0)
                {
                    if (TheStackImage == null)
                        TheStackImage = GameObject.FindGameObjectWithTag("StackImage").GetComponent<StackImage>();
                    beforeELv = 1;
                    if (AsheHawkCount > 0)
                    {
                        TheStackImage.ImageDic["AsheE"].gameObject.SetActive(true);
                        TheStackImage.TextDic["AsheE"].text = AsheHawkCount.ToString();
                    }
                }
            }
        }
        if (TheChampionData.skill_Q > 0)
        {
            if (TheStackImage == null)
                TheStackImage = GameObject.FindGameObjectWithTag("StackImage").GetComponent<StackImage>();
            if (isQ)
            {
                qTIme -= Time.deltaTime;
                if (qTIme <= 0)
                {
                    isQ = false;
                    qTIme = 4f;
                    if (photonView.isMine)
                    {
                        TheChampionData.skillPlusAtkDam = 0;
                        TheChampionData.TotalStatDamDefUpdate();
                        TheChampionData.UIStat.Refresh();
                    }
                    QSkillObj.SetActive(false);
                }
            }
            else if (qStackCount > 0)
            {
                keepQStackTime -= Time.deltaTime;
                if (keepQStackTime < 0)
                {
                    reduceQStackTime -= Time.deltaTime;
                    if (reduceQStackTime < 0)
                    {
                        --qStackCount;
                        if (IAmAshe == true)
                        {
                            if (qStackCount.Equals(0))
                            {
                                TheStackImage.TextDic["AsheQ"].text = "";
                                TheStackImage.ImageDic["AsheQ"].gameObject.SetActive(false);
                            }
                            else
                            {
                                TheStackImage.TextDic["AsheQ"].text = qStackCount.ToString();
                            }
                        }
                        reduceQStackTime = 0.75f;
                    }
                }
            }
        }
        if (TheChampionData.skill_E > 0 && AsheHawkCount < 2)
        {
            AsheHawkChargeTime -= Time.deltaTime;
            if (AsheHawkChargeTime < 0)
            {
                AsheHawkChargeTime = 100 - (TheChampionData.skill_E * 10);
                ++AsheHawkCount;
                if (IAmAshe == true)
                {
                    if (AsheHawkCount.Equals(1))
                    {
                        TheStackImage.ImageDic["AsheE"].gameObject.SetActive(true);
                    }
                    TheStackImage.TextDic["AsheE"].text = AsheHawkCount.ToString();
                }
            }
        }
        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
        {//스킬선택해제
            CancelSkill();
        }
        if (skillselect.Equals(SSelect.E))
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
                        TheChampionData.UsedE();
                        TempVector1 = hit.point;
                        TempVector1.y = 0.5f;
                        Invoke("E", 0.1f);

                        championAnimation.AnimationApply("E", true);
                        championAnimation.AnimationApply("E", false, 0.7f);
                        break;
                    }
                }
            }
        }
        if (skillselect.Equals(SSelect.R))
        {
            if (Input.GetMouseButtonDown(0))
            {
                skillselect = SSelect.none;
                if (audio != null)
                    ChampionSound.instance.Skill(PlayerData.Instance.championName, 3, audio);
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
                        TheChampionData.UsedR();
                        TempVector1 = hit.point;
                        TempVector1.y = 0.5f;
                        Invoke("R", 0.1f);

                        championAnimation.AnimationApply("R", true);
                        championAnimation.AnimationApply("R", false, 0.8f);
                        break;
                    }
                }
            }
        }
        if (skillselect.Equals(SSelect.W))
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
                        TheChampionData.UsedW();
                        TempVector1 = hit.point;
                        TempVector1.y = 0.5f;
                        Invoke("W", 0.1f);

                        championAnimation.AnimationApply("W", true);
                        championAnimation.AnimationApply("W", false, 0.8f);
                        break;
                    }
                }
            }
        }
    }

    public override void QCasting()
    {
        if (qStackCount > 3)
        {
            isSkillIng = true;
            qStackCount = 0;
            if (IAmAshe == true)
            {
                TheStackImage.TextDic["AsheQ"].text = "";
                TheStackImage.ImageDic["AsheQ"].gameObject.SetActive(false);
            }
            TheSplatManager.Cancel();
            TheChampionData.UsedQ();
            HitEffectRPC("Ashe", "Q");
            Q();
            SkillEnd(0f);
        }
    }

    public override void WCasting()
    {
        isSkillIng = true;
        skillselect = SSelect.W;
        TheSplatManager.Cone.Select();
    }

    public override void ECasting()
    {
        if (AsheHawkCount > 0)
        {
            isSkillIng = true;
            skillselect = SSelect.E;
            TheSplatManager.Cancel();
        }
    }

    public override void RCasting()
    {
        isSkillIng = true;
        skillselect = SSelect.R;
        TheSplatManager.Direction.Select();
        TheSplatManager.Direction.Scale = 25f;
    }
    public override void Q()
    {
        isQ = true;
        QSkillObj.SetActive(true);
        qTIme = 4f;
        if (photonView.isMine)
        {
            float dam = 1;
            switch (TheChampionData.skill_Q)
            {
                case 1: dam = 0.31f; break;
                case 2: dam = 0.46f; break;
                case 3: dam = 0.64f; break;
                case 4: dam = 0.84f; break;
                case 5: dam = 1.08f; break;
            }
            TheChampionData.skillPlusAtkDam = Mathf.Round(dam * (TheChampionData.mystat.Attack_Damage + TheChampionData.itemstat.attack_damage));
            TheChampionData.TotalStatDamDefUpdate();
            TheChampionData.UIStat.Refresh();
        }
    }

    public override void W()
    {
        Vector3 dest = TempVector1;
        TempVector1 = Vector3.zero;
        float length = 25f;
        Vector3 cPos = transform.position;
        Vector3 v = (dest - cPos).normalized * length;
        float[] degree = new float[9];
        float d = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        Vector3[] dests = new Vector3[9];
        for (int i = 0; i < 9; ++i)
        {
            degree[i] = (d + (7f * (float)(i - 4))) * Mathf.Deg2Rad;
            dests[i] = new Vector3(length * Mathf.Sin(degree[i]), 0.5f, length * Mathf.Cos(degree[i]));
            dests[i] += cPos;
            GameObject obj = SkillObj["W"][0];
            SkillObj["W"].RemoveAt(0);
            SkillObj["W"].Add(obj);
            obj.SetActive(true);
            obj.transform.position = cPos;
            obj.transform.DOLookAt(dests[i], 0);
            obj.GetComponent<AsheW>().SkillOn(dests[i]);
        }
        transform.DOLookAt(dests[4], 0);
        HitEffectVectorRPC("Ashe", "W", dests[4]);
        PauseMove(0.8f);
        SkillEnd(0.8f);
    }

    public override void E()
    {
        Vector3 dest = TempVector1;
        TempVector1 = Vector3.zero;
        if (!ESkillObj[0].activeInHierarchy)
        {
            if (!ESkillObj[0].GetComponent<AsheE>().HawkWard.activeInHierarchy)
            {
                ESkillObj[0].SetActive(true);
                ESkillObj[0].transform.position = transform.position;
                ESkillObj[0].transform.DOLookAt(dest, 0);
                HitEffectVectorRPC("Ashe", "E", dest);
                ESkillObj[0].GetComponent<AsheE>().SkillOn(dest);
            }
        }
        else if (!ESkillObj[1].activeInHierarchy)
        {
            if (!ESkillObj[1].GetComponent<AsheE>().HawkWard.activeInHierarchy)
            {
                ESkillObj[1].SetActive(true);
                ESkillObj[1].transform.position = transform.position;
                ESkillObj[1].transform.DOLookAt(dest, 0);
                HitEffectVectorRPC("Ashe", "E", dest);
                ESkillObj[1].GetComponent<AsheE>().SkillOn(dest);
            }
        }
        transform.DOLookAt(dest, 0);
        PauseMove(0.7f);
        SkillEnd(0.7f);
        --AsheHawkCount;

        if (AsheHawkCount.Equals(1))
        {
            AsheHawkChargeTime = 100 - (TheChampionData.skill_E * 10);
            if (IAmAshe == true)
            {
                TheStackImage.TextDic["AsheE"].text = AsheHawkCount.ToString();
            }
        }
        else if (IAmAshe == true)
        {
            TheStackImage.TextDic["AsheE"].text = AsheHawkCount.ToString("");
            TheStackImage.ImageDic["AsheE"].gameObject.SetActive(false);
        }

    }

    public override void R()
    {
        Vector3 dest = TempVector1;
        TempVector1 = Vector3.zero;

        RSkillObj.SetActive(true);
        RSkillObj.transform.position = transform.position;
        transform.DOLookAt(dest, 0);
        RSkillObj.transform.DOLookAt(dest, 0);
        HitEffectVectorRPC("Ashe", "R", dest);
        RSkillObj.GetComponent<AsheR>().SkillOn(dest);
        PauseMove(0.8f);
        SkillEnd(0.8f);
    }

    public override void QEffect()
    {
        Q();
    }

    public override void WVecEffect()
    {
        Vector3 dest = invokeVec;
        invokeVec = Vector3.zero;
        float length = 25f;
        Vector3 cPos = transform.position;
        Vector3 v = (dest - cPos).normalized * length;
        float[] degree = new float[9];
        float d = Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
        Vector3[] dests = new Vector3[9];
        //for (int i = 0; i < 9; ++i)
        for (int i = 0; i < 9; ++i)
        {
            degree[i] = (d + (7f * (float)(i - 4))) * Mathf.Deg2Rad;
            dests[i] = new Vector3(length * Mathf.Sin(degree[i]), 0.5f, length * Mathf.Cos(degree[i]));
            dests[i] += cPos;
            GameObject obj = SkillObj["W"][0];
            if (obj.activeInHierarchy)
            {
                Pooling(WSkillprefab, "W", 20);
                while (SkillObj["W"][0].activeInHierarchy)
                {
                    GameObject temp = SkillObj["W"][0];
                    SkillObj["W"].RemoveAt(0);
                    SkillObj["W"].Add(obj);
                }
                obj = SkillObj["W"][0];
            }
            SkillObj["W"].RemoveAt(0);
            SkillObj["W"].Add(obj);
            obj.SetActive(true);
            obj.transform.position = cPos;
            obj.transform.DOLookAt(dests[i], 0);
            obj.GetComponent<AsheW>().SkillOn(dests[i]);
        }
        transform.DOLookAt(dests[4], 0);
        SkillEnd(0);
    }

    public override void EVecEffect()
    {
        Vector3 dest = invokeVec;
        invokeVec = Vector3.zero;
        if (!ESkillObj[0].activeInHierarchy)
        {
            if (!ESkillObj[0].GetComponent<AsheE>().HawkWard.activeInHierarchy)
            {
                ESkillObj[0].SetActive(true);
                ESkillObj[0].transform.position = transform.position;
                ESkillObj[0].transform.DOLookAt(dest, 0);
                ESkillObj[0].GetComponent<AsheE>().SkillOn(dest);
            }
        }
        else if (!ESkillObj[1].activeInHierarchy)
        {
            if (!ESkillObj[1].GetComponent<AsheE>().HawkWard.activeInHierarchy)
            {
                ESkillObj[1].SetActive(true);
                ESkillObj[1].transform.position = transform.position;
                ESkillObj[1].transform.DOLookAt(dest, 0);
                ESkillObj[1].GetComponent<AsheE>().SkillOn(dest);
            }
        }
        transform.DOLookAt(dest, 0);
        SkillEnd(0);
        --AsheHawkCount;

        if (AsheHawkCount.Equals(1))
        {
            AsheHawkChargeTime = 100 - (TheChampionData.skill_E * 10);
            if (IAmAshe == true)
            {
                TheStackImage.TextDic["AsheE"].text = AsheHawkCount.ToString();
            }
        }
        else if (IAmAshe == true)
        {
            TheStackImage.TextDic["AsheE"].text = AsheHawkCount.ToString("");
            TheStackImage.ImageDic["AsheE"].gameObject.SetActive(false);
        }
    }

    public override void RVecEffect()
    {
        Vector3 dest = invokeVec;
        invokeVec = Vector3.zero;
        RSkillObj.SetActive(true);
        RSkillObj.transform.position = transform.position;
        transform.DOLookAt(dest, 0);
        RSkillObj.transform.DOLookAt(dest, 0);
        RSkillObj.GetComponent<AsheR>().SkillOn(dest);
        SkillEnd(0);
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
}