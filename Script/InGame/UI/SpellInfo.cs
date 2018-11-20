using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellInfo : MonoBehaviour
{

    public string spellkey = "";

    private GameObject Tooltip;
    private ChampionData cd;
    private int myspell = 0;
    private float myspellcooldown = 0;
    private string myspellname = "";
    private string myspelldescription = "";
    // Use this for initialization
    void Start()
    {

        UICanvas UIcanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        Tooltip = UIcanvas.Tooltip;
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            StructureSetting.instance.ActiveTrue();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        cd = Player.GetComponent<ChampionData>();

        if (spellkey == "D")
        {
            myspell = cd.spell_D;
            myspellcooldown = cd.Cooldown_D;
        }
        else if (spellkey == "F")
        {
            myspell = cd.spell_F;
            myspellcooldown = cd.Cooldown_F;
        }
        getname(myspell);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void getname(int id)
    {
        switch (id)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 현재 6개만 사용. 수정함. 점멸 유체화 회복 강타 점화 텔
            case 0:
                myspellname = "정화";
                myspelldescription = "챔피언에 걸린 모든 이동 불가와 (제압 및 공중으로 띄우는 효과 제외) 소환사 주문에 의한 해로운 효과를 제거하고 새로 적용되는 이동 불가 효과들의 지속시간을 3초간 65% 감소시킵니다.";
                break;
            case 1:
                myspellname = "탈진";
                myspelldescription = "적 챔피언을 지치게 만들어 2.5초 동안 이동 속도를 30% 낮추며, 가하는 피해량을 40% 낮춥니다.";
                break;
            case 2:
                myspellname = "점멸";
                myspelldescription = "커서 방향으로 챔피언이 짧은 거리를 순간이동합니다.";
                break;
            case 3:
                myspellname = "유체화";
                myspelldescription = "챔피언이 10초 동안 이동 속도가 상승합니다. 이동 속도는 2초동안 점차 빨라져 28%까지 상승합니다.";
                break;
            case 4:
                myspellname = "회복";
                myspelldescription = "챔피언과 대상 아군의 체력을 90만큼 회복시키고 2초 동안 이동 속도가 30% 증가합니다.";
                break;
            case 5:
                myspellname = "강타";
                myspelldescription = "대상 에픽 및 대형/중형 몬스터, 혹은 적 미니언에게 390의 고정 피해를 입힙니다. 강타를 사용하면 자신의 최대체력의 10%만큼 회복됩니다.";
                break;
            case 6:
                myspellname = "순간이동";
                myspelldescription = "4.5초 동안 정신 집중을 한 후 근처의 아군 미니언이나 포탑, 혹은 와드로 순간이동합니다.\n\n다시 사용하면 순간이동이 취소되며 재사용 대기시간이 적용되지 않습니다.";
                break;
            case 7:
                myspellname = "점화";
                myspelldescription = "적 챔피언을 불태워 5초 동안 80의 고정 피해를 입힙니다.";
                break;
            case 8:
                myspellname = "방어막";
                myspelldescription = "2초 동안 방어막으로 감싸 피해를 115만큼 흡수합니다.";
                break;
            default:
                break;
        }
    }

    public void tooltip_on()
    {
        if (Tooltip != null)
        {
            Tooltip.SetActive(true);

            float tooltip_height = 30;
            Tooltip.transform.Find("TitleText").GetComponent<Text>().text = myspellname;
            Tooltip.transform.Find("HotKey").GetComponent<Text>().text = "[" + spellkey + "]";
            Tooltip.transform.Find("Title_Description").GetComponent<Text>().text = "소모값 없음";
            Tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "재사용 대기시간 " + Mathf.RoundToInt(myspellcooldown).ToString() + "초";
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
                = myspelldescription.Replace("\n\n", "\n");
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