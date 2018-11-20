﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{

    public GameObject ShopCanvas = null;

    public GameObject[] myItem = new GameObject[6];
    public GameObject accessory = null;
    public GameObject price = null;

    void Update()
    {
        StatusUpdate();
    }

    public void ShopButton()
    {
        if (ShopCanvas.activeSelf)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.UI_Close);
            ShopCanvas.SetActive(false);
        }
        else
        {
            SoundManager.instance.PlaySound(SoundManager.instance.UI_Open);
            ShopCanvas.SetActive(true);
        }
    }

    public void StatusUpdate()
    {
        price.GetComponent<Text>().text = PlayerData.Instance.gold.ToString();

        for (int i = 0; i < PlayerData.Instance.item.Length; i++)
        {
            if (PlayerData.Instance.item[i] != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[PlayerData.Instance.item[i]];
                myItem[i].GetComponent<ItemInfo>().myItem = it.ClassCopy();
                myItem[i].transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Item_Image/" + it.icon_name);
                myItem[i].transform.Find("Icon").GetComponent<Image>().color = Color.white;
            }
            else
            {
                myItem[i].GetComponent<ItemInfo>().myItem = null;
                myItem[i].transform.Find("Icon").GetComponent<Image>().sprite = null;
                myItem[i].transform.Find("Icon").GetComponent<Image>().color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }

        if (PlayerData.Instance.accessoryItem != 0)
        {
            ShopItem.Item it = ShopItem.Instance.itemlist[PlayerData.Instance.accessoryItem];
            accessory.GetComponent<ItemInfo>().myItem = it.ClassCopy();
            accessory.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>("Item_Image/" + it.icon_name);
            accessory.transform.Find("Icon").GetComponent<Image>().color = Color.white;
        }
        else
        {
            accessory.GetComponent<ItemInfo>().myItem = null;
            accessory.transform.Find("Icon").GetComponent<Image>().sprite = null;
            accessory.transform.Find("Icon").GetComponent<Image>().color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
        }
    }
}
