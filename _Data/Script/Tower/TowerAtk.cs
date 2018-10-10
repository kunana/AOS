using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TowerAtk : MonoBehaviour
{
    private Vector3 targetDir;
    private Transform target;
    public GameObject nowTarget = null;
    public float rotationDegreePerSecond = 4;
    public GameObject crystal;
    public GameObject projectileBlue;
    public float fireInterval = 1.5f;
    private float currentRotation = 0;
    public Transform shootPointBlue;
    public GameObject magicMissilePoolObj;
    public Queue<GameObject> ProjectilePool;
    public List<GameObject> enemiesList;
    public string enemyColor = "Blue";
    public GameObject myTower = null;
    public Coroutine AtkCoroutine = null;
    public TowerBehaviour myTowerBehav = null;
    public bool isAfterDelaying = false;
    private void Awake()
    {
        enemiesList = new List<GameObject>();
        ProjectilePool = new Queue<GameObject>();
        PoolingProjectile();
        myTowerBehav = myTower.GetComponent<TowerBehaviour>();
    }

    private void PoolingProjectile(int amount = 10)
    {
        for (int i = 0; i < amount; ++i)
        {
            //GameObject obj = Instantiate(projectileBlue, magicMissilePoolObj.transform);
            GameObject obj = Instantiate(projectileBlue, shootPointBlue.position, Quaternion.identity, magicMissilePoolObj.transform);
            obj.SetActive(false);
            ProjectilePool.Enqueue(obj);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains(enemyColor) && other.tag.Equals("Minion"))
        {
            AddEnemiesList(other);
        }
        else if (other.tag.Equals("Player"))
        {
            if (other.GetComponent<ChampionBehavior>().Team.Equals(enemyColor))
                AddEnemiesList(other);
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
            if (other.gameObject.Equals(nowTarget))
                nowTarget = null;
            enemiesList.Remove(other.gameObject);
            if (enemiesList.Count.Equals(0))
            {
                nowTarget = null;
            }
        }
    }

    private void Update()
    {
        if (nowTarget == null)
        {
            if (enemiesList.Count > 0)
            {
                int priority = -1, nowP = -1; // 0 = champ, 1 = magician, 2 = melee, 3 = siege or super
                float dist = 1000000, nowD;
                GameObject temp = null;
                for (int i = 0; i < enemiesList.Count; ++i)
                {
                    if (enemiesList[i].tag.Equals("Minion"))
                    {
                        if (enemiesList[i].name.Contains("Siege") || enemiesList[i].name.Contains("Super"))
                            nowP = 3;
                        else if (enemiesList[i].name.Contains("Melee"))
                            nowP = 2;
                        else if (enemiesList[i].name.Contains("Magician"))
                            nowP = 1;
                    }
                    else if (enemiesList[i].tag.Equals("Player"))
                        nowP = 0;
                    else
                        nowP = -1;
                    if (nowP >= priority && nowP > -1)
                    {
                        priority = nowP;
                        nowD = (enemiesList[i].transform.position - myTower.transform.position).sqrMagnitude;
                        if (dist > nowD)
                        {
                            dist = nowD;
                            temp = enemiesList[i];
                        }
                    }
                }
                if (temp != null)
                    nowTarget = temp;
            }
        }
        else if (!nowTarget.activeInHierarchy)
        {
            nowTarget = null;
            target = null;
            targetDir = Vector3.forward;
        }

        if (nowTarget != null)
        {
            target = nowTarget.transform;
            targetDir = Vector3.Normalize(target.transform.position - myTower.transform.position);
            targetDir.y = 0;
        }
        currentRotation = Mathf.MoveTowardsAngle(currentRotation, Mathf.Atan2(targetDir.x, targetDir.z) / Mathf.PI * 180, rotationDegreePerSecond);
        transform.localRotation = Quaternion.AngleAxis(currentRotation, Vector3.forward);


        if (nowTarget == null)
        {
            if (AtkCoroutine != null)
            {
                StopCoroutine(AtkCoroutine);
                AtkCoroutine = null;
            }
        }
        else
        {
            if (AtkCoroutine == null)
            {
                AtkCoroutine = StartCoroutine(AtkMotion());
            }
        }
    }

    IEnumerator AtkMotion()
    {
        while (true)
        {
            if (!isAfterDelaying)
            {
                float moveTime = 0.2f;
                GameObject obj = ProjectilePool.Dequeue();
                ProjectilePool.Enqueue(obj);
                obj.SetActive(true);
                obj.transform.DOMove(target.position, moveTime, true);
                obj.GetComponent<TowerProjectile>().ActiveFalse(moveTime);
                isAfterDelaying = true;
                Invoke("AfterDelayFinish", fireInterval);
                StartCoroutine(ProjectileAtk(moveTime, nowTarget));
                yield return new WaitForSeconds(fireInterval);
            }
            else
                yield return null;
        }
    }

    IEnumerator ProjectileAtk(float moveTime, GameObject myTarget)
    {
        yield return new WaitForSeconds(moveTime);
        if (myTarget != null)
        {
            if (myTarget.tag.Equals("Minion"))
            {
                MinionBehavior behav;
                behav = myTarget.GetComponent<MinionBehavior>();
                if (behav != null)
                {
                    if (behav.HitMe(myTowerBehav.attack_Damage))
                    {
                        enemiesList.Remove(nowTarget);
                    }
                }
            }
            else if (myTarget.tag.Equals("Player"))
            {
                ChampionBehavior behav;
                behav = myTarget.GetComponent<ChampionBehavior>();
                if (behav != null)
                {
                    if (behav.HitMe(myTowerBehav.attack_Damage))
                    {
                        enemiesList.Remove(nowTarget);
                    }
                }
            }
        }
    }

    public void AfterDelayFinish()
    {
        isAfterDelaying = false;
    }
}