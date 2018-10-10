using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
using Pathfinding.RVO;
using Pathfinding;

/// <summary>
/// 근우 & 호동
/// 미니언 오브젝트 이름은 Minion_팀색깔_미니언타입으로 설정
/// ex) Minion_Red_Melee, Minion_Blue_Siege 
/// </summary>

public class MinionBehavior : MonoBehaviour
{
    public Vector3 spawnPoint;
    //public Vector3 deadVec = new Vector3(-100, -100, -100);
    //public Vector3 deadOffsetVec = new Vector3(0, -10, 0);
    AIDestinationSetter TheAIDest;
    public MinionAtk minAtk;
    AIPath TheAIPath;
    //현재 상황을 나타내는 변수

    public enum AIPriority { CC = 0, KeepAttack, FindNewTarget, FindWaypoint, GoWaypoint, Dead }
    public enum AtkPriority { ecAtk_mc, emAtk_mc, emAtk_mm, etAtk_mm, ecAtk_mm, nearEm, nearEc }
    protected enum MinionType { Melee, Magic, Siege, Super }
    protected enum Path { Top, Mid, Bot };
    protected enum TeamColor { Red, Blue }
    //ec = enemyChamp / mc = myTeamChamp / em = EnemyMinion / mm = myTeamMinion / et = EnemyTower

    [Header("---현재상황---")]
    [SerializeField]
    private TeamColor team;
    [SerializeField]
    private Path path;
    [SerializeField]
    private MinionType minionType;
    [SerializeField]
    public AIPriority AI;

    [Header("---공격,추적,충돌 변수---")]
    //공격및 추적관련 
    public bool CC_hit = false;
    public bool isChampATk = false;
    //유효, 공격, 충돌범위
    private float Colider_Range = 1;
    public bool Help_Signal = false;
    public bool canAttack = true;
    //public List<Transform> Target_List;
    [SerializeField]
    public Transform CurTarget;

    //웨이포인트
    [Header("---웨이포인트---")]
    public Vector3[] waypoints = null;
    public int CurWaypoint = 0;
    public int NextWaypoint = 0;
    public Vector3 targetWaypoint;
    public bool isLane = true;
    //AStar TheAStar;
    Player ThePlayer;
    public Vector2[] gridWayPoints;
    public int nowWayPointNum = 1;

    //미니언 이동속도 공격속도는 일단 defalut 나중에 json받아서 사용 
    //웨이포인트(호동)
    public float Minionspeed = 5; //미니언 이동속도(수정해야함)
    public Transform pathHolder; //웨이포인트 묶음을 넣는 것

    //애니메이션
    protected Animation animation;
    protected Animator animator;
    protected bool isAttacked = false;
    protected bool iswalk = false;
    public bool isDead = false;
    protected bool isFallback = false;
    public bool knowPath = false;
    //public List<GridData> nowPath;
    public Vector3 DepVec;
    //스탯 클래스
    public StatClass.Stat stat;

    //캐싱
    GameObject MinionVaildCol;
    GameObject pool;
    GameObject minionManager;

    int checkNum = 0;


    public void SetStat(string championName)
    {
        stat = StatClass.instance.characterData[championName].ClassCopy();
    }

    public void SetGridWayPoints(Vector2[] grid, int size = 4)
    {
        gridWayPoints = new Vector2[size];
        for (int i = 0; i < grid.Length && i < gridWayPoints.Length; ++i)
        {
            gridWayPoints[i] = grid[i];
        }
    }

    private void Awake()
    {
        TheAIDest = gameObject.GetComponent<AIDestinationSetter>();
        minAtk = transform.GetComponentInChildren<MinionAtk>();
        if (minAtk == null)
            print("minatk is null");
        TheAIPath = gameObject.GetComponent<AIPath>();
    }

    /// <summary>
    ///사용할 코루틴은 우선 2개? AI행동 / 유효범위 탐색용 코루틴 
    ///각 0.25초
    /// </summary>
    /// 
    private void Start()
    {
        //deadVec = transform.position;
        CheckPath();
        //TheAStar = AStar.instance;
        ThePlayer = GetComponent<Player>();
        MinionVaildCol = GameObject.FindGameObjectWithTag("VaildColider");
        pool = GameObject.FindGameObjectWithTag("MinionPooling");
        //minionManager = GameObject.FindGameObjectWithTag("MinionManager");

        animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        SetMinion(); //오브젝트 풀링 때문에 실시간 업데이트로 적용

        //Target_List = new List<Transform>();
        //StartWay();


    }
    //private void StartWay()
    //{
    //    //웨이포인트(호동)
    //    if (pathHolder != null)
    //    {

