// ?‘ì„±?¼ì : 2018??10??19???¤ì „ 10??41ë¶?
// ?‘ì„±??: ?°ì •ë¬?
// ê°„ë‹¨?¤ëª… : ê²Œì„?ë‚˜ê³?(?„ì´ì½??¤í  kda cs item)??Result?¬ìœ¼ë¡??˜ê²¨ì£¼ëŠ” ?¤ë¸Œ?íŠ¸

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