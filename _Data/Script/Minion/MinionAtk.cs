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
    /* 타겟팅 우선순위
     * 1. 아챔 때린 적챔
     * 2. 아챔 때린 적미니언
     * 3. 가까운 적미니언
     * 4. 가까운 적포탑
     * 5. 가까운 적챔피언
     */
    private void Awake()
    {
        TheAIPath = myMinion.GetComponent<AIPath>();
        //TheRichAI = GetComponent<RichAI>();
        TheAIDest = myMinion.GetComponent<AIDestinationSetter>();
        Anim = myMinion.GetComponent<Animator>();
    }

    private void Start()
    {
        Anim.SetBool("walking", true);
        if (myMinion.name.Contains("Blue"))
            enemyColor = "Red";
        else
            enemyColor = "Blue";
        //MoveTarget = GetComponent<AIDestinationSetter>().target.gameObject;
        myBehav = myMinion.GetComponent<MinionBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        int tp = 6;
        if (other.name.Contains(enemyColor) && other.tag.Equals("Minion"))
        {
            AddEnemiesList(other);
            tp = 3;
        }
        else if (other.tag.Equals("Player"))
        {
            if (other.GetComponent<ChampionBehavior>().Team.Equals(enemyColor))
            {
                AddEnemiesList(other);
                tp = 5;
            }
        }
        else if (other.tag.Equals("Tower"))
        {
            if (other.GetComponent<TowerBehaviour>().Team.Equals(enemyColor))
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
        if (other.name.Contains(enemyColor) && other.tag.Equals("Minion"))
        {
            RemoveEnemiesList(other);
        }
        else if (other.tag.Equals("Player"))
        {
            if (other.GetComponent<ChampionBehavior>().Team.Equals(enemyColor))
                RemoveEnemiesList(other);
        }
        else if (other.tag.Equals("Tower"))
        {
            if (other.GetComponent<TowerBehaviour>().Team.Equals(enemyColor))
                RemoveEnemiesList(other);
        }
    }

    private void AddEnemiesList(Collider other, int tp = 6)
    {
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
        if (enemiesList.Contains(other.gameObject))
        {
            if (other.gameObject.Equals(nowTarget))
                nowTarget = null;
            enemiesList.Remove(other.gameObject);
            if (enemiesList.Count.Equals(0))
            {
                TheAIDest.target = MoveTarget.transform;
                nowTarget = MoveTarget;
            }
        }
    }
    
    public void SetTarget(GameObject g)
    {//타겟을 특수 상황일 때 억지로 고정시킴. (1 = 아군 챔피언을 적 챔피언이 때렸을 때, 2 = 아군 챔피언을 적 미니언이 때렸을 때)
        //결과적으로 얘기하면 그냥 다구리용 애들 소집 함수임. 챔피언이 쳐맞았을 때 일정 반경에 있는 아군 미니언들에게 이걸 쏨
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
        if (nowTarget != null)
        {
            if (enemiesList.Contains(nowTarget))
                enemiesList.Remove(nowTarget);
            nowTarget = null;
            TheAIDest.target = MoveTarget.transform;
        }
    }

    private void TargetSearch()
    {
        /* 타겟팅 우선순위
         * 1. 아챔 때린 적챔
         * 2. 아챔 때린 적미니언
         * 3. 가까운 적미니언
         * 4. 가까운 적포탑
         * 5. 가까운 적챔피언
         */
        targetPriority = 6;
        float dist = 1000000, nowD;
        for (int i = 0, tp = 6; i < enemiesList.Count; ++i)
        {
            if (enemiesList[i].tag.Equals("Player"))
            {
                tp = 5;
            }
            else if (enemiesList[i].tag.Equals("Minion"))
            {
                tp = 3;
            }
            else if (enemiesList[i].tag.Equals("Tower"))
            {
                tp = 4;
            }
            if (targetPriority >= tp)
            {
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
    }

    private void Update()
    {
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
            if (check)
            {//주변에 적 있는데 타겟팅 안한거 확인. 타겟팅 하자.
                TargetSearch();
            }
            if (nowTarget.tag != "WayPoint")
            {//현재 타겟이 웨이포인트가 아닌 공격할 대상이다.
                if (TheAIPath == null)
                    TheAIPath = myMinion.GetComponent<AIPath>();
                float a = Vector3.Distance(nowTarget.transform.position, myMinion.transform.position);
                //if ((nowTarget.transform.position - myMinion.transform.position).sqrMagnitude > AtkRange)
                float atkRevision = 0;
                if (nowTarget.tag.Equals("Tower") && AtkRange < 5)
                    atkRevision = 3f;
                tempVec1 = nowTarget.transform.position;
                tempVec2 = myMinion.transform.position;
                tempVec1.y = 0;
                tempVec2.y = 0;
                if (Vector3.Distance(tempVec1, tempVec2) > AtkRange + atkRevision)
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
                    if (TheAIPath.canMove)
                    {
                        Anim.SetBool("walking", false);
                        TheAIPath.canMove = false;
                        TheAIPath.canSearch = false;
                        AtkCoroutine = StartCoroutine(AtkMotion());
                        myMinion.transform.DOLookAt(nowTarget.transform.position, 1);
                    }
                }
            }
        }
        else if (nowTarget != null)
        {//주변에 적이 없는데 내가 타겟을 가지고 있다.
            if (nowTarget.tag != "WayPoint")
            {//거기다 타겟이 웨이포인트가 아니다.
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
        }
        else
        {
            if (MoveTarget != null)
            {
                nowTarget = MoveTarget;
                TheAIDest.target = nowTarget.transform;
            }
        }

        //if (enemiesList.Count > 0)
        //{
        //    if (nowTarget == null)
        //    {
        //        MoveTarget = TheAIDest.target.gameObject;
        //        float dist = 1000000, nowD;
        //        for (int i = 0; i < enemiesList.Count; ++i)
        //        {
        //            nowD = (enemiesList[i].transform.position - transform.position).sqrMagnitude;
        //            if (dist > nowD)
        //            {
        //                dist = nowD;
        //                nowTarget = enemiesList[i];
        //            }
        //        }
        //        TheAIDest.target = nowTarget.transform;
        //    }
        //    else if (!nowTarget.activeInHierarchy)
        //        RemoveNowTarget();
        //}




        //if (nowTarget != null)
        //{
        //    if ((nowTarget.transform.position - transform.position).sqrMagnitude > AtkRange)
        //    {//이동
        //        if (AtkCoroutine != null)
        //        {
        //            StopCoroutine(AtkCoroutine);
        //            AtkCoroutine = null;
        //        }
        //        if (!TheAIPath.canMove)
        //        {
        //            Anim.SetBool("walking", true);
        //            TheAIPath.canMove = true;
        //            TheAIPath.canSearch = true;
        //        }
        //    }
        //    else
        //    {//공격
        //        if (TheAIPath.canMove)
        //        {
        //            Anim.SetBool("walking", false);
        //            TheAIPath.canMove = false;
        //            TheAIPath.canSearch = false;
        //            AtkCoroutine = StartCoroutine(AtkMotion());
        //            transform.DOLookAt(nowTarget.transform.position, 1);
        //        }
        //    }
        //}
        //else if (!TheAIPath.canMove)
        //{
        //    TheAIPath.canMove = true;
        //    TheAIPath.canSearch = true;
        //    nowTarget = MoveTarget;
        //    TheAIDest.target = nowTarget.transform;
        //}
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
                    myMinion.transform.DOLookAt(nowTarget.transform.position, 1);
                    if (nowTarget.tag.Equals("Minion"))
                    {
                        if (myMinion.name.Contains("Melee"))
                        {
                            MinionBehavior behav = nowTarget.GetComponent<MinionBehavior>();
                            if (behav != null)
                            {
                                if (behav.HitMe(myBehav.stat.Attack_Damage))
                                {
                                    enemiesList.Remove(nowTarget);
                                }
                            }
                        }
                        else if (myMinion.name.Contains("Magician"))
                        {
                            float moveTime = 0.2f;
                            GameObject Arrow = Minion_ObjectPool.current.GetPooledArrow();
                            Arrow.SetActive(true);
                            Arrow.transform.position = myMinion.transform.position;
                            Arrow.transform.LookAt(nowTarget.transform.position);
                            Arrow.transform.DOMove(nowTarget.transform.position, moveTime, true);
                            Arrow.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
                            Invoke("ProjectileAtk", moveTime);

                        }
                        else if (myMinion.name.Contains("Siege"))
                        {
                            float moveTime = 0.2f;
                            GameObject Cannonball = Minion_ObjectPool.current.GetPooledCannonball();
                            Cannonball.SetActive(true);
                            Cannonball.transform.position = myMinion.transform.position + (nowTarget.transform.position - myMinion.transform.position).normalized * 2f;
                            Cannonball.transform.DOMove(nowTarget.transform.position, moveTime, true);
                            Cannonball.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
                            Invoke("ProjectileAtk", moveTime);
                        }
                    }
                    else if (nowTarget.tag.Equals("Player"))
                    {
                        if (myMinion.name.Contains("Melee"))
                        {
                            ChampionBehavior behav = nowTarget.GetComponent<ChampionBehavior>();
                            if (behav != null)
                            {
                                if (behav.HitMe(myBehav.stat.Attack_Damage, "AD", gameObject))
                                {
                                    enemiesList.Remove(nowTarget);
                                }
                            }
                        }
                        else if (myMinion.name.Contains("Magician"))
                        {
                            float moveTime = 0.2f;
                            GameObject Arrow = Minion_ObjectPool.current.GetPooledArrow();
                            Arrow.SetActive(true);
                            Arrow.transform.position = myMinion.transform.position;
                            Arrow.transform.LookAt(nowTarget.transform.position);
                            Arrow.transform.DOMove(nowTarget.transform.position, moveTime, true);
                            Arrow.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
                            Invoke("ProjectileAtk", moveTime);

                        }
                        else if (myMinion.name.Contains("Siege"))
                        {
                            float moveTime = 0.2f;
                            GameObject Cannonball = Minion_ObjectPool.current.GetPooledCannonball();
                            Cannonball.SetActive(true);
                            Cannonball.transform.position = myMinion.transform.position + (nowTarget.transform.position - myMinion.transform.position).normalized * 2f;
                            Cannonball.transform.DOMove(nowTarget.transform.position, moveTime, true);
                            Cannonball.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
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
                                if (behav.HitMe(myBehav.stat.Attack_Damage))
                                {
                                    enemiesList.Remove(nowTarget);
                                }
                            }
                        }
                        else if (myMinion.name.Contains("Magician"))
                        {
                            float moveTime = 0.2f;
                            GameObject Arrow = Minion_ObjectPool.current.GetPooledArrow();
                            Arrow.SetActive(true);
                            Arrow.transform.position = myMinion.transform.position;
                            Arrow.transform.LookAt(nowTarget.transform.position);
                            Arrow.transform.DOMove(nowTarget.transform.position, moveTime, true);
                            Arrow.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
                            Invoke("ProjectileAtk", moveTime);

                        }
                        else if (myMinion.name.Contains("Siege"))
                        {
                            float moveTime = 0.2f;
                            GameObject Cannonball = Minion_ObjectPool.current.GetPooledCannonball();
                            Cannonball.SetActive(true);
                            Cannonball.transform.position = myMinion.transform.position + (nowTarget.transform.position - myMinion.transform.position).normalized * 2f;
                            Cannonball.transform.DOMove(nowTarget.transform.position, moveTime, true);
                            Cannonball.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
                            Invoke("ProjectileAtk", moveTime);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }



        //if (nowTarget != null)
        //{
        //    if (nowTarget.activeInHierarchy)
        //    {
        //        Anim.SetTrigger("attack");
        //        if (name.Contains("Melee"))
        //        {
        //            if (nowTarget.tag.Equals("Minion"))
        //            {
        //                MinionBehavior behav = nowTarget.GetComponent<MinionBehavior>();
        //                behav.stat.Hp -= myBehav.stat.Attack_Damage;
        //                print(behav.stat.Hp);
        //                if (behav.stat.Hp < 0)
        //                {
        //                    behav.stat.Hp = 0;
        //                    behav.IamDead();
        //                    enemiesList.Remove(nowTarget);
        //                    nowTarget = null;
        //                    TheAIPath.canMove = true;
        //                    TheAIPath.canSearch = true;
        //                    StopCoroutine(AtkMotion());
        //                    //적 사망 코드
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        TheAIPath.canMove = true;
        //        TheAIPath.canSearch = true;
        //        StopCoroutine(AtkMotion());
        //    }
        //}
        //else
        //{
        //    TheAIPath.canMove = true;
        //    TheAIPath.canSearch = true;
        //    StopCoroutine(AtkMotion());
        //}
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
                    if (behav.HitMe(myBehav.stat.Attack_Damage))
                    {
                        enemiesList.Remove(nowTarget);
                    }
                }
            }
            else if (nowTarget.tag.Equals("Player"))
            {
                ChampionBehavior behav;
                behav = nowTarget.GetComponent<ChampionBehavior>();
                if (behav != null)
                {
                    if (behav.HitMe(myBehav.stat.Attack_Damage, "AD", gameObject))
                    {
                        enemiesList.Remove(nowTarget);
                    }
                }
            }
            else if (nowTarget.tag.Equals("Tower"))
            {
                TowerBehaviour behav;
                behav = nowTarget.GetComponent<TowerBehaviour>();
                if (behav != null)
                {
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
        myMinion.transform.DOMove(finish, time);
    }
}