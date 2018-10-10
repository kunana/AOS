using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopItem : Singleton<ShopItem> {

    public class Item
    {
        // 기본정보
        public int id = 0;
        public string name = "";
        public int price = 0;
        public string icon_name = "";

        // 하위템
        public int subitem_id1 = 0;
        public int subitem_id2 = 0;
        public int subitem_id3 = 0;

        // 액티브여부
        public bool active = false;
        public int active_cooldown = 0;

        // 스탯
        public int attack_damage = 0;
        public int attack_speed = 0;
        public int critical_percent = 0;
        public int life_steal = 0;

        public int ability_power = 0;
        public int mana = 0;
        public int mana_regen = 0;
        public int cooldown_reduce = 0;

        public int armor = 0;
        public int magic_resist = 0;
        public int health = 0;
        public int health_regen = 0;

        public int movement_speed = 0;

        // 상점 분류용
        public bool consumable = false;
        public bool boots = false;
        public bool accessory = false;

        public string effect_kind = "";
        public string effect_description = "";
        public string additional_kind = "";
        public string additional_description = "";

        public Item ClassCopy()
        {
            return (Item)this.MemberwiseClone();
        }
    }

    public Dictionary<int, Item> itemlist = new Dictionary<int, Item>();
    public List<Item> sorted_itemlist = new List<Item>();
    public List<Item> search_itemlist = new List<Item>();
    public List<Item> making_itemlist = new List<Item>();

    public void readItem()
    {
        itemlist.Clear();

        string fileName = Application.streamingAssetsPath;
        fileName = Path.Combine(fileName, "csv/itemlist.csv");
        if (File.Exists(fileName) == false)
            return;

        FileStream fStream = new FileStream(fileName, FileMode.Open);
        if (fStream != null)
        {
            StreamReader streamReader = new StreamReader(fStream);
            string itemcsv = streamReader.ReadToEnd();
            string[] lines = itemcsv.Split("\r\n".ToCharArray());

            foreach (string line in lines)
            {
                Item newitem = new Item();

                if (line.Length > 0)
                {
                    string[] data = line.Split(',');

                    newitem.id = int.Parse(data[0]);
                    newitem.name = data[1];
                    newitem.price = int.Parse(data[2]);
                    newitem.icon_name = data[3];

                    if(data[4] != string.Empty)
                        newitem.subitem_id1 = int.Parse(data[4]);
                    if(data[5] != string.Empty)
                        newitem.subitem_id2 = int.Parse(data[5]);
                    if(data[6] != string.Empty)
                        newitem.subitem_id3 = int.Parse(data[6]);

                    if (data[7] != string.Empty)
                        newitem.active = true;
                    if (data[8] != string.Empty)
                        newitem.active_cooldown = int.Parse(data[8]);

                    if (data[9] != string.Empty)
                        newitem.attack_damage = int.Parse(data[9]);
                    if (data[10] != string.Empty)
                        newitem.attack_speed = int.Parse(data[10]);
                    if (data[11] != string.Empty)
                        newitem.critical_percent = int.Parse(data[11]);
                    if (data[12] != string.Empty)
                        newitem.life_steal = int.Parse(data[12]);

                    if (data[13] != string.Empty)
                        newitem.ability_power = int.Parse(data[13]);
                    if (data[14] != string.Empty)
                        newitem.mana = int.Parse(data[14]);
                    if (data[15] != string.Empty)
                        newitem.mana_regen = int.Parse(data[15]);
                    if (data[16] != string.Empty)
                        newitem.cooldown_reduce = int.Parse(data[16]);

                    if (data[17] != string.Empty)
                        newitem.armor = int.Parse(data[17]);
                    if (data[18] != string.Empty)
                        newitem.magic_resist = int.Parse(data[18]);
                    if (data[19] != string.Empty)
                        newitem.health = int.Parse(data[19]);
                    if (data[20] != string.Empty)
                        newitem.health_regen = int.Parse(data[20]);

                    if (data[21] != string.Empty)
                        newitem.movement_speed = int.Parse(data[21]);

                    if (data[22] != string.Empty)
                        newitem.consumable = true;
                    if (data[23] != string.Empty)
                        newitem.boots = true;
                    if (data[24] != string.Empty)
                        newitem.accessory = true;

                    if (data[25] != string.Empty)
                        newitem.effect_kind = data[25];
                    if (data[26] != string.Empty)
                        newitem.effect_description = data[26];
                    if (data[27] != string.Empty)
                        newitem.additional_kind = data[27];
                    if (data[28] != string.Empty)
                        newitem.additional_description = data[28];

                    itemlist[newitem.id] = newitem;
                }
            }
            streamReader.Close();
            fStream.Close();
        }
    }
}
