using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Newtonsoft.Json;

// 전사 미니언

public class Minion_Melee : MonoBehaviour {

    //스텟 상속
    private StatClass.Stat stats;

    //스텟 조정용 변수
    [SerializeField]
    private int waveCount = 0;

    //상황 변수 및 사용 컴포넌트 변수
    public enum Status {Move, Attack, Trace, Return, Dead }
    public Status status;
    private Animation animation;
    private Animator animator;
    
    //최대 이동범위 최초 크기 저장용
    public float MaxMove = 15f;
    private Vector3 Orignal_Pos;

    //공격 및 추적 관련
    [SerializeField]
    private float Distance;
    [SerializeField]
    private GameObject target = null;
    private Dictionary<GameObject, Vector3> Targets;
    private List<GameObject> Players;

    ////최대 이동범위 최초 크기 저장용
    //public float MaxMove = 15f;
    //private Vector3 Orignal_Pos;

    //애니메이션 제한 변수
    private bool isAttacked = false;
    private bool canAttack = true;
    private bool canWalk = false;
    private bool isDead = false;
    public float attackTime = 2f;
    private bool isReturn = false;

    private void Awake()
    {
        animation = GetComponent<Animation>();
        animator = GetComponent<Animator>();
        stats = new StatClass.Stat();
        Targets = new Dictionary<GameObject, Vector3>();
        Players = new List<GameObject>();
        status = Status.Move;
        attackTime = stats.Attack_Speed;
    }

    IEnumerator Action()
    {
        yield return new WaitForSeconds(0.25f);
        switch (status)
        {
            case Status.Move: //이동
                {
                    //애니메이션 이동 코드
                }
                break;
            case Status.Attack: //공격
                {
                    if(canAttack)
                    {
                        transform.DOLookAt(target.transform.position, 0.7f);
                        //애니메이션 공격 코드
                        canAttack = false;
                        StartCoroutine(AtkDealay());
                    }
                }
                break;
            case Status.Trace: //추적
                {
                    // 에셋 받으면 이동속도 받아서 추가해줄것.
                    if (!canWalk)
                    {
                        transform.DOLookAt(target.transform.position, 1f);
                        transform.DOMove(target.transform.position, 5.0f);
                    }
                    StartCoroutine(MoveDelay());
                }
                break;
            case Status.Return: //라인 복귀
                {
                    isReturn = true;
                    transform.DOLookAt(Orignal_Pos, 1f);
                    transform.DOMove(Orignal_Pos, 6.0f);
                    //웨이 포인트 찾아가기
                }
                break;
            case Status.Dead:
                {
                    StopAllCoroutines();
                    StartCoroutine(DeadAction());
                    //돈 획득 코드, 이펙트 추가
                }
                break;
        }
    }

    IEnumerator DeadAction()
    {
        isDead = true;
        yield return new WaitForSecondsRealtime(3f);
        //죽는 모션을 회색으로 페이드하여 땅으로 없어지는 애니메이션
        this.gameObject.SetActive(false);

    }

    IEnumerator MoveDelay()
    {
        yield return new WaitForSeconds(5f);
        canWalk = false;
    }

    IEnumerator AtkDealay()
    {
        if (!canAttack)
        {
            yield return new WaitForSeconds(stats.Attack_Speed);
            canAttack = true;

            //Re_detection(); 
        }
    }
}
