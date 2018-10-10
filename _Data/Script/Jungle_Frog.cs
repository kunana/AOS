using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;


/// <summary>
/// 정글몬스터. 
/// 1. 일정 시간 후에 생성
/// 2. 플레이어한테 피격시, 자신과 가장 가까운 적에게 다가가 공격  
/// 3. 제한됨 범위까지는 플레이어 추격 및 공격 -> 범위를 넘어서면 리턴및 회복속도 급격히 증가
/// 4. 필요 변수 체력, 물리방어, 마법저항, 공격속도 + 사정거리 -> 모노를 상속받는 스텟클래스 만들어서 사용.
/// 5. 이외 변수 최초 등장시간, 부활시간, 골드, 경험치, 
/// 6. 소멸후 재등장시 증가할 스텟의 수치 (딱 한번만)
/// 7. 이펙트, 애니메이션, 사운드, + 체력바
/// </summary>
public class Jungle_Frog : MonoBehaviour
{

    //스탯 상속을 위한 클래스
    private StatClass.Stat stats = new StatClass.Stat();
    
    // 스탯 조정용 변수
    [SerializeField]
    private int deathCount = 0;

    //상황 변수 및 사용 컴포넌트 변수
    public enum Status { Idle, Attack, Trace, Fallback, Dead }
    public Status status;
    private Animation animation;
    private Animator animator;

    //공격및 추적관련 
    [SerializeField]
    private float Distance;
    [SerializeField]
    private GameObject target = null;
    private Dictionary<GameObject, Vector3> Targets;
    private List<GameObject> Players;

    //최대 이동범위 최초 크기 저장용
    public float MaxMove = 15f;
    private Vector3 Orignal_Pos;

    //애니메이션 제한 변수
    private bool isAttacked = false;
    private bool canAttack = true;
    private bool canWalk = false;
    private bool isDead = false;
    public float attackTime = 2f;
    private bool isFallback = false;

    private void Awake()
    {
        animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
         
        Targets = new Dictionary<GameObject, Vector3>();
        Players = new List<GameObject>();
        Orignal_Pos = transform.position;
        status = Status.Idle;
        //StartCoroutine(CheckStatus());
        //StartCoroutine(Action());
        attackTime = stats.Attack_Speed;

        //stats = SetJson("Jungle_Frog");
        gameObject.SetActive(false);
        Invoke("ResetMonster", stats.first_Create_Time);
    }

    void Update()
    {
        //test
        if (Input.GetKeyDown(KeyCode.Q))
        {
            stats.Hp = 0;
        }
        if(stats.Hp <=0)
        {
            stats.Hp = 0;
        }
    }
     
