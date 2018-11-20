using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MundoW : MonoBehaviour
{
    public MundoSkill mySkill;
    public List<GameObject> EnemyList = new List<GameObject>();
    public Stack<GameObject> EnemyDeleteStack = new Stack<GameObject>();
    float damagetime = 0.5f;
    AudioSource audio;
    private SystemMessage sysmsg;

    void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Contains("InGame"))
        {
            if (!sysmsg)
                sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        }
    }
    private void OnEnable()
    {
        EnemyList.Clear();
        damagetime = 0.5f;
        audio = transform.parent.GetComponentInChildren<AudioSource>();
        if (audio != null)
        {
            audio.loop = true;
            audio.clip = ChampionSound.instance.Mundo_W;
            audio.Play();
        }
    }

    private void Update()
    {
        damagetime -= Time.deltaTime;
        if (damagetime <= 0)
        {
            damagetime = 0.5f;
            for (int i = 0; i < EnemyList.Count; ++i)
            {
                if (EnemyList[i].Equals(mySkill.gameObject))
                    continue;
                float damage = mySkill.skillData.wDamage[mySkill.TheChampionData.skill_W - 1]
+ mySkill.Acalculate(mySkill.skillData.wAstat, mySkill.skillData.wAvalue);
                if (EnemyList[i].layer.Equals(LayerMask.NameToLayer("Champion")))
                {
                    ChampionBehavior cB = EnemyList[i].GetComponent<ChampionBehavior>();
                    if (cB.Team != mySkill.TheChampionBehaviour.Team)
                    {
                        if (cB != null)
                        {
                            if (cB.HitMe(damage, "AP", mySkill.gameObject, mySkill.name))
                            {
                                mySkill.TheChampionAtk.ResetTarget();
                                //if (!sysmsg)
                                //    sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
                                //sysmsg.sendKillmsg("mundo", EnemyList[i].GetComponent<ChampionData>().ChampionName, mySkill.TheChampionBehaviour.Team.ToString());
                                // 스킬쏜애 주인이 나면 킬올리자
                                if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                                {
                                    mySkill.TheChampionData.Kill_CS_Gold_Exp(EnemyList[i].gameObject.name, 0, EnemyList[i].transform.position);
                                }
                                EnemyDeleteStack.Push(EnemyList[i]);
                            }
                            if (cB.myChampionData.totalstat.Hp <= 0)
                                EnemyDeleteStack.Push(EnemyList[i]);
                        }
                    }
                }
                else if (EnemyList[i].layer.Equals(LayerMask.NameToLayer("Monster")))
                {
                    MonsterBehaviour mB = EnemyList[i].GetComponent<MonsterBehaviour>();
                    if (mB != null)
                    {
                        if (mB.HitMe(damage, "AP", mySkill.gameObject))
                        {
                            mySkill.TheChampionAtk.ResetTarget();

                            //// 스킬쏜애 주인이 나면 킬올리자
                            //if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                            //{
                            //    mySkill.TheChampionData.Kill_CS_Gold_Exp(EnemyList[i].gameObject.name, 3, EnemyList[i].transform.position);
                            //}
                            EnemyDeleteStack.Push(EnemyList[i]);
                        }
                        if (mB.stat.Hp <= 0)
                            EnemyDeleteStack.Push(EnemyList[i]);
                    }
                }
                else if (EnemyList[i].tag.Equals("Minion"))
                {
                    MinionBehavior mB = EnemyList[i].GetComponent<MinionBehavior>();
                    if (!EnemyList[i].name.Contains(mySkill.TheChampionBehaviour.Team))
                    {
                        if (mB != null)
                        {
                            if (mB.HitMe(damage, "AP", mySkill.gameObject))
                            {
                                mySkill.TheChampionAtk.ResetTarget();

                                // 스킬쏜애 주인이 나면 킬올리자
                                if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                                {
                                    mySkill.TheChampionData.Kill_CS_Gold_Exp(EnemyList[i].gameObject.name, 1, EnemyList[i].transform.position);
                                }
                                EnemyDeleteStack.Push(EnemyList[i]);
                            }
                            if (mB.stat.Hp <= 0)
                                EnemyDeleteStack.Push(EnemyList[i]);
                        }
                    }
                }
                while (EnemyDeleteStack.Count > 0)
                {
                    GameObject g = EnemyDeleteStack.Pop();
                    if (EnemyList.Contains(g))
                        EnemyList.Remove(g);
                }
            }
        }
    }

    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        bool isT = false;
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
            isT = true;
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
            isT = true;
        else if (other.tag.Equals("Minion"))
            isT = true;

        if (isT)
            if (!EnemyList.Contains(other.gameObject))
                EnemyList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (EnemyList.Contains(other.gameObject))
            EnemyList.Remove(other.gameObject);
    }
    void OnDisable()
    {
        if (audio != null)
        {
            audio.loop = false;
            audio.clip = null;
            audio.Stop();
        }
    }
}