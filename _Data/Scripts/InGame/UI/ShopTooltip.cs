using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTooltip : MonoBehaviour {

    private string titlename = "챔피언 아이템 상점";
    private string hotkey = "P";
    private string description = "아이템 상점에 가까이 있을 때만 물건을 살 수 있습니다. 상점은 소환사의 제단 근처에 있습니다.";

    private string titlename2 = "귀환";
    private string hotkey2 = "B";
    private string status = "<color=#D08005>클릭하여 사용</color>";
    private string description2 = "8초 뒤 챔피언을 소환사의 제단으로 순간이동시킵니다. 이 때 피해를 입으면 순간이동은 취소됩니다.";

    private GameObject Tooltip;
    private GameObject ItemTooltip;
    // Use this for initialization
    void Start () {
        UICanvas UIcanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        Tooltip = UIcanvas.Tooltip;
        ItemTooltip = UIcanvas.ItemTooltip;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void tooltip_on()
    {
        if (Tooltip != null)
        {
            Tooltip.SetActive(true);

            float tooltip_height = 30;
            Tooltip.transform.Find("TitleText").GetComponent<Text>().text = titlename;
            Tooltip.transform.Find("HotKey").GetComponent<Text>().text = "[" + hotkey + "]";
            Tooltip.transform.Find("Title_Description").GetComponent<Text>().text = description;
            Tooltip.transform.Find("Cooldown").GetComponent<Text>().text = "";
            tooltip_height += 5.0f;
            Canvas.ForceUpdateCanvases();

            int description_lineCount = Tooltip.transform.Find("Title_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 15.0f * description_lineCount;

            Tooltip.transform.Find("Line1").gameObject.SetActive(false);
            Tooltip.transform.Find("Line2").gameObject.SetActive(false);
            Tooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";
            Tooltip.transform.Find("Additional_Description2").GetComponent<Text>().text = "";

            tooltip_height += 5.0f;
            Tooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(Tooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void tooltip_off()
    {
        if (Tooltip != null)
            Tooltip.SetActive(false);
    }

    public void recall_tooltip_on()
    {
        if (ItemTooltip != null)
        {
            ItemTooltip.SetActive(true);

            float tooltip_height = 30;
            ItemTooltip.transform.Find("ItemName").GetComponent<Text>().text = titlename2;
            ItemTooltip.transform.Find("SellPrice").GetComponent<Text>().text = "[" + hotkey2 + "]";
            ItemTooltip.transform.Find("Status").GetComponent<Text>().text = status;
            tooltip_height += 20.0f;

            ItemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition =
                        new Vector3(ItemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);
            ItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text = description2;
            ItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";

            Canvas.ForceUpdateCanvases();
            int description_lineCount = ItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 15.0f * description_lineCount;

            tooltip_height += 5.0f;
            ItemTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void recall_tooltip_off()
    {
        if (ItemTooltip != null)
            ItemTooltip.SetActive(false);
    }
}
