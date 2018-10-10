using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionBehavior : MonoBehaviour
{
    ChampionData myChampionData = null;
    public ChampionAtk myChampAtk = null;
    public string Team = "Red";

    private void OnEnable()
    {
        myChampionData = GetComponent<ChampionData>();

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

    public bool HitMe(float damage = 0, string atkType = "AD", GameObject atker = null) // AD, AP, FD(고정 데미지 = Fixed damage)
    {
        bool isDead = false;
        if (atkType.Equals("AD"))
        {
            damage = (damage * 100f) / (100f + myChampionData.mystat.Attack_Def);
        }
        else if (atkType.Equals("AP"))
        {
            damage = (damage * 100f) / (100f + myChampionData.mystat.Ability_Def);
        }
        myChampionData.mystat.Hp -= damage;
        if (myChampionData.mystat.Hp < 1)
        {
            myChampionData.mystat.Hp = 0;
            IamDead(0.2f);
            isDead = true;
        }
        if (atker != null)
        {//공격한 사람이 지정되어있다(챔피언이나 미니언이 뚜까팬경우)
            if (atker.tag.Equals("ChampionAtkRange"))
            {//챔피언이냐
                Collider[] cols = Physics.OverlapSphere(transform.position, 10);
                for (int i = 0; i < cols.Length; ++i)//지구의 모든 아군 미니언들아 나에게 힘을 줘
                {
                    if (cols[i].tag.Equals("Minion"))
                    {
                        if (cols[i].name.Contains(Team))
                        {//원기옥대신다구리퓽퓽
                            cols[i].GetComponent<MinionBehavior>().minAtk.SetTarget(atker);
                        }
                    }
                }
            }
            else if (atker.tag.Equals("MinionAtkRange"))
            {//미니언이냐
                Collider[] cols = Physics.OverlapSphere(transform.position, 10);
                for (int i = 0; i < cols.Length; ++i)
                {
                    if (cols[i].tag.Equals("Minion"))
                    {
                        if (cols[i].name.Contains(Team))
                        {//너도다구리퓽퓽
                            cols[i].GetComponent<MinionBehavior>().minAtk.SetTarget(atker);
                        }
                    }
                }
            }
        }
        return isDead;
    }
}