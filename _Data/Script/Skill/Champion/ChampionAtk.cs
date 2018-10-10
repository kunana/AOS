using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using DG.Tweening;

public class ChampionAtk : MonoBehaviour
{
    public bool isTargetting = false;
    public AIPath TheAIPath;
    AIDestinationSetter TheAIDest;
    public float AtkRange = 3f;
    Coroutine AtkCoroutine;
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
    }

    private void Update()
    {
        if (willAtkAround)
        {
            float dist = 1000000, nowD;
            GameObject temp = null;
            for (int i = 0; i < enemiesList.Count; ++i)
            {
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
                    atkRevision = 3f;
                //if ((AtkTargetObj.transform.position - myChamp.transform.position).sqrMagnitude > AtkRange * AtkRange + atkRevision)
                if (Vector3.Distance(tempVec1, tempVec2) > AtkRange + atkRevision)
                {//머니까 겁내 뛰어가자
                    if (!TheAIPath.canMove)
                    {
                        TheAIPath.canMove = true;
                        TheAIPath.canSearch = true;
                    }
                    if (AtkCoroutine != null)
                    {
                        StopCoroutine(AtkCoroutine);
                        AtkCoroutine = null;
                    }
                }
                else
                {//패자 계속 갈구자
                    if (TheAIPath.canMove)
                    {
                        TheAIPath.canMove = false;
                        TheAIPath.canSearch = false;
                    }
                    if (AtkCoroutine == null)
                    {
                        AtkCoroutine = StartCoroutine(AtkMotion());
                    }
                }
            }
            else
                ResetTarget();
        }
        else
            ResetTarget();
        //if(TheAIDest.target.Equals(AStarTargetObj))
        //    if(Vector3.Distance(AStarTargetObj.transform.position,myChamp.transform.position) < 0.5f)
        //    {
        //        willAtkAround = false;
        //        ResetTarget();
        //    }
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
                    if (AtkTargetObj.tag.Equals("Minion"))
                    {
                        MinionBehavior behav = AtkTargetObj.GetComponent<MinionBehavior>();
                        if (behav != null)
                        {
                            if (behav.HitMe(myChampionData.mystat.Attack_Damage))
                            {
                                ResetTarget();
                            }
                        }
                    }
                    else if (AtkTargetObj.tag.Equals("Player"))
                    {//이 태그대로 할건지 바뀌는지는 모르겠음. 우선 챔피언 공격임.
                        ChampionBehavior behav = AtkTargetObj.GetComponent<ChampionBehavior>();
                        if (behav != null)
                        {
                            if (behav.HitMe(myChampionData.mystat.Attack_Damage))
                            {
                                ResetTarget();
                            }
                        }
                    }
                    else if (AtkTargetObj.tag.Equals("Tower"))
                    {
                        TowerBehaviour behav = AtkTargetObj.GetComponent<TowerBehaviour>();
                        if (behav != null)
                        {
                            if (behav.HitMe(myChampionData.mystat.Attack_Damage))
                            {
                                ResetTarget();
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void ResetTarget()
    {
        if (AtkCoroutine != null)
            StopCoroutine(AtkCoroutine);
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
            if (!myChampBehav.Team.Equals(behav.Team))
            {
                AddEnemiesList(other);
            }
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
    }

    private void AddEnemiesList(Collider other)
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
}