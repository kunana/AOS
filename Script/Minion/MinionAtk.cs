using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MinionAtk : MonoBehaviour
{
    //RichAI TheRichAI;
    public AIPath TheAIPath;
    public AIDestinationSetter TheAIDest;

    public List<GameObject> enemiesList = new List<GameObject>();
    string enemyColor;
    public GameObject MoveTarget = null;
    public GameObject nowTarget = null;
    public float AtkRange = 10; // 이건 애들마다 다르게 설정해야하는데 당장은 모르겠으니까 다 1로 고정
    Animator Anim;
    Coroutine AtkCoroutine;
    public MinionBehavior myBehav;
    public float AtkTriggerRange = 10;
    public bool isAtkPause = false;
    public GameObject myMinion = null;
    public Vector3 tempVec1, tempVec2;
    public int targetPriority = 6; // default = 6
    public float helpTime = 0; // 주변에 아군 챔피언을 친 놈이 있으면 다구리 타겟팅하는 시간
    public float atkDelayTime = 1f;
    public bool isAtkDelayTime = false;
    /* 타겟팅 우선순위
     * 1. 아챔 때린 적챔
     * 2. 아챔 때린 적미니언
     * 3. 가까운 적미니언
     * 4. 가까운 적포탑
     * 5. 가까운 적챔피언
     */
    public InGameManager inGameManager;
    private SystemMessage sysmsg;
    public bool isPushing = false;
    Tweener pushTween = null;

    private void Awake()
    {
        TheAIPath = myMinion.GetComponent<AIPath>();
        //TheRichAI = GetComponent<RichAI>();
        TheAIDest = myMinion.GetComponent<AIDestinationSetter>();
        Anim = myMinion.GetComponent<Animator>();
        sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
    }

    private void OnEnable()
    {
        Anim.SetBool("walking", true);
    }

    private void Start()
    {
        if (myMinion.name.Contains("Blue"))
            enemyColor = "Red";
        else
            enemyColor = "Blue";
        //MoveTarget = GetComponent<AIDestinationSetter>().target.gameObject;
        myBehav = myMinion.GetComponent<MinionBehavior>();
        inGameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        int tp = 6;
        if (other.name.Contains(enemyColor) && other.tag.Equals("Minion"))
        {
            AddEnemiesList(other);
            tp = 3;
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            if (other.GetComponent<ChampionBehavior>().Team.Equals(enemyColor))
            {
                AddEnemiesList(other);
                tp = 5;
            }
        }
        else if (other.tag.Equals("Tower"))
        {
            TowerBehaviour t = other.GetComponent<TowerBehaviour>();
            if (t.isCanAtkMe)
                if (t.Team.Equals(enemyColor))
                {
                    AddEnemiesList(other);
                    tp = 4;
                }
        }
        else if (other.tag.Equals("Suppressor") || other.tag.Equals("Nexus"))
        {
            SuppressorBehaviour s = other.GetComponent<SuppressorBehaviour>();
            if (s.isCanAtkMe)
                if (s.Team.Equals(enemyColor))
                {
                    AddEnemiesList(other);
                    tp = 4;
                }
        }
        if (tp < targetPriority)
        {
            TargetSearch();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (other.name.Contains(enemyColor) && other.tag.Equals("Minion"))
        {
            RemoveEnemiesList(other);
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            if (other.GetComponent<ChampionBehavior>().Team.Equals(enemyColor))
                RemoveEnemiesList(other);
        }
        else if (other.tag.Equals("Tower"))
        {
            if (other.GetComponent<TowerBehaviour>().Team.Equals(enemyColor))
                RemoveEnemiesList(other);
        }
        else if (other.tag.Equals("Suppressor") || other.tag.Equals("Nexus"))
        {
            if (other.GetComponent<SuppressorBehaviour>().Team.Equals(enemyColor))
                RemoveEnemiesList(other);
        }
    }

    private void AddEnemiesList(Collider other, int tp = 6)
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (!enemiesList.Contains(other.gameObject))
            enemiesList.Add(other.gameObject);
        if (tp < targetPriority)
        {
            nowTarget = other.gameObject;
            TheAIDest.target = nowTarget.transform;
        }
    }

    private void RemoveEnemiesList(Collider other)
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (enemiesList.Contains(other.gameObject))
        {
            if (other.gameObject.Equals(nowTarget))
                nowTarget = null;
            enemiesList.Remove(other.gameObject);
            if (enemiesList.Count.Equals(0))
            {
                nowTarget = MoveTarget;
                TheAIDest.target = MoveTarget.transform;
            }
        }
    }

    public void SetTarget(GameObject g)
    {//타겟을 특수 상황일 때 억지로 고정시킴. (1 = 아군 챔피언을 적 챔피언이 때렸을 때, 2 = 아군 챔피언을 적 미니언이 때렸을 때)
        //결과적으로 얘기하면 그냥 다구리용 애들 소집 함수임. 챔피언이 쳐맞았을 때 일정 반경에 있는 아군 미니언들에게 이걸 쏨
        if (!PhotonNetwork.isMasterClient)
            return;

        helpTime = 3;
        int tp = 6;
        if (g.tag.Equals("ChampionAtkRange"))
        {
            tp = 1;
        }
        else if (g.tag.Equals("MinionAtkRange"))
        {
            tp = 2;
        }
        if (targetPriority > tp)
        {
            targetPriority = tp;
            nowTarget = g;
            TheAIDest.target = nowTarget.transform;
        }
    }

    public void RemoveNowTarget()
    {//쫓던 애가 정글 등 쫓으면 안되는 영역으로 들어감
        if (!PhotonNetwork.isMasterClient)
            return;

        if (nowTarget != null)
        {
            if (enemiesList.Contains(nowTarget))
                enemiesList.Remove(nowTarget);
        }
    }

    private void TargetSearch()
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        /* 타겟팅 우선순위
         * 1. 아챔 때린 적챔
         * 2. 아챔 때린 적미니언
         * 3. 가까운 적미니언
         * 4. 가까운 적포탑
         * 5. 가까운 적챔피언
         */
        nowTarget = MoveTarget;
        targetPriority = 6;
        float dist = 1000000, nowD;
        bool isLockOn = false;
        for (int i = 0, tp = 6; i < enemiesList.Count; ++i)
        {
            if (enemiesList[i].layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                FogOfWarEntity f = enemiesList[i].GetComponent<FogOfWarEntity>();
                //if (enemiesList[i].GetComponent<FogOfWarEntity>().isCanTargeting)
                if (!f.isInTheBush)
                    tp = 5;
                else if (f.isInTheBushMyEnemyToo)
                    tp = 5;
                else
                    continue;
            }
            else if (enemiesList[i].tag.Equals("Minion"))
            {
                tp = 3;
            }
            else if (enemiesList[i].tag.Equals("Tower"))
            {
                if (enemiesList[i].GetComponent<TowerBehaviour>().isCanAtkMe)
                    tp = 4;
            }
            else if (enemiesList[i].tag.Equals("Suppressor") || enemiesList[i].tag.Equals("Nexus"))
            {
                if (enemiesList[i].GetComponent<SuppressorBehaviour>().isCanAtkMe)
                    tp = 4;
            }
            if (targetPriority >= tp)
            {
                isLockOn = true;
                targetPriority = tp;
                nowD = (enemiesList[i].transform.position - myMinion.transform.position).sqrMagnitude;
                if (dist > nowD)
                {
                    dist = nowD;
                    nowTarget = enemiesList[i];
                }
                TheAIDest.target = nowTarget.transform;
            }
        }
        if (!isLockOn)
        {
            TheAIDest.target = nowTarget.transform;
            if (AtkCoroutine != null)
            {//공격을 끈 후
                StopCoroutine(AtkCoroutine);
                AtkCoroutine = null;
            }
            Anim.SetBool("walking", true);//이동을 풀어준다.
            if (TheAIPath != null)
            {
                TheAIPath.canMove = true;
                TheAIPath.canSearch = true;
            }
        }
    }

    private void Update()
    {
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

        if (helpTime > 0 && targetPriority < 3)
        {//주변에 아군 챔피언이 맞았으면 헬프타임이 3이 된다.
            helpTime -= Time.deltaTime;
            if (helpTime <= 0)
            {//헬프타임이 처음 뜬 지 3초가 지났다
                if (!enemiesList.Contains(nowTarget))
                {//적이 범위를 벗어났다 => 타겟 초기화
                    nowTarget = null;
                }
                else
                { //적이 범위 안에 아직 있다 => 3초 더 팬다
                    helpTime += 3;
                }
            }
        }
        if (enemiesList.Count > 0)
        {
            bool check = false;
            if (nowTarget == null)
            {//주변에 적이 있는데 내가 타겟이 아예 없다 = 타겟팅 하자
                check = true;
            }
            else if (nowTarget.tag.Equals("WayPoint"))
            {//주변에 적이 있는데 잡은 타겟이 웨이포인트거나, 액티브 펄스다 = 타겟팅 하자
                check = true;
            }
            else if (!nowTarget.activeInHierarchy)
            {
                check = true;
                enemiesList.Remove(nowTarget);
            }
            else if (nowTarget.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                FogOfWarEntity f = nowTarget.GetComponent<FogOfWarEntity>();
                if (f.isInTheBush)
                    if (!f.isInTheBushMyEnemyToo)
                        check = true;
            }
            if (check)
            {//주변에 적 있는데 타겟팅 안한거 확인. 타겟팅 하자.
                TargetSearch();
            }

            if (nowTarget.tag != "WayPoint")
            {//현재 타겟이 웨이포인트가 아닌 공격할 대상이다.
                if (TheAIPath == null)
                    TheAIPath = myMinion.GetComponent<AIPath>();
                //float a = Vector3.Distance(nowTarget.transform.position, myMinion.transform.position);
                //if ((nowTarget.transform.position - myMinion.transform.position).sqrMagnitude > AtkRange)
                float atkRevision = 0;
                if (nowTarget.tag.Equals("Tower") /*&& AtkRange < 5*/)
                {
                    atkRevision = 3f;
                }
                else if (nowTarget.tag.Equals("Suppressor") /*&& AtkRange < 5*/)
                {
                    atkRevision = 2.5f;
                }
                else if (nowTarget.tag.Equals("Nexus") /*&& AtkRange < 5*/)
                {
                    atkRevision = 8f;
                }
                tempVec1 = nowTarget.transform.position;
                tempVec2 = myMinion.transform.position;
                tempVec1.y = 0;
                tempVec2.y = 0;
                float distance = Vector3.Distance(tempVec1, tempVec2);
                if (distance > AtkRange + atkRevision)
                {//공격 범위 밖에 적이 있다.
                    if (!TheAIPath.canMove)
                    {//이동 안한다고 해뒀던 상황이면
                        if (AtkCoroutine != null)
                        {//공격을 끈 후
                            StopCoroutine(AtkCoroutine);
                            AtkCoroutine = null;
                        }
                        Anim.SetBool("walking", true);//이동을 풀어준다.
                        TheAIPath.canMove = true;
                        TheAIPath.canSearch = true;
                    }
                }
                else
                {//공격 범위 안에 적이 있다.
                    if (!isAtkDelayTime)
                        if (TheAIPath.canMove)
                        {
                            Anim.SetBool("walking", false);
                            TheAIPath.canMove = false;
                            TheAIPath.canSearch = false;
                            AtkCoroutine = StartCoroutine(AtkMotion());
                            Vector3 v = nowTarget.transform.position;
                            v.y = 0;
                            myMinion.transform.DOLookAt(v, 1);
                        }
                }
            }
        }
        else if (nowTarget != null)
        {//주변에 적이 없는데 내가 타겟을 가지고 있다.
            if (nowTarget.tag != "WayPoint")
            {//거기다 타겟이 웨이포인트가 아니다.
                if (TheAIPath == null)
                    TheAIPath = myMinion.GetComponent<AIPath>();
                if (!TheAIPath.canMove)
                {
                    TheAIPath.canMove = true;
                    TheAIPath.canSearch = true;
                    Anim.SetBool("walking", true);
                    if (AtkCoroutine != null)
                    {//공격을 끈 후
                        StopCoroutine(AtkCoroutine);
                        AtkCoroutine = null;
                    }
                }
                nowTarget = MoveTarget; // 그럼 타겟을 깔끔하게 새로 잡자.
                TheAIDest.target = nowTarget.transform;
            }
            else
            {
                if (TheAIPath == null)
                    TheAIPath = myMinion.GetComponent<AIPath>();
                if (!TheAIPath.canMove)
                {
                    TheAIPath.canMove = true;
                    TheAIPath.canSearch = true;
                    Anim.SetBool("walking", true);
                    if (AtkCoroutine != null)
                    {//공격을 끈 후
                        StopCoroutine(AtkCoroutine);
                        AtkCoroutine = null;
                    }
                }
            }
        }
        else
        {
            if (MoveTarget != null)
            {
                nowTarget = MoveTarget;
                if (TheAIDest == null)
                    TheAIDest = myMinion.GetComponent<AIDestinationSetter>();
                TheAIDest.target = nowTarget.transform;
                if (TheAIPath == null)
                    TheAIPath = myMinion.GetComponent<AIPath>();
                if (!TheAIPath.canMove)
                {
                    TheAIPath.canMove = true;
                    TheAIPath.canSearch = true;
                }
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
                if (nowTarget == null)
                    check = false;
                else if (!nowTarget.activeInHierarchy)
                    check = false;

                if (check)
                {
                    if (Anim.GetBool("walking"))
                        Anim.SetBool("walking", false);
                    Anim.SetTrigger("attack");
                    Vector3 v = nowTarget.transform.position;
                    v.y = 0;
                    myMinion.transform.DOLookAt(v, 1);
                    if (nowTarget.tag.Equals("Minion"))
                    {
                        if (myMinion.name.Contains("Melee"))
                        {
                            MinionBehavior behav = nowTarget.GetComponent<MinionBehavior>();

                            if (behav != null)
                            {
                                int viewID = behav.GetComponent<PhotonView>().viewID;
                                myBehav.HitRPC(viewID);
                                if (behav.HitMe(myBehav.stat.Attack_Damage))
                                {
                                    enemiesList.Remove(nowTarget);
                                }
                            }
                        }
                        else if (myMinion.name.Contains("Magician"))
                        {
                            float moveTime = 0.4f;
                            myBehav.ArrowRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);

                        }
                        else if (myMinion.name.Contains("Siege"))
                        {
                            float moveTime = 0.4f;
                            myBehav.CannonballRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);
                        }
                    }
                    else if (nowTarget.layer.Equals(LayerMask.NameToLayer("Champion")))
                    {
                        if (myMinion.name.Contains("Melee"))
                        {
                            ChampionBehavior behav = nowTarget.GetComponent<ChampionBehavior>();

                            if (behav != null)
                            {
                                int viewID = behav.GetComponent<PhotonView>().viewID;
                                myBehav.HitRPC(viewID);
                                if (behav.HitMe(myBehav.stat.Attack_Damage, "AD", myMinion, myMinion.name))
                                {
                                    sysmsg.sendKillmsg("minion", behav.GetComponent<ChampionData>().ChampionName, myBehav.team.ToString());
                                    enemiesList.Remove(nowTarget);
                                }
                            }
                        }
                        else if (myMinion.name.Contains("Magician"))
                        {
                            float moveTime = 0.4f;
                            myBehav.ArrowRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);

                        }
                        else if (myMinion.name.Contains("Siege"))
                        {
                            float moveTime = 0.4f;
                            myBehav.CannonballRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);
                        }
                    }
                    else if (nowTarget.tag.Equals("Tower"))
                    {
                        if (myMinion.name.Contains("Melee"))
                        {
                            TowerBehaviour behav = nowTarget.GetComponent<TowerBehaviour>();

                            if (behav != null)
                            {
                                string key = "";
                                char[] keyChar = behav.gameObject.name.ToCharArray();
                                for (int i = 13; i < 16; ++i)
                                {
                                    key += keyChar[i];
                                }
                                myBehav.HitRPC(key);
                                if (behav.HitMe(myBehav.stat.Attack_Damage))
                                {
                                    if (enemyColor.Equals("Red"))
                                        inGameManager.blueTeamPlayer[0].GetComponent<PhotonView>().RPC("GlobalGold", PhotonTargets.All, "blue", 100);
                                    else
                                        inGameManager.redTeamPlayer[0].GetComponent<PhotonView>().RPC("GlobalGold", PhotonTargets.All, "red", 100);
                                    enemiesList.Remove(nowTarget);
                                }
                            }
                        }
                        else if (myMinion.name.Contains("Magician"))
                        {
                            float moveTime = 0.4f;
                            myBehav.ArrowRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);
                        }
                        else if (myMinion.name.Contains("Siege"))
                        {
                            float moveTime = 0.4f;
                            myBehav.CannonballRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);
                        }
                    }
                    else if (nowTarget.tag.Equals("Suppressor") || nowTarget.tag.Equals("Nexus"))
                    {
                        if (myMinion.name.Contains("Melee"))
                        {
                            SuppressorBehaviour behav = nowTarget.GetComponent<SuppressorBehaviour>();

                            if (behav != null)
                            {
                                string key = "";
                                char[] keyChar = behav.gameObject.name.ToCharArray();
                                if (nowTarget.tag.Equals("Nexus"))
                                    key += keyChar[6];
                                else
                                    for (int i = 11; i < 14; ++i)
                                    {
                                        key += keyChar[i];
                                    }
                                myBehav.HitRPC(key);
                                if (behav.HitMe(myBehav.stat.Attack_Damage))
                                {
                                    enemiesList.Remove(nowTarget);
                                }
                            }
                        }
                        else if (myMinion.name.Contains("Magician"))
                        {
                            float moveTime = 0.4f;
                            myBehav.ArrowRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);

                        }
                        else if (myMinion.name.Contains("Siege"))
                        {
                            float moveTime = 0.4f;
                            myBehav.CannonballRPC(nowTarget.transform.position, moveTime);
                            Invoke("ProjectileAtk", moveTime);
                        }
                    }
                }
            }
            atkDelayTime = 1f;
            yield return new WaitForSeconds(1);
        }
    }
    private void ProjectileAtk()
    {
        if (nowTarget != null)
        {
            if (nowTarget.tag.Equals("Minion"))
            {
                MinionBehavior behav;
                behav = nowTarget.GetComponent<MinionBehavior>();

                if (behav != null)
                {
                    int viewID = behav.GetComponent<PhotonView>().viewID;
                    myBehav.HitRPC(viewID);
                    if (behav.HitMe(myBehav.stat.Attack_Damage))
                    {
                        enemiesList.Remove(nowTarget);
                    }
                }
            }
            else if (nowTarget.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                ChampionBehavior behav;
                behav = nowTarget.GetComponent<ChampionBehavior>();

                if (behav != null)
                {
                    int viewID = behav.GetComponent<PhotonView>().viewID;
                    myBehav.HitRPC(viewID);
                    if (behav.HitMe(myBehav.stat.Attack_Damage, "AD", myMinion, myMinion.name))
                    {
                        enemiesList.Remove(nowTarget);
                        sysmsg.sendKillmsg("minion", behav.GetComponent<ChampionData>().ChampionName, myBehav.team.ToString());
                    }
                }
            }
            else if (nowTarget.tag.Equals("Tower"))
            {
                TowerBehaviour behav;
                behav = nowTarget.GetComponent<TowerBehaviour>();

                if (behav != null)
                {
                    string key = "";
                    char[] keyChar = behav.gameObject.name.ToCharArray();
                    for (int i = 13; i < 16; ++i)
                    {
                        key += keyChar[i];
                    }
                    myBehav.HitRPC(key);
                    if (behav.HitMe(myBehav.stat.Attack_Damage))
                    {
                        if (enemyColor.Equals("Red"))
                            inGameManager.blueTeamPlayer[0].GetComponent<ChampionData>().photonView.RPC("GlobalGold", PhotonTargets.All, "blue", 100);
                        else
                            inGameManager.redTeamPlayer[0].GetComponent<ChampionData>().photonView.RPC("GlobalGold", PhotonTargets.All, "red", 100);
                        enemiesList.Remove(nowTarget);
                    }
                }
            }
            else if (nowTarget.tag.Equals("Suppressor") || nowTarget.tag.Equals("Nexus"))
            {
                SuppressorBehaviour behav;
                behav = nowTarget.GetComponent<SuppressorBehaviour>();

                if (behav != null)
                {
                    string key = "";
                    char[] keyChar = behav.gameObject.name.ToCharArray();
                    if (nowTarget.tag.Equals("Nexus"))
                        key += keyChar[6];
                    else
                        for (int i = 11; i < 14; ++i)
                        {
                            key += keyChar[i];
                        }
                    myBehav.HitRPC(key);
                    if (behav.HitMe(myBehav.stat.Attack_Damage))
                    {
                        enemiesList.Remove(nowTarget);
                    }
                }
            }
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
            TheAIPath = myMinion.GetComponent<AIPath>();
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
        pushTween = myMinion.transform.DOMove(finish, time).OnUpdate(() =>
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

    public void InitMinionStatus()
    {
        isAtkPause = false;
        targetPriority = 6;
        helpTime = 0;
    }

    public void PushWall()
    {
        if (pushTween != null)
            pushTween.Kill();
    }
}