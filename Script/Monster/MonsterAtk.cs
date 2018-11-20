using DG.Tweening;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAtk : MonoBehaviour
{
    public AIPath TheAIPath;
    public AIDestinationSetter TheAIDest;
    public GameObject centerTarget = null; // <- 이건 자기 껍데기로 하면 될듯. 거기가 좌표니까.
    public GameObject nowTarget = null;
    public float AtkRange = 10;
    public Animator Anim;
    Coroutine AtkCoroutine = null;
    public MonsterBehaviour myBehav;
    public float AtkTriggerRange = 10;
    public bool isAtkPause = false;
    public GameObject myMonster = null;
    public List<GameObject> enemiesList = new List<GameObject>();
    //public List<GameObject> friendsList = new List<GameObject>();
    public bool isAtking = false;
    public bool isReturn = false;
    public float atkDelayTime = 1f;
    public bool isAtkDelayTime = false;
    private SystemMessage sysmsg;

    public bool isPushing = false;
    Tweener pushTween = null;

    public void InitValue()
    {
        if (TheAIPath == null)
            TheAIPath = myMonster.GetComponent<AIPath>();
        isReturn = false;
        isAtking = false;
        isAtkPause = false;
        if (AtkCoroutine != null)
        {
            StopCoroutine(AtkCoroutine);
            AtkCoroutine = null;
        }
        //nowTarget = centerTarget;
        TheAIDest.target = centerTarget.transform;
        atkDelayTime = 1;
        isAtkDelayTime = false;
        enemiesList.Clear();
    }

    public void LateInit()
    {
        myMonster = transform.parent.gameObject;
        centerTarget = myMonster.transform.parent.gameObject;
        TheAIPath = myMonster.GetComponent<AIPath>();
        TheAIDest = myMonster.GetComponent<AIDestinationSetter>();
        TheAIDest.target = centerTarget.transform;
        Anim = myMonster.GetComponent<Animator>();
        Anim.SetBool("walking", false);
        Anim.SetBool("attack", false);
        myBehav = myMonster.GetComponent<MonsterBehaviour>();
    }

    void Awake()
    {
        sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
            if (!enemiesList.Contains(other.gameObject))
                enemiesList.Add(other.gameObject);
        //else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        //    friendsList.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            if (enemiesList.Contains(other.gameObject))
            {
                if (other.gameObject.Equals(nowTarget))
                {
                    nowTarget = null;
                }
                enemiesList.Remove(other.gameObject);
            }
        }
        //else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        //    friendsList.Remove(other.gameObject);
    }

    private void Update()
    {
        if (isReturn)
        {
            if (myBehav.stat.Hp < myBehav.stat.MaxHp)
                myBehav.stat.Hp += 250 * Time.deltaTime;
            if (myBehav.stat.Hp > myBehav.stat.MaxHp)
                myBehav.stat.Hp = myBehav.stat.MaxHp;
        }

        if (!PhotonNetwork.isMasterClient)
            return;

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

        if (isAtking)
        {
            bool check = false;
            if (enemiesList.Count < 1)
            {
                isAtking = false;
                nowTarget = null;
            }
            else if (nowTarget == null)
            {
                check = true;
            }

            if (!isReturn)
            {//공격상태임
                if (check)
                {//타겟을 잃음
                    float dist = 1000000, nowD;
                    for (int i = 0; i < enemiesList.Count; ++i)
                    {
                        nowD = (enemiesList[i].transform.position - myMonster.transform.position).sqrMagnitude;
                        if (dist > nowD)
                        {
                            dist = nowD;
                            nowTarget = enemiesList[i];
                        }
                        TheAIDest.target = nowTarget.transform;
                    }
                    if (AtkCoroutine != null)
                        StopCoroutine(AtkCoroutine);
                    AtkCoroutine = null;
                    Anim.SetBool("attack", false);
                }
                if (nowTarget == null)
                {
                    if (AtkCoroutine != null)
                    {
                        StopCoroutine(AtkCoroutine);
                        AtkCoroutine = null;
                        Anim.SetBool("attack", false);
                    }
                    if (enemiesList.Count < 1)
                    {
                        nowTarget = null;
                        TheAIDest.target = centerTarget.transform;
                        return;
                    }
                    else
                    {
                        nowTarget = null;
                        return;
                    }
                }
                float distance = Vector3.Distance(nowTarget.transform.position, myMonster.transform.position);
                if (distance > AtkRange)
                {//공격범위 밖에 적이 있음
                    if (!TheAIPath.canMove)
                    {//이동을 켬
                        TheAIPath.canMove = true;
                        TheAIPath.canSearch = true;
                    }
                    Anim.SetBool("walking", true);
                    if (AtkCoroutine != null)
                    {
                        StopCoroutine(AtkCoroutine);
                        AtkCoroutine = null;
                        Anim.SetBool("attack", false);
                    }
                }
                else
                {//공격범위 안에 적이 있음
                    if (!isAtkDelayTime)
                    {
                        if (TheAIPath.canMove)
                        {//이동을 끔
                            TheAIPath.canMove = false;
                            TheAIPath.canSearch = false;
                        }
                        Anim.SetBool("walking", false);
                        //뚜까패야댐
                        if (AtkCoroutine == null)
                            AtkCoroutine = StartCoroutine("AtkMotion");
                    }
                }
            }
            else
            {//공격이 꺼짐. 제자리가 아니라면 제자리로 돌려보내야 함.
                Return();
            }
        }
        else if (isReturn)
        {
            Return();
        }
        else if (enemiesList.Count < 1)
        {
            isReturn = true;
            Return();
        }

        // 집으로 다돌아가면 idle
        if (isReturn == false && isAtking == false)
        {
            if (Anim == null)
            {
                LateInit();
                Anim = myMonster.GetComponent<Animator>();
            }

            Anim.SetBool("walking", false);
            Anim.SetBool("attack", false);
        }
    }

    public void Return()
    {
        if (!TheAIPath.canMove)
        {
            TheAIPath.canMove = true;
            TheAIPath.canSearch = true;
        }
        if (nowTarget != null)
        {//공격이 꺼졌는데 타겟을 가지고 있으면 타겟을 삭제하고 갈 곳으로 보낸다.
            nowTarget = null;
        }
        TheAIDest.target = centerTarget.transform;
        if (AtkCoroutine != null)
        {
            StopCoroutine(AtkCoroutine);
            AtkCoroutine = null;
        }
        Anim.SetBool("attack", false);

        if (myBehav.stat.Hp < myBehav.stat.MaxHp)
            myBehav.stat.Hp += 250 * Time.deltaTime;
        if (myBehav.stat.Hp > myBehav.stat.MaxHp)
            myBehav.stat.Hp = myBehav.stat.MaxHp;
        if (myBehav.stat.Hp == myBehav.stat.MaxHp)
            if (Vector3.Distance(myMonster.transform.position, centerTarget.transform.position) < 0.5f)
            {
                myBehav.myCenter.SetPosition();
                isAtking = false;
                isReturn = false;
                myBehav.ReturnOtherClients(isReturn);
            }
    }

    public void StartReturn()
    {
        isReturn = true;
        // 돌아갈땐 걷기
        Anim.SetBool("walking", true);
        Anim.SetBool("attack", false);

        if (myBehav != null)
            if (myBehav.enabled)
            {
                myBehav.ReturnOtherClients(isReturn);
            }
        if (TheAIPath != null)
            if (TheAIPath.enabled)
            {
                TheAIPath.canMove = true;
                TheAIPath.canSearch = true;
            }
    }

    IEnumerator AtkMotion()
    {
        while (true)
        {
            if (!isAtkPause)
            {
                bool check = true;
                if (nowTarget == null)
                    check = false;
                else if (!nowTarget.activeInHierarchy)
                    check = false;
                if (check)
                {
                    Anim.SetBool("walking", false);
                    Anim.SetBool("attack", true);

                    myMonster.transform.DOLookAt(nowTarget.transform.position, 1);

                    if (nowTarget.layer.Equals(LayerMask.NameToLayer("Champion")))
                    {
                        ChampionBehavior behav = nowTarget.GetComponent<ChampionBehavior>();
                        if (behav != null)
                        {
                            int viewID = behav.GetComponent<PhotonView>().viewID;
                            myBehav.HitRPC(viewID);
                            if (behav.HitMe(myBehav.stat.Attack_Damage, "AD", myMonster, myMonster.name))
                            {
                                //ResetTarget();
                                sysmsg.sendKillmsg("monster", behav.GetComponent<ChampionData>().ChampionName, "ex");
                                enemiesList.Remove(nowTarget);
                                if (enemiesList.Count < 1)
                                {
                                    isReturn = true;
                                    for (int i = 0; i < myBehav.friendsList.Count; ++i)
                                        if (myBehav.friendsList[i] != null)
                                            if (myBehav.friendsList[i].activeInHierarchy)
                                            {
                                                MonsterBehaviour m = myBehav.friendsList[i].GetComponent<MonsterBehaviour>();
                                                if (m.monAtk.enemiesList.Contains(nowTarget))
                                                    m.monAtk.enemiesList.Remove(nowTarget);
                                                m.monAtk.isReturn = true;
                                            }
                                }
                            }
                        }
                    }
                }
            }
            atkDelayTime = 1f;
            yield return new WaitForSeconds(1);
        }
    }

    private void AtkPauseOff()
    {
        isAtkPause = false;
    }

    public void PauseAtk(float f, bool moveToo = false)
    {
        isAtkPause = true;
        Invoke("AtkPauseOff", f);
        if (moveToo)
            PauseMove(f);
    }

    public void PauseMove(float f)
    {
        if (TheAIPath == null)
            TheAIPath = myMonster.GetComponent<AIPath>();
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
        pushTween = myMonster.transform.DOMove(finish, time).OnUpdate(() =>
        {
            if (myBehav.isDead)
                if (pushTween != null)
                    pushTween.Kill();
        }).OnKill(() =>
        {
            isPushing = false;
            pushTween = null;
        });
    }

    public void ResetTarget()
    {
        if (AtkCoroutine != null)
        {
            StopCoroutine(AtkCoroutine);
            AtkCoroutine = null;
        }
        nowTarget = null;
        TheAIDest.target = centerTarget.transform;
        Anim.SetBool("attack", false);
        Anim.SetBool("walking", true);

        if (!TheAIPath.canMove)
        {
            TheAIPath.canMove = true;
            TheAIPath.canSearch = true;
        }
    }

    public void PushWall()
    {
        if (pushTween != null)
            pushTween.Kill();
    }
}