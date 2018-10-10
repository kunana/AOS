using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionBehavior : MonoBehaviour
{
    ChampionData myChampionData = null;
    public ChampionAtk myChampAtk = null;
    public string Team = "";

    private void OnEnable()
    {
        myChampionData = GetComponent<ChampionData>();
        if(PhotonNetwork.player.IsLocal)
        {
            if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
            {
                Team = "Red";
            }
            else
            {
                Team = "Blue";
            }
        }
    }

    public void IamDead(float time = 0)
    {
        Invoke("Dead", time);
    }

    private void Dead()
    {
        myChampAtk.StopAllCoroutines();
        myChampAtk.ResetTarget();
        gameObject.SetActive(false);
    }

    public bool HitMe(float damage = 0)
    {
        bool isDead = false;
        myChampionData.mystat.Hp -= damage;
        if(myChampionData.mystat.Hp < 1)
        {
            myChampionData.mystat.Hp = 0;
            IamDead(0.2f);
            isDead = true;
        }
        return isDead;
    }
}
