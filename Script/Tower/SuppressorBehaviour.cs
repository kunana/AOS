using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuppressorBehaviour : MonoBehaviour
{
    public StatClass.Stat towerstat;
    public float HP = 3300;
    public float defence = 55;
    public string Team = "Red";
    SystemMessage sysmsg;
    public SuppressorBehaviour myNext;
    public TowerBehaviour myNextTower1;
    public TowerBehaviour myNextTower2;
    public bool isNexus = false;
    public int nexusAtkNum = 0;
    public bool isCanAtkMe = false;
    bool isDead = false;
    public bool bomb = false;
    private GameObject Destroy_Effect;
    private GameObject Destroy_Effect2;
    public GameObject Destroy_Effect3;
    private AOSMouseCursor cursor;
    SuppressorRevive TheSupRevive;
    AudioSource Audio;
    GameObject crystal;
    GameObject Stone;
    SupHP suphp;

    private void Awake()
    {
        if (!this.gameObject.name.Contains("Sup_Container"))
        {
            Destroy_Effect = transform.GetChild(transform.childCount - 2).gameObject;
            Destroy_Effect2 = transform.GetChild(transform.childCount - 1).gameObject;
        }


        if (!sysmsg)
            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        towerstat = new StatClass.Stat();
        towerstat.Hp = HP;
        towerstat.MaxHp = HP;
        towerstat.Attack_Def = defence;
        towerstat.Ability_Def = defence;
        towerstat.Attack_Speed = 0.83f;
        towerstat.Level = 1;
        Audio = GetComponentInParent<AudioSource>();
        if (!Audio)
            Audio = gameObject.AddComponent<AudioSource>();
        Audio.maxDistance = 20;
        Audio.volume = 0.5f;
        Stone = transform.GetChild(0).gameObject;
        crystal = transform.GetChild(1).gameObject;
        suphp = GetComponent<SupHP>();
    }
    private void OnEnable()
    {
        if (!isNexus)
        {
            int num = (myNext.nexusAtkNum % 10);
            if (num > 0)
            {
                if (num == 1)
                {
                    if (myNextTower1.gameObject.activeInHierarchy)
                    {
                        myNextTower1.isCanAtkMe = false;
                    }
                    if (myNextTower2.gameObject.activeInHierarchy)
                    {
                        myNextTower2.isCanAtkMe = false;
                    }
                }
                myNext.nexusAtkNum -= 1;

            }
        }
        towerstat.Hp = towerstat.MaxHp;
        HP = towerstat.Hp;
        isDead = false;

        if (!cursor)
            cursor = GameObject.FindGameObjectWithTag("MouseCursor").GetComponent<AOSMouseCursor>();
    }
    private void Update()
    {
        if (bomb && isNexus && gameObject.activeInHierarchy)
        {
            HitMe(10000);
            bomb = false;
        }
    }
    public bool HitMe(float damage = 0)
    {
        //bool isDead = false;
        if (towerstat.Hp < 1)
            return false;
        towerstat.Hp -= damage;

        HP = towerstat.Hp; //디버그용
        if (towerstat.Hp < 1)
        {
            towerstat.Hp = 0;
            if (!isDead)
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

        if (isNexus)
        {//넥서스임
         //넥서스 터졌으니 승리 or 패배 넣으면 됨
         //넥서스콰코카ㅘㅇ 파티클켜기
            Destroy_Effect.SetActive(true);
            SoundManager.instance.PlaySound(SoundManager.instance.Nexus_Destroy);
            Destroy_Effect.GetComponent<ParticleSystem>().Play();
            crystal.GetComponent<Crystal>().isdead = true;
            crystal.transform.DOMoveY(-10f, 2.8f);
            //Destroy_Effect2.SetActive(true);
            //Destroy_Effect2.GetComponent<ParticleSystem>().Play();
            Invoke("NexusDestroy_Delay", 5f);

            return;

        }
        else
        {//억제기임
            myNext.nexusAtkNum += 1;
            if (myNext.nexusAtkNum >= 21)
                myNext.isCanAtkMe = true;
            SoundManager.instance.environmentFx(Audio, SoundManager.instance.Building_Destroy);
            if (Destroy_Effect3)
                Destroy_Effect3.GetComponent<ParticleSystem>().Play();
            if (myNext.nexusAtkNum % 10 > 0)
            {
                if (myNextTower1.gameObject.activeInHierarchy)
                {
                    myNextTower1.isCanAtkMe = true;
                }
                if (myNextTower2.gameObject.activeInHierarchy)
                {
                    myNextTower2.isCanAtkMe = true;
                }
            }

            if (PhotonNetwork.isMasterClient) // 마스터가 한번만.
            {
                if (Team.ToLower().Contains("red")) // 억제기가 레드팀이면
                {
                    sysmsg.Annoucement(11, false, "red"); // 파괴
                    sysmsg.Annoucement(10, false, "blue"); // 적 파괴
                }
                else
                {
                    sysmsg.Annoucement(11, false, "blue");
                    sysmsg.Annoucement(10, false, "red");
                }
            }
        }
        suphp.HpbarOff();
        gameObject.SetActive(false);
    }

    private void NexusDestroy_Delay()
    {

        if (Team.Equals("red") || Team.Equals("Red"))
        {
            if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
                sysmsg.GameEndUI(true);
            else
                sysmsg.GameEndUI(false);
        }
        else if (Team.Equals("blue") || Team.Equals("Blue"))
        {
            if (PhotonNetwork.player.GetTeam().ToString().Equals("blue"))
                sysmsg.GameEndUI(true);
            else
                sysmsg.GameEndUI(false);
        }
        Audio.PlayOneShot(SoundManager.instance.Nexus_Destroy);

        gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (Team.ToString().ToLower().Equals(PhotonNetwork.player.GetTeam().ToString().ToLower()))
            cursor.SetCursor(1, Vector2.zero);
        else if (!Team.ToString().ToLower().Equals(PhotonNetwork.player.GetTeam().ToString().ToLower()))
            cursor.SetCursor(2, Vector2.zero);
    }

    private void OnMouseExit()
    {
        cursor.SetCursor(cursor.PreCursor, Vector2.zero);
    }
}