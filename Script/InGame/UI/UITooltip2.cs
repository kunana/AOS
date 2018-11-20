using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITooltip2 : MonoBehaviour
{
    public string titleText = "";
    public string titleDescription = "";
    public string addtionalDescription = "";

    private GameObject Tooltip;
    private UIStat stat;
    private UISkill skill;
    // Use this for initialization
    void Start()
    {
        UICanvas UIcanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        Tooltip = UIcanvas.Tooltip;
        stat = UIcanvas.Stat.GetComponent<UIStat>();
        skill = UIcanvas.Skill.GetComponent<UISkill>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DescriptionUpdate()
    {
        switch (titleText)
        {
            case "공격력":
                addtionalDescription = "현재 공격력: <color=#4EB4D2>" + stat.AttackDamage.text + "</color>\n"
                    + "<color=#76C478>기본 공격으로 </color><color=#4EB4D2>" + stat.AttackDamage.text + "</color><color=#76C478>만큼의 피해를 입힐 수 있습니다.</color>";
                break;
            case "주문력":
                addtionalDescription = "현재 주문력: <color=#4EB4D2>" + stat.AbilityPower.text + "</color>\n"
                    + "<color=#76C478>스킬을 사용할 때 최대 </color><color=#4EB4D2>" + stat.AbilityPower.text + "</color><color=#76C478>만큼의 피해를 추가로 입힐 수 있습니다.</color>";
                break;
            case "방어력":
                float DamageReducePercent = stat.stat.Attack_Def / (1 + 0.01f * stat.stat.Attack_Def);
                addtionalDescription = "현재 방어력: <color=#4EB4D2>" + stat.Defence.text + "</color>\n"
                    + "<color=#76C478>물리 피해를 </color><color=#4EB4D2>" + Mathf.RoundToInt(DamageReducePercent).ToString() + "</color><color=#76C478>%만큼 덜 받습니다.</color>";
                break;
            case "마법 저항력":
                float MagicDamageReducePercent = stat.stat.Ability_Def / (1 + 0.01f * stat.stat.Ability_Def);
                addtionalDescription = "현재 마법 저항력: <color=#4EB4D2>" + stat.MagicResist.text + "</color>\n"
                    + "<color=#76C478>마법 피해를 </color><color=#4EB4D2>" + Mathf.RoundToInt(MagicDamageReducePercent).ToString() + "</color><color=#76C478>%만큼 덜 받습니다.</color>";
                break;
            case "공격 속도":
                addtionalDescription = "초당 공격 횟수: <color=#4EB4D2>" + stat.AttackSpeed.text + "</color>\n"
                    + "<color=#76C478>공격 속도 계수 </color><color=#4EB4D2>" + stat.originstat.Attack_Speed.ToString() + "</color>";
                break;
            case "재사용 대기시간 감소":
                addtionalDescription = "현재 재사용 대기시간 감소: <color=#4EB4D2>" + stat.CooldownReduce.text + "</color>\n"
                    + "<color=#76C478>스킬이 </color><color=#4EB4D2>" + stat.CooldownReduce.text + "</color><color=#76C478>% 더 빠르게 충전됩니다.</color>";
                break;
            case "치명타":
                addtionalDescription = "현재 치명타율: <color=#4EB4D2>" + stat.Critical.text + "</color>\n"
                    + "<color=#76C478>현재 치명타율이 </color><color=#4EB4D2>" + stat.Critical.text + "</color><color=#76C478>% 증가하였습니다.</color>";
                break;
            case "이동 속도":
                addtionalDescription = "현재 이동 속도: <color=#4EB4D2>" + stat.AttackDamage.text + "</color>\n"
                    + "<color=#76C478>초당 </color><color=#4EB4D2>" + stat.AttackDamage.text + "</color><color=#76C478> 유닛의 속도로 움직입니다.</color>";
                break;
        }
    }

    public void PointerEnter()
    {
        if (titleText == "레벨 업" && skill.getSkillPoint() == 0)
        {
            Tooltip.SetActive(false);
            return;
        }

        DescriptionUpdate();
        if (Tooltip != null)
        {
            Tooltip.SetActive(true);

            float tooltip_height = 30;
            Tooltip.transform.Find("TitleText").GetComponent<Text>().text = titleText;
            Tooltip.transform.Find("HotKey").GetComponent<Text>().text = "";
            Tooltip.transform.Find("Title_Description").GetComponent<Text>().text = titleDescription.Replace("\\n", "\n");
            Tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "";
            tooltip_height += 5.0f;
            Canvas.ForceUpdateCanvases();

            int description_lineCount = Tooltip.transform.Find("Title_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 15.0f * description_lineCount;

            if(addtionalDescription != string.Empty)
            {
                // Line1 표시
                tooltip_height += 5.0f;
                Tooltip.transform.Find("Line1").gameObject.SetActive(true);
                Tooltip.transform.Find("Line1").GetComponent<RectTransform>().anchoredPosition = 
                    new Vector3(Tooltip.transform.Find("Line1").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
                tooltip_height += 10.0f;

                // 추가설명 갱신
                Tooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition =
                    new Vector3(Tooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
                Tooltip.transform.Find("Additional_Description").GetComponent<Text>().text = addtionalDescription.Replace("\\n", "\n");
                Canvas.ForceUpdateCanvases();

                int a_description_lineCount = Tooltip.transform.Find("Additional_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
                tooltip_height += 19.0f * a_description_lineCount;
            }
            else
            {
                Tooltip.transform.Find("Line1").gameObject.SetActive(false);
                Tooltip.transform.Find("Line2").gameObject.SetActive(false);
                Tooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";
                Tooltip.transform.Find("Additional_Description2").GetComponent<Text>().text = "";
            }

            tooltip_height += 5.0f;
            Tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(Tooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void PointerExit()
    {
        if (Tooltip != null)
            Tooltip.SetActive(false);
    }
}
