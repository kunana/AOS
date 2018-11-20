using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class ChampionBehavior : Photon.PunBehaviour
{
    public ChampionData myChampionData = null;
    public ChampionAtk myChampAtk = null;
    public string Team = "Red";
    private AOSMouseCursor cursor;
    public bool isDead = false;
    public ChampionHP ChampHP;

    private float reviveTime = 10.0f;
    private Text UIReviveTime;
    private DeadEffect deadEffect;

    public SkinnedMeshRenderer mesh;
    ChampionAnimation myChampionAnimation;
    private Vector3 startPos = new Vector3(-1000, 1, 200);
    private FogOfWarEntity fog;
    private Rigidbody rigidbody;
    private GameObject icon;

    public GameObject arrowPrefab = null;
    List<GameObject> arrow = new List<GameObject>();
    bool mouseChanged = false;
    bool hpinit = false;

    public PhotonView myPhotonView;
    public AudioSource audio;

    private List<assistData> assistCheckList = new List<assistData>();
    private class assistData
    {
        public int viewID = 0;
        public float LastDamagedTime = 0;
    }

    private void OnEnable()
    {
        audio = GetComponent<AudioSource>();
        fog = GetComponent<FogOfWarEntity>();
        mesh = GetComponent<SkinnedMeshRenderer>();
        myChampionData = GetComponent<ChampionData>();
        myChampionAnimation = GetComponent<ChampionAnimation>();
        ChampHP = transform.GetComponent<ChampionHP>();
        icon = transform.parent.GetComponentInChildren<ChampionIcon>().gameObject;
        rigidbody = GetComponent<Rigidbody>();
        myPhotonView = GetComponent<PhotonView>();
        if (myPhotonView.owner.GetTeam().ToString().Equals("blue"))
            Team = "Blue";
        if (photonView.owner.Equals(PhotonNetwork.player) && SceneManager.GetActiveScene().name.Equals("Selection"))
            ChampionSound.instance.SelectionVoice(PlayerData.Instance.championName);

        if (!myPhotonView.isMine) // 사운드
        {
            audio.volume = 0.5f;
            audio.loop = false;
            audio.spatialBlend = 1f;
            audio.rolloffMode = AudioRolloffMode.Linear;
            audio.maxDistance = 20f;
        }
    }

    private void Update()
    {
        if (GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
        {
            if (isDead)
            {
                if (UIReviveTime == null)
                    UIReviveTime = myChampionData.UIIcon.ReviveTime;

                if (deadEffect == null)
                    deadEffect = Camera.main.GetComponentInChildren<DeadEffect>();

                reviveTime -= Time.deltaTime;
                UIReviveTime.text = Mathf.FloorToInt(reviveTime).ToString();

                if (Camera.main.GetComponent<RTS_Cam.RTS_Camera>().targetFollow != null)
                    Camera.main.GetComponent<RTS_Cam.RTS_Camera>().ResetTarget();

                if (reviveTime < 0)
                {
                    reviveTime = 10.0f;
                    // UIText 변경
                    UIReviveTime.text = "";
                    PlayerData.Instance.isDead = false;

                    ReviveSync();
                    // 카메라 캐릭터위치로 옮김
                    Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z)
                        + Camera.main.GetComponent<RTS_Cam.RTS_Camera>().targetOffset;

                    // 카메라 회색화면 끄기
                    deadEffect.TurnOff();

                    myChampionData.UIIcon.ReviveCoverImage.enabled = false;

                    myChampionData.TheAIPath.canMove = true;
                    if (PhotonNetwork.player.GetTeam().ToString().Contains("red"))
                    {
                        transform.position = myChampionData.RedPos;
                        myChampAtk.AStarTargetObj.transform.position = myChampionData.RedPos;
                    }
                    else if (PhotonNetwork.player.GetTeam().ToString().Contains("blue"))
                    {
                        transform.position = myChampionData.BluePos;
                        myChampAtk.AStarTargetObj.transform.position = myChampionData.BluePos;
                    }

                    // 부활할때 idle로
                    myChampionAnimation.AnimationAllOff();
                    this.photonView.RPC("ReviveSync", PhotonTargets.Others, null);
                }
            }
        }

        assistCheck();
    }

    public void assistCheck()
    {
        // 리스트에 무엇인가 있을때만 체크
        if (assistCheckList.Count > 0)
        {
            for (int i = 0; i < assistCheckList.Count; i++)
            {
                // 마지막 공격받은 시간이 10초가 지나면 리스트에서 삭제함
                if (Time.time - assistCheckList[i].LastDamagedTime > 10.0f)
                {
                    assistCheckList.Remove(assistCheckList[i]);
                }
                //print(assistCheckList[i].viewID + "," + assistCheckList[i].LastDamagedTime + "," + Time.time);
            }
        }
    }

    [PunRPC]
    public void ReviveSync()
    {
        if (icon != null)
        {
            icon.gameObject.SetActive(true);
        }
        ChampHP.BasicSetting();
        SoundManager.instance.ChampSound(SoundManager.instance.Champion_Respawn);
        isDead = false;
        myChampionData.totalstat.Hp = myChampionData.totalstat.MaxHp;
        myChampionData.totalstat.Mp = myChampionData.totalstat.MaxMp;

        InGameManager inGameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();
        if (Team.Equals("Blue"))
            transform.position = inGameManager.BluePos;
        else
            transform.position = inGameManager.RedPos;

        var myAstarTarget = transform.parent.Find("PlayerA*Target");
        if (myAstarTarget != null)
            myAstarTarget.position = new Vector3(transform.position.x, myAstarTarget.localPosition.y, transform.position.z);

        myChampionAnimation.AnimationAllOff();
    }

    private void OnLevelWasLoaded(int level)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
        {
            if (!cursor)
                cursor = GameObject.FindGameObjectWithTag("MouseCursor").GetComponent<AOSMouseCursor>();
            Invoke("HPSet", 0.5f);
        }
    }

    private void HPSet()
    {
        ChampHP = transform.GetComponent<ChampionHP>();

        if (ChampHP == null)
        {
            //ChampHP.BasicSetting();
            hpinit = true;
        }
    }

    public void IamDead(float time = 0)
    {
        Invoke("Dead", time);
    }

    private void Dead()
    {
        if (icon != null)
        {
            icon.gameObject.SetActive(false);
        }
        ChampionSound.instance.IamDeadSound(myChampionData.ChampionName);
        // 죽은애가 나면 데스올려라, 그리고 죽는 더미 생성
        InitChampionStatus();
        ChampHP.InitProgressBar();
        if (GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
        {
            myChampionData.death++;
            myChampionData.UIRightTop.AllUpdate();

            string dummyName = gameObject.name.Split('_')[0];
            // 더미생성
            PhotonNetwork.Instantiate("Champion/" + dummyName + "Die", transform.position, transform.rotation, 0);

            reviveTime = (myChampionData.totalstat.Level - 1) * 2.0f + 10.0f;
            myChampionData.UIIcon.ReviveCoverImage.enabled = true;

            if (deadEffect == null)
                deadEffect = Camera.main.GetComponentInChildren<DeadEffect>();
            deadEffect.TurnOn();

            // 죽으면 상점 구매가능
            PlayerData.Instance.purchaseState = true;

            // 내가가진 어시리스트애들한테 어시올려줌
            assistRPC();
        }

        myChampAtk.StopAllCoroutines();
        myChampAtk.ResetTarget();

        // 죽으면 기존에 액티브끄는 대신에 좌표를 옮겨버림.
        //gameObject.SetActive(false);
        transform.position = new Vector3(transform.position.x, -100, transform.position.z);

        // 죽을때 마우스바뀐상태면 원래대로
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
        }

        //var myAstarTarget = transform.parent.Find("PlayerA*Target");
        //if (myAstarTarget != null)
        //    myAstarTarget.localPosition = startPos;
    }

    public void assistRPC()
    {
        foreach (var item in assistCheckList)
        {
            // 죽는애가 어시한애한테 죽은애clone에 ID를 넘겨줌
            this.photonView.RPC("AssistUP", PhotonView.Find(item.viewID).owner, item.viewID);
        }
        assistCheckList.Clear();
    }

    [PunRPC]
    public void AssistUP(int viewID)
    {
        // ID를 받으면 여기는 죽은애clone 인 오브젝트니까 실제 ID를 가진 자기 챔피언을 찾아서 어시올려줌
        PhotonView.Find(viewID).gameObject.GetComponent<ChampionData>().AssistUP();
    }

    public void InitChampionStatus()
    {
        //myChampionData.totalstat.Hp = myChampionData.totalstat.MaxHp;
        //myChampionData.totalstat.Mp = myChampionData.totalstat.MaxMp;
        ChampHP.InitProgressBar();
    }

    public bool HitMe(float damage = 0, string atkType = "AD", GameObject atker = null, string killerName = "") // AD, AP, FD(고정 데미지 = Fixed damage)
    {
        if (!GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
            return false;

        if (myChampionData.totalstat.Hp < 1)
            return false;

        //print(killerName);
        // 내꺼면 때린애 확인해서 챔피언이면 어시스트 리스트에 저장해라
        int atkerViewID = 0;
        if (atker != null)
        {
            if (atker.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                atkerViewID = atker.GetComponent<PhotonView>().viewID;

                bool find = false;
                // 리스트에서 같은애가 있으면 마지막 공격받은 시간을 갱신해줌.
                for (int i = 0; i < assistCheckList.Count; i++)
                {
                    if (assistCheckList[i].viewID.Equals(atkerViewID))
                    {
                        find = true;
                        assistCheckList[i].LastDamagedTime = Time.time;
                        break;
                    }
                }
                // 못찾으면 새로운값을 추가
                if (!find)
                    assistCheckList.Add(new assistData() { viewID = atkerViewID, LastDamagedTime = Time.time });
            }
        }


        if (atkType.Equals("AD"))
        {
            damage = (damage * 100f) / (100f + myChampionData.totalstat.Attack_Def);
        }
        else if (atkType.Equals("AP"))
        {
            damage = (damage * 100f) / (100f + myChampionData.totalstat.Ability_Def);
        }

        myChampionData.totalstat.Hp -= damage;

        if (myChampionData.totalstat.Hp < 1)
        {
            myChampionData.totalstat.Hp = 0;


            //if (GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
            //{
            //    PlayerData.Instance.isDead = true;
            //    // 어시스트리스트에서 킬낸애는 빼줌
            //    for (int i = 0; i < assistCheckList.Count; i++)
            //    {
            //        if (assistCheckList[i].viewID.Equals(atkerViewID))
            //        {
            //            assistCheckList.Remove(assistCheckList[i]);
            //            break;
            //        }
            //    }
            //}
            bool isChamp = true;
            if (atker == null)
                isChamp = false;
            else if (!atker.layer.Equals(LayerMask.NameToLayer("Champion")))
                isChamp = false;
            int atkViewID = -1;
            if (atker != null)
            {
                if (isChamp)
                    atkViewID = atker.GetComponent<PhotonView>().viewID;
            }
            KillManager.instance.SomebodyKillChampionRPC(myPhotonView.viewID, atkViewID, isChamp, killerName);
        }

        if (PhotonNetwork.isMasterClient)
        {
            if (atker != null)
            {//공격한 사람이 지정되어있다(챔피언이나 미니언이 뚜까팬경우)
                if (atker.tag.Equals("ChampionAtkRange"))
                {//챔피언이냐
                    Collider[] cols = Physics.OverlapSphere(transform.position, 10);
                    for (int i = 0; i < cols.Length; ++i)//지구의 모든 아군 미니언들아 나에게 힘을 줘
                    {
                        if (cols[i].tag.Equals("Minion"))
                        {
                            if (cols[i].name.Contains(Team))
                            {//원기옥대신다구리퓽퓽
                                cols[i].GetComponent<MinionBehavior>().minAtk.SetTarget(atker);
                            }
                        }
                    }
                }
            }
        }
        return isDead;
    }

    public void CallDead(float time, int atkViewID, bool atkIsChamp)
    {
        if (!isDead)
        {
            isDead = true;
            IamDead(time);

            //PlayerData.Instance.isDead = true;
            if (atkIsChamp)
            {
                for (int i = 0; i < assistCheckList.Count; i++)
                {
                    if (assistCheckList[i].viewID.Equals(atkViewID))
                    {
                        assistCheckList.Remove(assistCheckList[i]);
                        break;
                    }
                }
            }
        }
    }


    [PunRPC]
    public void HitSync(int viewID)
    {
        GameObject g = PhotonView.Find(viewID).gameObject;
        if (g != null)
        {
            if (g.tag.Equals("Minion"))
                g.GetComponent<MinionBehavior>().HitMe(myChampionData.totalstat.Attack_Damage, "AD", this.gameObject);
            else if (g.layer.Equals(LayerMask.NameToLayer("Champion")))
                g.GetComponent<ChampionBehavior>().HitMe(myChampionData.totalstat.Attack_Damage, "AD", this.gameObject, this.name);
            else if (g.layer.Equals(LayerMask.NameToLayer("Monster")))
                g.GetComponent<MonsterBehaviour>().HitMe(myChampionData.totalstat.Attack_Damage, "AD", this.gameObject);
        }
    }

    [PunRPC]
    public void HitSyncKey(string key)
    {
        if (TowersManager.towers[key] != null)
        {
            if (key.Contains("1") || key.Contains("2") || key.Contains("3"))
                TowersManager.towers[key].GetComponent<TowerBehaviour>().HitMe(myChampionData.totalstat.Attack_Damage);
            else
                TowersManager.towers[key].GetComponent<SuppressorBehaviour>().HitMe(myChampionData.totalstat.Attack_Damage);
            //if (key.Equals("s"))
            //{//억제기
            //    TowersManager.towers[key].GetComponent<SuppressorBehaviour>().HitMe(myChampionData.totalstat.Attack_Damage);
            //}
            //else//타워
            //    TowersManager.towers[key].GetComponent<TowerBehaviour>().HitMe(myChampionData.totalstat.Attack_Damage);
        }
    }

    [PunRPC]
    public void HitSyncEffect(int viewID, string name, string key, int number, float term)
    { // 추가해야함
        GameObject g = PhotonView.Find(viewID).gameObject;
        key += "Effect";
        if (name.Contains("Alistar"))
        {
            g.GetComponent<AlistarSkill>().InvokeEffect(key, number, term);
        }
        else if (name.Contains("Mundo"))
        {
            g.GetComponent<MundoSkill>().InvokeEffect(key, number, term);
        }
        else if (name.Contains("Ashe"))
        {
            g.GetComponent<AsheSkill>().InvokeEffect(key, number, term);
        }
        else if (name.Contains("Ahri"))
        {

        }
        else if (name.Contains("Garen"))
        {

        }
    }

    [PunRPC]
    public void HitSyncEffectVector(int viewID, string name, string key, Vector3 vec, int number, float term)
    {
        GameObject g = PhotonView.Find(viewID).gameObject;
        key += "VecEffect";
        //if (name.Contains("Alistar"))
        //{
        //    g.GetComponent<AlistarSkill>().InvokeEffect(key, number, term); // 안쓰임
        //}
        //else 
        if (name.Contains("Mundo"))
        {
            g.GetComponent<MundoSkill>().InvokeVecEffect(key, number, term, vec);
        }
        else if (name.Contains("Ashe"))
        {
            g.GetComponent<AsheSkill>().InvokeVecEffect(key, number, term, vec);
        }
        else if (name.Contains("Ahri"))
        {

        }
        else if (name.Contains("Garen"))
        {

        }
    }

    [PunRPC]
    public void HitSyncSkill(int viewID, float damage, string atktype, string cc, int senderViewID)
    {
        GameObject g = PhotonView.Find(viewID).gameObject;
        GameObject atker = PhotonView.Find(senderViewID).gameObject;
        if (g != null)
        {
            if (g.tag.Equals("Minion"))
            {
                g.GetComponent<MinionBehavior>().HitMe(damage, atktype, atker);
                if (!string.IsNullOrEmpty(cc))
                {
                    if (cc.Contains("Jump"))
                    {
                        g.transform.DOJump(g.transform.position, 3, 1, 1f);
                    }
                    if (cc.Contains("Push"))
                    {
                        g.GetComponentInChildren<MinionAtk>().PushMe(Vector3.up * 3 + g.transform.position
                            + (((g.transform.position - atker.transform.position).normalized) * 5), 1f);
                    }
                }
            }
            else if (g.layer.Equals(LayerMask.NameToLayer("Champion")))
            {
                g.GetComponent<ChampionBehavior>().HitMe(damage, atktype, atker, atker.name);
                if (!string.IsNullOrEmpty(cc))
                {
                    if (cc.Contains("Jump"))
                    {
                        g.transform.DOJump(g.transform.position, 3, 1, 1f);
                    }
                    if (cc.Contains("Push"))
                    {
                        g.GetComponentInChildren<ChampionAtk>().PushMe(Vector3.up * 3 + g.transform.position
                            + (((g.transform.position - atker.transform.position).normalized) * 5), 1f);
                    }
                }
            }
            else if (g.layer.Equals(LayerMask.NameToLayer("Monster")))
            {
                g.GetComponent<MonsterBehaviour>().HitMe(damage, atktype, atker);
                if (!string.IsNullOrEmpty(cc))
                {
                    if (cc.Contains("Jump"))
                    {
                        g.transform.DOJump(g.transform.position, 3, 1, 1f);
                    }
                    if (cc.Contains("Push"))
                    {
                        g.GetComponentInChildren<MonsterAtk>().PushMe(Vector3.up * 3 + g.transform.position
                            + (((g.transform.position - atker.transform.position).normalized) * 5), 1f);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void ArrowCreate(Vector3 targetPos, float moveTime)
    {
        if (this != null)
        {
            if (arrow.Count < 1)
            {
                GameObject a;
                for (int i = 0; i < 5; ++i)
                {
                    a = Instantiate(arrowPrefab, new Vector3(0, 0, 0), Quaternion.identity, transform.parent);
                    a.SetActive(false);
                    arrow.Add(a);
                }
            }
            GameObject Arrow = arrow[0];
            arrow.RemoveAt(0);
            arrow.Add(Arrow);
            Arrow.SetActive(true);
            Arrow.transform.position = transform.position;
            Arrow.transform.LookAt(targetPos);
            Arrow.transform.DOMove(targetPos, moveTime, true).OnKill(() => { Arrow.SetActive(false); });

            TargetProjectile tp = Arrow.GetComponent<TargetProjectile>();
            if (tp != null)
                tp.ActiveFalse(moveTime);
        }
    }

    public void ArrowRPC(Vector3 targetPos, float moveTime)
    {
        ArrowCreate(targetPos, moveTime);
        this.photonView.RPC("ArrowCreate", PhotonTargets.Others, targetPos, moveTime);
    }

    [PunRPC]
    public void WardSync(string team, int champLv, Vector3 wardVec)
    {
        GameObject ward = Minion_ObjectPool.current.GetPooledWard(team);
        wardVec.y = 1;
        ward.transform.position = wardVec;
        if (!team.Equals(Team))
        {
            ward.GetComponent<MeshRenderer>().enabled = false;
        }
        ward.SetActive(true);
        ward.GetComponent<Ward>().MakeWard(team, champLv);

    }

    public void HitRPC(int viewID)
    {
        this.photonView.RPC("HitSync", PhotonTargets.Others, viewID);
    }

    public void HitRPC(string key)
    {
        this.photonView.RPC("HitSyncKey", PhotonTargets.Others, key);
    }

    public void WardRPC(string team, int champLv, Vector3 wardVec)
    {
        this.photonView.RPC("WardSync", PhotonTargets.All, Team, myChampionData.mystat.Level, wardVec);
    }

    private void OnMouseOver()
    {
        if (!SceneManager.GetActiveScene().name.Equals("InGame"))
            return;
        if (Team.ToLower().Equals(PhotonNetwork.player.GetTeam().ToString()))
        {
            if (photonView.isMine)
                return;

            cursor.SetCursor(1, Vector2.zero);
            mouseChanged = true;
        }
        else
        {
            cursor.SetCursor(2, Vector2.zero);
            mouseChanged = true;
        }
    }

    private void OnMouseExit()
    {
        if (!SceneManager.GetActiveScene().name.Equals("InGame"))
            return;
        if (mouseChanged)
        {
            cursor.SetCursor(cursor.PreCursor, Vector2.zero);
            mouseChanged = false;
        }
    }
}