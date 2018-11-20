using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : MonoBehaviour
{
    public Vector3[] Krug_BigVec;
    public Vector3[] Krug_SmallVec;
    public Vector3[] GrompVec;
    public Vector3[] R_SentinelVec;
    public Vector3[] B_SentinelVec;
    public Vector3[] Wolf_BigVec;
    public Vector3[] Wolf_SmallVec;
    public Vector3[] Raptor_BigVec;
    public Vector3[] Raptor_SmallVec;
    public Vector3[] Rift_HeraldVec;
    public Vector3[] BaronVec;
    public Vector3[] DragonVec;
    public GameObject[] Krug_Big;
    public GameObject[] Krug_Small;
    public GameObject[] Gromp;
    public GameObject[] R_Sentinel;
    public GameObject[] B_Sentinel;
    public GameObject[] Wolf_Big;
    public GameObject[] Wolf_Small;
    public GameObject[] Raptor_Big;
    public GameObject[] Raptor_Small;
    public GameObject Rift_Herald;
    public GameObject Baron;
    public GameObject Dragon;

    void Start()
    {
        MakeMonster();
    }

    void MakeMonster()
    {
        if (!PhotonNetwork.isMasterClient)
            return;
        Krug_Big = new GameObject[2];
        Krug_Small = new GameObject[2];
        Gromp = new GameObject[2];
        R_Sentinel = new GameObject[2];
        B_Sentinel = new GameObject[2];
        Wolf_Big = new GameObject[2];
        Wolf_Small = new GameObject[4];
        Raptor_Big = new GameObject[2];
        Raptor_Small = new GameObject[4];
        for (int i = 0; i < 4; ++i)
        {
            //0123
            Wolf_Small[i] = PhotonNetwork.Instantiate("Monster/Wolf_Small", Wolf_SmallVec[i * 2], Quaternion.identity, 0);
            Wolf_Small[i].GetComponent<MonsterRespawn>().respawnRotating = Wolf_SmallVec[i * 2 + 1];
            //Wolf_Small[i].transform.SetParent(this.transform);
            Raptor_Small[i] = PhotonNetwork.Instantiate("Monster/Raptor_Small", Raptor_SmallVec[i * 2], Quaternion.identity, 0);
            Raptor_Small[i].GetComponent<MonsterRespawn>().respawnRotating = Raptor_SmallVec[i * 2 + 1];
            //Raptor_Small[i].transform.SetParent(this.transform);

            if (i > 1)
                continue;

            //01
            Krug_Big[i] = PhotonNetwork.Instantiate("Monster/Krug_Big", Krug_BigVec[i * 2], Quaternion.identity, 0);
            Krug_Big[i].GetComponent<MonsterRespawn>().respawnRotating = Krug_BigVec[i * 2 + 1];
            //Krug_Big[i].transform.SetParent(this.transform);
            Krug_Small[i] = PhotonNetwork.Instantiate("Monster/Krug_Small", Krug_SmallVec[i * 2], Quaternion.identity, 0);
            Krug_Small[i].GetComponent<MonsterRespawn>().respawnRotating = Krug_SmallVec[i * 2 + 1];
            //Krug_Small[i].transform.SetParent(this.transform);
            Gromp[i] = PhotonNetwork.Instantiate("Monster/Gromp", GrompVec[i * 2], Quaternion.identity, 0);
            Gromp[i].GetComponent<MonsterRespawn>().respawnRotating = GrompVec[i * 2 + 1];
            //Gromp[i].transform.SetParent(this.transform);
            R_Sentinel[i] = PhotonNetwork.Instantiate("Monster/R_Sentinel", R_SentinelVec[i * 2], Quaternion.identity, 0);
            R_Sentinel[i].GetComponent<MonsterRespawn>().respawnRotating = R_SentinelVec[i * 2 + 1];
            //R_Sentinel[i].transform.SetParent(this.transform);
            B_Sentinel[i] = PhotonNetwork.Instantiate("Monster/B_Sentinel", B_SentinelVec[i * 2], Quaternion.identity, 0);
            B_Sentinel[i].GetComponent<MonsterRespawn>().respawnRotating = B_SentinelVec[i * 2 + 1];
            //B_Sentinel[i].transform.SetParent(this.transform);
            Wolf_Big[i] = PhotonNetwork.Instantiate("Monster/Wolf_Big", Wolf_BigVec[i * 2], Quaternion.identity, 0);
            Wolf_Big[i].GetComponent<MonsterRespawn>().respawnRotating = Wolf_BigVec[i * 2 + 1];
            //Wolf_Big[i].transform.SetParent(this.transform);
            Raptor_Big[i] = PhotonNetwork.Instantiate("Monster/Raptor_Big", Raptor_BigVec[i * 2], Quaternion.identity, 0);
            Raptor_Big[i].GetComponent<MonsterRespawn>().respawnRotating = Raptor_BigVec[i * 2 + 1];
            //Raptor_Big[i].transform.SetParent(this.transform);

            //0
            if (i > 0)
                continue;
            Baron = PhotonNetwork.Instantiate("Monster/Baron", BaronVec[i * 2], Quaternion.identity, 0);
            Baron.GetComponent<MonsterRespawn>().respawnRotating = BaronVec[i * 2 + 1];
            //Baron.transform.SetParent(this.transform);
            Rift_Herald = PhotonNetwork.Instantiate("Monster/Rift_Herald", Rift_HeraldVec[i * 2], Quaternion.identity, 0);
            Rift_Herald.GetComponent<MonsterRespawn>().respawnRotating = Rift_HeraldVec[i * 2 + 1];
            //Rift_Herald.transform.SetParent(this.transform);
            Dragon = PhotonNetwork.Instantiate("Monster/Dragon", DragonVec[i * 2], Quaternion.identity, 0);
            Dragon.GetComponent<MonsterRespawn>().respawnRotating = DragonVec[i * 2 + 1];
            //Dragon.transform.SetParent(this.transform);
        }
    }
}