using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsheR : MonoBehaviour
{
    public Vector3 shootVec;
    public bool firstAtk = true;
    public AsheSkill mySkill;
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
        shootVec = transform.position;
        //float length = Vector3.Distance(dest, transform.position);
        Vector3 realDest = (Vector3.Normalize(dest - shootVec) * 400) + shootVec;
        float time = 400 * 0.04f;
        ActiveFalse(time);
        transform.DOMove(realDest, time);
    }
    public void ActiveFalse(float time)
    {
        Invoke("_ActiveFalse", time);
    }
    private void _ActiveFalse()
    {
        gameObject.SetActive(false);
    }
    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }

    private void Update()
    {
        if (transform.position.x < -10 || transform.position.x > 285 || transform.position.z < -10 || transform.position.z > 285)
        {
            gameObject.SetActive(false);
        }
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
                    float damage = mySkill.skillData.rDamage[mySkill.TheChampionData.skill_R - 1]
    + mySkill.Acalculate(mySkill.skillData.rAstat, mySkill.skillData.rAvalue);
                    if (cB != null)
                    {
                        if (cB.HitMe(damage, "AP", mySkill.gameObject, mySkill.name))
                        {
                            mySkill.TheChampionAtk.ResetTarget();
                        }
                        cB.myChampAtk.PauseAtk(3.5f, true);
                        cB.myChampAtk.StunEffectToggle(true, 0);
                        cB.myChampAtk.StunEffectToggle(false, 3.5f);
                        //cB.myChampAtk.PauseMove(3.5f * 10f);
                        Collider[] col = Physics.OverlapSphere(cB.transform.position, 12);
                        foreach (Collider c in col)
                        {
                            if (c.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                            {
                                ChampionBehavior cCB = c.GetComponent<ChampionBehavior>();
                                if (cCB != null)
                                    if (cCB.Team != mySkill.TheChampionBehaviour.Team)
                                    {
                                        //if(cCB.name.Contains("asd"))
                                        //{
                                        //    print("");

                                        if (cCB.HitMe(damage / 2f, "AP", mySkill.gameObject, mySkill.name))
                                        {
                                            // 스킬쏜애 주인이 나면 킬올리자
                                            if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                                            {
                                                mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 0, other.transform.position);
                                                //if (!sysmsg)
                                                //    sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
                                                //sysmsg.sendKillmsg("ashe", other.GetComponent<ChampionData>().ChampionName, mySkill.TheChampionBehaviour.Team.ToString());
                                            }
                                        }
                                    }
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