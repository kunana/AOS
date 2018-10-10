using System.Collections.Generic;
using UnityEngine;
using Werewolf.SpellIndicators;

public class Skills : MonoBehaviour
{
    protected GameObject SkillParticleManager = null;
    protected SkillClass TheSkillClass;
    public Dictionary<string, List<GameObject>> SkillObj;
    public ChampionData TheChampionData;
    public ChampionBehavior TheChampionBehaviour;
    public ChampionAtk TheChampionAtk
    {
        get
        {
            return TheChampionBehaviour.myChampAtk;
        }
    }
    public SplatManager TheSplatManager;
    public Vector3 TempVector1 = Vector3.zero;
    public Vector3 TempVector2 = Vector3.zero;
    public int TempInt1 = 0;
    public int TempInt2 = 0;
    public float TempFloat1 = 0;
    public float TempFloat2 = 0;
    public GameObject TempObject1 = null;
    public GameObject TempObject2 = null;
    public bool isSkillIng = false;
    public SkillClass.Skill skillData = null;
    public virtual void InitInstance()
    {   

        SkillObj = new Dictionary<string, List<GameObject>>();
        TheSkillClass = SkillClass.instance;
        //SkillParticleManager = GameObject.Find("SkillParticleManager");
        SkillParticleManager = new GameObject("SkillParticleManager");
        SkillParticleManager.transform.parent = this.transform.parent;
        TheChampionData = GetComponent<ChampionData>();
        TheSplatManager = GetComponentInChildren<SplatManager>();
        TheChampionBehaviour = GetComponent<ChampionBehavior>();
        InitTempValue();

    }
    public virtual void QCasting() { }
    public virtual void WCasting() { }
    public virtual void ECasting() { }
    public virtual void RCasting() { }

    public virtual void Q() { }
    public virtual void W() { }
    public virtual void E() { }
    public virtual void R() { }

    public virtual void InitTempValue()
    {
        Vector3 TempVector1 = Vector3.zero;
        Vector3 TempVector2 = Vector3.zero;
        int TempInt1 = 0;
        int TempInt2 = 0;
        float TempFloat1 = 0;
        float TempFloat2 = 0;
        GameObject TempObject1 = null;
        GameObject TempObject2 = null;
    }

    public int Acalculate(string Astat, float Avalue)
    {
        float result = 0;
        switch (Astat)
        {
            case "AD":
                result = TheChampionData.mystat.Attack_Damage;
                break;

            case "AP":
                result = TheChampionData.mystat.Ability_Power;
                break;

            case "DEF":
                result = TheChampionData.mystat.Attack_Def;
                break;

            case "MDEF":
                result = TheChampionData.mystat.Ability_Def;
                break;

            case "maxHP":
                result = TheChampionData.mystat.MaxHp;
                break;

            case "maxMP":
                result = TheChampionData.mystat.MaxMp;
                break;

            case "minusHP":
                result = TheChampionData.mystat.MaxHp - TheChampionData.mystat.Hp;
                break;

            case "Critical":
                result = TheChampionData.mystat.Critical_Percentage;
                break;

            default:
                break;
        }

        result *= Avalue;

        return Mathf.RoundToInt(result);
    }
}