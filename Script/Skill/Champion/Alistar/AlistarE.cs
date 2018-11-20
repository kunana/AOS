using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlistarE : MonoBehaviour
{
    public AlistarSkill mySkill;
    private SystemMessage sysmsg;

    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Minion"))
        {
            MinionBehavior mB = other.GetComponent<MinionBehavior>();
            if (!other.gameObject.name.Contains(mySkill.TheChampionBehaviour.Team))
            {
                float damage = (mySkill.skillData.eDamage[mySkill.TheChampionData.skill_E - 1]
                + mySkill.Acalculate(mySkill.skillData.eAstat, mySkill.skillData.eAvalue)) / 10f;
                if (mB != null)
                {
                    int viewID = mB.GetComponent<PhotonView>().viewID;
                    //mySkill.HitRPC(viewID, damage, "AP");
                    if (mB.HitMe(damage, "AP", mySkill.gameObject))
                    {
                        //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                        mySkill.TheChampionAtk.ResetTarget();

                        // 스킬쏜애 주인이 나면 킬올리자
                        if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        {
                            mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 1, other.transform.position);
                        }
                    }
                }
            }
        }
        //else if(other.tag.Equals("Player"))
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            ChampionBehavior cB = other.GetComponent<ChampionBehavior>();
            if (cB.Team != mySkill.TheChampionBehaviour.Team)
            {
                float damage = (mySkill.skillData.eDamage[mySkill.TheChampionData.skill_E - 1]
                + mySkill.Acalculate(mySkill.skillData.eAstat, mySkill.skillData.eAvalue)) / 10f;
                if (cB != null)
                {
                    int viewID = cB.GetComponent<PhotonView>().viewID;
                    //mySkill.HitRPC(viewID, damage, "AP");
                    if (cB.HitMe(damage, "AP", mySkill.gameObject, mySkill.name))
                    {
                        mySkill.TheChampionAtk.ResetTarget();
                        if (!sysmsg)
                            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
                        sysmsg.sendKillmsg("alistar", other.GetComponent<ChampionData>().ChampionName, mySkill.TheChampionBehaviour.Team.ToString());
                        // 스킬쏜애 주인이 나면 킬올리자
                        if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        {
                            mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 0, other.transform.position);
                        }
                    }
                }
            }
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            MonsterBehaviour mB = other.GetComponent<MonsterBehaviour>();

            float damage = (mySkill.skillData.eDamage[mySkill.TheChampionData.skill_E - 1]
            + mySkill.Acalculate(mySkill.skillData.eAstat, mySkill.skillData.eAvalue)) / 10f;
            if (mB != null)
            {
                int viewID = mB.GetComponent<PhotonView>().viewID;
                //mySkill.HitRPC(viewID, damage, "AP");
                if (mB.HitMe(damage, "AP", mySkill.gameObject))
                {
                    mySkill.TheChampionAtk.ResetTarget();

                    //// 스킬쏜애 주인이 나면 킬올리자
                    //if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                    //{
                    //    mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 3, other.transform.position);
                    //}
                }
            }
        }
    }
}