    //        waypoints = new Vector3[pathHolder.childCount];
    //        for (int i = 0; i < waypoints.Length; i++)
    //        {
    //            waypoints[i] = pathHolder.GetChild(i).position;
    //            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
    //        }
    //        StartCoroutine(AI_Priority_Check());
    //    }
    //    else
    //    {
    //        print("need a pathHolder");
    //    }
    //}

    //private void FixedUpdate()
    //{   //HP 가 0일때.
    //    if (stat.Hp <= 0 && !isDead)
    //    {
    //        stat.Hp = 0;
    //        CurTarget = null;
    //        animator.SetTrigger("Death");
    //        StartCoroutine(Death());
    //    }
    //}
    public int wayNum;
    /*private void LateUpdate()
    {
        if (!GetComponent<Player>().first && gridWayPoints != null)
            if (knowPath)
            {
                DepVec = nowPath[wayNum].transform.position;
                if (Vector3.Distance(transform.position, DepVec) > 0.2f)
                {
                    Vector3 arrow = (DepVec - transform.position).normalized;
                    transform.position += arrow * 0.2f;
                }
                else if (wayNum.Equals(0))
                {
                    ++nowWayPointNum;
                    knowPath = false;
                }
                else
                {
                    if (wayNum > 4)
                        wayNum -= 5;
                    else
                        wayNum = 0;
                }
            }
            else if (nowWayPointNum != gridWayPoints.Length)
            {
                Vector2 nowWayPoint = gridWayPoints[nowWayPointNum]; //new Vector2(7, 75);
                if (ThePlayer.gridX != nowWayPoint.x || ThePlayer.gridZ != nowWayPoint.y)
                {
                    List<GridData> temp = TheAStar.Move(ThePlayer.gridX, ThePlayer.gridZ, (int)nowWayPoint.x, (int)nowWayPoint.y);
                    //nowPath = TheAStar.Move(ThePlayer.gridX, ThePlayer.gridZ, (int)nowWayPoint.x, (int)nowWayPoint.y);
                    nowPath = new List<GridData>();
                    for (int i = 0; i < temp.Count; ++i)
                        nowPath.Add(temp[i]);


                    wayNum = nowPath.Count - 1;
                    knowPath = true;
                }
            }
    }*/

