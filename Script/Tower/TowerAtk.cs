using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TowerAtk : Photon.PunBehaviour
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

    Transform CurTargetForSound = null;
    private SystemMessage sysmsg;
    bool once = false;
    private void Awake()
    {
        enemiesList = new List<GameObject>();
        ProjectilePool = new Queue<GameObject>();
        shootPointBlue = transform.GetChild(0).transform;
        PoolingProjectile();
        myTowerBehav = myTower.GetComponent<TowerBehaviour>();
        sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
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
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
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
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
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
        if (!PhotonNetwork.isMasterClient)
            return;

        if (nowTarget == null)
        {
            if (enemiesList.Count > 0)
            {
                int priority = -1, nowP = -1; // 0 = champ, 1 = magician, 2 = melee, 3 = siege or super
                float dist = 1000000, nowD;
                GameObject temp = null;
                Stack<int> removeNum = new Stack<int>();
                for (int i = 0; i < enemiesList.Count; ++i)
                {
                    bool c = false;
                    if (enemiesList[i] == null)
                        c = true;
                    else if (!enemiesList[i].activeInHierarchy)
                        c = true;
                    else if (enemiesList[i].transform.position.y < -50f)
                        c = true;
                    if (c)
                        removeNum.Push(i);
                }
                for (; removeNum.Count > 0; enemiesList.RemoveAt(removeNum.Pop())) ;
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
                    else if (enemiesList[i].layer.Equals(LayerMask.NameToLayer("Champion")))
                    {
                        FogOfWarEntity f = enemiesList[i].GetComponent<FogOfWarEntity>();
                        if (!f.isInTheBush)
                            nowP = 0;
                        else if (f.isInTheBushMyEnemyToo)
                            nowP = 0;
                        else
                            nowP = -1;
                    }
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
            enemiesList.Remove(nowTarget);
            nowTarget = null;
            target = null;
            targetDir = Vector3.forward;
        }

        if (nowTarget != null)
        {
            target = nowTarget.transform;
            targetDir = Vector3.Normalize(target.transform.position - myTower.transform.position);
            targetDir.y = 0;

            if (nowTarget.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                if (CurTargetForSound != target)
                {
                    WarningSound();
                }
                FogOfWarEntity f = nowTarget.GetComponent<FogOfWarEntity>();
                if (f.isInTheBush)
                    if (!f.isInTheBushMyEnemyToo)
                    {
                        nowTarget = null;
                        target = null;
                        targetDir = Vector3.forward;
                    }
            }
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
                float moveTime = 0.5f;
                //ProjectileRPC(target.position, moveTime);
                ProjectileRPC(target.GetComponent<PhotonView>().viewID, moveTime);
                //ProjectileCreate(target.position, moveTime);
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
                    int viewID = behav.GetComponent<PhotonView>().viewID;
                    HitRPC(viewID, myTowerBehav.attack_Damage);
                    SoundManager.instance.PlaySound(SoundManager.instance.Tower_Attacked);
                    if (behav.HitMe(myTowerBehav.attack_Damage))
                    {
                        myTowerBehav.toweraudio.PlayOneShot(SoundManager.instance.Tower_Attacked);
                        enemiesList.Remove(nowTarget);
                        nowTarget = null;
                    }
                }
            }
            else if (myTarget.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                ChampionBehavior behav;
                behav = myTarget.GetComponent<ChampionBehavior>();
                if (behav != null)
                {
                    ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Tower_Attacked);

                    int viewID = behav.GetComponent<PhotonView>().viewID;
                    HitRPC(viewID, myTowerBehav.attack_Damage);
                    if (behav.HitMe(myTowerBehav.attack_Damage, "AD", myTower, myTower.name))
                    {
                        enemiesList.Remove(nowTarget);
                        nowTarget = null;
                        //시스템메세지
                        sysmsg.sendKillmsg("tower", behav.GetComponent<ChampionData>().ChampionName, "ex");
                    }
                }
            }
        }
    }

    [PunRPC]
    public void ProjectileCreate(int targetViewID, float moveTime)
    {
        if (this != null)
        {
            GameObject obj = ProjectilePool.Dequeue();
            ProjectilePool.Enqueue(obj);
            obj.SetActive(true);
            //obj.transform.DOMove(targetPos, moveTime, true);
            obj.GetComponent<TowerProjectile>().target = PhotonView.Find(targetViewID).transform;
            obj.GetComponent<TowerProjectile>().ActiveFalse(moveTime);
            myTowerBehav.toweraudio.PlayOneShot(SoundManager.instance.Tower_Attack);
            Invoke("TowerAttackedSound", moveTime);
        }
    }
    private void TowerAttackedSound()
    {
        myTowerBehav.toweraudio.PlayOneShot(SoundManager.instance.Tower_Attacked);
    }

    public void ProjectileRPC(int targetViewID, float moveTime)
    {
        ProjectileCreate(targetViewID, moveTime);
        this.photonView.RPC("ProjectileCreate", PhotonTargets.Others, targetViewID, moveTime);
    }

    [PunRPC]
    public void HitSync(int viewID, float attackDamage)
    {
        GameObject g = PhotonView.Find(viewID).gameObject;
        if (g != null)
        {
            if (g.tag.Equals("Minion"))
                g.GetComponent<MinionBehavior>().HitMe(attackDamage);
            else if (g.layer.Equals(LayerMask.NameToLayer("Champion")))
                g.GetComponent<ChampionBehavior>().HitMe(attackDamage, "AD", myTower, myTower.name);
        }
    }

    public void HitRPC(int viewID, float attackDamage)
    {
        this.photonView.RPC("HitSync", PhotonTargets.Others, viewID, attackDamage);
    }

    public void AfterDelayFinish()
    {
        isAfterDelaying = false;
    }

    private void WarningSound()
    {
        CurTargetForSound = target;
        if (!once)
        {
            once = true;
            SoundManager.instance.ChampSound(SoundManager.instance.Tower_Warnig);
            Invoke("Reset", 1.5f);
        }
    }

    private void Reset()
    {
        once = false;
    }
}