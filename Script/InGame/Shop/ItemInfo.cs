using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfo : MonoBehaviour {

    public ShopItem.Item myItem;

    private GameObject ItemTooltip;
    private Sprite Icon_image;

    private float tooltip_height = 55;
    private bool onMouseEnter = false;
    private bool selected = false;
    private bool searchSelected = false;
    [HideInInspector]
    public bool viewSelected = false;

    private GameObject ShopCanvas = null;
    private GameObject UI_ItemTooltip;

    // 아이템 업그레이드 하위템 검색용
    [HideInInspector]
    public bool[] search = new bool[6] { false, false, false, false, false, false };
    private bool subitem_check1 = false;
    private bool subitem_check2 = false;
    private bool subitem_check3 = false;
    private bool[,] subsubitem_check = new bool[3, 3];

    private void Start()
    {
        ShopCanvas = GameObject.FindGameObjectWithTag("ShopCanvas");

        if (myItem != null)
            Icon_image = Resources.Load<Sprite>("Item_Image/" + myItem.icon_name);

        if (ShopCanvas != null)
            ItemTooltip = ShopCanvas.GetComponent<Shop>().ItemTooltip;

        GameObject UICanvasObject = GameObject.FindGameObjectWithTag("UICanvas");
        if (UICanvasObject != null)
        {
            UICanvas UIcanvas = UICanvasObject.GetComponent<UICanvas>();
            UI_ItemTooltip = UIcanvas.ItemTooltip;
        }
    }

    private void Update()
    {
        if (onMouseEnter)
        {
            ItemTooltip.transform.position = Input.mousePosition;
            Vector2 TooltipPos = Input.mousePosition;

            // 툴팁이 화면 위를 넘어가면 아래로 뒤집음
            if (ItemTooltip.GetComponent<RectTransform>().localPosition.y + ItemTooltip.GetComponent<RectTransform>().sizeDelta.y > 340)
                TooltipPos.y -= (5 + ItemTooltip.GetComponent<RectTransform>().sizeDelta.y) * Screen.height / 720;
            else
                TooltipPos.y += 5 * Screen.height / 720;

            // 툴팁이 화면 오른쪽을 넘어가면 왼쪽으로 뒤집음
            if (ItemTooltip.GetComponent<RectTransform>().localPosition.x + ItemTooltip.GetComponent<RectTransform>().sizeDelta.x > 620)
                TooltipPos.x -= (10 + ItemTooltip.GetComponent<RectTransform>().sizeDelta.x) * Screen.width / 1280;
            else
                TooltipPos.x += 10 * Screen.width / 1280;

            ItemTooltip.transform.position = TooltipPos;
        }

        // 선택되서 테두리가 켜진 이후에 다른게 선택되면 기존 테두리 끔
        if(selected)
        {
            if (ShopCanvas.GetComponent<Shop>().selectedItemID != myItem.id 
                || ShopCanvas.GetComponent<Shop>().selectedObject != gameObject)
            {
                selected = false;
                transform.Find("SelectBorder").GetComponent<Image>().enabled = false;
            }
        }

        if(searchSelected)
        {
            if (ShopCanvas.GetComponent<Shop>().selectedItemID != myItem.id
                || ShopCanvas.GetComponent<Shop>().selectedObject != gameObject)
            {
                searchSelected = false;
                GetComponent<Outline>().effectColor = new Color(69f / 255f, 66f / 255f, 47f / 255f, 128f / 255f);
            }
        }

        if (viewSelected)
        {
            if (ShopCanvas.GetComponent<Shop>().selectedItemID != myItem.id
                || ShopCanvas.GetComponent<Shop>().selectedObject != gameObject)
            {
                viewSelected = false;
                //transform.Find("ItemBorder").GetComponent<Image>().color = new Color(125f / 255f, 116f / 255f, 82f / 255f, 1);
            }
        }
    }

    public void BasicSetting()
    {
        if (myItem != null)
        {
            Icon_image = Resources.Load<Sprite>("Item_Image/" + myItem.icon_name);
            transform.Find("ItemIcon").GetComponent<Image>().sprite = Icon_image;

            if (myItem.price == 0)
                transform.Find("Price").GetComponent<Text>().text = "무료";
            else
                transform.Find("Price").GetComponent<Text>().text = myItem.price.ToString();
        }
    }

    // price update
    public void ItemCheck()
    {
        subitem_check1 = false;
        subitem_check2 = false;
        subitem_check3 = false;

        System.Array.Clear(subsubitem_check, 0, subsubitem_check.Length);
        System.Array.Clear(search, 0, search.Length);

        int priceminus = 0;

        for (int i=0; i<6; i++)
        {
            // 해당슬롯에 아이템이 없으면 다음으로
            if (PlayerData.Instance.item[i] == 0)
                continue;

            // 하위템1을 체크안했으면 체크함. 해서 true가 되었으면 넘어가질 것
            if(subitem_check1 == false)
            {
                if(myItem.subitem_id1 == PlayerData.Instance.item[i] && myItem.subitem_id1 != 0)
                {
                    subitem_check1 = true;
                    search[i] = true;
                    priceminus += ShopItem.Instance.itemlist[myItem.subitem_id1].price;
                    continue;
                }
                else if (myItem.subitem_id1 != 0)
                {
                    ShopItem.Item myItemSub1 = ShopItem.Instance.itemlist[myItem.subitem_id1];

                    if(subsubitem_check[0, 0] == false)
                    {
                        if (myItemSub1.subitem_id1 == PlayerData.Instance.item[i] && myItemSub1.subitem_id1 != 0)
                        {
                            subsubitem_check[0, 0] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub1.subitem_id1].price;
                            continue;
                        }
                    }
                    if (subsubitem_check[0, 1] == false)
                    {
                        if (myItemSub1.subitem_id2 == PlayerData.Instance.item[i] && myItemSub1.subitem_id2 != 0)
                        {
                            subsubitem_check[0, 1] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub1.subitem_id2].price;
                            continue;
                        }
                    }
                    if (subsubitem_check[0, 2] == false)
                    {
                        if (myItemSub1.subitem_id3 == PlayerData.Instance.item[i] && myItemSub1.subitem_id3 != 0)
                        {
                            subsubitem_check[0, 2] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub1.subitem_id3].price;
                            continue;
                        }
                    }
                }
            }

            // 하위템2을 체크안했으면 체크함. 해서 true가 되었으면 넘어가질 것
            if (subitem_check2 == false)
            {
                if (myItem.subitem_id2 == PlayerData.Instance.item[i] && myItem.subitem_id2 != 0)
                {
                    subitem_check2 = true;
                    search[i] = true;
                    priceminus += ShopItem.Instance.itemlist[myItem.subitem_id2].price;
                    continue;
                }
                else if(myItem.subitem_id2 != 0)
                {
                    ShopItem.Item myItemSub2 = ShopItem.Instance.itemlist[myItem.subitem_id2];

                    if (subsubitem_check[1, 0] == false)
                    {
                        if (myItemSub2.subitem_id1 == PlayerData.Instance.item[i] && myItemSub2.subitem_id1 != 0)
                        {
                            subsubitem_check[1, 0] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub2.subitem_id1].price;
                            continue;
                        }
                    }
                    if (subsubitem_check[1, 1] == false)
                    {
                        if (myItemSub2.subitem_id2 == PlayerData.Instance.item[i] && myItemSub2.subitem_id2 != 0)
                        {
                            subsubitem_check[1, 1] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub2.subitem_id2].price;
                            continue;
                        }
                    }
                    if (subsubitem_check[1, 2] == false)
                    {
                        if (myItemSub2.subitem_id3 == PlayerData.Instance.item[i] && myItemSub2.subitem_id3 != 0)
                        {
                            subsubitem_check[1, 2] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub2.subitem_id3].price;
                            continue;
                        }
                    }
                }
            }

            // 하위템3을 체크안했으면 체크함. 해서 true가 되었으면 넘어가질 것
            if (subitem_check3 == false)
            {
                if (myItem.subitem_id3 == PlayerData.Instance.item[i] && myItem.subitem_id3 != 0)
                {
                    subitem_check3 = true;
                    search[i] = true;
                    priceminus += ShopItem.Instance.itemlist[myItem.subitem_id3].price;
                    continue;
                }
                else if(myItem.subitem_id3 != 0)
                {
                    ShopItem.Item myItemSub3 = ShopItem.Instance.itemlist[myItem.subitem_id3];

                    if (subsubitem_check[2, 0] == false)
                    {
                        if (myItemSub3.subitem_id1 == PlayerData.Instance.item[i] && myItemSub3.subitem_id1 != 0)
                        {
                            subsubitem_check[2, 0] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub3.subitem_id1].price;
                            continue;
                        }
                    }
                    if (subsubitem_check[2, 1] == false)
                    {
                        if (myItemSub3.subitem_id2 == PlayerData.Instance.item[i] && myItemSub3.subitem_id2 != 0)
                        {
                            subsubitem_check[2, 1] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub3.subitem_id2].price;
                            continue;
                        }
                    }
                    if (subsubitem_check[2, 2] == false)
                    {
                        if (myItemSub3.subitem_id3 == PlayerData.Instance.item[i] && myItemSub3.subitem_id3 != 0)
                        {
                            subsubitem_check[2, 2] = true;
                            search[i] = true;
                            priceminus += ShopItem.Instance.itemlist[myItemSub3.subitem_id3].price;
                            continue;
                        }
                    }
                }
            }
        }

        myItem.price = ShopItem.Instance.itemlist[myItem.id].price - priceminus;
        if (myItem.price != 0)
            transform.Find("Price").GetComponent<Text>().text = myItem.price.ToString();
        else
            transform.Find("Price").GetComponent<Text>().text = "무료";
    }

    // material check
    public void MaterialCheck()
    {
        Shop shop = GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>();

        if (subitem_check1 && shop.subitem1[0] != null)
            shop.subitem1[0].transform.Find("CheckImage").gameObject.SetActive(true);
        if (subitem_check2 && shop.subitem1[1] != null)
            shop.subitem1[1].transform.Find("CheckImage").gameObject.SetActive(true);
        if (subitem_check3 && shop.subitem1[2] != null)
            shop.subitem1[2].transform.Find("CheckImage").gameObject.SetActive(true);

        for(int i=0; i<3; i++)
        {
            for(int j=0; j<3; j++)
            {
                if(subsubitem_check[i,j] && shop.subitem2[i, j] != null)
                    shop.subitem2[i,j].transform.Find("CheckImage").gameObject.SetActive(true);
            }
        }
    }

    public void tooltip_on()
    {
        if (ItemTooltip != null)
        {
            ItemTooltip.SetActive(true);
            onMouseEnter = true;

            tooltip_height = 55;

            //아이콘, 이름, 가격 세팅
            ItemTooltip.transform.GetChild(0).GetComponent<Image>().sprite = Icon_image;
            ItemTooltip.transform.GetChild(1).GetComponent<Text>().text = myItem.name;
            if (myItem.price == 0)
                ItemTooltip.transform.GetChild(3).GetComponent<Text>().text = "무료";
            else
                ItemTooltip.transform.GetChild(3).GetComponent<Text>().text = myItem.price.ToString();

            //스탯체크하여 존재하는 스탯만 출력
            string stat_string = "";
            if (myItem.attack_damage != 0)
                stat_string += "공격력 +" + myItem.attack_damage.ToString() + "\n";
            if (myItem.attack_speed != 0)
                stat_string += "공격 속도 +" + myItem.attack_speed.ToString() + "%\n";
            if (myItem.critical_percent != 0)
                stat_string += "치명타 확률 +" + myItem.critical_percent.ToString() + "%\n";
            if (myItem.life_steal != 0)
                stat_string += "생명력 흡수 +" + myItem.life_steal.ToString() + "%\n";
            if (myItem.ability_power != 0)
                stat_string += "주문력 +" + myItem.ability_power.ToString() + "\n";
            if (myItem.mana != 0)
                stat_string += "마나 +" + myItem.mana.ToString() + "\n";
            if (myItem.mana_regen != 0)
                stat_string += "기본 마나 재생 +" + myItem.mana_regen.ToString() + "%\n";
            if (myItem.cooldown_reduce != 0)
                stat_string += "재사용 대기시간 감소 +" + myItem.cooldown_reduce.ToString() + "%\n";
            if (myItem.armor != 0)
                stat_string += "방어력 +" + myItem.armor.ToString() + "\n";
            if (myItem.magic_resist != 0)
                stat_string += "마법 저항력 +" + myItem.magic_resist.ToString() + "\n";
            if (myItem.health != 0)
                stat_string += "체력 +" + myItem.health.ToString() + "\n";
            if (myItem.health_regen != 0)
                stat_string += "기본 체력 재생 +" + myItem.health_regen.ToString() + "%\n";
            if (myItem.movement_speed != 0)
                stat_string += "이동 속도 +" + myItem.movement_speed.ToString() + "\n";

            ItemTooltip.transform.GetChild(4).GetComponent<Text>().text = stat_string;

            if (stat_string != string.Empty)
            {
                int stat_lineCount = stat_string.Split('\n').Length - 1;
                tooltip_height += 15.0f * stat_lineCount;
            }

            //효과 출력
            if (myItem.effect_description != string.Empty)
            {
                // 스탯과 효과사이 간격
                if (stat_string != string.Empty)
                    tooltip_height += 8.0f;

                // 효과종류와 설명의 위치를 스탯 밑으로
                ItemTooltip.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition = 
                    new Vector2(ItemTooltip.transform.GetChild(5).GetComponent<RectTransform>().anchoredPosition.x,
                        -tooltip_height);

                // 텍스트를 갱신
                if (myItem.effect_kind != string.Empty)
                {
                    ItemTooltip.transform.GetChild(5).GetComponent<Text>().text =
                        "<color=#FFEB17>" + myItem.effect_kind + ":</color> " + myItem.effect_description;
                }
                else
                    ItemTooltip.transform.GetChild(5).GetComponent<Text>().text = myItem.effect_description;

                // 자동으로 줄바꿈되는경우 cachedTextGenerator를 통해 줄바꿈결과를 받을수 있으나 다음 프레임이되야 갱신됨.
                // 그래서 Canvas.ForceUpdateCanvases() 함수를 통해 이전까지의 결과를 미리 업데이트하고 반영된 데이터를 얻음.
                Canvas.ForceUpdateCanvases();
                int description_lineCount = ItemTooltip.transform.GetChild(5).GetComponent<Text>().cachedTextGenerator.lineCount;
                tooltip_height += 15.0f * description_lineCount;

                // 추가효과가 없는경우 공백으로.
                ItemTooltip.transform.GetChild(6).GetComponent<Text>().text = "";

                // 추가효과가 있다면 효과와 마찬가지로 출력
                if (myItem.additional_description != string.Empty)
                {
                    tooltip_height += 8.0f;

                    ItemTooltip.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(ItemTooltip.transform.GetChild(6).GetComponent<RectTransform>().anchoredPosition.x,
                        -tooltip_height);

                    ItemTooltip.transform.GetChild(6).GetComponent<Text>().text =
                        "<color=#FFEB17>" + myItem.additional_kind + ":</color> " + myItem.additional_description;

                    Canvas.ForceUpdateCanvases();
                    int additional_lineCount = ItemTooltip.transform.GetChild(6).GetComponent<Text>().cachedTextGenerator.lineCount;
                    tooltip_height += 15.0f * additional_lineCount;
                }
            }
            else
            {
                ItemTooltip.transform.GetChild(5).GetComponent<Text>().text = "";
                ItemTooltip.transform.GetChild(6).GetComponent<Text>().text = "";
            }

            //전체 길이에 맞게 크기조정
            tooltip_height += 5.0f;
            ItemTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
        }
    }

    public void tooltip_off()
    {
        if (ItemTooltip != null)
            ItemTooltip.SetActive(false);

        onMouseEnter = false;
    }

    public void ItemSelect()
    {
        if (myItem != null)
            ShopCanvas.GetComponent<Shop>().ItemSelect(myItem.id, gameObject);
    }

    public void MaterialItemSelect()
    {
        if (myItem != null)
            ShopCanvas.GetComponent<Shop>().MaterialItemSelect(myItem.id, gameObject);
    }

    public void ItemSelectBorder()
    {
        selected = true;
        
        GameObject sb = transform.Find("SelectBorder").gameObject;
        if (sb.GetComponent<Image>().enabled == false)
            sb.GetComponent<Image>().enabled = true;
    }

    public void SearchItemSelectBorder()
    {
        searchSelected = true;

        GetComponent<Outline>().effectColor = new Color(46f / 255f, 159f / 255f, 131f / 255f, 128f / 255f);
    }

    public void ItemViewMouseEnter()
    {
        if (myItem != null)
        {
            tooltip_on();
            if (ItemTooltip != null)
            {
                Icon_image = Resources.Load<Sprite>("Item_Image/" + myItem.icon_name);
                ItemTooltip.transform.GetChild(0).GetComponent<Image>().sprite = Icon_image;
                ItemTooltip.transform.GetChild(3).GetComponent<Text>().text = "판매 가격: " + (myItem.price * 0.7f).ToString();
            }

            //if (ShopCanvas.GetComponent<Shop>().selectedItemID != myItem.id
            //|| ShopCanvas.GetComponent<Shop>().selectedObject != gameObject)
            //    transform.Find("ItemBorder").GetComponent<Image>().color = new Color(181f / 255f, 163f / 255f, 96f / 255f, 1);
        }
    }

    public void ItemViewMouseExit()
    {
        tooltip_off();

        //if(myItem != null)
        //{
        //    if (ShopCanvas.GetComponent<Shop>().selectedItemID != myItem.id
        //    || ShopCanvas.GetComponent<Shop>().selectedObject != gameObject)
        //        transform.Find("ItemBorder").GetComponent<Image>().color = new Color(125f / 255f, 116f / 255f, 82f / 255f, 1);
        //}
    }

    public void ItemViewSelect(int ViewNum)
    {
        ItemSelect();

        if (myItem != null)
        {
            ShopCanvas.GetComponent<Shop>().myItemSelect(ViewNum);

            viewSelected = true;
            //transform.Find("ItemBorder").GetComponent<Image>().color = new Color(181f / 255f, 163f / 255f, 96f / 255f, 1);
        }
    }

    public void UI_tooltip_on()
    {
        if (myItem != null)
        {
            if (UI_ItemTooltip != null)
            {
                UI_ItemTooltip.SetActive(true);

                float tooltip_height = 30;
                UI_ItemTooltip.transform.Find("ItemName").GetComponent<Text>().text = myItem.name;
                UI_ItemTooltip.transform.Find("SellPrice").GetComponent<Text>().text = "판매 가격: <color=#E4B803>" + (myItem.price * 0.7f).ToString() + "</color>";

                //스탯체크하여 존재하는 스탯만 출력
                string stat_string = "";
                if (myItem.attack_damage != 0)
                    stat_string += "공격력 +" + myItem.attack_damage.ToString() + "\n";
                if (myItem.attack_speed != 0)
                    stat_string += "공격 속도 +" + myItem.attack_speed.ToString() + "%\n";
                if (myItem.critical_percent != 0)
                    stat_string += "치명타 확률 +" + myItem.critical_percent.ToString() + "%\n";
                if (myItem.life_steal != 0)
                    stat_string += "생명력 흡수 +" + myItem.life_steal.ToString() + "%\n";
                if (myItem.ability_power != 0)
                    stat_string += "주문력 +" + myItem.ability_power.ToString() + "\n";
                if (myItem.mana != 0)
                    stat_string += "마나 +" + myItem.mana.ToString() + "\n";
                if (myItem.mana_regen != 0)
                    stat_string += "기본 마나 재생 +" + myItem.critical_percent.ToString() + "%\n";
                if (myItem.cooldown_reduce != 0)
                    stat_string += "재사용 대기시간 감소 +" + myItem.cooldown_reduce.ToString() + "%\n";
                if (myItem.armor != 0)
                    stat_string += "방어력 +" + myItem.armor.ToString() + "\n";
                if (myItem.magic_resist != 0)
                    stat_string += "마법 저항력 +" + myItem.magic_resist.ToString() + "\n";
                if (myItem.health != 0)
                    stat_string += "체력 +" + myItem.health.ToString() + "\n";
                if (myItem.health_regen != 0)
                    stat_string += "기본 체력 재생 +" + myItem.health_regen.ToString() + "%\n";
                if (myItem.movement_speed != 0)
                    stat_string += "이동 속도 +" + myItem.movement_speed.ToString() + "\n";

                UI_ItemTooltip.transform.Find("Status").GetComponent<Text>().text = stat_string;

                if (stat_string != string.Empty)
                {
                    tooltip_height += 5.0f;

                    int stat_lineCount = stat_string.Split('\n').Length - 1;
                    tooltip_height += 15.0f * stat_lineCount;
                }

                //효과 출력
                if (myItem.effect_description != string.Empty)
                {
                    UI_ItemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition =
                        new Vector3(UI_ItemTooltip.transform.Find("Effect_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);

                    // 텍스트를 갱신
                    if (myItem.effect_kind != string.Empty)
                    {
                        UI_ItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text =
                            "<color=#FFEB17>" + myItem.effect_kind + ":</color> " + myItem.effect_description;
                    }
                    else
                        UI_ItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text = myItem.effect_description;

                    Canvas.ForceUpdateCanvases();
                    int description_lineCount = UI_ItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
                    tooltip_height += 15.0f * description_lineCount;

                    // 추가효과가 없는경우 공백으로.
                    UI_ItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";

                    // 추가효과가 있다면 효과와 마찬가지로 출력
                    if (myItem.additional_description != string.Empty)
                    {
                        tooltip_height += 8.0f;

                        UI_ItemTooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition =
                            new Vector2(UI_ItemTooltip.transform.Find("Additional_Description").GetComponent<RectTransform>().anchoredPosition.x, -tooltip_height);

                        UI_ItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text =
                            "<color=#FFEB17>" + myItem.additional_kind + ":</color> " + myItem.additional_description;

                        Canvas.ForceUpdateCanvases();
                        int additional_lineCount = UI_ItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().cachedTextGenerator.lineCount;
                        tooltip_height += 15.0f * additional_lineCount;
                    }
                }
                else
                {
                    UI_ItemTooltip.transform.Find("Effect_Description").GetComponent<Text>().text = "";
                    UI_ItemTooltip.transform.Find("Additional_Description").GetComponent<Text>().text = "";
                }

                tooltip_height += 5.0f;
                UI_ItemTooltip.GetComponent<RectTransform>().sizeDelta = new Vector2(UI_ItemTooltip.GetComponent<RectTransform>().sizeDelta.x, tooltip_height);
            }
        }
    }

    public void UI_tooltip_off()
    {
        if (UI_ItemTooltip != null)
            UI_ItemTooltip.SetActive(false);
    }
}
