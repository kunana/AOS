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
            MinionAtk mA = other.GetComponent<MinionBehavior>().minAtk;
            mA.PauseAtk(1f, true);
            //other.GetComponent<Rigidbody>().AddForce(0, upPower, 0);
            other.transform.DOJump(other.transform.position, 3, 1, 1f);
            float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
+ mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
            //공격 코드(데미지 등) 삽입'
            //float damage = mySkill.QSkillInfo.myskill.Damage[mySkill.QSkillInfo.myskill.skillLevel]
            //    + mySkill.QSkillInfo.Acalculate(mySkill.QSkillInfo.myskill.Astat, mySkill.QSkillInfo.myskill.Avalue);

            if (other.GetComponent<MinionBehavior>().HitMe(damage, "AP"))
            {
                //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                mySkill.TheChampionAtk.ResetTarget();
            }
        }
        else if (other.tag.Equals("Player"))
        {
            ChampionAtk cA = other.GetComponent<ChampionBehavior>().myChampAtk;
            cA.PauseAtk(1f, true);
            other.transform.DOJump(other.transform.position, 3, 1, 1f);
            float damage = mySkill.skillData.qDamage[mySkill.TheChampionData.skill_Q - 1]
+ mySkill.Acalculate(mySkill.skillData.qAstat, mySkill.skillData.qAvalue);
            if(other.GetComponent<ChampionBehavior>().HitMe(damage, "AP"))
            {
                mySkill.TheChampionAtk.ResetTarget();
            }
        }
    }
}