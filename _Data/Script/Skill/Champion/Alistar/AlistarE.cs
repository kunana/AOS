using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlistarE : MonoBehaviour
{
    public AlistarSkill mySkill;
    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Minion"))
        {
            MinionAtk mA = other.GetComponent<MinionBehavior>().minAtk;
            float damage = mySkill.skillData.eDamage[mySkill.TheChampionData.skill_E - 1]
                + mySkill.Acalculate(mySkill.skillData.eAstat, mySkill.skillData.eAvalue);
            if (other.GetComponent<MinionBehavior>().HitMe(damage))
            {
                //여기에는 나중에 평타 만들면 플레이어의 현재 공격 타겟이 죽었을 시 초기화해주는 것을 넣자.
                mySkill.TheChampionAtk.ResetTarget();
            }
        }
    }
}
