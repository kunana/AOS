using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkill : MonoBehaviour {

    public Image[] Skill_Icon;
    public GameObject[] SkillDisabledImage;
    public Image[] SkillCooldownImage;
    public Text[] SkillCooldownText;
    public Text LevelUpText;

    public GameObject[] SkillUpButton;
    public GameObject[] SkillUpButton2;
    public GameObject[] SkillLevelLamp;

    [Space]
    public Image[] Spell_Icon;
    public GameObject[] SpellDisabledImage;
    public Image[] SpellCooldownImage;
    public Text[] SpellCooldownText;

    [Space]
    public ProgressBar HealthBar;
    public ProgressBar ManaBar;
    public Text HealthRegenText;
    public Text ManaRegenText;

    private int skillpoint = 1;
    private ChampionData cd;
    private PlayerData playerData;

    // Use this for initialization
    void Start () {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj == null)
        {
            StructureSetting.instance.ActiveTrue();
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }
        cd = playerObj.GetComponent<ChampionData>();
        playerData = PlayerData.Instance;

        Skill_Icon[0].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + cd.ChampionName + "/Passive");
        Skill_Icon[1].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + cd.ChampionName + "/Q");
        Skill_Icon[2].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + cd.ChampionName + "/W");
        Skill_Icon[3].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + cd.ChampionName + "/E");
        Skill_Icon[4].sprite = Resources.Load<Sprite>("Champion/ChampionSkill/" + cd.ChampionName + "/R");

        Spell_Icon[0].sprite = Resources.Load<Sprite>("Spell/" + cd.spell_D);
        Spell_Icon[1].sprite = Resources.Load<Sprite>("Spell/" + cd.spell_F);
    }
	
	// Update is called once per frame
	void Update () {
        ProgressRefresh();
    }

    public void skillUp(string Hotkey)
    {
        skillpoint--;
        LevelUpText.text = "레벨 업! +" + skillpoint.ToString();

        switch (Hotkey)
        {
            case "Q":
                cd.skill_Q++;
                cd.Cooldown_Q = cd.myskill.qCooldown[cd.skill_Q - 1];
                cd.mana_Q = cd.myskill.qMana[cd.skill_Q - 1];
                SkillLevelLamp[0].transform.Find(cd.skill_Q.ToString()).Find("on").gameObject.SetActive(true);
                SkillDisabledImage[1].SetActive(false);
                break;
            case "W":
                cd.skill_W++;
                cd.Cooldown_W = cd.myskill.wCooldown[cd.skill_W - 1];
                cd.mana_W = cd.myskill.wMana[cd.skill_W - 1];
                SkillLevelLamp[1].transform.Find(cd.skill_W.ToString()).Find("on").gameObject.SetActive(true);
                SkillDisabledImage[2].SetActive(false);
                break;
            case "E":
                cd.skill_E++;
                cd.Cooldown_E = cd.myskill.eCooldown[cd.skill_E - 1];
                cd.mana_E = cd.myskill.eMana[cd.skill_E - 1];
                SkillLevelLamp[2].transform.Find(cd.skill_E.ToString()).Find("on").gameObject.SetActive(true);
                SkillDisabledImage[3].SetActive(false);
                break;
            case "R":
                cd.skill_R++;
                cd.Cooldown_R = cd.myskill.rCooldown[cd.skill_R - 1];
                cd.mana_R = cd.myskill.rMana[cd.skill_R - 1];
                SkillLevelLamp[3].transform.Find(cd.skill_R.ToString()).Find("on").gameObject.SetActive(true);
                SkillDisabledImage[4].SetActive(false);
                break;
            default:
                break;
        }

        // 갱신
        if(skillpoint != 0)
        {
            skillLimit();
        }
        else
        {
            LevelUpText.gameObject.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                SkillUpButton[i].SetActive(false);
                SkillUpButton2[i].SetActive(false);
            }
        }

        // 툴팁 off
        GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>().Tooltip.SetActive(false);
    }

    public void LevelUp()
    {
        skillpoint++;
        LevelUpText.text = "레벨 업! +" + skillpoint.ToString();
        LevelUpText.gameObject.SetActive(true);

        skillLimit();
    }

    public void ProgressRefresh()
    {
        if (cd == null)
            return;
        HealthBar.value = cd.totalstat.Hp / cd.totalstat.MaxHp;
        HealthBar.text = Mathf.FloorToInt(cd.totalstat.Hp).ToString() + " / " + Mathf.FloorToInt(cd.totalstat.MaxHp).ToString();

        ManaBar.value = cd.totalstat.Mp / cd.totalstat.MaxMp;
        ManaBar.text = Mathf.FloorToInt(cd.totalstat.Mp).ToString() + " / " + Mathf.FloorToInt(cd.totalstat.MaxMp).ToString();

        if(playerData.isDead)
        {
            HealthRegenText.text = "";
            ManaRegenText.text = "";
        }
        else
        {
            if (cd.totalstat.Hp != cd.totalstat.MaxHp)
                HealthRegenText.text = "+" + (cd.totalstat.Health_Regen * 0.2f).ToString("N1");
            else
                HealthRegenText.text = "";
            if (cd.totalstat.Mp != cd.totalstat.MaxMp)
                ManaRegenText.text = "+" + (cd.totalstat.Mana_Regen * 0.2f).ToString("N1");
            else
                ManaRegenText.text = "";
        }
    }

    public void skillLimit()
    {
        int level = cd.mystat.Level;
        int q_level = cd.skill_Q;
        int w_level = cd.skill_W;
        int e_level = cd.skill_E;
        int r_level = cd.skill_R;

        for (int i = 0; i < 4; i++)
        {
            SkillUpButton[i].SetActive(false);
            SkillUpButton2[i].SetActive(false);
        }

        // 궁극기 스킬렙 6, 11, 16 제한
        if (level >= 6 && level < 11 && r_level < 1)
            SkillUpButton[3].SetActive(true);
        else if (level >= 11 && level < 16 && r_level < 2)
            SkillUpButton[3].SetActive(true);
        else if (level >= 16 && r_level < 3)
            SkillUpButton[3].SetActive(true);
        else
            SkillUpButton2[3].SetActive(true);

        // 스킬마다 1 3 5 7 9렙에 스킬레벨제한 풀림 Q
        if (level < 3 && q_level < 1)
            SkillUpButton[0].SetActive(true);
        else if (level >= 3 && level < 5 && q_level < 2)
            SkillUpButton[0].SetActive(true);
        else if (level >= 5 && level < 7 && q_level < 3)
            SkillUpButton[0].SetActive(true);
        else if (level >= 7 && level < 9 && q_level < 4)
            SkillUpButton[0].SetActive(true);
        else if (level >= 9 && q_level < 5)
            SkillUpButton[0].SetActive(true);
        else
            SkillUpButton2[0].SetActive(true);

        // W
        if (level < 3 && w_level < 1)
            SkillUpButton[1].SetActive(true);
        else if (level >= 3 && level < 5 && w_level < 2)
            SkillUpButton[1].SetActive(true);
        else if (level >= 5 && level < 7 && w_level < 3)
            SkillUpButton[1].SetActive(true);
        else if (level >= 7 && level < 9 && w_level < 4)
            SkillUpButton[1].SetActive(true);
        else if (level >= 9 && w_level < 5)
            SkillUpButton[1].SetActive(true);
        else
            SkillUpButton2[1].SetActive(true);

        // E
        if (level < 3 && e_level < 1)
            SkillUpButton[2].SetActive(true);
        else if (level >= 3 && level < 5 && e_level < 2)
            SkillUpButton[2].SetActive(true);
        else if (level >= 5 && level < 7 && e_level < 3)
            SkillUpButton[2].SetActive(true);
        else if (level >= 7 && level < 9 && e_level < 4)
            SkillUpButton[2].SetActive(true);
        else if (level >= 9 && e_level < 5)
            SkillUpButton[2].SetActive(true);
        else
            SkillUpButton2[2].SetActive(true);
    }

    public int getSkillPoint()
    {
        return skillpoint;
    }
}
