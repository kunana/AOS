using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsheW : MonoBehaviour
{
    public bool firstAtk = true;
    public AsheSkill mySkill;
    public List<GameObject> a = new List<GameObject>();
    private SystemMessage sysmsg;

    void OnLevelWasLoaded(int level)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Contains("InGame"))
        {
            if (!sysmsg)
                sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        }
    }

    public void SkillOn(Vector3 dest)
    {
        firstAtk = true;
        transform.position = mySkill.transform.position;
        ActiveFalse(0.5f);
        transform.DOMove(dest, 0.5f);
    }

    public void ActiveFalse(float time)
    {
        Invoke("_ActiveFalse", time);
    }

    private void _ActiveFalse()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        a.Add(other.gameObject);
        if (firstAtk)
        {
            bool trig = false;
            if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                if (!other.gameObject.Equals(mySkill.gameObject))
                {
                    ChampionBehavior mB = other.GetComponent<ChampionBehavior>();
                    if (mB.Team != mySkill.TheChampionBehaviour.Team)
                    {
                        trig = true;
                        float damage = mySkill.skillData.wDamage[mySkill.TheChampionData.skill_W - 1]
        + mySkill.Acalculate(mySkill.skillData.wAstat, mySkill.skillData.wAvalue);
                        if (mB != null)
                        {
                            if (mB.HitMe(damage, "AD", mySkill.gameObject, mySkill.name))
                            {
                                mySkill.TheChampionAtk.ResetTarget();
                                if (!sysmsg)
                                    sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
                                //sysmsg.sendKillmsg("ashe", other.GetComponent<ChampionData>().ChampionName, mySkill.TheChampionBehaviour.Team.ToString());
                                // 스킬쏜애 주인이 나면 킬올리자
                                if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                                {
                                    mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 0, other.transform.position);
                                }
                            }
                        }
                    }
                }
            }
            else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
            {
                MonsterBehaviour mB = other.GetComponent<MonsterBehaviour>();
                float damage = mySkill.skillData.wDamage[mySkill.TheChampionData.skill_W - 1]
+ mySkill.Acalculate(mySkill.skillData.wAstat, mySkill.skillData.wAvalue);
                if (mB != null)
                {
                    trig = true;
                    if (mB.HitMe(damage, "AD", mySkill.gameObject))
                    {
                        mySkill.TheChampionAtk.ResetTarget();
                        //// 스킬쏜애 주인이 나면 킬올리자
                        //if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        //{
                        //   //mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 3, other.transform.position);
                        //}
                    }
                }
            }
            else if (other.tag.Equals("Minion"))
            {
                MinionBehavior mB = other.GetComponent<MinionBehavior>();
                if (!other.name.Contains(mySkill.TheChampionBehaviour.Team))
                {
                    trig = true;
                    float damage = mySkill.skillData.wDamage[mySkill.TheChampionData.skill_W - 1]
    + mySkill.Acalculate(mySkill.skillData.wAstat, mySkill.skillData.wAvalue);
                    if (mB != null)
                    {
                        if (mB.HitMe(damage, "AD", mySkill.gameObject))
                        {
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

            if (trig)
            {
                gameObject.SetActive(false);
                firstAtk = false;
            }
        }
    }
}