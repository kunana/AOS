using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBehaviour : MonoBehaviour
{
    public StatClass.Stat towerstat;
    public float HP;
    public float defence = 55;
    public float attack_Damage;
    public string Team = "Red";
    TowerAtk myTowerAtk = null;
    SystemMessage sysmsg;
    public TowerBehaviour myNextTower;
    public SuppressorBehaviour myNextSup;
    AOSMouseCursor cursor;
    bool isDead = false;
    public bool isCanAtkMe = false;
    public AudioSource toweraudio;
    public TowerHP towerHP;
    public GameObject TowerDestroyEffect;
    private ParticleSystem DestroyEffect;
    private bool firstload = false;
    bool mouseChanged = false;
    private void Awake()
    {
        towerHP = transform.GetComponent<TowerHP>();
        if (!sysmsg)
            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        towerstat = new StatClass.Stat();
        towerstat.Hp = HP;
        towerstat.MaxHp = HP;
        towerstat.Attack_Damage = attack_Damage;
        towerstat.Attack_Def = defence;
        towerstat.Ability_Def = defence;
        towerstat.Attack_Speed = 0.83f;
        towerstat.Level = 1;
        if (TowerDestroyEffect)
        {
            TowerDestroyEffect.SetActive(false);
            DestroyEffect = TowerDestroyEffect.GetComponent<ParticleSystem>();
        }

        toweraudio = GetComponent<AudioSource>();
        toweraudio.minDistance = 1.0f;
        toweraudio.maxDistance = 30.0f;
        toweraudio.volume = 0.5f;
        toweraudio.spatialBlend = 0.5f;
        toweraudio.rolloffMode = AudioRolloffMode.Linear;
    }
    private void OnEnable()
    {
        if (firstload)
            towerHP.BasicSetting();
        firstload = true;
        myTowerAtk = transform.GetComponentInChildren<TowerAtk>();
        if (!cursor)
            cursor = GameObject.FindGameObjectWithTag("MouseCursor").GetComponent<AOSMouseCursor>();

    }

    private void Start()
    {
        firstload = true;
    }
    public void InitTowerStatus()
    {
        towerstat.Hp = towerstat.MaxHp;
        towerHP.InitProgressBar();
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

            if (Team.ToLower().Equals("red"))
            {
                GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>().blueTeamTowerKill++;
            }
            else
            {
                GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>().redTeamTowerKill++;
            }
        }
        return isDead;
    }

    public void IamDead(float time = 0)
    {
        Invoke("Dead", time);
    }
    private void Dead()
    {
        InitTowerStatus();
        if (myTowerAtk == null)
            myTowerAtk = transform.GetComponentInChildren<TowerAtk>();
        myTowerAtk.StopAllCoroutines();
        myTowerAtk.nowTarget = null;
        if (PhotonNetwork.isMasterClient) // 마스터가 한번만.
        {
            if (Team.ToLower().Equals("red")) // 타워가 레드팀이면
            {
                sysmsg.Annoucement(8, false, "red"); // 파괴
                sysmsg.Annoucement(9, false, "blue"); // 적 파괴
            }
            else if (Team.ToLower().Equals("blue"))
            {
                sysmsg.Annoucement(8, false, "blue");
                sysmsg.Annoucement(9, false, "red");
            }
        }
        if (myNextTower != null)
            myNextTower.isCanAtkMe = true;
        else if (myNextSup != null)
        {
            if (myNextSup.tag.Equals("Nexus"))
            {
                myNextSup.nexusAtkNum += 10;
                if (myNextSup.nexusAtkNum >= 21)
                    myNextSup.isCanAtkMe = true;
            }
            else
                myNextSup.isCanAtkMe = true;
        }
        TowerDestroyEffect.SetActive(true);
        DestroyEffect.Play();
        toweraudio.PlayOneShot(SoundManager.instance.Building_Destroy);
        gameObject.SetActive(false);

        // 죽을때 마우스바뀐상태면 원래대로
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
        }
    }

    private void OnMouseOver()
    {
        if (Team.ToString().ToLower().Equals(PhotonNetwork.player.GetTeam().ToString().ToLower()))
        {
            cursor.SetCursor(1, Vector2.zero);
            mouseChanged = true;
        }
        else if (!Team.ToString().ToLower().Equals(PhotonNetwork.player.GetTeam().ToString().ToLower()))
        {
            cursor.SetCursor(2, Vector2.zero);
            mouseChanged = true;
        }
    }

    private void OnMouseExit()
    {
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
            mouseChanged = false;
        }
    }
}