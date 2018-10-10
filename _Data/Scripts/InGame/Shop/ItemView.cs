using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour {

    public GameObject[] myItem = new GameObject[6];
    public GameObject accessory = null;

    public GameObject price = null;

	// Use this for initialization
	void Start () {
        StatusUpdate();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StatusUpdate()
    {
        price.GetComponent<Text>().text = PlayerData.Instance.gold.ToString();

        for (int i = 0; i < PlayerData.Instance.item.Length; i++)
        {
            // 아이템이 있으면 아이템뷰에 표시
            if (PlayerData.Instance.item[i] != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[PlayerData.Instance.item[i]];
                myItem[i].GetComponent<ItemInfo>().myItem = it.ClassCopy();
                myItem[i].transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("RPG icons/64X64/" + it.icon_name);
                myItem[i].transform.Find("Icon").GetComponent<Image>().color = Color.white;
            }
            // 아이템이 없으면
            else
            {
                myItem[i].GetComponent<ItemInfo>().myItem = null;
                myItem[i].transform.Find("Icon").GetComponent<Image>().sprite = null;
                myItem[i].transform.Find("Icon").GetComponent<Image>().color = new Color(14f / 255f, 22f / 255f, 23f / 255f, 150f / 255f);
                myItem[i].GetComponent<ItemInfo>().viewSelected = false;
            }
        }

        if(PlayerData.Instance.accessoryItem != 0)
        {
            ShopItem.Item it = ShopItem.Instance.itemlist[PlayerData.Instance.accessoryItem];
            accessory.GetComponent<ItemInfo>().myItem = it.ClassCopy();
            accessory.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("RPG icons/64X64/" + it.icon_name);
            accessory.transform.Find("Icon").GetComponent<Image>().color = Color.white;
        }
        else
        {
            accessory.GetComponent<ItemInfo>().myItem = null;
            accessory.transform.Find("Icon").GetComponent<Image>().sprite = null;
            accessory.transform.Find("Icon").GetComponent<Image>().color = new Color(14f / 255f, 22f / 255f, 23f / 255f, 150f / 255f);
            accessory.GetComponent<ItemInfo>().viewSelected = false;
        }
    }
}
