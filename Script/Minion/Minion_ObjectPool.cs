using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_ObjectPool : MonoBehaviour
{
    public static Minion_ObjectPool current; //모든 클래스에서 직접 호출 가능

    //풀링할 오브젝트
    public GameObject Pool_MeleeMinion;
    public GameObject Pool_CasterMinion;
    public GameObject Pool_SiegeMinion;
    public GameObject Pool_SuperMinion;
    public GameObject Pool_Arrow;
    public GameObject Pool_Cannonball;
    public GameObject Pool_Ward;

    public GameObject Minion_Storage; //미니언들을 Minion_Storage 밑으로 생성

    //풀링 개수
    public int PoolAmount_Melee = 50;
    public int PoolAmount_Caster = 50;
    public int PoolAmount_Siege = 50;
    public int PoolAmount_Super = 50;

    //리스트
    public List<GameObject> Pool_RedMeleeMinionList;
    public List<GameObject> Pool_RedCasterMinionList;
    public List<GameObject> Pool_RedSiegeMinionList;
    public List<GameObject> Pool_RedSuperMinionList;
    public List<GameObject> Pool_BlueMeleeMinionList;
    public List<GameObject> Pool_BlueCasterMinionList;
    public List<GameObject> Pool_BlueSiegeMinionList;
    public List<GameObject> Pool_BlueSuperMinionList;
    public List<GameObject> Pool_ArrowList;
    public List<GameObject> Pool_CannonballList;
    public List<GameObject> Pool_BlueWardList;
    public List<GameObject> Pool_RedWardList;

    public Material red_Melee;
    public Material red_Magician;
    public Material red_Siege;
    private void Awake()
    {
        //static으로 선언한 Minion_ObjectPool current에 접근
        current = this;
        PhotonNetwork.OnEventCall += ClientPoolSetting;
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= ClientPoolSetting;
    }
    // Use this for initialization
    void Start()
    {
        Pool_RedMeleeMinionList = new List<GameObject>();
        Pool_RedCasterMinionList = new List<GameObject>();
        Pool_RedSiegeMinionList = new List<GameObject>();
        Pool_RedSuperMinionList = new List<GameObject>();
        Pool_BlueMeleeMinionList = new List<GameObject>();
        Pool_BlueCasterMinionList = new List<GameObject>();
        Pool_BlueSiegeMinionList = new List<GameObject>();
        Pool_BlueSuperMinionList = new List<GameObject>();
        Pool_ArrowList = new List<GameObject>();
        Pool_CannonballList = new List<GameObject>();
        Pool_RedWardList = new List<GameObject>();
        Pool_BlueWardList = new List<GameObject>();

        MakeList(PoolAmount_Melee, Pool_MeleeMinion, Pool_RedMeleeMinionList, "Red", 0);
        MakeList(PoolAmount_Caster, Pool_CasterMinion, Pool_RedCasterMinionList, "Red", 1);
        MakeList(PoolAmount_Siege, Pool_SiegeMinion, Pool_RedSiegeMinionList, "Red", 2);
        MakeList(PoolAmount_Super, Pool_SuperMinion, Pool_RedSuperMinionList, "Red", 3);
        MakeList(PoolAmount_Melee, Pool_MeleeMinion, Pool_BlueMeleeMinionList, "Blue", 0);
        MakeList(PoolAmount_Caster, Pool_CasterMinion, Pool_BlueCasterMinionList, "Blue", 1);
        MakeList(PoolAmount_Siege, Pool_SiegeMinion, Pool_BlueSiegeMinionList, "Blue", 2);
        MakeList(PoolAmount_Super, Pool_SuperMinion, Pool_BlueSuperMinionList, "Blue", 3);
        MakeList(PoolAmount_Caster * 2, Pool_Arrow, Pool_ArrowList);
        MakeList(PoolAmount_Siege * 2, Pool_Cannonball, Pool_CannonballList);
        MakeList(15, Pool_Ward, Pool_RedWardList, "Red", -1);
        MakeList(15, Pool_Ward, Pool_BlueWardList, "Blue", -1);

        //red_Melee = Resources.Load<Material>("Minion/Material/RedT");
        //red_Magician = Resources.Load<Material>("RedMagic");
        //red_Siege = Resources.Load<Material>("RedSiege");
    }
    public void MakeList(int amount, GameObject projectile, List<GameObject> list)
    {
        for (int i = 0; i < amount; ++i)
        {
            GameObject proj = (GameObject)Instantiate(projectile, Minion_Storage.transform);
            proj.SetActive(false);
            list.Add(proj);
        }
    }
    //미니언 타입 -> 0 = 밀리, 1 = 궁수, 2 = 공성, 3 = 슈퍼
    public void MakeList(int amount, GameObject minion, List<GameObject> list, string color, int minionType)
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        for (int i = 0; i < amount; i++)
        {
            //GameObject Minion = (GameObject)Instantiate(minion);
            GameObject Minion;
            if (list.Equals(Pool_RedWardList) || list.Equals(Pool_BlueWardList))
                Minion = PhotonNetwork.Instantiate(minion.name, Vector3.zero, Quaternion.identity, 0);
            else
                Minion = PhotonNetwork.Instantiate("Minion/" + minion.name, Vector3.zero, Quaternion.identity, 0);
            int listID = 0;
            if (list.Equals(Pool_RedMeleeMinionList))
                listID = 1;
            else if (list.Equals(Pool_RedCasterMinionList))
                listID = 2;
            else if (list.Equals(Pool_RedSiegeMinionList))
                listID = 3;
            else if (list.Equals(Pool_RedSuperMinionList))
                listID = 4;
            else if (list.Equals(Pool_BlueMeleeMinionList))
                listID = 5;
            else if (list.Equals(Pool_BlueCasterMinionList))
                listID = 6;
            else if (list.Equals(Pool_BlueSiegeMinionList))
                listID = 7;
            else if (list.Equals(Pool_BlueSuperMinionList))
                listID = 8;
            else if (list.Equals(Pool_ArrowList))
                listID = 9;
            else if (list.Equals(Pool_CannonballList))
                listID = 10;
            else if (list.Equals(Pool_RedWardList))
                listID = 11;
            else if (list.Equals(Pool_BlueWardList))
                listID = 12;
            //this.photonView.RPC("ClientPoolSetting", PhotonTargets.Others, Minion.GetComponent<PhotonView>().viewID, listID, color, minionType);
            object[] datas = new object[] { (int)Minion.GetComponent<PhotonView>().viewID, (int)listID, (string)color, (int)minionType };
            PhotonNetwork.RaiseEvent((byte)20, datas, true, new RaiseEventOptions()
            {
                CachingOption = EventCaching.AddToRoomCacheGlobal,
                Receivers = ReceiverGroup.Others
            });
            // ↑ 위로 변경사항

            Minion.transform.SetParent(this.transform); // 자식을 부모 밑으로 생성
            if (color.Equals("Red"))
            {
                Minion.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
                switch (minionType)
                {
                    case 0:
                        for (int j = 0; j < Minion.transform.childCount; ++j)
                            if (Minion.transform.GetChild(j).name.Equals("mesh"))
                                Minion.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = red_Melee;
                        break;
                    case 1:
                        for (int j = 0; j < Minion.transform.childCount; ++j)
                            if (Minion.transform.GetChild(j).name.Equals("Ranged_complete_low"))
                                Minion.transform.GetChild(j).GetComponentInChildren<SkinnedMeshRenderer>().material = red_Magician;
                        break;
                    case 2:
                        for (int j = 0; j < Minion.transform.childCount; ++j)
                            if (Minion.transform.GetChild(j).name.Equals("Siege Creep"))
                                Minion.transform.GetChild(j).GetComponentInChildren<SkinnedMeshRenderer>().material = red_Siege;
                        break;
                    case 3:
                        break;
                }
            }
            else
                Minion.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
            Minion.SetActive(false);
            list.Add(Minion); //리스트에 추가
        }
    }

    public void ClientPoolSetting(byte eventcode, object content, int senderid)
    {
        if (this == null)
            return;

        if (eventcode != 20)
            return;

        object[] datas = content as object[];
        int viewID = (int)datas[0];
        int listID = (int)datas[1];
        string color = (string)datas[2];
        int minionType = (int)datas[3];

        List<GameObject> list;
        switch (listID)
        {
            case 1:
                list = Pool_RedMeleeMinionList;
                break;
            case 2:
                list = Pool_RedCasterMinionList;
                break;
            case 3:
                list = Pool_RedSiegeMinionList;
                break;
            case 4:
                list = Pool_RedSuperMinionList;
                break;
            case 5:
                list = Pool_BlueMeleeMinionList;
                break;
            case 6:
                list = Pool_BlueCasterMinionList;
                break;
            case 7:
                list = Pool_BlueSiegeMinionList;
                break;
            case 8:
                list = Pool_BlueSuperMinionList;
                break;
            case 9:
                list = Pool_ArrowList;
                break;
            case 10:
                list = Pool_CannonballList;
                break;
            case 11:
                list = Pool_RedWardList;
                break;
            case 12:
                list = Pool_BlueWardList;
                break;
            default:
                return;
        }

        GameObject Minion = PhotonView.Find(viewID).gameObject;
        Minion.transform.SetParent(this.transform); // 자식을 부모 밑으로 생성
        if (color.Equals("Red"))
        {
            Minion.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
            switch (minionType)
            {
                case 0:
                    for (int j = 0; j < Minion.transform.childCount; ++j)
                        if (Minion.transform.GetChild(j).name.Equals("mesh"))
                            Minion.transform.GetChild(j).GetComponent<SkinnedMeshRenderer>().material = red_Melee;
                    break;
                case 1:
                    for (int j = 0; j < Minion.transform.childCount; ++j)
                        if (Minion.transform.GetChild(j).name.Equals("Ranged_complete_low"))
                            Minion.transform.GetChild(j).GetComponentInChildren<SkinnedMeshRenderer>().material = red_Magician;
                    break;
                case 2:
                    for (int j = 0; j < Minion.transform.childCount; ++j)
                        if (Minion.transform.GetChild(j).name.Equals("Siege Creep"))
                            Minion.transform.GetChild(j).GetComponentInChildren<SkinnedMeshRenderer>().material = red_Siege;
                    break;
                case 3:
                    break;
            }
        }
        else
            Minion.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
        Minion.SetActive(false);
        list.Add(Minion); //리스트에 추가
    }

    public GameObject GetPooledMelee(string color) //비활성화 되어있는 전사 미니언을 활성화 시킬 때 호출
    {
        List<GameObject> l;
        if (color == "Red")
            l = Pool_RedMeleeMinionList;
        else
            l = Pool_BlueMeleeMinionList;

        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = l[i];
            if (!g.activeInHierarchy)
            {
                l.RemoveAt(i);
                l.Add(g);
                return g;
            }
        }
        return null;
    }

    public GameObject GetPooledCaster(string color) //비활성화 되어있는 전사 미니언을 활성화 시킬 때 호출
    {
        List<GameObject> l;
        if (color == "Red")
            l = Pool_RedCasterMinionList;
        else
            l = Pool_BlueCasterMinionList;
        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = l[i];
            if (!g.activeInHierarchy)
            {
                l.RemoveAt(i);
                l.Add(g);
                return g;
            }
        }
        return null;
    }

    public GameObject GetPooledSiege(string color) //비활성화 되어있는 전사 미니언을 활성화 시킬 때 호출
    {
        List<GameObject> l;
        if (color == "Red")
            l = Pool_RedSiegeMinionList;
        else
            l = Pool_BlueSiegeMinionList;
        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = l[i];
            if (!g.activeInHierarchy)
            {
                l.RemoveAt(i);
                l.Add(g);
                return g;
            }
        }
        return null;
    }

    public GameObject GetPooledSuper(string color) //비활성화 되어있는 전사 미니언을 활성화 시킬 때 호출
    {
        List<GameObject> l;
        if (color == "Red")
            l = Pool_RedSuperMinionList;
        else
            l = Pool_BlueSuperMinionList;
        for (int i = 0; i < l.Count; ++i)
        {
            GameObject g = l[i];
            if (!g.activeInHierarchy)
            {
                l.RemoveAt(i);
                l.Add(g);
                return g;
            }
        }
        return null;
    }

    public GameObject GetPooledArrow()
    {
        for (int i = 0; i < Pool_ArrowList.Count; ++i)
        {
            GameObject g = Pool_ArrowList[0];
            if (!g.activeInHierarchy)
            {
                Pool_ArrowList.RemoveAt(0);
                Pool_ArrowList.Add(g);
                return g;
            }
        }
        return null;
    }

    public GameObject GetPooledCannonball()
    {
        for (int i = 0; i < Pool_CannonballList.Count; ++i)
        {
            GameObject g = Pool_CannonballList[0];
            if (!g.activeInHierarchy)
            {
                Pool_CannonballList.RemoveAt(0);
                Pool_CannonballList.Add(g);
                return g;
            }
        }
        return null;
    }

    public GameObject GetPooledWard(string team)
    {
        GameObject g = null;
        if (team.Equals("Red"))
        {
            for (int i = 0; i < Pool_RedWardList.Count; ++i)
            {
                g = Pool_RedWardList[0];
                if (!g.activeInHierarchy)
                {
                    Pool_RedWardList.RemoveAt(0);
                    Pool_RedWardList.Add(g);
                }
                return g;
            }
        }
        else if (team.Equals("Blue"))
        {
            for (int i = 0; i < Pool_BlueWardList.Count; ++i)
            {
                g = Pool_BlueWardList[0];
                if (!g.activeInHierarchy)
                {
                    Pool_BlueWardList.RemoveAt(0);
                    Pool_BlueWardList.Add(g);
                }
                return g;
            }
        }
        return null;
    }

    /// <summary>
    /// 미니언이 죽을때 풀에 브로드캐스팅
    /// </summary>
    /// <param name="minion"></param>
    public void minionDeath(GameObject minion)
    {
        foreach (GameObject min in Pool_RedMeleeMinionList)
        {   //컴포넌트 확인
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
        foreach (GameObject min in Pool_RedCasterMinionList)
        {
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
        foreach (GameObject min in Pool_RedSiegeMinionList)
        {
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
        foreach (GameObject min in Pool_RedSuperMinionList)
        {
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
        foreach (GameObject min in Pool_BlueMeleeMinionList)
        {   //컴포넌트 확인
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
        foreach (GameObject min in Pool_BlueCasterMinionList)
        {
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
        foreach (GameObject min in Pool_BlueSiegeMinionList)
        {
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
        foreach (GameObject min in Pool_BlueSuperMinionList)
        {
            var mb = min.GetComponent<MinionBehavior>();
            if (mb)
            {
                if (mb != minion)
                    mb.deadMinion(minion);
            }
        }
    }
}