using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsheE : MonoBehaviour
{
    public AsheSkill mySkill;
    public FogOfWarEntity myFogEntity;
    public GameObject HawkWard;
    public void SkillOn(Vector3 dest)
    {
        transform.position = mySkill.transform.position;
        float length = Vector3.Distance(dest, transform.position);
        float time = length * 0.04f;
        ActiveFalse(dest, time);
        transform.DOMove(dest, time);
        //Vector3 v = dest - transform.position;
        //ParticleSystem.MainModule _main = transform.GetChild(0).GetComponent<ParticleSystem>().main;

        //float a = 90f - (Mathf.Atan2(v.z, v.x) * Mathf.Rad2Deg);
        //print("value : " + a);
    }
    public void ActiveFalse(Vector3 dest, float time)
    {
        dest.y = 1.5f;
        HawkWard.transform.position = dest;
        HawkWard.SetActive(false);
        Invoke("SetHawkWard", time);
        Invoke("_ActiveFalse", time + 0.1f);
    }
    private void _ActiveFalse()
    {
        gameObject.SetActive(false);
    }
    private void SetHawkWard()
    {
        HawkWard.SetActive(true);
    }
    private void OnEnable()
    {
        if (mySkill.TheChampionBehaviour.Team.Equals("Red"))
        {
            myFogEntity.faction = FogOfWar.Players.Player00;
            HawkWard.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
        }
        else if (mySkill.TheChampionBehaviour.Team.Equals("Blue"))
        {
            myFogEntity.faction = FogOfWar.Players.Player01;
            HawkWard.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
        }
    }
    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
    }
}
