using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MundoQ : MonoBehaviour
{
    public bool firstAtk = true;
    public float distance = 20;
    public MundoSkill mySkill;
    private SystemMessage sysmsg;

    void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Contains("InGame"))
        {
            if (!sysmsg)
                sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        }
    }
    public void SkillOn(Vector3 dest)
    {
        firstAtk = true;
        //float length = Vector3.Distance(dest, transform.position);
        //if (length > distance)
        //    length = distance;
        transform.position = mySkill.transform.position;
        Vector3 arrow = (dest - transform.position).normalized;
        ActiveFalse(0.5f);
        transform.DOMove(transform.position + (arrow * distance), 0.5f);
    }

    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
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
        if (firstAtk)
        {
            bool trig = false;
            if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                if (other.gameObject.Equals(mySkill.gameObject))
                    return;
                ChampionBehavior cB = other.GetComponent<ChampionBehavior>();
                if (cB.Team != mySkill.TheChampionBehaviour.Team)
                {
                    trig = true;
                    float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
    + mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
                    if (cB != null)
                    {
                        //int viewID = mB.GetComponent<PhotonView>().viewID;
                        //mySkill.HitRPC(viewID, damage, "AP");
                        int x2 = 1;
                        ChampionSound.instance.PlayOtherFx(cB.GetComponentInChildren<AudioSource>(), ChampionSound.instance.Mundo_Q_Hit);
                        if (cB.HitMe(damage, "AP", mySkill.gameObject, mySkill.name))
                        {
                            x2 = 2;
                            mySkill.TheChampionAtk.ResetTarget();
                            if (!sysmsg)
                                sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
                            sysmsg.sendKillmsg("mundo", other.GetComponent<ChampionData>().ChampionName, mySkill.TheChampionBehaviour.Team.ToString());
                            // 스킬쏜애 주인이 나면 킬올리자
                            if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                            {
                                mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 0, other.transform.position);
                            }
                        }
                        mySkill.Heal(damage * x2);
                    }
                }
            }
            else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
            {
                MonsterBehaviour mB = other.GetComponent<MonsterBehaviour>();
                float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
+ mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
                if (mB != null)
                {
                    trig = true;
                    //int viewID = mB.GetComponent<PhotonView>().viewID;
                    //mySkill.HitRPC(viewID, damage, "AP");
                    int x2 = 1;
                    //몬스터 오디오 처리하면 켤것
                    //ChampionSound.instance.PlayOtherFx(mB.GetComponentInChildren<AudioSource>(), ChampionSound.instance.Mundo_Q_Hit); 
                    if (mB.HitMe(damage, "AP", mySkill.gameObject))
                    {
                        x2 = 2;
                        mySkill.TheChampionAtk.ResetTarget();

                        //// 스킬쏜애 주인이 나면 킬올리자
                        //if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        //{
                        //    mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 3, other.transform.position);

                        //}
                    }
                    mySkill.Heal(damage * x2);
                }
            }
            else if (other.tag.Equals("Minion"))
            {
                MinionBehavior mB = other.GetComponent<MinionBehavior>();
                if (!other.name.Contains(mySkill.TheChampionBehaviour.Team))
                {
                    trig = true;
                    float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
    + mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
                    if (mB != null)
                    {
                        //int viewID = mB.GetComponent<PhotonView>().viewID;
                        //mySkill.HitRPC(viewID, damage, "AP");
                        ChampionSound.instance.PlayOtherFx(mB.Audio, ChampionSound.instance.Mundo_Q_Hit);
                        int x2 = 1;
                        if (mB.HitMe(damage, "AP", mySkill.gameObject))
                        {
                            x2 = 2;
                            mySkill.TheChampionAtk.ResetTarget();

                            // 스킬쏜애 주인이 나면 킬올리자
                            if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                            {
                                mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 1, other.transform.position);
                            }
                        }
                        mySkill.Heal(damage * x2);
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