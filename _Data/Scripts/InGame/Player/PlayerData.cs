using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData> {

    /*private static PlayerData _instance;
    public static PlayerData Instance
    {
        get
        {
            if (_instance == null)
                _instance = new PlayerData();
            return _instance;
        }
    }*/

    // 챔피언이름
    public string championName = "";

    // 정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막 (0~8)
    // 스펠 ID
    public int spell_D = 7;
    public int spell_F = 2;

    //룬정보 (안쓸듯)
    public int mainRune = 0;
    public int subRune1 = 0;
    public int subRune2 = 0;
    public int subRune3 = 0;
    public int assistSubRune1 = 0;
    public int assistSubRune2 = 0;

    // 소유한 골드
    public int gold = 50000;

    // 가지고있는 아이템 ID
    public int[] item = new int[6] {0, 0, 0, 0, 0, 0};
    public int accessoryItem = 0;

    public class ItemUndoData
    {
        public string type = "";
        public int ViewNum = 0;
        public int itemID = 0;
        public int price = 0;
        public Stack<ItemUndoData> upgradeList = null;
    }

    private Stack<ItemUndoData> ItemUndoList = new Stack<ItemUndoData>();

    public void ItemUpgrade(bool[] search, int selectedID, int price, bool accessory)
    {
        ItemUndoData iud = new ItemUndoData();
        iud.type = "upgrade";
        iud.upgradeList = new Stack<ItemUndoData>();
        ItemUndoList.Push(iud);

        ItemDelete(search, iud.upgradeList);
        ItemPurchase(selectedID, price, accessory, iud.upgradeList);
    }

    public void ItemPurchase(int selectedID, int price, bool accessory, Stack<ItemUndoData> upgradeList, bool undo = false)
    {
        if (gold < price)
            return;

        if (!accessory)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] == 0)
                {
                    gold -= price;
                    item[i] = selectedID;

                    if (!undo)
                    {
                        ItemUndoData iud = new ItemUndoData();
                        iud.type = "buy";
                        iud.ViewNum = i + 1;
                        iud.itemID = selectedID;
                        iud.price = price;
                        upgradeList.Push(iud);
                    }
                    break;
                }
            }
        }
        else
        {
            if (accessoryItem == 0)
            {
                gold -= price;
                accessoryItem = selectedID;

                if (!undo)
                {
                    ItemUndoData iud = new ItemUndoData();
                    iud.type = "buy";
                    iud.ViewNum = 7;
                    iud.itemID = selectedID;
                    iud.price = price;
                    upgradeList.Push(iud);
                }
            }
        }
    }

    public void ItemPurchase(int selectedID, int price, bool accessory, bool undo = false)
    {
        if (gold < price)
            return;

        if (!accessory)
        {
            for (int i = 0; i < item.Length; i++)
            {
                if (item[i] == 0)
                {
                    gold -= price;
                    item[i] = selectedID;

                    if(!undo)
                    {
                        ItemUndoData iud = new ItemUndoData();
                        iud.type = "buy";
                        iud.ViewNum = i + 1;
                        iud.itemID = selectedID;
                        iud.price = price;
                        ItemUndoList.Push(iud);
                    }
                    break;
                }
            }
        }
        else
        {
            if (accessoryItem == 0)
            {
                gold -= price;
                accessoryItem = selectedID;

                if (!undo)
                {
                    ItemUndoData iud = new ItemUndoData();
                    iud.type = "buy";
                    iud.ViewNum = 7;
                    iud.itemID = selectedID;
                    iud.price = price;
                    ItemUndoList.Push(iud);
                }
            }
        }
    }

    // 되돌리기로 재구매할때는 뷰번호를 받으니까 순서대로 빈칸 찾아 구입하는게 아니라 그 번호에 다시 구입.
    public void ItemPurchase(int selectedViewNum, int selectedID, int price, bool undo = true)
    {
        if (gold < price)
            return;

        if (selectedViewNum != 7)
        {
            if (item[selectedViewNum - 1] == 0)
            {
                gold -= price;
                item[selectedViewNum - 1] = selectedID;
            }
        }
        else
        {
            if (accessoryItem == 0)
            {
                gold -= price;
                accessoryItem = selectedID;
            }
        }
    }

    public void ItemDelete(bool[] search, Stack<ItemUndoData> upgradeList)
    {
        for (int i = 0; i < 6; i++)
        {
            if (search[i] == true)
            {
                ItemSell(i + 1, item[i], 0, upgradeList);
            }
        }
    }

    public void ItemSell(int selectedViewNum, int selectedID, int price, Stack<ItemUndoData> upgradeList, bool undo = false)
    {
        // 장신구가 아니면
        if (selectedViewNum != 7)
        {
            gold += Mathf.RoundToInt(price * 0.7f);
            item[selectedViewNum - 1] = 0;
        }
        else
        {
            gold += Mathf.RoundToInt(price * 0.7f);
            accessoryItem = 0;
        }

        if (!undo)
        {
            ItemUndoData iud = new ItemUndoData();
            iud.type = "sell";
            iud.ViewNum = selectedViewNum;
            iud.itemID = selectedID;
            iud.price = Mathf.RoundToInt(price * 0.7f);
            upgradeList.Push(iud);
        }
    }

    public void ItemSell(int selectedViewNum, int selectedID, int price, bool undo = false)
    {
        // 장신구가 아니면
        if (selectedViewNum != 7)
        {
            gold += Mathf.RoundToInt(price * 0.7f);
            item[selectedViewNum - 1] = 0;
        }
        else
        {
            gold += Mathf.RoundToInt(price * 0.7f);
            accessoryItem = 0;
        }

        if (!undo)
        {
            ItemUndoData iud = new ItemUndoData();
            iud.type = "sell";
            iud.ViewNum = selectedViewNum;
            iud.itemID = selectedID;
            iud.price = Mathf.RoundToInt(price * 0.7f);
            ItemUndoList.Push(iud);
        }
    }

    public bool ItemUndo()
    {
        if(ItemUndoList.Count > 0)
        {
            ItemUndoData lastaction = ItemUndoList.Pop();
            if(lastaction.type == "buy")
            {
                ItemSell(lastaction.ViewNum, lastaction.itemID, lastaction.price, true);
                gold += Mathf.RoundToInt(lastaction.price * 0.3f);

                return true;
            }
            else if(lastaction.type == "sell")
            {
                ItemPurchase(lastaction.ViewNum, lastaction.itemID, lastaction.price, true);
            }
            else if(lastaction.type == "upgrade")
            {
                while(lastaction.upgradeList.Count > 0)
                {
                    ItemUndoData lastupgrade = lastaction.upgradeList.Pop();
                    if (lastupgrade.type == "buy")
                    {
                        ItemSell(lastupgrade.ViewNum, lastupgrade.itemID, lastupgrade.price, true);
                        gold += Mathf.RoundToInt(lastupgrade.price * 0.3f);
                    }
                    else if (lastupgrade.type == "sell")
                    {
                        ItemPurchase(lastupgrade.ViewNum, lastupgrade.itemID, lastupgrade.price, true);
                    }
                }
                return true;
            }
        }
        return false;
    }

    public void ItemUndoListReset()
    {
        ItemUndoList.Clear();
    }

    public void GoldReset()
    {
        gold = 500;
    }

    public void ItemReset()
    {
        for(int i=0; i<6; i++)
        {
            item[i] = 0;
        }
    }
}
