using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class MonsterRespawn : Photon.PunBehaviour
{
    Vector3 andromeda = new Vector3(200, -100, 200);
    public string myMonsterName;
    public GameObject respawnPrefab;
    public GameObject myMonster = null;
    private Vector3 _respawnRotating;
    private GameObject myMonsterManager;
    public Vector3 respawnRotating
    {
        set
        {
            _respawnRotating = value;
        }
        get
        {
            return respawnRotating;
        }
    }
    public float birthTime; //0이면 안됨.
    public float respawnTime; //0이면 안부활.
    public float outTime; //0이면 안아웃.
    public bool isDie = true; // 죽은 상태(시작도 죽은 걸로 침)
    public bool isOut = false; // 퇴장한 상태 (전령인가 금마꺼)
    MonsterBehaviour myMonsterBehav;

    private void Awake()
    {
        myMonsterManager = GameObject.FindGameObjectWithTag("MonsterManager");
        PhotonNetwork.OnEventCall += MonsterBasicSetting;
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= MonsterBasicSetting;
    }

    private void MonsterBasicSetting(byte eventCode, object content, int senderId)
    {
        if (eventCode != 190)
            return;

        object[] datas = content as object[];
        int viewID = (int)datas[0];
        int parentViewID = (int)datas[1];

        GameObject monster = PhotonView.Find(viewID).gameObject;
        GameObject parentMonster = PhotonView.Find(parentViewID).gameObject;
        if (parentMonster.Equals(this.gameObject))
        {
            myMonster = monster;
            monster.SetActive(false);
            transform.SetParent(myMonsterManager.transform);
            monster.transform.SetParent(parentMonster.transform);
            monster.transform.DORotate(_respawnRotating, 0);
            monster.transform.localPosition = Vector3.zero;
            myMonsterBehav = monster.GetComponent<MonsterBehaviour>();
            myMonsterBehav.myCenter = parentMonster.GetComponent<MonsterRespawn>();
            myMonsterBehav.LateInit();//세부설정 불러줌
        }
        //StartCoroutine("Birth");
        //if (outTime > 0)
        //    StartCoroutine("Out");
    }

    void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (myMonster == null)
            {
                myMonster = PhotonNetwork.Instantiate("Monster/" + myMonsterName + "Obj", andromeda, Quaternion.identity, 0);

                object[] datas = new object[] { (int)myMonster.GetComponent<PhotonView>().viewID, (int)GetComponent<PhotonView>().viewID };
                PhotonNetwork.RaiseEvent((byte)190, datas, true, new RaiseEventOptions()
                {
                    CachingOption = EventCaching.AddToRoomCacheGlobal,
                    Receivers = ReceiverGroup.Others
                });

                myMonster.SetActive(false);
                transform.SetParent(myMonsterManager.transform);
                myMonster.transform.SetParent(this.transform);
                myMonster.transform.DORotate(_respawnRotating, 0);
                myMonster.transform.localPosition = Vector3.zero;
                myMonsterBehav = myMonster.GetComponent<MonsterBehaviour>();
                myMonsterBehav.myCenter = this;
                myMonsterBehav.LateInit();
            }

            this.photonView.RPC("BirthStart", PhotonTargets.All, null);
            if (outTime > 0)
                this.photonView.RPC("OutStart", PhotonTargets.All, null);

            //StartCoroutine("Birth");
            //if (outTime > 0)
            //    StartCoroutine("Out");


            //PhotonNetwork.RaiseEvent((byte)191, null, true, new RaiseEventOptions()
            //{
            //    Receivers = ReceiverGroup.All
            //});
        }
    }

    [PunRPC]
    public void BirthStart()
    {
        StartCoroutine("Birth");
    }

    [PunRPC]
    public void OutStart()
    {
        StartCoroutine("Out");
    }

    public void SetPosition()
    {
        //myMonster.transform.position = transform.position;
        myMonster.transform.localPosition = Vector3.zero;
        myMonster.transform.DORotate(_respawnRotating, 0.5f);
    }

    IEnumerator Birth()
    {
        yield return new WaitForSeconds(birthTime);
        myMonsterBehav.SetStat(0);
        myMonster.SetActive(true);
        isDie = false;
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnTime);
        myMonsterBehav.SetStat(1);
        myMonsterBehav.ReturnOtherClients(false);
        myMonsterBehav.InitValue();
        myMonster.SetActive(true);
        isDie = false;
    }

    IEnumerator Out()
    {
        yield return new WaitForSeconds(outTime);
        myMonster.SetActive(false);
        isOut = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        if (other.gameObject.Equals(myMonster))
        {
            myMonsterBehav.monAtk.StartReturn();
        }
        else if (myMonsterBehav.friendsList.Contains(other.gameObject))
        {
            myMonsterBehav.monAtk.StartReturn();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            if (!other.gameObject.Equals(myMonster))
            {
                if (!myMonsterBehav.friendsList.Contains(other.gameObject))
                {
                    myMonsterBehav.friendsList.Add(other.gameObject);
                }
            }
        }
    }
}