    /// <summary>
    /// 0.2초마다 현재 상황체크 및 변경
    /// distance_from_Origin 범위 안에 있을때, 자신과 플레이어의 거리가 13 이내라면 Trace, attack.
    /// 13보다 크다면 Fallback 
    /// </summary>
    IEnumerator CheckStatus()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.25f);
            if (stats.Hp <= 0)
            {
                status = Status.Dead;
            }
            else
            {

                if (target != null)
                {
                    var distance_from_Origin = Vector3.Distance(target.transform.position, Orignal_Pos);
                    if (distance_from_Origin < MaxMove && !isFallback)
                    {
                        Distance = Vector3.Distance(transform.position, target.transform.position);
                        if (Distance > 13)
                        {
                            status = Status.Fallback;
                        }
                        else if (Distance > stats.Attack_Range && status != Status.Fallback)
                        {
                            status = Status.Trace;
                        }
                        else
                        {
                            status = Status.Attack;
                        }
                    }
                    else
                    {
                        status = Status.Fallback;
                    }
                }
                else
                {
                    status = Status.Idle;
                }
            }

        }

    }
    /// <summary>
    /// 평상시에는 Idle, 공격당하는 순간 범위내 가장 가까운 적을 인식.
    /// 공격 범위를 넘어가면 추격후 다시 공격
    /// 몬스터의 최대 이동반경이 넘어가는 순간 원래위치로 회귀, 및 회복속도 급증
    /// </summary>
    IEnumerator Action()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.25f);
            switch (status)
            {
                case Status.Idle:
                    {
                        animator.SetBool("Idle", true);
                    }
                    break;
                case Status.Attack:
                    {
                        //공격 모션이 끝나면 회전후 공격함.
                        //에셋 받았을때 공격모션 이 끝날때 공격 판정 들어가게 할것
                        if (canAttack)
                        {
                            transform.DOLookAt(target.transform.position, 0.7f);
                            animator.SetTrigger("Atk");
                            canAttack = false;
                            StartCoroutine(AtkDealy());
                        }
                    }
                    break;
                case Status.Trace:
                    {
                        // 에셋 받으면 이동속도 받아서 추가해줄것.
                        if (!canWalk)
                        {
                            animator.SetTrigger("Walk");
                            transform.DOLookAt(target.transform.position, 1f);
                            transform.DOMove(target.transform.position, 5.0f);
                        }

                        StartCoroutine(MoveDelay());

                    }
                    break;
                case Status.Fallback:
                    {
                        // MaxMove를 넘어서면 원래 자리로 Fallback 하며 체력회복. 회복이 완료되야 다른 동작 가능
                        isFallback = true;
                        transform.DOLookAt(Orignal_Pos, 1f);
                        transform.DOMove(Orignal_Pos, 6.0f);
                        RecoveryFast(true);

                        if (stats.Hp >= (1800))
                        {
                            stats.Hp = 1800;
                            RecoveryFast(false);
                            isFallback = false;
                        }
                    }
                    break;
                case Status.Dead:
                    {
                        StopAllCoroutines();
                        // animator.SetBool("Dead", true);

                        StartCoroutine(DeadAction());
                        //페이드아웃 넣을것,비활성? 재생성? 정할것
                    }
                    break;
            }
        }

    }
    /// <summary>
    /// 애니메이션 재생이 끝나면 페이드 아웃, 페이드가 끝나면 비활성화
    /// </summary>
    IEnumerator DeadAction()
    {
        isDead = true;
        yield return new WaitForSecondsRealtime(3f);
        this.gameObject.SetActive(false);

    }
    /// <summary>
    /// 몬스터가 비활성화되었을때, 일정시간후에 리셋시키고 활성화
    /// </summary>
    private void OnDisable()
    {
        Invoke("ResetMonster", stats.Respawn_Time);
    }
    /// <summary>
    /// 죽었을때 카운팅, 0일때는 기본 스탯. 1일때는 상승스탯 불러옴
    /// </summary>
    void ResetMonster()
    {
        if (deathCount.Equals(0))
        {
            isDead = false;
            this.gameObject.SetActive(true);
            transform.position = Orignal_Pos;
            //부활애니메이션
            StartCoroutine(CheckStatus());
            StartCoroutine(Action());
            deathCount++;
        }
        else if(deathCount > 0)
        {
            isDead = false;
            this.gameObject.SetActive(true);
            transform.position = Orignal_Pos;
            //부활애니메이션
            StartCoroutine(CheckStatus());
            StartCoroutine(Action());
            deathCount++;
        }

    }

    #region DelayFunction
    IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(5f);
        canWalk = false;
    }
    IEnumerator AtkDealy()
    {
        if (!canAttack)
        {
            yield return new WaitForSeconds(stats.Attack_Speed);
            canAttack = true;

            //Re_detection(); 
        }
    }
    IEnumerator hitDelay()
    {
        yield return new WaitForSeconds(1f);
        isAttacked = false;
    }

    #endregion

    /// <summary>
    /// 가장 가까운 플레이어를 타깃으로 인식.
    /// 몬스터에 따라 콜라이더 수정필요.
    /// </summary>
    public void Re_detection()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100f);
        Transform tr = null;
        float maxDistance = 10000f;

        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].tag.Equals("Player"))
            {
                float distance = Vector3.Distance(hitColliders[i].transform.position, transform.position);
                if (maxDistance > distance)
                {
                    tr = hitColliders[i].transform;
                    maxDistance = distance;
                    target = tr.gameObject;

                }
            }
        }
    }
    /// <summary>
    /// 애니메이션 타이밍용 콜백함수
    /// </summary>
    public void AttackCallback()
    {

    }

    public void Recovery(float Dmg)
    {
        stats.Hp -= Dmg;
    }
    public void RecoveryFast(bool onOff)
    {
        if (onOff.Equals(true))
            stats.Hp += 4000 * Time.deltaTime;
    }

    public void hpDecrease(float Dmg)
    {
        stats.Hp -= Dmg;
    }


    /// <summary>
    /// 현재는 콜라이더 에 닿으면 호출, 나중에는 챔피언에게 공격 당하면으로 수정.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            isAttacked = true;
            Re_detection();
            StartCoroutine(hitDelay());
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Orignal_Pos, MaxMove);
    }
}
