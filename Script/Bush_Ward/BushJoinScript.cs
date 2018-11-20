using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushJoinScript : MonoBehaviour
{
    public bool playerTeamSightOn = false;
    public bool enemyTeamSightOn = false;

    List<GameObject> playerTeamList = new List<GameObject>();
    List<GameObject> enemyTeamList = new List<GameObject>();

    public string playerTeam;
    public string enemyTeam;
    private void Start()
    {
        playerTeam = PhotonNetwork.player.GetTeam().ToString();
        if (playerTeam == "none") playerTeam = "red"; //나중에 지워야할 디버그용 코드
        if (playerTeam.Contains("red"))
        {
            playerTeam = "Red";
            enemyTeam = "Blue";
        }
        else if (playerTeam.Contains("blue"))
        {
            playerTeam = "Blue";
            enemyTeam = "Red";
        }
        else
        {
            //print("BushJoinScript.cs :: 23 :: Warning :: Player Don't Have Team.");
            playerTeam = "Red";
            enemyTeam = "Blue";
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        string team = "";
        //if (other.tag.Equals("Player"))//챔피언일때. 나중에 태그 고쳐야하면 고쳐라.
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            team = other.gameObject.GetComponent<ChampionBehavior>().Team;
        }
        if (other.tag.Equals("Minion"))
        {//미니언일때
            if (other.gameObject.name.Contains("Blue"))
                team = "Blue";
            else if (other.gameObject.name.Contains("Red"))
                team = "Red";
        }
        if (other.tag.Equals("Ward"))
        {
            team = other.gameObject.GetComponent<Ward>().team;
        }
        if (team.Equals(""))
            return;

        //if (team.Equals(playerTeam))
        //{//아군 챔피언이 부쉬에 들어왔다.
        //    if (playerTeamList.Count < 1)
        //    {//기존에는 없었던 거다.
        //        for (int i = 0; i < enemyTeamList.Count; ++i)
        //        {//적에게 부쉬에 우리팀 들어왔지롱 하고 쏴준다.
        //            enemyTeamList[i].GetComponent<FogOfWarEntity>().isInTheBushMyEnemyToo = true;
        //        }
        //    }
        //    playerTeamList.Add(other.gameObject);
        //}
        //else
        //{
        //    FogOfWarEntity f = other.gameObject.GetComponent<FogOfWarEntity>();
        //    f.isInTheBush = true;
        //    //f.Check();
        //    enemyTeamList.Add(other.gameObject);
        //}

        if (team.Equals(playerTeam))
        {//아군이 들어왔다.
            FogOfWarEntity f = other.GetComponent<FogOfWarEntity>();
            if (enemyTeamList.Count > 0)
                f.isInTheBushMyEnemyToo = true;
            if (playerTeamList.Count < 1)
            {//아군이 원래 이 부시에 없었다.
                for (int i = 0; i < enemyTeamList.Count; ++i)//적들에게 '자신의 적들도 부쉬에 있었다'를 켜준다.
                {
                    FogOfWarEntity nowF = enemyTeamList[i].GetComponent<FogOfWarEntity>();
                    nowF.isInTheBushMyEnemyToo = true;
                    nowF.Check();
                }
            }
            playerTeamList.Add(other.gameObject);
            f.isInTheBush = true;
        }
        else if (team.Equals(enemyTeam))
        {//적군이 들어왔다.
            FogOfWarEntity f = other.GetComponent<FogOfWarEntity>();
            if (playerTeamList.Count > 0)
                f.isInTheBushMyEnemyToo = true;
            if (enemyTeamList.Count < 1)
            {//적군이 원래 이 부시에 없었다.
                for (int i = 0; i < playerTeamList.Count; ++i)
                {
                    FogOfWarEntity nowF = playerTeamList[i].GetComponent<FogOfWarEntity>();
                    nowF.isInTheBushMyEnemyToo = true;
                    nowF.Check();
                }
            }
            enemyTeamList.Add(other.gameObject);
            other.GetComponent<FogOfWarEntity>().isInTheBush = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string team = "";
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))//챔피언이다.
        {
            team = other.gameObject.GetComponent<ChampionBehavior>().Team;//팀을 가져온다.
        }
        if (other.tag.Equals("Minion"))
        {
            if (other.gameObject.name.Contains("Blue"))
                team = "Blue";
            else if (other.gameObject.name.Contains("Red"))
                team = "Red";
        }
        if (other.tag.Equals("Ward"))
        {
            team = other.gameObject.GetComponent<Ward>().team;
        }
        if (team.Equals(""))
            return;

        //if (team.Equals(playerTeam))
        //{//챔피언의 팀이 플레이어 팀이랑 같다.
        //    playerTeamList.Remove(other.gameObject);
        //    if (playerTeamList.Count < 1)
        //    {//아군이 이제 부쉬에 없음
        //        for (int i = 0; i < enemyTeamList.Count; ++i)
        //        {//부쉬에 있던 적들에게 아군 나갔다고 알림
        //            enemyTeamList[i].GetComponent<FogOfWarEntity>().isInTheBushMyEnemyToo = false;
        //        }
        //    }
        //}
        //else
        //{//적이 나감. 부쉬에 있다는 신호와 부쉬 안에 아군이 있다는 신호 빼줌.
        //    FogOfWarEntity f = other.gameObject.GetComponent<FogOfWarEntity>();
        //    f.isInTheBush = false;
        //    f.isInTheBushMyEnemyToo = false;
        //    //f.Check();
        //    enemyTeamList.Remove(other.gameObject);
        //}

        if (team.Equals(playerTeam))
        {
            FogOfWarEntity f = other.GetComponent<FogOfWarEntity>();
            f.isInTheBush = false;
            f.isInTheBushMyEnemyToo = false;
            f.isInTheBush = false;
            playerTeamList.Remove(other.gameObject);
            if (playerTeamList.Count < 1)
                for (int i = 0; i < enemyTeamList.Count; ++i)
                {
                    enemyTeamList[i].GetComponent<FogOfWarEntity>().isInTheBushMyEnemyToo = false;
                }
        }
        else if (team.Equals(enemyTeam))
        {
            FogOfWarEntity f = other.GetComponent<FogOfWarEntity>();
            f.isInTheBush = false;
            f.isInTheBushMyEnemyToo = false;
            f.isInTheBush = false;
            enemyTeamList.Remove(other.gameObject);
            if (enemyTeamList.Count < 1)
                for (int i = 0; i < playerTeamList.Count; ++i)
                    playerTeamList[i].GetComponent<FogOfWarEntity>().isInTheBushMyEnemyToo = false;
        }
    }
}