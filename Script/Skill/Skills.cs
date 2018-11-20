using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Werewolf.SpellIndicators;

public class Skills : Photon.PunBehaviour
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
    [HideInInspector]
    public Vector3 TempVector1 = Vector3.zero;
    [HideInInspector]
    public Vector3 TempVector2 = Vector3.zero;
    [HideInInspector]
    public int TempInt1 = 0;
    [HideInInspector]
    public int TempInt2 = 0;
    [HideInInspector]
    public float TempFloat1 = 0;
    [HideInInspector]
    public float TempFloat2 = 0;
    [HideInInspector]
    public GameObject TempObject1 = null;
    [HideInInspector]
    public GameObject TempObject2 = null;
    public bool isSkillIng = false;
    public SkillClass.Skill skillData = null;
    public UIStat TheUIStat = null;
    protected ChampionAnimation championAnimation;

    public virtual void CancelSkill()
    {

    }
    public virtual void InitInstance()
    {
        SkillObj = new Dictionary<string, List<GameObject>>();
        TheSkillClass = SkillClass.instance;
        SkillParticleManager = new GameObject("SkillParticleManager");
        SkillParticleManager.transform.parent = this.transform.parent;
        TheChampionData = GetComponent<ChampionData>();
        TheSplatManager = GetComponentInChildren<SplatManager>();
        TheChampionBehaviour = GetComponent<ChampionBehavior>();
        championAnimation = GetComponent<ChampionAnimation>();
        InitTempValue();

    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
            Invoke("FindUICanvas", 3f);
    }

    public void FindUICanvas()
    {
        TheUIStat = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>().Stat.GetComponent<UIStat>();
    }
    public virtual void QCasting() { } // QCasting - Q - UsedQ(ChampionData) 순으로 불림
    public virtual void WCasting() { }
    public virtual void ECasting() { }
    public virtual void RCasting() { }

    public virtual void Q() { }
    public virtual void W() { }
    public virtual void E() { }
    public virtual void R() { }

    public virtual void QEffect() { } // 이펙트 동기화 함수 (벡터 필요 x일때)
    public virtual void WEffect() { }
    public virtual void EEffect() { }
    public virtual void REffect() { }

    public virtual void QVecEffect() { }
    public virtual void WVecEffect() { }
    public virtual void EVecEffect() { }
    public virtual void RVecEffect() { }

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
                result = TheChampionData.totalstat.Attack_Damage;
                break;

            case "AP":
                result = TheChampionData.totalstat.Ability_Power;
                break;

            case "DEF":
                result = TheChampionData.totalstat.Attack_Def;
                break;

            case "MDEF":
                result = TheChampionData.totalstat.Ability_Def;
                break;

            case "maxHP":
                result = TheChampionData.totalstat.MaxHp;
                break;

            case "maxMP":
                result = TheChampionData.totalstat.MaxMp;
                break;

            case "minusHP":
                result = TheChampionData.totalstat.MaxHp - TheChampionData.totalstat.Hp;
                break;

            case "Critical":
                result = TheChampionData.totalstat.Critical_Percentage;
                break;

            default:
                break;
        }
        result *= Avalue;
        return Mathf.RoundToInt(result);
    }
}