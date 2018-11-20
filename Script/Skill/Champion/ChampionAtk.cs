using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class ChampionAtk : MonoBehaviour
{
    public bool isTargetting = false;
    public AIPath TheAIPath;
    AIDestinationSetter TheAIDest;
    public float AtkRange = 3f;
    public Coroutine AtkCoroutine;
    public bool isAtkPause = false;
    public GameObject myChamp = null;
    public GameObject AStarTargetObj = null;
    public GameObject AtkTargetObj = null;
    PlayerMouse ThePlayerMouse;
    ChampionData myChampionData = null;
    public Vector3 tempVec1, tempVec2;
    public bool willAtkAround = false;
    public List<GameObject> enemiesList;
    public ChampionBehavior myChampBehav;
    public bool isWarding = false;
    public int wardAmount = 1;
    public float wardMadeCooldown = 240f;
    const float wardMadeMinTime = 120f;
    const float wardMadeMaxTime = 240f;
    const float wardMadeTermTime = 120f;
    public float atkDelayTime = 1f;
    public bool isAtkDelayTime = false;
    ChampionAnimation myChampionAnimation;
    public string champname; //스킬이 평타 데미지를 변화시키거나 할 때 사용
    public AsheSkill asheSkill = null;
    public bool isAshe = false;
    public GameObject stunParticle = null;
    public bool isStun = false;

    public string skillKey = ""; //스킬이 평타 데미지를 변화시키거나 할 때 사용
    public int skillKeyNum = 0; // 스킬 평타 관련이 1대가 아닌 여러 대인데 시간 제한도 아니고 횟수 제한인 경우
    private SystemMessage sysmsg;

    public bool isPushing = false;
    Tweener pushTween = null;

    private void Awake()
    {
        if (myChamp == null)
            myChamp = transform.parent.gameObject;
        myChampionData = myChamp.GetComponent<ChampionData>();
        TheAIPath = myChamp.GetComponent<AIPath>();
        TheAIDest = myChamp.GetComponent<AIDestinationSetter>();
        ThePlayerMouse = myChamp.GetComponent<PlayerMouse>();
        AStarTargetObj = ThePlayerMouse.myTarget;
        enemiesList = new List<GameObject>();
        myChampBehav = myChamp.GetComponent<ChampionBehavior>();
        myChampionAnimation = myChamp.GetComponent<ChampionAnimation>();
        champname = PlayerData.Instance.championName;
        if (myChamp.transform.parent.name.Contains("Ashe"))
        {
            asheSkill = myChamp.GetComponent<AsheSkill>();
            if (asheSkill != null)
                isAshe = true;
        }


    }
    void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
    }

    public void Stop()
    {
        willAtkAround = false;
        isTargetting = false;
        if (AtkCoroutine != null)
        {
            myChampionAnimation.AttackAnimation(false);
            StopCoroutine(AtkCoroutine);
        }
        AtkTargetObj = null;
        isWarding = false;
        myChampionData.playerSkill.TheSplatManager.Cancel();
        myChampionData.playerSkill.CancelSkill();
        StopOnlyMove();
    }

    public void StopOnlyMove()
    {
        TheAIPath.canMove = false;
        TheAIPath.canSearch = false;
        AStarTargetObj.transform.position = myChamp.transform.position;

    }

    private void Update()
    {
        if (atkDelayTime > 0)
        {
            atkDelayTime -= Time.deltaTime;
            if (!isAtkDelayTime)
                isAtkDelayTime = true;
        }
        else
        {
            if (isAtkDelayTime)
                isAtkDelayTime = false;
        }
        if (isWarding)
        {
            if (willAtkAround)
                willAtkAround = false;
            if (Vector3.Distance(transform.position, AStarTargetObj.transform.position) < 15f)
            {
                isWarding = false;
                --wardAmount;
                myChampBehav.WardRPC(myChampBehav.Team, myChampionData.mystat.Level, AStarTargetObj.transform.position);
                AStarTargetObj.transform.position = transform.position;
                ResetTarget();
            }
        }
        if (willAtkAround)
        {
            float dist = 1000000, nowD;
            GameObject temp = null;
            for (int i = 0; i < enemiesList.Count; ++i)
            {
                if (enemiesList[i].tag.Equals("Tower"))
                {
                    if (!enemiesList[i].GetComponent<TowerBehaviour>().isCanAtkMe)
                        continue;
                }
                else if (enemiesList[i].tag.Equals("Suppressor") || enemiesList[i].tag.Equals("Nexus"))
                {
                    if (!enemiesList[i].GetComponent<SuppressorBehaviour>().isCanAtkMe)
                        continue;
                }
                else if (enemiesList[i].layer.Equals(LayerMask.NameToLayer("Monster")))
                {
                    MonsterBehaviour m = enemiesList[i].GetComponent<MonsterBehaviour>();
                    if (!m.TheFogEntity.isCanTargeting)
                        continue;
                    if (!m.monAtk.isAtking)
                        continue;
                }
                nowD = (enemiesList[i].transform.position - myChamp.transform.position).sqrMagnitude;
                if (dist > nowD)
                {
                    dist = nowD;
                    temp = enemiesList[i];
                }
                if (temp != null)
                {
                    AtkTargetObj = temp;
                    isTargetting = true;
                }
            }
        }
        if (AtkTargetObj != null)
        {
            if (isTargetting && AtkTargetObj.activeInHierarchy)
            {
                if (TheAIDest.target != AtkTargetObj.transform)
                    TheAIDest.target = AtkTargetObj.transform;

                tempVec1 = AtkTargetObj.transform.position;
                tempVec2 = myChamp.transform.position;
                tempVec1.y = 0;
                tempVec2.y = 0;
                float atkRevision = 0;
                if (AtkTargetObj.tag.Equals("Tower") && AtkRange < 5)//타워 크기 보정
                    atkRevision = 1f;
                else if (AtkTargetObj.tag.Equals("Suppressor") && AtkRange < 5)//억제기 반지름 보정
                    atkRevision = 2.5f;
                else if (AtkTargetObj.tag.Equals("Nexus") && AtkRange < 5)
                    atkRevision = 7f;

                if (Vector3.Distance(tempVec1, tempVec2) > AtkRange + atkRevision)
                {//머니까 겁내 뛰어가자
                    if (!TheAIPath.canMove)
                    {
                        TheAIPath.canMove = true;
                        TheAIPath.canSearch = true;
                        myChampionAnimation.AttackAnimation(false);
                    }
                    if (AtkCoroutine != null)
                    {
                        myChampionAnimation.AttackAnimation(false);
                        StopCoroutine(AtkCoroutine);
                        AtkCoroutine = null;
                    }
                }
                else
                {//패자 계속 갈구자
                    if (!isAtkDelayTime)
                    {
                        if (TheAIPath.canMove)
                        {
                            TheAIPath.canMove = false;
                            TheAIPath.canSearch = false;

                            myChampionAnimation.AttackAnimation(true);
                        }
                        if (AtkCoroutine == null)
                        {
                            AtkCoroutine = StartCoroutine(AtkMotion());
                        }
                    }
                }
            }
            else
                ResetTarget();
        }
        else
            ResetTarget();

        if (wardAmount < 2)
        {
            wardMadeCooldown -= Time.deltaTime;
            if (wardMadeCooldown <= 0f)
            {
                ++wardAmount;
                wardMadeCooldown = Mathf.Round(wardMadeMaxTime - ((wardMadeTermTime * ((float)(myChampionData.mystat.Level - 1))) / 17f));
            }
        }
    }

    IEnumerator AtkMotion()
    {
        while (true)
        {
            if (!isAtkPause)
            {
                bool check = true;
                if (!isTargetting)
                    check = false;
                else if (AtkTargetObj == null)
                    check = false;
                else if (AtkTargetObj.Equals(AStarTargetObj))
                    check = false;
                if (check)
                {
                    myChampBehav.transform.DOLookAt(AtkTargetObj.transform.position, 0);
                    if (AtkTargetObj.tag.Equals("Minion"))
                    {
                        if (isAshe)
                        {
                            float moveTime = 0.4f;
                            myChampBehav.ArrowRPC(AtkTargetObj.transform.position, moveTime);
                        }
                        MinionBehavior behav = AtkTargetObj.GetComponent<MinionBehavior>();
                        AudioSource minAudio = behav.transform.GetChild(behav.transform.childCount - 1).GetComponent<AudioSource>();
                        if (behav != null)
                        {
                            int viewID = behav.GetComponent<PhotonView>().viewID;
                            myChampBehav.HitRPC(viewID);
                            ChampionSound.instance.IamAttackedSound(minAudio, champname);
                            if (isAshe)
                                asheSkill.qCountUp();
                            if (behav.HitMe(myChampionData.totalstat.Attack_Damage, "AD", myChampBehav.gameObject))
                            {
                                // 미니언쳤는데 죽었으면 cs, 골드 경험치 올려라
                                myChampionData.Kill_CS_Gold_Exp(AtkTargetObj.name, 1, AtkTargetObj.transform.position);
                                ResetTarget();
                            }
                            if (!skillKey.Equals(""))
                                if (--skillKeyNum < 1)
                                    skillKey = "";
                        }
                    }
                    //else if (AtkTargetObj.tag.Equals("Player"))
                    else if (AtkTargetObj.layer.Equals(LayerMask.NameToLayer("Champion")))
                    {//이 태그대로 할건지 바뀌는지는 모르겠음. 우선 챔피언 공격임.
                        if (isAshe)
                        {
                            float moveTime = 0.4f;
                            myChampBehav.ArrowRPC(AtkTargetObj.transform.position, moveTime);
                        }
                        ChampionBehavior behav = AtkTargetObj.GetComponent<ChampionBehavior>();
                        AudioSource champaudio = behav.gameObject.GetComponent<AudioSource>();
                        if (behav != null)
                        {
                            int viewID = behav.GetComponent<PhotonView>().viewID;
                            myChampBehav.HitRPC(viewID);
                            ChampionSound.instance.IamAttackedSound(champaudio, champname);
                            if (isAshe)
                                asheSkill.qCountUp();
                            if (behav.HitMe(myChampionData.totalstat.Attack_Damage, "AD", myChampBehav.gameObject, myChampBehav.name))
                            {
                                // 유저쳤는데 죽었으면 kill 골드 경험치 올려라
                                myChampionData.Kill_CS_Gold_Exp(AtkTargetObj.name, 0, AtkTargetObj.transform.position);
                                sysmsg.sendKillmsg(myChampionData.ChampionName, behav.GetComponent<ChampionData>().ChampionName, myChampBehav.Team);
                                ResetTarget();
                            }
                            if (!skillKey.Equals(""))
                                if (--skillKeyNum < 1)
                                    skillKey = "";
                        }
                    }
                    else if (AtkTargetObj.tag.Equals("Tower"))
                    {
                        if (isAshe)
                        {
                            float moveTime = 0.4f;
                            myChampBehav.ArrowRPC(AtkTargetObj.transform.position, moveTime);
                        }
                        TowerBehaviour behav = AtkTargetObj.GetComponent<TowerBehaviour>();
                        AudioSource towerAudio = behav.GetComponent<AudioSource>();
                        if (behav != null)
                        {
                            string key = "";
                            char[] keyChar = behav.gameObject.name.ToCharArray();
                            for (int i = 13; i < 16; ++i)
                            {
                                key += keyChar[i];
                            }
                            myChampBehav.HitRPC(key);
                            ChampionSound.instance.IamAttackedSound(towerAudio, champname);
                            if (isAshe)
                                asheSkill.qCountUp();
                            if (behav.HitMe(myChampionData.totalstat.Attack_Damage))
                            {
                                // 타워쳤는데 죽으면 cs 골드 경험치 올려라
                                myChampionData.Kill_CS_Gold_Exp(AtkTargetObj.name, 2, AtkTargetObj.transform.position);
                                ResetTarget();
                            }
                            if (!skillKey.Equals(""))
                                if (--skillKeyNum < 1)
                                    skillKey = "";
                        }
                    }
                    else if (AtkTargetObj.tag.Equals("Suppressor"))
                    {
                        if (isAshe)
                        {
                            float moveTime = 0.4f;
                            myChampBehav.ArrowRPC(AtkTargetObj.transform.position, moveTime);
                        }
                        SuppressorBehaviour behav = AtkTargetObj.GetComponent<SuppressorBehaviour>();
                        if (behav != null)
                        {
                            string key = "";
                            char[] keyChar = behav.gameObject.name.ToCharArray();
                            for (int i = 11; i < 14; ++i)
                                key += keyChar[i];
                            myChampBehav.HitRPC(key);
                            if (isAshe)
                                asheSkill.qCountUp();
                            if (behav.HitMe(myChampionData.totalstat.Attack_Damage))
                            {
                                // 억제기쳤는데 죽으면 cs 골드 경험치 올려라
                                myChampionData.Kill_CS_Gold_Exp(AtkTargetObj.name, 2, AtkTargetObj.transform.position);
                                ResetTarget();
                            }
                            if (!skillKey.Equals(""))
                                if (--skillKeyNum < 1)
                                    skillKey = "";
                        }
                    }
                    else if (AtkTargetObj.tag.Equals("Nexus"))
                    {
                        if (isAshe)
                        {
                            float moveTime = 0.4f;
                            myChampBehav.ArrowRPC(AtkTargetObj.transform.position, moveTime);
                        }
                        SuppressorBehaviour behav = AtkTargetObj.GetComponent<SuppressorBehaviour>();
                        if (behav != null)
                        {
                            string key = "";
                            char[] keyChar = behav.gameObject.name.ToCharArray();
                            key += keyChar[6];
                            myChampBehav.HitRPC(key);
                            if (isAshe)
                                asheSkill.qCountUp();
                            if (behav.HitMe(myChampionData.totalstat.Attack_Damage))
                                ResetTarget();
                            if (!skillKey.Equals(""))
                                if (--skillKeyNum < 1)
                                    skillKey = "";
                        }
                    }
                    else if (AtkTargetObj.layer.Equals(LayerMask.NameToLayer("Monster")))
                    {
                        if (isAshe)
                        {
                            float moveTime = 0.4f;
                            myChampBehav.ArrowRPC(AtkTargetObj.transform.position, moveTime);
                        }
                        MonsterBehaviour behav = AtkTargetObj.GetComponent<MonsterBehaviour>();
                        if (behav != null)
                        {
                            int viewID = behav.GetComponent<PhotonView>().viewID;
                            myChampBehav.HitRPC(viewID);
                            if (isAshe)
                                asheSkill.qCountUp();
                            if (behav.HitMe(myChampionData.totalstat.Attack_Damage, "AD", myChamp))
                            {
                                // 죽었으면 cs올려라
                                //myChampionData.Kill_CS_Gold_Exp(AtkTargetObj.name, 3, AtkTargetObj.transform.position);
                                ResetTarget();
                            }
                            if (!skillKey.Equals(""))
                                if (--skillKeyNum < 1)
                                    skillKey = "";
                        }
                    }
                }
            }
            float AS = myChampionData.mystat.Attack_Speed * (1 + (myChampionData.totalstat.UP_AttackSpeed * (myChampionData.totalstat.Level - 1) + (myChampionData.totalstat.Attack_Speed - myChampionData.mystat.Attack_Speed)) / 100);
            atkDelayTime = 1f / AS;
            yield return new WaitForSeconds(atkDelayTime);
        }
    }

    public void ResetTarget()
    {
        if (AtkCoroutine != null)
        {
            myChampionAnimation.AttackAnimation(false);
            StopCoroutine(AtkCoroutine);
            AtkCoroutine = null;
        }
        if (isTargetting)
            isTargetting = false;
        if (AtkTargetObj != null)
            AtkTargetObj = null;
        if (!willAtkAround)
        {
            if (TheAIDest.target != AStarTargetObj.transform)
            {
                TheAIDest.target = AStarTargetObj.transform;
                AStarTargetObj.transform.position = transform.position;
            }
        }
        else
        {
            TheAIDest.target = AStarTargetObj.transform;
        }
        if (!TheAIPath.canMove)
        {
            TheAIPath.canMove = true;
            TheAIPath.canSearch = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.name.Contains(myChampBehav.Team) && other.tag.Equals("Minion"))
        {
            AddEnemiesList(other);
        }
        else if (other.tag.Equals("Tower"))
        {
            TowerBehaviour behav = other.gameObject.GetComponent<TowerBehaviour>();
            //if (behav.isCanAtkMe)
            if (!myChampBehav.Team.Equals(behav.Team))
            {
                AddEnemiesList(other);
            }
        }
        else if (other.tag.Equals("Suppressor") || other.tag.Equals("Nexus"))
        {
            SuppressorBehaviour behav = other.gameObject.GetComponent<SuppressorBehaviour>();
            //if (behav.isCanAtkMe)
            if (!myChampBehav.Team.Equals(behav.Team))
                AddEnemiesList(other);
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            AddEnemiesList(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.name.Contains(myChampBehav.Team) && other.tag.Equals("Minion"))
        {
            RemoveEnemiesList(other);
        }
        else if (other.tag.Equals("Tower"))
        {
            TowerBehaviour behav = other.gameObject.GetComponent<TowerBehaviour>();
            if (!myChampBehav.Team.Equals(behav.Team))
            {
                RemoveEnemiesList(other);
            }
        }
        else if (other.tag.Equals("Suppressor") || other.tag.Equals("Nexus"))
        {
            SuppressorBehaviour behav = other.gameObject.GetComponent<SuppressorBehaviour>();
            //if (behav.isCanAtkMe)
            if (!myChampBehav.Team.Equals(behav.Team))
                RemoveEnemiesList(other);
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            RemoveEnemiesList(other);
        }
    }

    public void AddEnemiesList(Collider other)
    {
        if (!enemiesList.Contains(other.gameObject))
            enemiesList.Add(other.gameObject);
    }

    private void RemoveEnemiesList(Collider other)
    {
        if (enemiesList.Contains(other.gameObject))
        {
            if (other.gameObject.Equals(AtkTargetObj))
                AtkTargetObj = null;
            enemiesList.Remove(other.gameObject);
            if (enemiesList.Count.Equals(0))
            {
                AtkTargetObj = null;
            }
        }
    }

    //public void IAggroingEnemyMinion()
    //{//적 챔피언이 없어서 잘 동작하는지 아직 확인 불가. 나중에 확인할 것.
    //   for(int i = 0, j = enemiesList.Count; i < j; ++i)
    //    {
    //        if (enemiesList[i] == null)
    //            continue;
    //        if (!enemiesList[i].activeInHierarchy)
    //            continue;
    //        if(enemiesList[i].tag.Equals("Minion"))
    //        {
    //            MinionBehavior behav = enemiesList[i].GetComponent<MinionBehavior>();
    //            MinionAtk minAtk = behav.minAtk;
    //            if(minAtk.enemiesList.Contains(gameObject))
    //            {
    //                if (minAtk.nowTarget.tag.Equals("Waypoint"))
    //                    minAtk.MoveTarget = minAtk.nowTarget;
    //                minAtk.nowTarget = gameObject;
    //                minAtk.TheAIDest.target = transform;
    //            }
    //        }
    //    }
    //}

    private void AtkPauseOff()
    {
        isAtkPause = false;
        myChampionData.canSkill = true;
    }

    public void StunEffectToggle(bool stun, float time = 0f)
    {
        if (stun)
        {
            Invoke("_StunEffectOn", time);
        }
        else
        {
            Invoke("_StunEffectOff", time);
        }
    }

    private void _StunEffectOn()
    {
        isStun = true;
        stunParticle.SetActive(true);
    }

    private void _StunEffectOff()
    {
        isStun = false;
        stunParticle.SetActive(false);
    }

    public void PauseAtk(float f, bool moveToo = false)
    {
        isAtkPause = true;
        Invoke("AtkPauseOff", f);
        if (moveToo)
            PauseMove(f);
        myChampionData.canSkill = false;
        myChampionData.playerSkill.CancelSkill();
    }

    public void PauseMove(float f)
    {
        if (TheAIPath == null)
            TheAIPath = myChamp.GetComponent<AIPath>();
        TheAIPath.isStopped = true;
        Invoke("OnMove", f);
    }

    private void OnMove()
    {
        if (TheAIPath != null)
            TheAIPath.isStopped = false;
    }

    public void PushMe(Vector3 finish, float time = 0.1f)
    {
        PauseAtk(time, true);
        isPushing = true;
        finish.y = 0;
        pushTween = myChamp.transform.DOMove(finish, time).OnUpdate(() =>
        {
            if (myChampBehav.isDead)
                if (pushTween != null)
                    pushTween.Kill();
        }).OnKill(() =>
        {
            isPushing = false;
            pushTween = null;
        });
    }

    public void WantBuildWard(Vector3 v)
    {
        if (wardAmount > 0)
        {
            wardMadeCooldown = Mathf.Round(wardMadeMaxTime - ((wardMadeTermTime * ((float)(myChampionData.mystat.Level - 1))) / 17f));
            isWarding = true;
            AStarTargetObj.transform.position = v;
        }
    }

    public void PushWall()
    {
        if (pushTween != null)
            pushTween.Kill();
    }

    public void IKillChamp()
    {
        if (AtkTargetObj != null)
        {
            ChampionBehavior behav = AtkTargetObj.GetComponent<ChampionBehavior>();
            //myChampionData.Kill_CS_Gold_Exp(AtkTargetObj.name, 0, AtkTargetObj.transform.position);
            sysmsg.sendKillmsg(myChampionData.ChampionName, behav.GetComponent<ChampionData>().ChampionName, myChampBehav.Team);
            ResetTarget();
        }
    }
}