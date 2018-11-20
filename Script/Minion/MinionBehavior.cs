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

public class MinionBehavior : Photon.PunBehaviour, IPunObservable
{
    public int key = -1;
    public Vector3 spawnPoint;
    AIDestinationSetter TheAIDest;
    public MinionAtk minAtk;
    AIPath TheAIPath;
    //현재 상황을 나타내는 변수

    public enum AIPriority { CC = 0, KeepAttack, FindNewTarget, FindWaypoint, GoWaypoint, Dead }
    public enum AtkPriority { ecAtk_mc, emAtk_mc, emAtk_mm, etAtk_mm, ecAtk_mm, nearEm, nearEc }
    protected enum MinionType { Melee, Magic, Siege, Super }
    protected enum Path { Top, Mid, Bot };
    public enum TeamColor { red, blue }
    //ec = enemyChamp / mc = myTeamChamp / em = EnemyMinion / mm = myTeamMinion / et = EnemyTower

    [Header("---현재상황---")]
    [SerializeField]
    public TeamColor team;
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
    [SerializeField]
    public Transform CurTarget;

    //웨이포인트
    [Header("---웨이포인트---")]
    public Vector3[] waypoints = null;
    public int CurWaypoint = 0;
    public int NextWaypoint = 0;
    public Vector3 targetWaypoint;
    public bool isLane = true;
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
    public Vector3 DepVec;
    //스탯 클래스
    public StatClass.Stat stat;

    //캐싱
    GameObject MinionVaildCol;
    GameObject pool;
    GameObject minionManager;
    InGameManager ingameManager;
    public GameObject hpbar;
    public FogOfWarEntity fog;
    public SkinnedMeshRenderer mesh;
    public AudioSource Audio;

    int checkNum = 0;

    //마우스 커서 변경
    private AOSMouseCursor cursor;
    string curteam;
    public MinionHP minHP;
    private bool firstload = false;
    public int wayNum;
    bool mouseChanged = false;

    public void SetStat(string championName)
    {
        stat = StatClass.instance.characterData[championName].ClassCopy();
    }

    private void Awake()
    {
        ingameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();
        mesh = GetComponentInChildren<SkinnedMeshRenderer>();
        fog = GetComponent<FogOfWarEntity>();
        minHP = transform.GetComponent<MinionHP>();
        TheAIDest = gameObject.GetComponent<AIDestinationSetter>();
        minAtk = transform.GetComponentInChildren<MinionAtk>();
        Audio = GetComponentInChildren<AudioSource>();
        Audio.minDistance = 1.0f;
        Audio.maxDistance = 10.0f;
        Audio.volume = 1f;
        Audio.spatialBlend = 0.5f;
        Audio.rolloffMode = AudioRolloffMode.Linear;
        if (minAtk == null)
            print("minatk is null");
        TheAIPath = gameObject.GetComponent<AIPath>();

        if (!cursor)
            cursor = GameObject.FindGameObjectWithTag("MouseCursor").GetComponent<AOSMouseCursor>();

    }

