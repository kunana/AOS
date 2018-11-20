using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : Singleton<PlayerData>
{

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

    private ChampionData myChampionData;

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
    public int gold = 1000;

    // 가지고있는 아이템 ID
    public int[] item = new int[6] { 0, 0, 0, 0, 0, 0 };
    public int accessoryItem = 0;

    // 구매가능 상태체크
    public bool purchaseState = false;

    // 죽어있는지 체크
    public bool isDead = false;

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
        if (!purchaseState)
            return;

        if (gold < price)
            return;

        ItemUndoData iud = new ItemUndoData();
        iud.type = "upgrade";
        iud.upgradeList = new Stack<ItemUndoData>();
        ItemUndoList.Push(iud);

        ItemDelete(search, iud.upgradeList);
        ItemPurchase(selectedID, price, accessory, iud.upgradeList);
    }

    public void ItemPurchase(int selectedID, int price, bool accessory, Stack<ItemUndoData> upgradeList, bool undo = false)
    {
        if (!purchaseState)
            return;

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

        ItemUpdate();
    }

    public void ItemPurchase(int selectedID, int price, bool accessory, bool undo = false)
    {
        if (!purchaseState)
            return;

        if (gold < price)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
            return;
        }

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
                        ItemUndoList.Push(iud);
                        SoundManager.instance.PlaySound(SoundManager.instance.Shop_Buy);
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
                    SoundManager.instance.PlaySound(SoundManager.instance.Shop_Buy);
                }
            }
        }

        ItemUpdate();
    }

    // 되돌리기로 재구매할때는 뷰번호를 받으니까 순서대로 빈칸 찾아 구입하는게 아니라 그 번호에 다시 구입.
    public void ItemPurchase(int selectedViewNum, int selectedID, int price, bool undo = true)
    {
        if (!purchaseState)
            return;

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

        ItemUpdate();
    }

    public void ItemDelete(bool[] search, Stack<ItemUndoData> upgradeList)
    {
        if (!purchaseState)
            return;

        for (int i = 0; i < 6; i++)
        {
            if (search[i] == true)
            {
                ItemSell(i + 1, item[i], 0, upgradeList);
            }
        }

        ItemUpdate();
    }

    public void ItemSell(int selectedViewNum, int selectedID, int price, Stack<ItemUndoData> upgradeList, bool undo = false)
    {
        if (!purchaseState)
            return;

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

        ItemUpdate();
    }

    public void ItemSell(int selectedViewNum, int selectedID, int price, bool undo = false)
    {
        if (!purchaseState)
            return;

        // 되돌리기로 파는거면
        if (undo)
        {
            if (selectedViewNum != 7)
            {
                gold += price;
                item[selectedViewNum - 1] = 0;
            }
            else
            {
                gold += price;
                accessoryItem = 0;
            }
            return;
        }

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

        ItemUpdate();
    }

    public bool ItemUndo()
    {
        if (!purchaseState)
            return false;

        if (ItemUndoList.Count > 0)
        {
            ItemUndoData lastaction = ItemUndoList.Pop();
            if (lastaction.type == "buy")
            {
                ItemSell(lastaction.ViewNum, lastaction.itemID, lastaction.price, true);
                //gold += Mathf.RoundToInt(lastaction.price * 0.3f);

                ItemUpdate();
                return true;
            }
            else if (lastaction.type == "sell")
            {
                ItemPurchase(lastaction.ViewNum, lastaction.itemID, lastaction.price, true);
            }
            else if (lastaction.type == "upgrade")
            {
                while (lastaction.upgradeList.Count > 0)
                {
                    ItemUndoData lastupgrade = lastaction.upgradeList.Pop();
                    if (lastupgrade.type == "buy")
                    {
                        ItemSell(lastupgrade.ViewNum, lastupgrade.itemID, lastupgrade.price, true);
                        //gold += Mathf.RoundToInt(lastupgrade.price * 0.3f);
                    }
                    else if (lastupgrade.type == "sell")
                    {
                        ItemPurchase(lastupgrade.ViewNum, lastupgrade.itemID, lastupgrade.price, true);
                    }
                }
                ItemUpdate();
                return true;
            }
        }
        ItemUpdate();
        return false;
    }

    public void ItemUndoListReset()
    {
        ItemUndoList.Clear();
    }

    public void GoldReset()
    {
        gold = 1000;
        isDead = false;
    }

    public void ItemReset()
    {
        for (int i = 0; i < 6; i++)
        {
            item[i] = 0;
        }
        accessoryItem = 0;
    }

    public void ItemUpdate()
    {
        if (myChampionData == null)
            myChampionData = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>();

        myChampionData.ItemUpdate(item, accessoryItem);
    }

    /// <summary>
    /// 킬냈을때 골드 올려주는 함수임
    /// </summary>
    /// <param name="name">오브젝트 이름 넣어라</param>
    /// <param name="type">0 : 챔피언 / 1 : 미니언 / 2 : 타워 / 3 : 정글몹(디폴트)</param>
    public int KillGold(string name, int type = 3)
    {
        if(myChampionData == null)
            myChampionData = GameObject.FindGameObjectWithTag("Player").GetComponent<ChampionData>();

        // 챔피언이면 300골드받음
        if (type == 0)
        {
            gold += 300;
            return 300;
        }
        // 미니언이면
        else if(type == 1)
        {
            // 미니언 따라 골드 증가
            if (name.Contains("Melee"))
            {
                gold += 21;
                return 21;
            }
            else if (name.Contains("Magician"))
            {
                gold += 14;
                return 14;
            }
            else if (name.Contains("Siege") || name.Contains("Super"))
            {
                gold += 70;
                return 70;
            }
        }
        // 타워면 팀전체 100골드 고정(억제기는 50인데 그냥 100줌)
        else if(type == 2)
        {
            myChampionData.photonView.RPC("GlobalGold", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 100);
            return 100;
        }
        // 정글이면
        else if(type == 3)
        { 
            if (name.Contains("Raptor_Big")) // 칼날부리
            {
                gold += 62;
                return 62;
            }
            else if (name.Contains("Raptor_Small")) // 칼날부리 작은애
            {
                gold += 10;
                return 10;
            }
            else if (name.Contains("Wolf")) // 늑대
            {
                gold += 68;
                return 68;
            }
            else if (name.Contains("Wolf_Small")) // 늑대 작은애
            {
                gold += 16;
                return 16;
            }
            else if (name.Contains("Krug_Big")) // 작골
            {
                gold += 70;
                return 70;
            }
            else if (name.Contains("Krug_Small")) // 작골 작은애
            {
                gold += 10;
                return 10;
            }
            else if (name.Contains("Gromp")) // 두꺼비
            {
                gold += 86;
                return 86;
            }
            // 블루 레드 용 전령
            else if (name.Contains("B_Sentinel") || name.Contains("R_Sentinel") || name.Contains("Dragon") || name.Contains("Rift_Herald"))
            {
                // 용은 원래 25골드인데 따로 버프가없다보니 100골드로 올림
                gold += 100;
                return 100;
            }
            else if (name.Contains("Baron")) // 내셔남작
            {
                myChampionData.photonView.RPC("GlobalGold", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 300);
                return 300;
            }
        }
        return 0;
    }
}
