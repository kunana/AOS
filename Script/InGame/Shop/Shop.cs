using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

    public GameObject Item;
    public GameObject SearchItem;
    public GameObject MakingItem;
    public GameObject MaterialItem;

    public GameObject ItemTooltip;
    public GameObject SortTooltip;

    public GameObject ItemContent;
    public GameObject SearchContent;
    public GameObject MakingContent;
    public GameObject MaterialItemContent;
    public GameObject Description;

    public int LineMaxItemCount = 6;
    public int SearchLineMaxCount = 3;
    private Vector3 StartPos = new Vector3(-130f, -5f);
    private Vector3 SearchStartPos = new Vector3(-158f, -5f);
    private int ItemWidth = 60;
    private int ItemHeight = 70;
    private int SearchItemWidth = 155;
    private int SearchItemHeight = 56;
    private int MakingItemWidth = 50;

    [Space]
    public GameObject LineContent;
    public GameObject Line2_1;
    public GameObject Line2_2;
    public GameObject Line2_3;
    public GameObject Line3_2;
    public GameObject Line3_3;

    [Space]
    public int selectedItemID = 100;
    public GameObject selectedObject = null;
    public bool myItemSelected = false;
    public int selectedViewNum = 0;

    [HideInInspector]
    public GameObject[] subitem1 = new GameObject[3];
    public GameObject[,] subitem2 = new GameObject[3, 3];

    // Use this for initialization
    void Start()
    {
        ItemCreate(0);
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.UI_Close);
            gameObject.SetActive(false);
        }
	}

    public void ItemUpgrade()
    {
        int price = selectedObject.GetComponent<ItemInfo>().myItem.price;
        PlayerData.Instance.ItemUpgrade(selectedObject.GetComponent<ItemInfo>().search, selectedItemID, price, ShopItem.Instance.itemlist[selectedItemID].accessory);

        GameObject.FindGameObjectWithTag("ItemView").GetComponent<ItemView>().StatusUpdate();
        GameObject.FindGameObjectWithTag("ItemUI").GetComponent<ItemUI>().StatusUpdate();

        foreach (Transform item in ItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        foreach (Transform item in MaterialItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        MaterialCheck();
    }

    public void ItemPurchase()
    {
        if (selectedItemID != 100)
        {
            bool[] search = selectedObject.GetComponent<ItemInfo>().search;
            for(int i=0; i<6; i++)
            {
                if(search[i] == true)
                {
                    ItemUpgrade();
                    return;
                }
            }
            int price = selectedObject.GetComponent<ItemInfo>().myItem.price;
            PlayerData.Instance.ItemPurchase(selectedItemID, price, ShopItem.Instance.itemlist[selectedItemID].accessory);
        }
        GameObject.FindGameObjectWithTag("ItemView").GetComponent<ItemView>().StatusUpdate();
        GameObject.FindGameObjectWithTag("ItemUI").GetComponent<ItemUI>().StatusUpdate();
        
        foreach(Transform item in ItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        foreach (Transform item in MaterialItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        MaterialCheck();
    }

    public void ItemSell()
    {
        if(myItemSelected)
        {
            if (selectedItemID != 100)
            {
                PlayerData.Instance.ItemSell(selectedViewNum, selectedItemID, ShopItem.Instance.itemlist[selectedItemID].price);
                selectedItemID = 100;
                myItemSelected = false;
            }
        }
        GameObject.FindGameObjectWithTag("ItemView").GetComponent<ItemView>().StatusUpdate();
        GameObject.FindGameObjectWithTag("ItemUI").GetComponent<ItemUI>().StatusUpdate();

        foreach (Transform item in ItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        foreach (Transform item in MaterialItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        MaterialCheck();
        SoundManager.instance.PlaySound(SoundManager.instance.Shop_Sell);
    }

    public void ItemUndo()
    {
        // 되돌리고 판매했을때만 true로 리턴
        bool sell = PlayerData.Instance.ItemUndo();
        if(sell)
            myItemSelected = false;

        GameObject.FindGameObjectWithTag("ItemView").GetComponent<ItemView>().StatusUpdate();
        GameObject.FindGameObjectWithTag("ItemUI").GetComponent<ItemUI>().StatusUpdate();

        foreach (Transform item in ItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        foreach (Transform item in MaterialItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        MaterialCheck();
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }

    public void ItemSelect(int itemID, GameObject itemObject)
    {
        int childCount = MaterialItemContent.transform.childCount;

        //제작창, 재료아이템창, 설명창 변경
        selectedItemID = itemID;
        selectedObject = itemObject;

        MakingItemSearch(itemID);
        MaterialItemSearch(itemID);
        DescriptionUpdate(itemID);

        myItemSelected = false;
        selectedViewNum = 0;

        foreach (Transform item in MaterialItemContent.transform)
        {
            item.GetComponent<ItemInfo>().ItemCheck();
        }
        MaterialCheck(childCount);
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }

    public void MaterialCheck(int prevChildCount = 0)
    {
        foreach (Transform t in MaterialItemContent.transform)
        {
            t.Find("CheckImage").gameObject.SetActive(false);
        }

        // 구매, 판매시에 재료아이템창이 변하지않는경우는 그냥 0번을 불러줌
        // 재료아이템창이 변하는경우 아직 프레임이 지나지않아 이전 오브젝트들이 destroy되지않아
        // 0번은 삭제될 이전아이템이므로 이전자식의 개수뒤(새아이템)의 자식을 불러줘야함
        if (selectedItemID != 100)
            MaterialItemContent.transform.GetChild(prevChildCount).GetComponent<ItemInfo>().MaterialCheck();
    }

    public void myItemSelect(int ViewNum)
    {
        myItemSelected = true;
        selectedViewNum = ViewNum;
    }

    public void MaterialItemSelect(int itemID, GameObject itemObject)
    {
        // 재료아이템창 빼고 다 변경
        selectedItemID = itemID;
        selectedObject = itemObject;

        MakingItemSearch(itemID);
        DescriptionUpdate(itemID);
    }

    // 제작아이템을 검색하여 제작창에 만들어주는 함수
    public void MakingItemSearch(int itemID)
    {
        ShopItem.Instance.making_itemlist.Clear();
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].subitem_id1 == itemID
                || ShopItem.Instance.itemlist[i + 1].subitem_id2 == itemID
                || ShopItem.Instance.itemlist[i + 1].subitem_id3 == itemID)
            {
                ShopItem.Instance.making_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }

        foreach (Transform tr in MakingContent.transform)
        {
            Destroy(tr.gameObject);
        }

        int count = 0;
        foreach (ShopItem.Item i in ShopItem.Instance.making_itemlist)
        {
            GameObject createdItem = Instantiate(MakingItem, MakingContent.transform);
            createdItem.GetComponent<RectTransform>().localPosition = new Vector3(count * MakingItemWidth, 0);
            count++;

            createdItem.GetComponent<ItemInfo>().myItem = i.ClassCopy();
            createdItem.GetComponent<Image>().sprite = Resources.Load<Sprite>("Item_Image/" + i.icon_name);
        }
        GameObject.FindGameObjectWithTag("Making").GetComponent<MakingItem>().ItemCount(count);
    }

    // 재료 아이템 생성
    public void MaterialItemSearch(int itemID)
    {
        ShopItem.Item selectItem = ShopItem.Instance.itemlist[itemID];

        foreach (Transform tr in MaterialItemContent.transform)
        {
            Destroy(tr.gameObject);
        }
        foreach (Transform tr in LineContent.transform)
        {
            Destroy(tr.gameObject);
        }

        GameObject createdItem = Instantiate(MaterialItem, MaterialItemContent.transform);
        createdItem.transform.localPosition = Vector3.zero;
        createdItem.GetComponent<ItemInfo>().myItem = selectItem.ClassCopy();
        createdItem.GetComponent<ItemInfo>().BasicSetting();

        for (int i = 0; i < 3; i++)
        {
            subitem1[i] = null;
            for (int j = 0; j < 3; j++)
            {
                subitem2[i, j] = null;
            }
        }
        subItemCreate(selectItem, 2, createdItem);
    }

    public void subItemCreate(ShopItem.Item pItem, int floor, GameObject Parent, int parentcheckid = 100)
    {
        if (floor > 3)
            return;

        int distance2 = 0;
        int distance3 = 0;
        if (floor == 2)
        {
            distance2 = 70;
            distance3 = 100;
        }
        else if(floor == 3)
        {
            distance2 = 30;
            distance3 = 45;
        }

        int subitem_count = 0;
        if (pItem.subitem_id1 != 0)
            subitem_count++;
        if (pItem.subitem_id2 != 0)
            subitem_count++;
        if (pItem.subitem_id3 != 0)
            subitem_count++;

        if (subitem_count > 0)
        {
            if (subitem_count == 1)
            {
                GameObject subItem1 = Instantiate(MaterialItem, MaterialItemContent.transform);
                subItem1.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -60);
                subItem1.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitem_id1].ClassCopy();
                subItem1.GetComponent<ItemInfo>().BasicSetting();

                subItemCreate(ShopItem.Instance.itemlist[pItem.subitem_id1], floor + 1, subItem1, 0);

                GameObject subLine = Instantiate(Line2_1, LineContent.transform);
                subLine.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -52);

                if (floor == 2)
                    subitem1[0] = subItem1;
                else if (floor == 3)
                    subitem2[parentcheckid, 0] = subItem1;
            }
            else if (subitem_count == 2)
            {
                GameObject subItem1 = Instantiate(MaterialItem, MaterialItemContent.transform);
                subItem1.transform.localPosition = Parent.transform.localPosition + new Vector3(-distance2, -60);
                subItem1.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitem_id1].ClassCopy();
                subItem1.GetComponent<ItemInfo>().BasicSetting();

                subItemCreate(ShopItem.Instance.itemlist[pItem.subitem_id1], floor + 1, subItem1, 0);


                GameObject subItem2 = Instantiate(MaterialItem, MaterialItemContent.transform);
                subItem2.transform.localPosition = Parent.transform.localPosition + new Vector3(distance2, -60);
                subItem2.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitem_id2].ClassCopy();
                subItem2.GetComponent<ItemInfo>().BasicSetting();

                subItemCreate(ShopItem.Instance.itemlist[pItem.subitem_id2], floor + 1, subItem2, 1);


                GameObject subLine = null;
                if (floor == 2)
                    subLine = Instantiate(Line2_2, LineContent.transform);
                else if(floor == 3)
                    subLine = Instantiate(Line3_2, LineContent.transform);

                subLine.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -52);

                if (floor == 2)
                {
                    subitem1[0] = subItem1;
                    subitem1[1] = subItem2;
                }                    
                else if (floor == 3)
                {
                    subitem2[parentcheckid, 0] = subItem1;
                    subitem2[parentcheckid, 1] = subItem2;
                }
            }
            else if (subitem_count == 3)
            {
                GameObject subItem1 = Instantiate(MaterialItem, MaterialItemContent.transform);
                subItem1.transform.localPosition = Parent.transform.localPosition + new Vector3(-distance3, -60);
                subItem1.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitem_id1].ClassCopy();
                subItem1.GetComponent<ItemInfo>().BasicSetting();

                subItemCreate(ShopItem.Instance.itemlist[pItem.subitem_id1], floor + 1, subItem1, 0);


                GameObject subItem2 = Instantiate(MaterialItem, MaterialItemContent.transform);
                subItem2.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -60);
                subItem2.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitem_id2].ClassCopy();
                subItem2.GetComponent<ItemInfo>().BasicSetting();

                subItemCreate(ShopItem.Instance.itemlist[pItem.subitem_id2], floor + 1, subItem2, 1);


                GameObject subItem3 = Instantiate(MaterialItem, MaterialItemContent.transform);
                subItem3.transform.localPosition = Parent.transform.localPosition + new Vector3(distance3, -60);
                subItem3.GetComponent<ItemInfo>().myItem = ShopItem.Instance.itemlist[pItem.subitem_id3].ClassCopy();
                subItem3.GetComponent<ItemInfo>().BasicSetting();

                subItemCreate(ShopItem.Instance.itemlist[pItem.subitem_id3], floor + 1, subItem3, 2);


                GameObject subLine = null;
                if (floor == 2)
                    subLine = Instantiate(Line2_3, LineContent.transform);
                else if (floor == 3)
                    subLine = Instantiate(Line3_3, LineContent.transform);

                subLine.transform.localPosition = Parent.transform.localPosition + new Vector3(0, -52);

                if (floor == 2)
                {
                    subitem1[0] = subItem1;
                    subitem1[1] = subItem2;
                    subitem1[2] = subItem3;
                }
                else if (floor == 3)
                {
                    subitem2[parentcheckid, 0] = subItem1;
                    subitem2[parentcheckid, 1] = subItem2;
                    subitem2[parentcheckid, 2] = subItem3;
                }
            }
        }
    }

    public void DescriptionUpdate(int itemID)
    {
        // 처음 실행될때 항목들을 켜줌 (처음에는 선택이 없으니 꺼져있을테니)
        Description.transform.Find("ItemIcon").GetComponent<Image>().enabled = true;
        Description.transform.Find("ItemName").GetComponent<Text>().enabled = true;
        Description.transform.Find("GoldImage").GetComponent<Image>().enabled = true;
        Description.transform.Find("Price").GetComponent<Text>().enabled = true;
        Description.transform.Find("Status").GetComponent<Text>().enabled = true;
        Description.transform.Find("Effect_Description").GetComponent<Text>().enabled = true;
        Description.transform.Find("Additional_Description").GetComponent<Text>().enabled = true;

        ShopItem.Item selectItem = ShopItem.Instance.itemlist[itemID];
        // 아이콘 이름 가격
        Sprite Icon_image = Resources.Load<Sprite>("Item_Image/" + selectItem.icon_name);
        Description.transform.Find("ItemIcon").GetComponent<Image>().sprite = Icon_image;
        Description.transform.Find("ItemName").GetComponent<Text>().text = selectItem.name;

        if (ShopItem.Instance.itemlist[itemID].price == 0)
            Description.transform.Find("Price").GetComponent<Text>().text = "무료";
        else
            Description.transform.Find("Price").GetComponent<Text>().text = selectItem.price.ToString();

        //스탯체크
        float height = 68;

        string stat_string = "";
        if (selectItem.attack_damage != 0)
            stat_string += "공격력 +" + selectItem.attack_damage.ToString() + "\n";
        if (selectItem.attack_speed != 0)
            stat_string += "공격 속도 +" + selectItem.attack_speed.ToString() + "%\n";
        if (selectItem.critical_percent != 0)
            stat_string += "치명타 확률 +" + selectItem.critical_percent.ToString() + "%\n";
        if (selectItem.life_steal != 0)
            stat_string += "생명력 흡수 +" + selectItem.life_steal.ToString() + "%\n";
        if (selectItem.ability_power != 0)
            stat_string += "주문력 +" + selectItem.ability_power.ToString() + "\n";
        if (selectItem.mana != 0)
            stat_string += "마나 +" + selectItem.mana.ToString() + "\n";
        if (selectItem.mana_regen != 0)
            stat_string += "기본 마나 재생 +" + selectItem.mana_regen.ToString() + "%\n";
        if (selectItem.cooldown_reduce != 0)
            stat_string += "재사용 대기시간 감소 +" + selectItem.cooldown_reduce.ToString() + "%\n";
        if (selectItem.armor != 0)
            stat_string += "방어력 +" + selectItem.armor.ToString() + "\n";
        if (selectItem.magic_resist != 0)
            stat_string += "마법 저항력 +" + selectItem.magic_resist.ToString() + "\n";
        if (selectItem.health != 0)
            stat_string += "체력 +" + selectItem.health.ToString() + "\n";
        if (selectItem.health_regen != 0)
            stat_string += "기본 체력 재생 +" + selectItem.health_regen.ToString() + "%\n";
        if (selectItem.movement_speed != 0)
            stat_string += "이동 속도 +" + selectItem.movement_speed.ToString() + "\n";

        Description.transform.Find("Status").GetComponent<Text>().text = stat_string;

        if (stat_string != string.Empty)
        {
            int stat_lineCount = stat_string.Split('\n').Length - 1;
            height += 17.0f * stat_lineCount;
        }
        

        // 효과
        if (selectItem.effect_description != string.Empty)
        {
            // 스탯과 효과사이 간격
            if (stat_string != string.Empty)
                height += 8.0f;

            GameObject Effect_Discription = Description.transform.Find("Effect_Description").gameObject;

            // 효과종류와 설명의 위치를 스탯 밑으로
            Effect_Discription.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Effect_Discription.GetComponent<RectTransform>().anchoredPosition.x,
                    -height);

            // 텍스트를 갱신
            if (selectItem.effect_kind != string.Empty)
            {
                Effect_Discription.GetComponent<Text>().text =
                    "<color=#FFEB17>" + selectItem.effect_kind + ":</color> " + selectItem.effect_description;
            }
            else
                Effect_Discription.GetComponent<Text>().text = selectItem.effect_description;

            // 자동으로 줄바꿈되는경우 cachedTextGenerator를 통해 줄바꿈결과를 받을수 있으나 다음 프레임이되야 갱신됨.
            // 그래서 Canvas.ForceUpdateCanvases() 함수를 통해 이전까지의 결과를 미리 업데이트하고 반영된 데이터를 얻음.
            Canvas.ForceUpdateCanvases();
            int description_lineCount = Effect_Discription.GetComponent<Text>().cachedTextGenerator.lineCount;
            height += 17.0f * description_lineCount;

            // 추가효과가 없는경우 공백으로.
            GameObject Additional_Discription = Description.transform.Find("Additional_Description").gameObject;
            Additional_Discription.GetComponent<Text>().text = "";

            // 추가효과가 있다면 효과와 마찬가지로 출력
            if (selectItem.additional_description != string.Empty)
            {
                height += 8.0f;

                Additional_Discription.GetComponent<RectTransform>().anchoredPosition =
                new Vector2(Additional_Discription.GetComponent<RectTransform>().anchoredPosition.x,
                    -height);

                Additional_Discription.GetComponent<Text>().text =
                    "<color=#FFEB17>" + selectItem.additional_kind + ":</color> " + selectItem.additional_description;
            }
        }
        else
        {
            Description.transform.Find("Effect_Description").GetComponent<Text>().text = "";
            Description.transform.Find("Additional_Description").GetComponent<Text>().text = "";
        }
    }

    // 검색어 입력시 아이템을 검색하여 검색창에 만들어주는 함수
    public void ItemSearch(string input)
    {
        ShopItem.Instance.search_itemlist.Clear();
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].name.Contains(input))
            {
                ShopItem.Instance.search_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }

        foreach (Transform tr in SearchContent.transform)
        {
            Destroy(tr.gameObject);
        }

        int count = 0;
        foreach (ShopItem.Item i in ShopItem.Instance.search_itemlist)
        {
            GameObject createdItem = Instantiate(SearchItem, SearchContent.transform);
            createdItem.GetComponent<RectTransform>().localPosition = SearchStartPos + new Vector3(count % SearchLineMaxCount * SearchItemWidth, count / SearchLineMaxCount * -SearchItemHeight);
            count++;

            createdItem.GetComponent<ItemInfo>().myItem = i.ClassCopy();
            createdItem.GetComponent<ItemInfo>().BasicSetting();
            createdItem.transform.Find("Name").GetComponent<Text>().text = i.name;
        }

        // 검색결과가 3줄이상이면 스크롤 적용
        if ((ShopItem.Instance.search_itemlist.Count - 1) / SearchLineMaxCount >= 2)
        {
            float SearchHeight = (((ShopItem.Instance.search_itemlist.Count - 1) / SearchLineMaxCount) + 1) * 57f;
            SearchContent.GetComponent<RectTransform>().sizeDelta = new Vector2(SearchContent.GetComponent<RectTransform>().sizeDelta.x, SearchHeight);
        }
        // 아니면 원래대로
        else
            SearchContent.GetComponent<RectTransform>().sizeDelta = new Vector2(SearchContent.GetComponent<RectTransform>().sizeDelta.x, 150);
    }

    // 메인 아이템 화면에 아이템을 만들어주는 함수
    public void ItemCreate(int id)
    {
        foreach (Transform tr in ItemContent.transform)
        {
            Destroy(tr.gameObject);
        }
        ShopItem.Instance.sorted_itemlist.Clear();

        // 정렬
        if (id == 0)
            CreateItemAll();
        else if (id == 1)
            CreateItemSpecial();
        else if (id == 2)
            CreateItemConsumable();
        else if (id == 3)
            CreateItemAccessory();
        else if (id == 4)
            CreateItemDefense();
        else if (id == 5)
            CreateItemArmor();
        else if (id == 6)
            CreateItemMagicResist();
        else if (id == 7)
            CreateItemHealth();
        else if (id == 8)
            CreateItemHealthRegen();
        else if (id == 9)
            CreateItemAttack();
        else if (id == 10)
            CreateItemAttackDamage();
        else if (id == 11)
            CreateItemAttackSpeed();
        else if (id == 12)
            CreateItemCritical();
        else if (id == 13)
            CreateItemLifeSteal();
        else if (id == 14)
            CreateItemMagic();
        else if (id == 15)
            CreateItemAbilityPower();
        else if (id == 16)
            CreateItemMana();
        else if (id == 17)
            CreateItemManaRegen();
        else if (id == 18)
            CreateItemCooldownReduce();
        else if (id == 19)
            CreateItemMove();
        else if (id == 20)
            CreateItemBoots();
        else if (id == 21)
            CreateItemMovementSpeed();


        int count = 0;
        foreach(ShopItem.Item i in ShopItem.Instance.sorted_itemlist)
        {
            GameObject createdItem = Instantiate(Item, ItemContent.transform);
            createdItem.transform.localPosition = StartPos + new Vector3(count % LineMaxItemCount * ItemWidth, count / LineMaxItemCount * -ItemHeight);
            count++;

            createdItem.GetComponent<ItemInfo>().myItem = i.ClassCopy();
            createdItem.GetComponent<ItemInfo>().BasicSetting();
        }

        // 검색결과가 6줄이상이면 스크롤 적용

        if ((ShopItem.Instance.sorted_itemlist.Count - 1) / LineMaxItemCount >= 5)
        {
            float ListHeight = (((ShopItem.Instance.sorted_itemlist.Count - 1) / LineMaxItemCount) + 1) * 70f;
            ItemContent.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemContent.GetComponent<RectTransform>().sizeDelta.x, ListHeight);
        }
        // 아니면 원래대로
        else
            ItemContent.GetComponent<RectTransform>().sizeDelta = new Vector2(ItemContent.GetComponent<RectTransform>().sizeDelta.x, 400);
    }

    public void CreateItemAll()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
        }
    }

    public void CreateItemSpecial()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].consumable == true || ShopItem.Instance.itemlist[i + 1].accessory == true)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemConsumable()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].consumable == true)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAccessory()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].accessory == true)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemDefense()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].armor != 0 || ShopItem.Instance.itemlist[i + 1].magic_resist != 0
                || ShopItem.Instance.itemlist[i + 1].health != 0 || ShopItem.Instance.itemlist[i + 1].health_regen != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemArmor()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].armor != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMagicResist()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].magic_resist != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemHealth()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].health != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemHealthRegen()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].health_regen != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAttack()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].attack_damage != 0 || ShopItem.Instance.itemlist[i + 1].attack_speed != 0
                || ShopItem.Instance.itemlist[i + 1].critical_percent != 0 || ShopItem.Instance.itemlist[i + 1].life_steal != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAttackDamage()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].attack_damage != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAttackSpeed()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].attack_speed != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemCritical()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].critical_percent != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemLifeSteal()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].life_steal != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMagic()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].ability_power != 0 || ShopItem.Instance.itemlist[i + 1].mana != 0
                || ShopItem.Instance.itemlist[i + 1].mana_regen != 0 || ShopItem.Instance.itemlist[i + 1].cooldown_reduce != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemAbilityPower()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].ability_power != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMana()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].mana != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemManaRegen()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].mana_regen != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemCooldownReduce()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].cooldown_reduce != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMove()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].boots == true || ShopItem.Instance.itemlist[i + 1].movement_speed != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemBoots()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].boots == true)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }

    public void CreateItemMovementSpeed()
    {
        for (int i = 0; i < ShopItem.Instance.itemlist.Count; i++)
        {
            if (ShopItem.Instance.itemlist[i + 1].movement_speed != 0)
            {
                ShopItem.Instance.sorted_itemlist.Add(ShopItem.Instance.itemlist[i + 1]);
            }
        }
    }
}
