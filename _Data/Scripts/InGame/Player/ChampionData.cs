using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChampionData : Photon.MonoBehaviour
{
    public Skills playerSkill;
    // 기본스탯, 스킬
    public StatClass.Stat mystat = new StatClass.Stat();
    public SkillClass.Skill myskill = new SkillClass.Skill();

    // 챔피언명

    public string ChampionName = "Ashe";

    // 스킬레벨
    public int skill_Q = 0;
    public int skill_W = 0;
    public int skill_E = 0;
    public int skill_R = 0;
    public float Cooldown_Passive = 0;
    public float Cooldown_Q = 0;
    public float Cooldown_W = 0;
    public float Cooldown_E = 0;
    public float Cooldown_R = 0;
    public float current_Cooldown_Passive = 0;
    public float current_Cooldown_Q = 0;
    public float current_Cooldown_W = 0;
    public float current_Cooldown_E = 0;
    public float current_Cooldown_R = 0;
    // 스킬 쿨타임도중 스킬레벨업하여 쿨감소 방지하기위해
    public float temp_Cooldown_Q = 0;
    public float temp_Cooldown_W = 0;
    public float temp_Cooldown_E = 0;
    public float temp_Cooldown_R = 0;
    public float mana_Q = 0;
    public float mana_W = 0;
    public float mana_E = 0;
    public float mana_R = 0;

    // 스펠쿨타임. ID는 PlayerData에서 받아옴.(왜냐면 캐릭터 선택창에서 받아와야하니까)
    public int spell_D = 0;
    public int spell_F = 0;
    public float Cooldown_D = 0;
    public float Cooldown_F = 0;
    public float current_Cooldown_D = 0;
    public float current_Cooldown_F = 0;

    //룬정보
    public int mainRune = 0;
    public int subRune1 = 0;
    public int subRune2 = 0;
    public int subRune3 = 0;
    public int assistSubRune1 = 0;
    public int assistSubRune2 = 0;

    // 귀환시간
    private float RecallTime = 8.0f;
    private float currentRecallTime = 8.0f;
    [HideInInspector]
    public bool RecallStart = false;

    // 오브젝트 받아옴
    private GameObject UIRecall;
    private GameObject UIStat;
    private GameObject UIIcon;
    private GameObject UISkill;

    // 경험치 테스트용
    private float testTime = 0;
    private float regenTime = 0;

    //UI캔버스 찾았는지 체크
    private bool Find = false;

    //리콜시 못움직이게 현재위치 저장
    Vector3 CurPos = Vector3.zero;
    //리콜시 체력 저장
    float CurHp = 0;
  

    private void Awake()
    {   
        if(PhotonNetwork.player.IsLocal)
        {
            ChampionName = PlayerData.Instance.championName;
            setSpell();
            setStatSkill(ChampionName);
        }
       
    }
    private void OnLevelWasLoaded(int level)
    {
        Invoke("FindUICanvas", 3f);
    }

    private void FindUICanvas()
    {
        UICanvas UIcanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        UIStat = UIcanvas.Stat;
        UIIcon = UIcanvas.Icon;
        UISkill = UIcanvas.Skill;
        UIRecall = UIcanvas.Recall;
        Find = true;
    }

    void Update()
    {
        if (transform.position.y < -1)
        {
            Vector3 a = transform.position;
            a.y = 1;
            transform.position = a;
        }
        if (Find)
        {
            // 스킬
            SkillCheck();

            //스펠
            SpellCheck();

            // Recall
            RecallCheck();

            // 체력, 마나재생
            HealthManaRegen();

            // 경험치 증가 테스트
            testTime += Time.deltaTime;
            if (testTime >= 1.0f && mystat.Level < 18)
            {
                mystat.Exp += 150;
                testTime = 0;
            }

            if (mystat.Exp > mystat.RequireExp)
                LevelUp();
        }
    }

    public void HealthManaRegen()
    {
        regenTime += Time.deltaTime;
        if (regenTime >= 0.5f)
        {
            regenTime -= 0.5f;
            if (mystat.Hp < mystat.MaxHp)
                mystat.Hp += mystat.Health_Regen * 0.1f;
            if (mystat.Mp < mystat.MaxMp)
                mystat.Mp += mystat.Mana_Regen * 0.1f;

            if (mystat.Hp > mystat.MaxHp)
                mystat.Hp = mystat.MaxHp;
            if (mystat.Mp > mystat.MaxMp)
                mystat.Mp = mystat.MaxMp;
        }

        if (mystat.Hp != mystat.MaxHp)
            UISkill.GetComponent<UISkill>().HealthRegenText.text = "+" + (mystat.Health_Regen * 0.2f).ToString("N1");
        else
            UISkill.GetComponent<UISkill>().HealthRegenText.text = "";
        if (mystat.Mp != mystat.MaxMp)
            UISkill.GetComponent<UISkill>().ManaRegenText.text = "+" + (mystat.Mana_Regen * 0.2f).ToString("N1");
        else
            UISkill.GetComponent<UISkill>().ManaRegenText.text = "";
    }

    public void SkillCheck()
    {
        // 현재쿨타임이 0일때만(쿨타임이 안돌아갈때만) 스킬 써짐
        if (!playerSkill.isSkillIng)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                if (current_Cooldown_Q == 0 && skill_Q >= 1)
                {
                    if (mystat.Mp >= mana_Q)
                    {
                        playerSkill.QCasting();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (current_Cooldown_W == 0 && skill_W >= 1)
                {
                    if (mystat.Mp >= mana_W)
                    {
                        playerSkill.WCasting();
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (current_Cooldown_E == 0 && skill_E >= 1)
                {
                    if (mystat.Mp >= mana_E)
                    {
                        playerSkill.ECasting();

                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (current_Cooldown_R == 0 && skill_R >= 1)
                {
                    if (mystat.Mp >= mana_R)
                    {
                        playerSkill.RCasting();
                    }
                }
            }
        }

        // 스킬쿨이 돌고있으면 시간마다 점점 쿨감소
        if (current_Cooldown_Q != 0)
        {
            current_Cooldown_Q -= Time.deltaTime;
            if (current_Cooldown_Q < 0)
            {
                current_Cooldown_Q = 0;
                UISkill.GetComponent<UISkill>().SkillDisabledImage[1].SetActive(false);
                UISkill.GetComponent<UISkill>().SkillCooldownImage[1].fillAmount = 0;
                UISkill.GetComponent<UISkill>().SkillCooldownText[1].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.GetComponent<UISkill>().SkillCooldownImage[1].fillAmount = current_Cooldown_Q / temp_Cooldown_Q;
                if (current_Cooldown_Q > 1.0f)
                    UISkill.GetComponent<UISkill>().SkillCooldownText[1].text = Mathf.FloorToInt(current_Cooldown_Q).ToString();
                else
                    UISkill.GetComponent<UISkill>().SkillCooldownText[1].text = current_Cooldown_Q.ToString("N1");
            }
        }
        if (current_Cooldown_W != 0)
        {
            current_Cooldown_W -= Time.deltaTime;
            if (current_Cooldown_W < 0)
            {
                current_Cooldown_W = 0;
                UISkill.GetComponent<UISkill>().SkillDisabledImage[2].SetActive(false);
                UISkill.GetComponent<UISkill>().SkillCooldownImage[2].fillAmount = 0;
                UISkill.GetComponent<UISkill>().SkillCooldownText[2].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.GetComponent<UISkill>().SkillCooldownImage[2].fillAmount = current_Cooldown_W / temp_Cooldown_W;
                if (current_Cooldown_W > 1.0f)
                    UISkill.GetComponent<UISkill>().SkillCooldownText[2].text = Mathf.FloorToInt(current_Cooldown_W).ToString();
                else
                    UISkill.GetComponent<UISkill>().SkillCooldownText[2].text = current_Cooldown_W.ToString("N1");
            }
        }
        if (current_Cooldown_E != 0)
        {
            current_Cooldown_E -= Time.deltaTime;
            if (current_Cooldown_E < 0)
            {
                current_Cooldown_E = 0;
                UISkill.GetComponent<UISkill>().SkillDisabledImage[3].SetActive(false);
                UISkill.GetComponent<UISkill>().SkillCooldownImage[3].fillAmount = 0;
                UISkill.GetComponent<UISkill>().SkillCooldownText[3].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.GetComponent<UISkill>().SkillCooldownImage[3].fillAmount = current_Cooldown_E / temp_Cooldown_E;
                if (current_Cooldown_E > 1.0f)
                    UISkill.GetComponent<UISkill>().SkillCooldownText[3].text = Mathf.FloorToInt(current_Cooldown_E).ToString();
                else
                    UISkill.GetComponent<UISkill>().SkillCooldownText[3].text = current_Cooldown_E.ToString("N1");
            }
        }
        if (current_Cooldown_R != 0)
        {
            current_Cooldown_R -= Time.deltaTime;
            if (current_Cooldown_R < 0)
            {
                current_Cooldown_R = 0;
                UISkill.GetComponent<UISkill>().SkillDisabledImage[4].SetActive(false);
                UISkill.GetComponent<UISkill>().SkillCooldownImage[4].fillAmount = 0;
                UISkill.GetComponent<UISkill>().SkillCooldownText[4].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.GetComponent<UISkill>().SkillCooldownImage[4].fillAmount = current_Cooldown_R / temp_Cooldown_R;
                if (current_Cooldown_R > 1.0f)
                    UISkill.GetComponent<UISkill>().SkillCooldownText[4].text = Mathf.FloorToInt(current_Cooldown_R).ToString();
                else
                    UISkill.GetComponent<UISkill>().SkillCooldownText[4].text = current_Cooldown_R.ToString("N1");
            }
        }
    }

    public void UsedQ()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_Q = Cooldown_Q;
        current_Cooldown_Q = Cooldown_Q;
        mystat.Mp -= mana_Q;
        playerSkill.Q();
        if (Cooldown_Q != 0)
            UISkill.GetComponent<UISkill>().SkillDisabledImage[1].SetActive(true);
    }

    public void UsedW()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_W = Cooldown_W;
        current_Cooldown_W = Cooldown_W;
        mystat.Mp -= mana_W;

        if (Cooldown_W != 0)
            UISkill.GetComponent<UISkill>().SkillDisabledImage[2].SetActive(true);
    }

    public void UsedE()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_E = Cooldown_E;
        current_Cooldown_E = Cooldown_E;
        mystat.Mp -= mana_E;

        if (Cooldown_W != 0)
            UISkill.GetComponent<UISkill>().SkillDisabledImage[3].SetActive(true);
    }

    public void UsedR()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_R = Cooldown_R;
        current_Cooldown_R = Cooldown_R;
        mystat.Mp -= mana_R;

        if (Cooldown_R != 0)
            UISkill.GetComponent<UISkill>().SkillDisabledImage[4].SetActive(true);
    }


    public void SpellCheck()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (current_Cooldown_D == 0)
            {
                if (RecallStart)
                    RecallCancel();

                // d스펠
                current_Cooldown_D = Cooldown_D;
                if (Cooldown_D != 0)
                    UISkill.GetComponent<UISkill>().SpellDisabledImage[0].SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (current_Cooldown_F == 0)
            {
                if (RecallStart)
                    RecallCancel();

                // f스펠
                current_Cooldown_F = Cooldown_F;
                if (Cooldown_F != 0)
                    UISkill.GetComponent<UISkill>().SpellDisabledImage[1].SetActive(true);
            }
        }

        if (current_Cooldown_D != 0)
        {
            current_Cooldown_D -= Time.deltaTime;
            if (current_Cooldown_D < 0)
            {
                current_Cooldown_D = 0;
                UISkill.GetComponent<UISkill>().SpellDisabledImage[0].SetActive(false);
                UISkill.GetComponent<UISkill>().SpellCooldownImage[0].fillAmount = 0;
                UISkill.GetComponent<UISkill>().SpellCooldownText[0].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.GetComponent<UISkill>().SpellCooldownImage[0].fillAmount = current_Cooldown_D / Cooldown_D;
                if (current_Cooldown_D > 1.0f)
                    UISkill.GetComponent<UISkill>().SpellCooldownText[0].text = Mathf.FloorToInt(current_Cooldown_D).ToString();
                else
                    UISkill.GetComponent<UISkill>().SpellCooldownText[0].text = current_Cooldown_D.ToString("N1");
            }
        }
        if (current_Cooldown_F != 0)
        {
            current_Cooldown_F -= Time.deltaTime;
            if (current_Cooldown_F < 0)
            {
                current_Cooldown_F = 0;
                UISkill.GetComponent<UISkill>().SpellDisabledImage[1].SetActive(false);
                UISkill.GetComponent<UISkill>().SpellCooldownImage[1].fillAmount = 0;
                UISkill.GetComponent<UISkill>().SpellCooldownText[1].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.GetComponent<UISkill>().SpellCooldownImage[1].fillAmount = current_Cooldown_F / Cooldown_F;
                if (current_Cooldown_F > 1.0f)
                    UISkill.GetComponent<UISkill>().SpellCooldownText[1].text = Mathf.FloorToInt(current_Cooldown_F).ToString();
                else
                    UISkill.GetComponent<UISkill>().SpellCooldownText[1].text = current_Cooldown_F.ToString("N1");
            }
        }
    }

    public void RecallCheck()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Recall();
            CurPos = transform.position;
            CurHp = mystat.Hp;
        }
        if (RecallStart)
        {
            currentRecallTime -= Time.deltaTime;
            UIRecall.GetComponent<RecallUI>().RecallProgressBar.value = currentRecallTime / RecallTime;
            UIRecall.GetComponent<RecallUI>().RemainTime.text = currentRecallTime.ToString("N1");


            //리콜시 제자리에서 리콜하도록
            transform.position = CurPos;
            // 리콜용 애니메이션 추가할것

            if (currentRecallTime <= 0)
            {
                RecallStart = false;
                UIRecall.SetActive(false);
                RecallComplete();
            }

            RecallCancelCheck();
        }
    }

    public void setStatSkill(string championName)
    {
        mystat = StatClass.instance.characterData[championName].ClassCopy();
        myskill = SkillClass.instance.skillData[championName].ClassCopy();
        Cooldown_Passive = myskill.passiveCooldown;
    }

    public void setSpell()
    {
        spell_D = PlayerData.Instance.spell_D;
        spell_F = PlayerData.Instance.spell_F;
        //스펠 쿨세팅
        switch (spell_D)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                Cooldown_D = 210;
                break;
            // 탈진
            case 1:
                Cooldown_D = 210;
                break;
            // 점멸
            case 2:
                Cooldown_D = 300;
                break;
            // 유체화
            case 3:
                Cooldown_D = 180;
                break;
            // 회복
            case 4:
                Cooldown_D = 240;
                break;
            // 강타
            case 5:
                Cooldown_D = 15;
                break;
            // 순간이동
            case 6:
                Cooldown_D = 360;
                break;
            // 점화
            case 7:
                Cooldown_D = 210;
                break;
            // 방어막
            case 8:
                Cooldown_D = 180;
                break;
            default:
                break;
        }
        switch (spell_F)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                Cooldown_F = 210;
                break;
            // 탈진
            case 1:
                Cooldown_F = 210;
                break;
            // 점멸
            case 2:
                Cooldown_F = 300;
                break;
            // 유체화
            case 3:
                Cooldown_F = 180;
                break;
            // 회복
            case 4:
                Cooldown_F = 240;
                break;
            // 강타
            case 5:
                Cooldown_F = 15;
                break;
            // 순간이동
            case 6:
                Cooldown_F = 360;
                break;
            // 점화
            case 7:
                Cooldown_F = 210;
                break;
            // 방어막
            case 8:
                Cooldown_F = 180;
                break;
            default:
                break;
        }
    }

    public void LevelUp()
    {
        mystat.Level++;
        mystat.Exp -= mystat.RequireExp;
        mystat.Hp += mystat.UP_HP;
        mystat.MaxHp += mystat.UP_HP;
        mystat.Mp += mystat.UP_MP;
        mystat.MaxMp += mystat.UP_MP;
        mystat.Health_Regen += mystat.UP_HPRegen;
        mystat.Mana_Regen += mystat.UP_MPRegen;
        mystat.Attack_Damage += mystat.UP_AttackDamage;
        mystat.Attack_Def += mystat.UP_Def;
        mystat.Ability_Def += mystat.UP_MagicDef;

        if (mystat.Level <= 17)
            mystat.RequireExp = StatClass.instance.RequireExp[mystat.Level - 1];
        else if (mystat.Level == 18)
        {
            mystat.RequireExp = 0;
            mystat.Exp = 0;
        }
        // 공속은 계산법이 복잡해서 일단 UI상에서만 계산하여 표시

        UIStat.GetComponent<UIStat>().Refresh();
        UIIcon.GetComponent<UIIcon>().LevelUp();
        UISkill.GetComponent<UISkill>().LevelUp();
    }

    public void Recall()
    {
        if (RecallStart)
            return;

        RecallStart = true;
        UIRecall.SetActive(true);

        currentRecallTime = RecallTime;
    }

    public void RecallComplete()
    {
        transform.position = transform.parent.position;
        mystat.Hp = mystat.MaxHp;
        mystat.Mp = mystat.MaxMp;
    }

    public void RecallCancelCheck() // 수정바람
    {
        // 마우스 클릭했을때, 공격받았을때,
        if (Input.GetMouseButtonDown(1) || CurHp > mystat.Hp)
        {
            RecallCancel();
        }
    }

    public void RecallCancel()
    {
        RecallStart = false;
        UIRecall.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own mystat player: send the others our data
            stream.SendNext(mystat);
        }
        else
        {
            // Network player, receive data
            mystat = (StatClass.Stat)stream.ReceiveNext();
        }
    }
}
