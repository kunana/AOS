using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChampionAnimation : Photon.PunBehaviour
{
    private AIDestinationSetter TheAIDestinationSetter;
    private AIPath TheAIPath;
    private Animator animator;
    private ChampionAtk myChampAtk;

    private Transform target;
    private Vector3 startPos = new Vector3(-1000, 1, 1000);

    private InGameManager inGameManager;
    private PhotonView myPhotonView;
    private void Awake()
    {
        TheAIDestinationSetter = GetComponent<AIDestinationSetter>();
        TheAIPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        myChampAtk = GetComponentInChildren<ChampionAtk>();
        myPhotonView = GetComponent<PhotonView>();

        target = TheAIDestinationSetter.target;
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
        {
            inGameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();
        }
    }

    private void Update()
    {
        if (myPhotonView.owner.Equals(PhotonNetwork.player))
            RunCheck();
    }

    public void RunCheck()
    {
        if (inGameManager == null && SceneManager.GetActiveScene().name.Equals("InGame"))
            inGameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();

        // 시작하거나 부활하면 Run을 끔
        if (Vector3.Distance(target.position, startPos) < 0.1f)
        {
            animator.SetBool("Run", false);
            LoadCheckRPC("Run", false);
        }
        // 목표와 챔피언 사이의 거리가 일정수치보다 작아도 멈춤(도착했다고 생각)
        // 디버그해보니까 x,z는 거의 일치하고 y가 0.5차이나서 0.5 살짝넘게 차이남
        else if (Vector3.Distance(target.position, transform.position) < 0.51f)
        {
            animator.SetBool("Run", false);
            LoadCheckRPC("Run", false);
        }
        // 그보다 멀경우에는 camMove 상태일때만 뛰기(공격할때 false됨)
        else
        {
            if (TheAIPath.canMove && !TheAIPath.reachedDestination && !myChampAtk.isStun)
            {
                animator.SetBool("Run", true);
                LoadCheckRPC("Run", true);
            }
            else
            {
                animator.SetBool("Run", false);
                LoadCheckRPC("Run", false);
            }
        }
    }
    public void AttackAnimation(bool value)
    {

        animator.SetBool("Attack", value);
        LoadCheckRPC("Attack", value);
    }

    public void AnimationApply(string name, bool value, float delaytime = 0)
    {
        if (delaytime == 0)
        {
            //if (value)
            //    AnimationAllOff();

            animator.SetBool(name, value);
            LoadCheckRPC(name, value);
        }
        else
        {
            StartCoroutine(DelayAnimationApply(name, value, delaytime));
        }
    }

    IEnumerator DelayAnimationApply(string name, bool value, float delaytime)
    {
        yield return new WaitForSeconds(delaytime);

        //if (value)
        //    AnimationAllOff();

        animator.SetBool(name, value);
        LoadCheckRPC(name, value);
    }

    private void LoadCheckRPC(string name, bool value)
    {
        if (inGameManager == null)
            return;

        if (!inGameManager.runOnce)
            return;

        this.photonView.RPC("AnimationSync", PhotonTargets.Others, name, value);
    }

    [PunRPC]
    public void AnimationSync(string setani, bool condition)
    {
        //if (condition)
        //    AnimationAllOff();

        animator.SetBool(setani, condition);
    }

    public void AnimationAllOff()
    {
        animator.SetBool("Run", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Q", false);
        animator.SetBool("W", false);
        animator.SetBool("E", false);
        animator.SetBool("R", false);
    }
}