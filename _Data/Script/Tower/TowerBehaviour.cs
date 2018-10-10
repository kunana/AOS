using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public float HP;
    public float defence = 55;
    public float attack_Damage;
    public string Team = "Red";
    TowerAtk myTowerAtk = null;
    private void OnEnable()
    {
        myTowerAtk = transform.GetComponentInChildren<TowerAtk>();
    }
    public bool HitMe(float damage = 0, string atkType = "AD") // AD, AP, FD(고정 데미지 = Fixed damage)
    {
        bool isDead = false;
        damage = (damage * 100f) / (100f + defence);
        HP -= damage;
        if (HP < 1)
        {
            HP = 0;
            IamDead(0.2f);
            isDead = true;
        }
        return isDead;
    }

    public void IamDead(float time = 0)
    {
        Invoke("Dead", time);
    }
    private void Dead()
    {
        if (myTowerAtk == null)
            myTowerAtk = transform.GetComponentInChildren<TowerAtk>();
        myTowerAtk.StopAllCoroutines();
        myTowerAtk.nowTarget = null;
        gameObject.SetActive(false);
    }
}