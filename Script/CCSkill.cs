using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// 근우. Json 스킬 수치. 자신의 속성과 효과를 가지고 있음.
/// 자신의 트리거에 닿은 대상을 찾아서 효과를 줌. 함수(대상,시간) 
/// +이펙트,
/// </summary>
public class CCSkill : MonoBehaviour {

    public enum CCType { Stun, Slow, Snare, KnockBack}
    public enum Target { Mionion, Champion, Monster}
    public CCType cc;
    public Target tar;

    public float KnockbackPower = 5.0f;
    public float SlowPower = 2;
    public float SlowTime = 3;
    public float StunTime = 3;

 
    private void OnTriggerEnter(Collider other)
    {
        if(other.name.Contains("Minion"))
        {
            if (!other.GetComponent<MinionBehavior>().CC_hit)
                StartCoroutine(SKill(other));
            
        }
        else if (other.tag.Contains("Champion"))
        {
            
        }
        else if(other.name.Contains("Monster"))
        {
           
        }
    }

    public IEnumerator SKill(Collider target)
    {   
        switch (cc)
        {
            case CCType.Stun:
                switch (tar)
                {
                    case Target.Mionion:
                        var minion = target.GetComponent<MinionBehavior>();
                        minion.CC_hit = true;
                        minion.Minionspeed = 0;
                        yield return new WaitForSeconds(StunTime);
                        minion.CC_hit = false;

                        break;
                    case Target.Champion:
                        break;
                    case Target.Monster:
                        break;
                }
                break;
            case CCType.Slow:
                switch (tar)
                {
                    case Target.Mionion:
                        var minions = target.GetComponent<MinionBehavior>();
                        minions.CC_hit = true;
                        minions.Minionspeed /= SlowPower;
                        yield return new WaitForSeconds(SlowTime);
                        minions.CC_hit = false;
                        break;
                    case Target.Champion:
                        break;
                    case Target.Monster:
                        break;
                    default:
                        break;
                }
                
                break;
            case CCType.Snare:
                break;
            case CCType.KnockBack:
                switch (tar)
                {
                    case Target.Mionion:
                        var minions = target.GetComponent<MinionBehavior>();
                        minions.CC_hit = true;
                        var dir = target.transform.position - transform.position;
                        if(minions.CC_hit)
                        {
                           target.transform.DOMove(transform.position + dir * KnockbackPower, 0.15f);
                        }
                       
                        yield return new WaitForSeconds(3f);
                        minions.CC_hit = false;
                        break;
                    case Target.Champion:
                        break;
                    case Target.Monster:
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        
    }
}
