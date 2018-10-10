using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class StatClass {
    private static StatClass _instance;
    public static StatClass instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new StatClass();
            }
            return _instance;
        }
    }

    private StatClass()
    {
        SetJson();
    }

    public class Stat
    {
        public int Level = 0;
        public int Exp = 0;
        public int RequireExp = 0;
        public float Hp = 0;
        public float MaxHp = 0;
        public float Mp = 0;
        public float MaxMp = 0;
        public float Attack_Damage = 0;
        public float Ability_Power = 0;
        public float Attack_Speed = 0;
        public float Attack_Def = 0;
        public float Ability_Def = 0;
        public float CoolTime_Decrease = 0;
        public float Critical_Percentage = 0;
        public float Move_Speed = 0;
        public float Attack_Range = 0;
        public int Gold = 0;
        public float first_Create_Time = 0;
        public float Respawn_Time = 0;
        public float Exp_Increase = 0;
        public float Health_Regen = 0;
        public float Mana_Regen = 0;
        public float UP_HP = 0;
        public float UP_MP = 0;
        public float UP_HPRegen = 0;
        public float UP_MPRegen = 0;
        public float UP_AttackDamage = 0;
        public float UP_AttackSpeed = 0;
        public float UP_Def = 0;
        public float UP_MagicDef = 0;

        public Stat ClassCopy()
        {
            return (Stat)this.MemberwiseClone();
        }
    }
    public Dictionary<string, Stat> characterData = new Dictionary<string, Stat>();
    public int[] RequireExp = new int[17]
        {280, 380, 480, 580, 680, 780, 880, 980, 1080, 1180
        , 1280, 1380, 1480, 1580, 1680, 1780, 1880};

    ///<summary>
    ///Json에서 해당 오브젝트를 찾아, 참조할 스탯 클래스 
    ///<para> SetJson("name") </para>
    ///<para>  Jungle_Frog, Jungle_Frog2, Jungle_Blue, Jungle_Blue2, Jungle_BWolf, Jungle_BWolf2 , Jungle_SWolf</para>
    ///<para>Jungle_SWolf2, Jungle_BKalnal, Jungle_BKalnal2, Jungle_SKalnal, Jungle_SKalnal2, Jungle_BGolem2</para>
    ///<para>Jungle_SGolem, Jungle_Crab, Jungle_Red, Jungle_Red2, Jungle_Dragon1, Jungle_Dragon2, Jungle_Dragon3</para>
    ///<para>Jungle_Dragon4, Jungle_ElderDragon, Jungle_Baron,  Minion_Warrior,  Minion_Magician  Minion_Super, Minion_Siege</para>
    ///</summary>
    ///


    public void SetJson()
    {
        string json = System.IO.File.ReadAllText(Application.streamingAssetsPath + "/Json/AOS_Stats.json");
        JObject parse = JObject.Parse(json);

        string[] dataName = new string[] {"Jungle_Frog", "Jungle_Frog2", "Jungle_Blue", "Jungle_Blue2"
            , "Jungle_BWolf", "Jungle_BWolf2", "Jungle_SWolf", "Jungle_SWolf2", "Jungle_BKalnal"
            , "Jungle_BKalnal2", "Jungle_SKalnal", "Jungle_SKalnal2", "Jungle_BGolem2"
            , "Jungle_SGolem", "Jungle_Crab", "Jungle_Red", "Jungle_Red2", "Jungle_Dragon1"
            , "Jungle_Dragon2", "Jungle_Dragon3", "Jungle_Dragon4", "Jungle_ElderDragon"
            ,"Minion_Warrior","Minion_Magician","Minion_Super","Minion_Siege"
            ,"Ashe", "Garen", "Mundo", "Alistar", "Ahri"};

        for (int i = 0; i < dataName.Length; ++i)
        {
            Stat stat = new Stat();

            stat.Level = parse.SelectToken(dataName[i]).SelectToken("Level").Value<int>();
            stat.Exp = parse.SelectToken(dataName[i]).SelectToken("Exp").Value<int>();
            stat.RequireExp = parse.SelectToken(dataName[i]).SelectToken("RequireExp").Value<int>();
            stat.Hp = parse.SelectToken(dataName[i]).SelectToken("Hp").Value<float>();
            stat.MaxHp = parse.SelectToken(dataName[i]).SelectToken("MaxHp").Value<float>();
            stat.Mp = parse.SelectToken(dataName[i]).SelectToken("Mp").Value<float>();
            stat.MaxMp = parse.SelectToken(dataName[i]).SelectToken("MaxMp").Value<float>();
            stat.Attack_Damage = parse.SelectToken(dataName[i]).SelectToken("Attack_Damage").Value<float>();
            stat.Ability_Power = parse.SelectToken(dataName[i]).SelectToken("Ability_Power").Value<float>();
            stat.Attack_Speed = parse.SelectToken(dataName[i]).SelectToken("Attack_Speed").Value<float>();
            stat.Attack_Def = parse.SelectToken(dataName[i]).SelectToken("Attack_Def").Value<float>();
            stat.Ability_Def = parse.SelectToken(dataName[i]).SelectToken("Ability_Def").Value<float>();
            stat.CoolTime_Decrease = parse.SelectToken(dataName[i]).SelectToken("CoolTime_Decrease").Value<float>();
            stat.Critical_Percentage = parse.SelectToken(dataName[i]).SelectToken("Critical_Percentage").Value<float>();
            stat.Move_Speed = parse.SelectToken(dataName[i]).SelectToken("Move_Speed").Value<float>();
            stat.Attack_Range = parse.SelectToken(dataName[i]).SelectToken("Attack_Range").Value<float>();
            stat.Gold = parse.SelectToken(dataName[i]).SelectToken("Gold").Value<int>();
            stat.first_Create_Time = parse.SelectToken(dataName[i]).SelectToken("first_Create_Time").Value<float>();
            stat.Respawn_Time = parse.SelectToken(dataName[i]).SelectToken("Respawn_Time").Value<float>();
            stat.Exp_Increase = parse.SelectToken(dataName[i]).SelectToken("Exp_Increase").Value<float>();
            stat.Health_Regen = parse.SelectToken(dataName[i]).SelectToken("Health_Regen").Value<float>();
            stat.Mana_Regen = parse.SelectToken(dataName[i]).SelectToken("Mana_Regen").Value<float>();
            stat.UP_HP = parse.SelectToken(dataName[i]).SelectToken("UP_HP").Value<float>();
            stat.UP_MP = parse.SelectToken(dataName[i]).SelectToken("UP_MP").Value<float>();
            stat.UP_MagicDef = parse.SelectToken(dataName[i]).SelectToken("UP_MagicDef").Value<float>();
            stat.UP_MPRegen = parse.SelectToken(dataName[i]).SelectToken("UP_MPRegen").Value<float>();
            stat.UP_AttackDamage = parse.SelectToken(dataName[i]).SelectToken("UP_AttackDamage").Value<float>();
            stat.UP_AttackSpeed = parse.SelectToken(dataName[i]).SelectToken("UP_AttackSpeed").Value<float>();
            stat.UP_Def = parse.SelectToken(dataName[i]).SelectToken("UP_Def").Value<float>();
            stat.UP_MagicDef = parse.SelectToken(dataName[i]).SelectToken("UP_MagicDef").Value<float>();

            characterData.Add(dataName[i], stat);
        }
    }
}