    private void OnEnable()
    {
        isDead = false;
        //팀 설정
        if (transform.name.Contains("Red"))
        {
            team = TeamColor.red;
        }
        else if (gameObject.name.Contains("Blue"))
        {
            team = TeamColor.blue;
        }
        if (firstload)
        {
            minHP.BasicSetting();
            InitMinionStatus();
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    /// <summary>
    ///사용할 코루틴은 우선 2개? AI행동 / 유효범위 탐색용 코루틴 
    ///각 0.25초
    /// </summary>
    /// 
    private void Start()
    {
        ThePlayer = GetComponent<Player>();

        pool = GameObject.FindGameObjectWithTag("MinionPooling");
        //minionManager = GameObject.FindGameObjectWithTag("MinionManager");
        animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        SetMinion(); //오브젝트 풀링 때문에 실시간 업데이트로 적용
        InitMinionStatus();
        //Target_List = new List<Transform>();
        //StartWay();
        //minHP.BasicSetting();
        firstload = true;

    }

    private void Update()
    {
        if (stat != null)
        {
            if (stat.Hp <1 )
            {
                stat.Hp = 0;
                IamDead(0.2f);
                //NearExp();

                if (minAtk == null)
                    minAtk = transform.GetComponentInChildren<MinionAtk>();
                minAtk.enemiesList.Clear();
            }
        }
    }

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

        StopAllCoroutines();
        gameObject.SetActive(false);
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

            isAttacked = true;
            yield return new WaitForSeconds(1f);
            isAttacked = false;
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
        //미니언 경로 설정

    }

    public void IamDead(float time = 0)
    {
        //if (this == null)
        //    return;
        Invoke("Dead", time);
    }

    private void Dead()
    {
        //ingameManager.minionDeadCount += 1;
        InitMinionStatus();
        minHP.InitProgressBar();
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

        // 죽을때 마우스바뀐상태면 원래대로
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
        }
        //gameObject.transform.position = deadVec + deadOffsetVec;
    }

    public bool HitMe(float damage = 0, string atkType = "AD", GameObject atker = null) // AD, AP, FD(고정 데미지 = Fixed damage)
    {
        if (!PhotonNetwork.isMasterClient)
            return false;

        bool iamDead = false;
        if (atkType.Equals("AD") || atkType.Equals("ad"))
        {
            damage = (damage * 100f) / (100f + stat.Attack_Def);
        }
        else if (atkType.Equals("AP") || atkType.Equals("ap"))
        {
            damage = (damage * 100f) / (100f + stat.Ability_Def);
        }

        if (stat.Hp <= 0)
            return false;
        stat.Hp -= damage;
        //print(stat.Hp);
        if (stat.Hp < 1)
        {
            stat.Hp = 0;
            bool isChamp = (atker == null) ? false : true;
            int id;
            if (isChamp)
                id = atker.GetPhotonView().viewID;
            else
                id = -1;
            //if (isChamp)
            //    print("ChampAtkMinion");
            KillManager.instance.SomebodyKillMinionRPC(key, id, isChamp);
        }
        else
        {
            KillManager.instance.ChangeMinionHPRPC(key, stat.Hp);
        }
        return iamDead;
    }