    //private void LateUpdate()
    //{
    //    if (!GetComponent<Player>().first && gridWayPoints != null)
    //        if (knowPath)
    //        {
    //            if (Vector3.Distance(transform.position, DepVec) > 0.2f)
    //            {
    //                Vector3 arrow = (DepVec - transform.position).normalized;
    //                transform.position += arrow * 0.2f;
    //            }
    //            else
    //            {
    //                ++nowWayPointNum;
    //                knowPath = false;
    //            }
    //        }
    //        else if (nowWayPointNum != gridWayPoints.Length)
    //        {
    //            Vector2 nowWayPoint = gridWayPoints[nowWayPointNum]; //new Vector2(7, 75);
    //            if (ThePlayer.gridX != nowWayPoint.x || ThePlayer.gridZ != nowWayPoint.y)
    //            {
    //                nowPath = TheAStar.Move(ThePlayer.gridX, ThePlayer.gridZ, (int)nowWayPoint.x, (int)nowWayPoint.y);
    //                for (int i = 0; i < nowPath.Count;)
    //                {
    //                    bool collectWay = false;
    //                    bool first = true;
    //                    Vector3 v = (nowPath[i].transform.position - nowPath[nowPath.Count - 1].transform.position).normalized;
    //                    RaycastHit[] r = Physics.RaycastAll(nowPath[nowPath.Count - 1].transform.position, v, Vector3.Distance(nowPath[nowPath.Count - 1].transform.position,
    //                        nowPath[i].transform.position));
    //                    print("ShootRay");
    //                    DepVec = transform.position;
    //                    for(int j = 0; j < r.Length; ++j)
    //                    //foreach (RaycastHit hit in r)
    //                    {
    //                        RaycastHit hit = r[j];
    //                        if (hit.collider.name.Equals("Grid(Clone)"))
    //                        {
    //                            Vector3 ffwefwef = hit.collider.transform.position;
    //                            transform.DOLookAt(ffwefwef, 0);
    //                            if (first)
    //                            {
    //                                first = false;
    //                            }
    //                            GridData._type t = hit.collider.GetComponent<GridData>().type;
    //                            if (t.Equals(GridData._type.obstacle))
    //                            {
    //                                collectWay = false;
    //                                break;
    //                            }
    //                            if (hit.transform.position.Equals(nowPath[i].transform.position))
    //                            {
    //                                collectWay = true;
    //                                DepVec = hit.transform.position;
    //                                break;
    //                            }
    //                        }
    //                    }
    //                    if (collectWay)
    //                    {
    //                        knowPath = true;
    //                        break;
    //                    }
    //                    else if (i.Equals(nowPath.Count - 1))
    //                    {
    //                        int tempX = 0, tempZ = 0, gridNum = nowPath[0].gridNum;
    //                        int depGridX = gridNum % MakeGrid.instance.gridMaxX, depGridZ = gridNum / MakeGrid.instance.gridMaxZ;
    //                        if (depGridX > ThePlayer.gridX)
    //                        {
    //                            ++tempX;
    //                        }
    //                        else if (depGridX < ThePlayer.gridX)
    //                        {
    //                            --tempX;
    //                        }
    //                        if (depGridZ > ThePlayer.gridZ)
    //                        {
    //                            ++tempZ;
    //                        }
    //                        else if (depGridZ < ThePlayer.gridZ)
    //                        {
    //                            --tempZ;
    //                        }
    //                        if (tempZ != 0 && tempX != 0)
    //                        {
    //                            knowPath = true;
    //                            DepVec = MakeGrid.instance.GridList[(ThePlayer.gridZ + tempZ) * MakeGrid.instance.gridMaxX
    //                                + (ThePlayer.gridX + tempX)].transform.position;
    //                            break;
    //                        }
    //                    }
    //                    else
    //                    {//다음 웨이로 갱신

    //                        //knowPath = false;
    //                    }
    //                    if (i.Equals(i + ((nowPath.Count - i) / 2)))
    //                        ++i;
    //                    else
    //                        i += ((nowPath.Count - i) / 2);

    //                }
    //            }
    //        }
    //}









    public void CheckPath()
    {
        if (transform.name.Contains("Top"))
            path = Path.Top;
        if (transform.name.Contains("Mid"))
            path = Path.Mid;
        if (transform.name.Contains("Bot"))
            path = Path.Bot;
    }

    /// <summary>
    /// 우선순위 체크 0.25초마다.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AI_Priority_Check()
    {
        if (GetComponent<MinionBehavior>() != null)
        {
            while (!isDead)
            {   //bool. CC기에 맞았는가
                yield return new WaitForSeconds(0.2f);

                if (CC_hit)
                {
                    AI = AIPriority.CC;
                    //행동 못하게 고정, CC기 해제 코루틴
                }
                else if (!isLane)
                {
                    //길 벗어났을때.
                }
                else if (CurTarget != null && CurTarget.gameObject.activeInHierarchy == false)
                {
                    CurTarget = null;
                }
                //현재 설정된 타겟이 있거나, bool. help_signal true일때
                else if (CurTarget != null || Help_Signal)
                {

                    AI = AIPriority.KeepAttack;

                    if (Help_Signal)
                    {
                        //아군 챔프는 레이어로 판단?
                        //헬프시그널, 유효한 범위에 아군이 있고 그 아군이 적챔프한테 맞았을때, 
                        //타겟을 적 챔프로 바꾼다 . 
                        //각각의 챔피언은 챔피언을 타격하고있는지에 대한 bool 값을 가지고 있다.
                        //curtarget = Attacked_Champion
                    }
                    //챔피언이 부쉬 속으로 도망갔다면.
                    //if(ChampInBush)
                    //{
                    //  CurTarget = null;
                    //  break;
                    //}
                    var dist = Vector3.Distance(CurTarget.transform.position, this.transform.position);
                    if (dist > 6)
                    {
                        CurTarget = null;
                    }
                    if (CurTarget != null)
                    {   //공격함수
                        Attack();
                    }
                }
                //타겟리스트 속의 우선순위 설정
                // 타겟에 리스트가 0보다 크다면
                //else if (CurTarget == null)
                //{
                //    AI = AIPriority.FindNewTarget;
                //    //TargetPriority();
                //    ////공격 우선순위 따라.......
                //    //// 우선순위 1 챔피언 공격 하는자.
                //    //// 가장 가까운 미니언 

                //}
                else if (CurTarget == null)
                {
                    AI = AIPriority.GoWaypoint;
                    //한번만 실행하게.
                    Minionspeed = 5;
                    animator.SetTrigger("Walk");
                    if (!iswalk)
                    {
                        iswalk = true;
                        //StartCoroutine(FollowPath(waypoints));
                    }
                }
                // 아무것도 하지않음

            }
        }
    }

