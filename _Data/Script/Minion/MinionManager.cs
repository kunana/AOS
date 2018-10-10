//using Newtonsoft.Json.Linq;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;


//public class MinionManager : MonoBehaviour
//{
//    public static Dictionary<string, StatClass.Stats> StatsData = new Dictionary<string, StatClass.Stats>();

//    private void Awake()
//    {
//        string json = System.IO.File.ReadAllText(Application.dataPath + "/Json/AOS_Stats.json");
//        JObject parse = JObject.Parse(json);
//        string[] dataName = new string[] {"Jungle_Frog", "Jungle_Frog2", "Jungle_Blue", "Jungle_Blue2"
//            , "Jungle_BWolf", "Jungle_BWolf2", "Jungle_SWolf", "Jungle_SWolf2", "Jungle_BKalnal"
//            , "Jungle_BKalnal2", "Jungle_SKalnal", "Jungle_SKalnal2", "Jungle_BGolem"
//            , "Jungle_SGolem", "Jungle_Crab", "Jungle_Red", "Jungle_Red2", "Jungle_Dragon1"
//            , "Jungle_Dragon2", "Jungle_Dragon3", "Jungle_Dragon4", "Jungle_ElderDragon"
//            ,"Minion_Warrior","Minion_Magician","Minion_Super","Minion_Siege" };
//        for (int i = 0; i < dataName.Length; ++i)
//        {
//            StatClass.Stats stats = new StatClass.Stats();
//            stats.Level = parse.SelectToken(dataName[i]).SelectToken("Level").Value<int>();
//            stats.Exp = parse.SelectToken(dataName[i]).SelectToken("Exp").Value<int>();
//            stats.Hp = parse.SelectToken(dataName[i]).SelectToken("Hp").Value<float>();
//            stats.Hp_Restore_Time = parse.SelectToken(dataName[i]).SelectToken("Hp_Restore_Time").Value<float>();
//            stats.Mp = parse.SelectToken(dataName[i]).SelectToken("Mp").Value<float>();
//            stats.Mp_Restore_Time = parse.SelectToken(dataName[i]).SelectToken("Mp_Restore_Time").Value<float>();
//            stats.Attack_Damage = parse.SelectToken(dataName[i]).SelectToken("Attack_Damage").Value<float>();
//            stats.Ability_Power = parse.SelectToken(dataName[i]).SelectToken("Ability_Power").Value<float>();
//            stats.Attack_Speed = parse.SelectToken(dataName[i]).SelectToken("Attack_Speed").Value<float>();
//            stats.Attack_Def = parse.SelectToken(dataName[i]).SelectToken("Attack_Def").Value<float>();
//            stats.Ability_Def = parse.SelectToken(dataName[i]).SelectToken("Ability_Def").Value<float>();
//            stats.CoolTime_Decrease = parse.SelectToken(dataName[i]).SelectToken("CoolTime_Decrease").Value<float>();
//            stats.Critical_Percentage = parse.SelectToken(dataName[i]).SelectToken("Critical_Percentage").Value<float>();
//            stats.Move_Speed = parse.SelectToken(dataName[i]).SelectToken("Move_Speed").Value<float>();
//            stats.Attack_Range = parse.SelectToken(dataName[i]).SelectToken("Attack_Range").Value<float>();
//            stats.first_Create_Time = parse.SelectToken(dataName[i]).SelectToken("first_Create_Time").Value<float>();
//            stats.Respawn_Time = parse.SelectToken(dataName[i]).SelectToken("Respawn_Time").Value<float>();
//            stats.Exp_Increase = parse.SelectToken(dataName[i]).SelectToken("Exp_Increase").Value<float>();
//            StatsData.Add(dataName[i], stats);
//        }
//    }
//}
