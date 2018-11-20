// ?�성?�자 : 2018??10??19???�전 10??41�?
// ?�성??: ?�정�?
// 간단?�명 : 게임?�나�?(?�이�??�펠 kda cs item)??Result?�으�??�겨주는 ?�브?�트

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : Singleton<ResultManager>
{
    public class ResultData
    {
        public bool me = false;
        public string championName;
        public string nickName;
        public int level;
        public int kill;
        public int death;
        public int assist;
        public int cs;
        public int[] items = new int[6];
        public int accessoryItem;

        public ResultData ClassCopy()
        {
            return (ResultData)this.MemberwiseClone();
        }
    }

    public List<ResultData> blueTeamResults = new List<ResultData>();
    public List<ResultData> redTeamResults = new List<ResultData>();

    public string result = "";

    public void ListReset()
    {
        blueTeamResults.Clear();
        redTeamResults.Clear();
    }

    public void ResultInput(ResultData result, string team)
    {
        ResultData newResult = result.ClassCopy();
        if(team.Equals("red"))
        {
            redTeamResults.Add(newResult);
        }
        else if(team.Equals("blue"))
        {
            blueTeamResults.Add(newResult);
        }
    }
}