    //다음 웨이포인트 방향으로 바라보기(호동)
    protected IEnumerator TurnToFace(Vector3 lookTarget)
    {
        Vector3 dirToLookTarget = (lookTarget - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(dirToLookTarget.z, dirToLookTarget.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05f)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, 90 * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    /// <summary>
    /// 길을 벗어났을때 길의 목록중에서 가장 가까운 거리를 찾아서 돌아감
    /// 나중에 Astar 로 변환
    /// </summary>
    //protected IEnumerator OfftheLane()
    //{
    //    if (!isLane)
    //    {
    //        // 길에서 벗어났을때 에이스타

    //        //Dictionary<float, Vector3> shortWaypoint = new Dictionary<float, Vector3>();
    //        //shortWaypoint.Clear();
    //        //for (int i = 0; i < waypoints.Length; i++)
    //        //{
    //        //    float dist = Vector3.Distance(transform.position, waypoints[i]);
    //        //    shortWaypoint.Add(dist,waypoints[i]);
    //        //}
    //        ////오름차순 으로 정렬
    //        //Vector3 sorted = shortWaypoint.Min(KeyValuePair<float,Vector3)());
    //        ////targetWaypoint = waypoints[sorted];

    //        //transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, Minionspeed * Time.deltaTime);
    //        //if (transform.position == targetWaypoint)
    //        //{
    //        //    CurWaypoint = NextWaypoint;
    //        //    NextWaypoint = (NextWaypoint + 1) % waypoints.Length;
    //        //    targetWaypoint = waypoints[NextWaypoint];
    //        //    yield return new WaitForSeconds(waitTime);
    //        //    StartCoroutine(TurnToFace(targetWaypoint));
    //        //}
    //        yield return null;
    //    }
    //}

    //private void OnDrawGizmos() //씬에서 웨이포인트를 보여주는 함수(호동)
    //{
    //    Vector3 startPosition = pathHolder.GetChild(0).position;
    //    Vector3 previousPosition = startPosition;

    //    foreach (Transform waypoint in pathHolder)
    //    {
    //        previousPosition = waypoint.position;
    //        if (CurTarget != null)
    //        {
    //            Gizmos.color = Color.blue;

    //            Gizmos.DrawLine(transform.position, CurTarget.position);
    //        }
    //    }
    //}
    /// <summary>
    /// 1. 챔피언 스크립트 bool ChampAttacked 참조
    /// 2. 미니언 스크립트 bool isChampATK
    /// 3. 가장 가까운 미니언
    /// 만약 각 항목이 다수일때, 가까운 대상을 공격
    /// </summary>
    void TargetPriority()
    {
        var Colider = MinionVaildCol.GetComponent<MinionColider>().GetComponent<SphereCollider>();

        //풀링 게임오브젝트의 미니언들 중에 챔피언을 공격하는 미니언이 있는지 검사한다.
        //List<GameObject> targetCandidate = new List<GameObject>();
        //targetCandidate.Clear();
        //if (pool)
        //{
        //    var objectPool = pool.GetComponent<Minion_ObjectPool>();
        //    foreach (GameObject min in objectPool.Pool_MeleeMinionList)
        //    {
        //        if (min.gameObject.name.Contains("Minion"))
        //        {
        //            if (min.GetComponent<MinionBehavior>().isChampATk)
        //                targetCandidate.Add(min);
        //        }
        //    }
        //    foreach (GameObject min in objectPool.Pool_CasterMinionList)
        //    {
        //        if (min.gameObject.name.Contains("Minion"))
        //        {
        //            if (min.GetComponent<MinionBehavior>().isChampATk)
        //                targetCandidate.Add(min);
        //        }
        //    }
        //    foreach (GameObject min in objectPool.Pool_SiegeMinionList)
        //    {
        //        if (min.gameObject.name.Contains("Minion"))
        //        {
        //            if (min.GetComponent<MinionBehavior>().isChampATk)
        //                targetCandidate.Add(min);
        //        }
        //    }
        //    foreach (GameObject min in objectPool.Pool_SuperMinionList)
        //    {
        //        if (min.gameObject.name.Contains("Minion"))
        //        {
        //            if (min.GetComponent<MinionBehavior>().isChampATk)
        //                targetCandidate.Add(min);
        //        }
        //    }
        //}
        ////챔프를 공격하는 미니언이 있으면
        //if (targetCandidate.Count > 0)
        //{
        //    for (int i = 0; i < targetCandidate.Count; i++)
        //    {
        //        var dst = Vector3.Distance(transform.position, targetCandidate[i].transform.position);
        //        Transform tar = null;
        //        float maxDistance1 = 30;
        //        if (maxDistance1 > dst && targetCandidate[i].gameObject.activeInHierarchy)
        //        {
        //            tar = targetCandidate[i].transform;
        //            maxDistance1 = dst;
        //            CurTarget = tar;
        //        }
        //    }
        //}
        //else
        //{
        //    //MinionColider 에서 가까운 타겟을 할당
        //    return;
        //}
    }

    //공격판정용. 콜백 함수 
    protected void Callback_AttackAnimation(string Animation_Name)
    {
        CurTarget.GetComponent<MinionBehavior>().stat.Hp -= this.stat.Attack_Damage;
    }


    /// <summary>
    /// 초기에 enum 미니언 종류 설정후  스위치마다 애니메이션, 공격속도, 등 변경
    /// </summary>
    protected void Attack()
    {
        if (GetComponent<MinionBehavior>() != null)
        {
            switch (minionType)
            {
                case MinionType.Melee:
                    //var targetDead = CurTarget.GetComponent<MinionBehavior>().isDead;

                    float dis = Vector3.Distance(CurTarget.transform.position, this.transform.position);
                    if (dis < 4)
                    {
                        Minionspeed = 0;
                        if (stat.Hp < 0)
                        {
                            //int num = 0;
                            //num = FindTargetIndex(CurTarget);
                            //Target_List.RemoveAt(num);
                            CurTarget = null;
                            break;
                        }
                        if (!isAttacked)
                            StartCoroutine(AtkSDelay());
                    }
                    else
                    {
                        CurTarget = null;
                    }
                    break;
                case MinionType.Magic:
                    //var targetDead2 = CurTarget.GetComponent<MinionBehavior>().isDead;
                    float dis2 = Vector3.Distance(CurTarget.transform.position, this.transform.position);
                    if (dis2 < 10)
                    {
                        Minionspeed = 0;
                        if (stat.Hp < 0)
                        {
                            //int num = 0;
                            //num = FindTargetIndex(CurTarget);
                            //Target_List.RemoveAt(num);
                            CurTarget = null;
                            break;
                        }
                        if (!isAttacked)
                            StartCoroutine(AtkSDelay());
                    }
                    else
                    {

                    }
                    break;
                case MinionType.Siege:
                    //var targetDead3 = CurTarget.GetComponent<MinionBehavior>().isDead;
                    float dis3 = Vector3.Distance(CurTarget.transform.position, this.transform.position);
                    if (dis3 < 13)
                    {
                        Minionspeed = 0;
                        if (stat.Hp < 0)
                        {
                            //int num = 0;
                            //num = FindTargetIndex(CurTarget);
                            //Target_List.RemoveAt(num);
                            CurTarget = null;
                            break;
                        }
                        if (!isAttacked)
                            StartCoroutine(AtkSDelay());
                    }
                    else
                    {

                    }
                    break;
                case MinionType.Super:
                    break;
                default:
                    break;
            }
        }
    }
    //풀에서 받은 브로드캐스팅으로 현재타겟 제거
    public void deadMinion(GameObject min)
    {
        if (CurTarget == min.transform)
        {
            //Target_List.Remove(CurTarget.transform);
            CurTarget = null;
        }
    }
    protected IEnumerator Death()
    {
        print("Dead");
        isDead = true;
        //fadeout;

        //현재 미니언이 죽으면 미니언 풀 리스트에 미니언 타입에 따라 브로드캐스트
        if (pool)
        {
            var objectPool = pool.GetComponent<Minion_ObjectPool>();
            if (objectPool)
            {
                objectPool.minionDeath(this.gameObject);
            }
        }
        yield return new WaitForSeconds(1f);
        //Target_List.Clear();
        //isDead = false;
        //isAttacked = false;
        //CurWaypoint = 0;
        //NextWaypoint = 1;
        //AI = AIPriority.GoWaypoint;
        //SetMinion();
        //Minionspeed = 5;
        //웨이, 
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        //var MinionList = minionManager.GetComponent<MinionManager>().minionID;
        //MinionList.Add(this.transform);

        Destroy(GetComponent<MinionBehavior>());
    }
    IEnumerator AtkSDelay()
    {
        if (CurTarget.GetComponent<MinionBehavior>() != null)
        {
            iswalk = true;
            //타겟에게 일정 거리 이상 다가갔을때,
            animator.SetTrigger("Walk");
            Minionspeed = 0;
            transform.DOLookAt(CurTarget.position, 0.1f);
            animator.SetTrigger("Atk");
            CurTarget.GetComponent<MinionBehavior>().stat.Hp -= stat.Attack_Damage;
            CurTarget.GetComponent<MinionBehavior>().stat.Hp -= stat.Ability_Power;
            //애니메이션이 끝났으면
            //Callback_AttackAnimation("Attack");
            //yield return new WaitForSeconds(2f);
            isAttacked = true;
            yield return new WaitForSeconds(1f);
            isAttacked = false;
            //if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
            //{
            //}
        }


    }
    /// <summary>
    ///  미니언의 종류 구분, 레이어에 따른 팀 설정
    /// </summary>
    protected void SetMinion()
    {
        //미니언 타입 설정
        if (transform.name.Contains("Super"))
        {
            minionType = MinionType.Super;
            SetStat("Minion_Super");
        }
        else if (transform.name.Contains("Melee"))
        {
            minionType = MinionType.Melee;
            SetStat("Minion_Warrior");

        }
        else if (transform.name.Contains("Magician"))
        {
            minionType = MinionType.Magic;
            SetStat("Minion_Magician");
        }
        else if (transform.name.Contains("Siege"))
        {
            minionType = MinionType.Siege;
            SetStat("Minion_Siege");
        }
        //팀 설정
        if (transform.name.Contains("Red"))
        {
            team = TeamColor.Red;
            gameObject.layer = 14;
        }
        else if (gameObject.name.Contains("Blue"))
        {
            team = TeamColor.Blue;
            gameObject.layer = 15;
        }
        //미니언 경로 설정

    }

    public void IamDead(float time = 0)
    {
        Invoke("Dead", time);
    }

    private void Dead()
    {
        //gameObject.GetComponent<GridData>().RemoveGridData();
        if (minAtk == null)
            minAtk = transform.GetComponentInChildren<MinionAtk>();
        if (TheAIDest == null)
            TheAIDest = gameObject.GetComponent<AIDestinationSetter>();
        if (TheAIPath == null)
            TheAIPath = gameObject.GetComponent<AIPath>();
        minAtk.TheAIPath = null;
        minAtk.MoveTarget = null;
        minAtk.nowTarget = null;
        minAtk.StopAllCoroutines();
        TheAIPath.canMove = true;
        TheAIPath.canSearch = true;
        gameObject.SetActive(false);
        gameObject.GetComponent<AIDestinationSetter>().target = null;

        //gameObject.transform.position = deadVec + deadOffsetVec;
    }

    public bool HitMe(float damage = 0)
    {
        bool isDead = false;
        stat.Hp -= damage;
        //print(stat.Hp);
        if (stat.Hp < 1)
        {
            stat.Hp = 0;
            IamDead(0.2f);
            if (minAtk == null)
                minAtk = transform.GetComponentInChildren<MinionAtk>();
            minAtk.enemiesList.Clear();
            isDead = true;
        }
        return isDead;
    }
}