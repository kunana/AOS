using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfo : MonoBehaviour {

    public SkillClass.Skill2 myskill = new SkillClass.Skill2();
    public string skillkey = "";

    private GameObject Tooltip;
    private ChampionData cd;

    // Use this for initialization
    void Start () {
        UICanvas UIcanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        Tooltip = UIcanvas.Tooltip;

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            StructureSetting.instance.ActiveTrue();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        cd = Player.GetComponent<ChampionData>();
        switch (skillkey)
        {
            case "Passive":
                myskill.Name = cd.myskill.passiveName;
                myskill.Description = cd.myskill.passiveDescription;
                myskill.Cooldown[0] = cd.myskill.passiveCooldown;
                myskill.Damage[0] = cd.myskill.passiveDamage;
                myskill.Astat = cd.myskill.passiveAstat;
                myskill.Avalue = cd.myskill.passiveAvalue;
                myskill.skillLevel = 0;
                break;

            case "Q":
                myskill.Name = cd.myskill.qName;
                myskill.Description = cd.myskill.qDescription;
                myskill.Range = cd.myskill.qRange;
                myskill.Mana = cd.myskill.qMana;
                myskill.Cooldown = cd.myskill.qCooldown;
                myskill.Damage = cd.myskill.qDamage;
                myskill.Astat = cd.myskill.qAstat;
                myskill.Avalue = cd.myskill.qAvalue;
                myskill.skillLevel = 0;
                break;

            case "W":
                myskill.Name = cd.myskill.wName;
                myskill.Description = cd.myskill.wDescription;
                myskill.Range = cd.myskill.wRange;
                myskill.Mana = cd.myskill.wMana;
                myskill.Cooldown = cd.myskill.wCooldown;
                myskill.Damage = cd.myskill.wDamage;
                myskill.Astat = cd.myskill.wAstat;
                myskill.Avalue = cd.myskill.wAvalue;
                myskill.skillLevel = 0;
                break;

            case "E":
                myskill.Name = cd.myskill.eName;
                myskill.Description = cd.myskill.eDescription;
                myskill.Range = cd.myskill.eRange;
                myskill.Mana = cd.myskill.eMana;
                myskill.Cooldown = cd.myskill.eCooldown;
                myskill.Damage = cd.myskill.eDamage;
                myskill.Astat = cd.myskill.eAstat;
                myskill.Avalue = cd.myskill.eAvalue;
                myskill.skillLevel = 0;
                break;

            case "R":
                myskill.Name = cd.myskill.rName;
                myskill.Description = cd.myskill.rDescription;
                myskill.Range = cd.myskill.rRange;
                myskill.Mana = cd.myskill.rMana;
                myskill.Cooldown = cd.myskill.rCooldown;
                myskill.Damage = cd.myskill.rDamage;
                myskill.Astat = cd.myskill.rAstat;
                myskill.Avalue = cd.myskill.rAvalue;
                myskill.skillLevel = 0;
                break;

            default:
                break;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void skillLevelRefresh()
    {
        switch (skillkey)
        {
            case "Q":
                myskill.skillLevel = cd.skill_Q - 1;
                break;
            case "W":
                myskill.skillLevel = cd.skill_W - 1;
                break;
            case "E":
                myskill.skillLevel = cd.skill_E - 1;
                break;
            case "R":
                myskill.skillLevel = cd.skill_R - 1;
                break;

            default:
                break;
        }
        if (myskill.skillLevel < 0)
            myskill.skillLevel = 0;
    }

    public int Acalculate(string Astat, float Avalue)
    {
        float result = 0;
        switch (Astat)
        {
            case "AD":
                result = cd.mystat.Attack_Damage;
                break;

            case "AP":
                result = cd.mystat.Ability_Power;
                break;

            case "DEF":
                result = cd.mystat.Attack_Def;
                break;

            case "MDEF":
                result = cd.mystat.Ability_Def;
                break;

            case "maxHP":
                result = cd.mystat.MaxHp;
                break;

            case "maxMP":
                result = cd.mystat.MaxMp;
                break;

            case "minusHP":
                result = cd.mystat.MaxHp - cd.mystat.Hp;
                break;

            case "Critical":
                result = cd.mystat.Critical_Percentage;
                break;

            default:
                break;
        }

        result *= Avalue;

        return Mathf.RoundToInt(result);
    }

    public string AstatColor(string Astat)
    {
        string colorstring;
        string colorcode;

        switch (Astat)
        {
            case "AD":
                colorcode = "#D97800";
                break;
            case "AP":
                colorcode = "#83DA84";
                break;
            case "Critical":
                colorcode = "#999999";
                break;
            default:
                colorcode = "#FF1010";
                break;
        }
        colorstring = "<color=" + colorcode + ">";

        return colorstring;
    }

    public void tooltip_on()
    {
        skillLevelRefresh();
        if (Tooltip != null)
        {
            Tooltip.SetActive(true);

            float tooltip_height = 30;
            Tooltip.transform.Find("TitleText").GetComponent<Text>().text = myskill.Name;
            if (skillkey != "Passive")
            {
                Tooltip.transform.Find("TitleText").GetComponent<Text>().text += " (" + (myskill.skillLevel + 1).ToString() + "레벨)";
                Tooltip.transform.Find("HotKey").GetComponent<Text>().text = "[" + skillkey + "]";
                if (myskill.Mana[myskill.skillLevel] == 0)
                    Tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "소모값 없음";
                else
                    Tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "마나 " + myskill.Mana[myskill.skillLevel].ToString();

                if(myskill.Cooldown[myskill.skillLevel] == 0)
                    Tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "재사용 대기시간 없음";
                else
                    Tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "재사용 대기시간 " + myskill.Cooldown[myskill.skillLevel].ToString() + "초";
            }
            else
            {
                Tooltip.transform.Find("HotKey").GetComponent<Text>().text = "";
                Tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "";
                Tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "";
                tooltip_height -= 20.0f;
            }
            tooltip_height += 5.0f;
            Canvas.ForceUpdateCanvases();

            int description_lineCount = Tooltip.transform.Find("Title_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 15.0f * description_lineCount;

            // Line1 표시
            tooltip_height += 5.0f;
            Tooltip.transform.Find("Line1").gameObject.SetActive(true);
            Tooltip.transform.Find("Line1").GetComponent<RectTransform>().anchoredPosition =
                new Vector3(Tooltip.transform.Find("Line1").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
            tooltip_height += 10.0f;

            // 추가설명 갱신
            Tooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition =
                new Vector3(Tooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
            Tooltip.transform.Find("Additional_Description").GetComponent<Text>().text
                = myskill.Description.Replace("@", myskill.Damage[myskill.skillLevel].ToString())
                .Replace("(+$)", AstatColor(myskill.Astat) + "(+" + Acalculate(myskill.Astat, myskill.Avalue).ToString() + ")</color>")
                .Replace("(+$%)", AstatColor(myskill.Astat) + "(+" + Acalculate(myskill.Astat, myskill.Avalue).ToString() + "%)</color>")
                .Replace("$", AstatColor(myskill.Astat) + Acalculate(myskill.Astat, myskill.Avalue).ToString() + "</color>")
                .Replace("기본 지속 효과:", "<color=#D97800>기본 지속 효과:</color>")
                .Replace("사용 시:", "<color=#D97800>사용 시:</color>")
                .Replace("사용 효과:", "<color=#D97800>사용 효과:</color>")
                .Replace("활성화/비활성화:", "<color=#D97800>활성화/비활성화:</color>")
                .Replace("\\n", "\n");
            Canvas.ForceUpdateCanvases();

            int a_description_lineCount = Tooltip.transform.Find("Additional_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 19.0f * a_description_lineCount;

            tooltip_height += 5.0f;
            Tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(Tooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void tooltip_off()
    {
        if (Tooltip != null)
            Tooltip.SetActive(false);
    }
}