    public void CallDead(float time)
    {
        isDead = true;
        stat.Hp = 0;
        IamDead(time);
        NearExp();
        if (minAtk == null)
            minAtk = transform.GetComponentInChildren<MinionAtk>();
        minAtk.enemiesList.Clear();
    }
    public void NearExp()
    {
        // 근처의 오브젝트를 찾음
        Collider[] nearCollider = Physics.OverlapSphere(transform.position, 15f);

        // 경험치
        int exp = 0;
        if (gameObject.name.Contains("Melee"))
            exp = 59;
        else if (gameObject.name.Contains("Magician"))
            exp = 29;
        else if (gameObject.name.Contains("Siege"))
            exp = 92;
        else if (gameObject.name.Contains("Super"))
            exp = 97;

        // 내가 블루팀이면 레드팀 챔피언에게만 경험치
        if (gameObject.name.Contains("Blue"))
        {
            foreach (Collider c in nearCollider)
            {
                // 근처 오브젝트 중 챔피언이면서 팀이 다른애가 있으면 경험치
                if (c.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                {
                    if (c.GetComponent<PhotonView>().owner.GetTeam().ToString().Equals("red"))
                        c.GetComponent<PhotonView>().RPC("MinionExp", c.GetComponent<PhotonView>().owner, exp);
                }
            }
        }
        // 레드팀이면 그반대
        else if (gameObject.name.Contains("Red"))
        {
            foreach (Collider c in nearCollider)
            {
                // 근처 오브젝트 중 챔피언이면서 팀이 다른애가 있으면 경험치
                if (c.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                {
                    if (c.GetComponent<PhotonView>().owner.GetTeam().ToString().Equals("blue"))
                        c.GetComponent<PhotonView>().RPC("MinionExp", c.GetComponent<PhotonView>().owner, exp);
                }
            }
        }
    }

    [PunRPC]
    public void ArrowCreate(Vector3 targetPos, float moveTime)
    {
        if (this != null)
        {
            GameObject Arrow = Minion_ObjectPool.current.GetPooledArrow();
            Arrow.SetActive(true);
            Arrow.transform.position = transform.position;
            Arrow.transform.LookAt(targetPos);
            Arrow.transform.DOMove(targetPos, moveTime, false);
            Arrow.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
        }
    }

    [PunRPC]
    public void CannonballCreate(Vector3 targetPos, float moveTime)
    {
        if (this != null)
        {
            GameObject Cannonball = Minion_ObjectPool.current.GetPooledCannonball();
            Cannonball.SetActive(true);
            Cannonball.transform.position = transform.position + (targetPos - transform.position).normalized * 2f;
            Cannonball.transform.DOMove(targetPos, moveTime, false);
            Cannonball.GetComponent<TargetProjectile>().ActiveFalse(moveTime);
        }
    }

    public void ArrowRPC(Vector3 targetPos, float moveTime)
    {
        ArrowCreate(targetPos, moveTime);
        this.photonView.RPC("ArrowCreate", PhotonTargets.Others, targetPos, moveTime);
    }

    public void CannonballRPC(Vector3 targetPos, float moveTime)
    {
        CannonballCreate(targetPos, moveTime);
        this.photonView.RPC("CannonballCreate", PhotonTargets.Others, targetPos, moveTime);
    }

    [PunRPC]
    public void HitSync(int viewID)
    {
        GameObject g = PhotonView.Find(viewID).gameObject;
        if (g != null)
        {
            if (g.tag.Equals("Minion"))
            {
                MinionBehavior mB = g.GetComponent<MinionBehavior>();
                if (mB != null)
                    mB.HitMe(stat.Attack_Damage, "AD", gameObject);
                    //print("MinionBehaviour is null");
            }
            else if (g.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                ChampionSound.instance.IamAttackedSound(Audio, g.name);
                g.GetComponent<ChampionBehavior>().HitMe(stat.Attack_Damage, "AD", gameObject, gameObject.name);
            }
        }
    }

    [PunRPC]
    public void HitSyncKey(string key)
    {
        if (TowersManager.towers[key] != null)
        {
            if (key.Contains("1") || key.Contains("2") || key.Contains("3"))
                TowersManager.towers[key].GetComponent<TowerBehaviour>().HitMe(stat.Attack_Damage);
            else
                TowersManager.towers[key].GetComponent<SuppressorBehaviour>().HitMe(stat.Attack_Damage);
        }
    }

    public void HitRPC(int viewID)
    {
        if (this.photonView == null)//나중에포톤뷰널이라고터지면무용지물인거니까지워라
            return;//이것도
        this.photonView.RPC("HitSync", PhotonTargets.Others, viewID);
    }

    public void HitRPC(string key)
    {
        this.photonView.RPC("HitSyncKey", PhotonTargets.Others, key);
    }

    public void InitMinionStatus()
    {
        stat.Hp = stat.MaxHp;
        minAtk.InitMinionStatus();

    }

    private void OnMouseOver()
    {
        if (team.ToString().ToLower().Equals(PhotonNetwork.player.GetTeam().ToString().ToLower()))
        {
            cursor.SetCursor(1, Vector2.zero);
            mouseChanged = true;
        }
        else if (!team.ToString().ToLower().Equals(PhotonNetwork.player.GetTeam().ToString().ToLower()))
        {
            cursor.SetCursor(2, Vector2.zero);
            mouseChanged = true;
        }
    }

    private void OnMouseExit()
    {
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
            mouseChanged = false;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stat != null)
        //    if (stream.isWriting)
        //    {
        //        stream.SendNext(stat.Hp);
        //    }
        //    else
        //    {
        //        stat.Hp = (float)stream.ReceiveNext();
        //    }
    }
}