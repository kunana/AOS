using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortMenu : MonoBehaviour {

    public int menu_id = 0;
    public string menu_description = "";

    private bool onMouseEnter = false;
    private GameObject SortTooltip;

    // Use this for initialization
    void Start () {
        if (GameObject.FindGameObjectWithTag("ShopCanvas") != null)
            SortTooltip = GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().SortTooltip;
    }
	
	// Update is called once per frame
	void Update () {
		if(onMouseEnter)
        {
            SortTooltip.transform.position = Input.mousePosition;
            Vector2 TooltipPos = Input.mousePosition;

            // 툴팁이 화면 위를 넘어가면 아래로 뒤집음
            if (SortTooltip.GetComponent<RectTransform>().localPosition.y + SortTooltip.GetComponent<RectTransform>().sizeDelta.y > 340)
                TooltipPos.y -= (5 + SortTooltip.GetComponent<RectTransform>().sizeDelta.y) * Screen.height / 720;
            else
                TooltipPos.y += 5 * Screen.height / 720;

            // 툴팁이 화면 오른쪽을 넘어가면 왼쪽으로 뒤집음
            if (SortTooltip.GetComponent<RectTransform>().localPosition.x + SortTooltip.GetComponent<RectTransform>().sizeDelta.x > 620)
                TooltipPos.x -= (10 + SortTooltip.GetComponent<RectTransform>().sizeDelta.x) * Screen.width / 1280;
            else
                TooltipPos.x += 10 * Screen.width / 1280;

            SortTooltip.transform.position = TooltipPos;
        }
	}

    public void PointerEnter()
    {
        onMouseEnter = true;

        GetComponent<Image>().enabled = true;
        if (SortTooltip != null)
        {
            SortTooltip.SetActive(true);

            float tooltip_height = 30;
            SortTooltip.transform.GetChild(0).GetComponent<Text>().text = menu_description.Replace("\\n", "\n");
            Canvas.ForceUpdateCanvases();

            int description_lineCount = SortTooltip.transform.GetChild(0).GetComponent<Text>().cachedTextGenerator.lineCount;
            tooltip_height += 16.0f * (description_lineCount - 1);
            SortTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(SortTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void PointerExit()
    {
        onMouseEnter = false;

        if (transform.parent.GetComponent<ItemSort>().selectedID != menu_id)
            GetComponent<Image>().enabled = false;

        if (SortTooltip != null)
            SortTooltip.SetActive(false);
    }

    public void PointerClick()
    {
        transform.parent.GetComponent<ItemSort>().select(menu_id, this.gameObject);
    }
}
