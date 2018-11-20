using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AlistarQ : MonoBehaviour
{
    public int upPower;
    public bool nowMake = true;
    public SphereCollider myCollider;
    public float skillRange;
    public AlistarSkill mySkill;
    private SystemMessage sysmsg;
    Sequence s;
    void OnLevelWasLoaded(int level)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Contains("InGame"))
        {
            if (!sysmsg)
                sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        }
    }
    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        myCollider.enabled = false;
        skillRange = myCollider.radius;

    }
    private void OnEnable()
    {
        if (nowMake)
        {
            nowMake = false;
        }
        else
        {
            myCollider.enabled = true;
            Invoke("OffCollider", 0.2f);
        }
    }
    private void OffCollider()
    {
        myCollider.enabled = false;
    }
    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Minion"))//나중에챔피언일때도적일때도조건에추가
        {//미니언의 경우 트리거 켜진건 공격 추-적 반경이라 디스턴스를 추가
         //if (Vector3.Distance(other.transform.position, transform.position) <= skillRange)
         //{
            MinionBehavior mB = other.GetComponent<MinionBehavior>();
            if (!other.gameObject.name.Contains(mySkill.TheChampionBehaviour.Team))
            {
                mB.minAtk.PauseAtk(1f, true);
                //other.GetComponent<Rigidbody>().AddForce(0, upPower, 0);
                //s = other.transform.DOJump(other.transform.position, 3, 1, 1f).OnUpdate(() => { if (mB.isDead) if (s != null) s.Kill(); });
                float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
    + mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
                //공격 코드(데미지 등) 삽입'
                //float damage = mySkill.QSkillInfo.myskill.Damage[mySkill.QSkillInfo.myskill.skillLevel]
                //    + mySkill.QSkillInfo.Acalculate(mySkill.QSkillInfo.myskill.Astat, mySkill.QSkillInfo.myskill.Avalue);

                if (mB != null)
                {
                    int viewID = mB.GetComponent<PhotonView>().viewID;
                    //mySkill.HitRPC(viewID, damage, "AP", "Jump");
                    if (mB.HitMe(damage, "AP", mySkill.gameObject))
                    {
                        //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                        mySkill.TheChampionAtk.ResetTarget();

                        // 스킬쏜애 주인이 나면 킬올리자
                        if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        {
                            mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 1, other.transform.position);
                        }
                    }
                }
            }
        }
        //else if (other.tag.Equals("Player"))
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            ChampionBehavior cB = other.GetComponent<ChampionBehavior>();
            if (cB.Team != mySkill.TheChampionBehaviour.Team)
            {
                cB.myChampAtk.PauseAtk(1f, true);
                float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
    + mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
                if (cB != null)
                {
                    int viewID = cB.GetComponent<PhotonView>().viewID;
                    //mySkill.HitRPC(viewID, damage, "AP", "Jump");
                    if (cB.HitMe(damage, "AP", mySkill.gameObject, mySkill.name))
                    {
                        mySkill.TheChampionAtk.ResetTarget();
                        if (!sysmsg)
                            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
                        sysmsg.sendKillmsg("alistar", other.GetComponent<ChampionData>().ChampionName, mySkill.TheChampionBehaviour.Team.ToString());
                        // 스킬쏜애 주인이 나면 킬올리자
                        if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                        {
                            mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 0, other.transform.position);
                        }
                    }
                }
                //s = other.transform.DOJump(other.transform.position, 3, 1, 1f).OnUpdate(() =>
                //{
                //    if (cB.myChampionData.totalstat.Hp - ((damage * 100f) / (100f + cB.myChampionData.totalstat.Ability_Def)) <= 1)
                //    {
                //        if (s != null)
                //            s.Kill();
                //    }
                //});
            }
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            MonsterBehaviour mB = other.GetComponent<MonsterBehaviour>();
            mB.monAtk.PauseAtk(1f, true);
            //s = other.transform.DOJump(other.transform.position, 3, 1, 1f).OnUpdate(() => { if (mB.isDead) if (s != null) s.Kill(); });
            float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
+ mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
            if (mB != null)
            {
                int viewID = mB.GetComponent<PhotonView>().viewID;
                //mySkill.HitRPC(viewID, damage, "AP", "Jump");
                if (mB.HitMe(damage, "AP", mySkill.gameObject))
                {
                    mySkill.TheChampionAtk.ResetTarget();

                    //// 스킬쏜애 주인이 나면 킬올리자
                    //if (mySkill.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                    //{
                    //    mySkill.TheChampionData.Kill_CS_Gold_Exp(other.gameObject.name, 3, other.transform.position);
                    //}
                }
            }
        }
    }
}