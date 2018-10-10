using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Spawner : MonoBehaviour
{
    //스폰 위치
    private Vector3 RedTopSpawnPointV;
    private Vector3 RedMidSpawnPointV;
    private Vector3 RedBottomSpawnPointV;
    public GameObject RedTopSpawnPoint;
    public GameObject RedMidSpawnPoint;
    public GameObject RedBottomSpawnPoint;

    private Vector3 BlueTopSpawnPointV;
    private Vector3 BlueMidSpawnPointV;
    private Vector3 BlueBottomSpawnPointV;
    public GameObject BlueTopSpawnPoint;
    public GameObject BlueMidSpawnPoint;
    public GameObject BlueBottomSpawnPoint;
    public GameObject TopCenterPoint;
    public GameObject MidCenterPoint;
    public GameObject BotCenterPoint;
    public GameObject TopNearRedPoint;
    public GameObject TopNearBluePoint;
    public GameObject MidNearRedPoint;
    public GameObject MidNearBluePoint;
    public GameObject BotNearRedPoint;
    public GameObject BotNearBluePoint;

    public bool RedTopSuppressorisBroken = false;
    public bool RedMidSuppressorisBroken = false;
    public bool RedBottomSuppressorisBroken = false;
    public bool BlueTopSuppressorisBroken = false;
    public bool BlueMidSuppressorisBroken = false;
    public bool BlueBottomSuppressorisBroken = false;

    private int WaveCount = 0;
    public float FirstSpawnT = 5;
    public float SpawnT = 60;

    float gridTerm = 0;

    public RespownZone RedTopRespownZone;
    public RespownZone RedMidRespownZone;
    public RespownZone RedBotRespownZone;
    public RespownZone BlueTopRespownZone;
    public RespownZone BlueMidRespownZone;
    public RespownZone BlueBotRespownZone;

    private bool first = true;
    //GameObject minionManager;

    // Use this for initialization
    void Start()
    {
        //minionManager = GameObject.FindGameObjectWithTag("MinionManager");
        RedTopSpawnPointV = RedTopSpawnPoint.transform.position;
        RedMidSpawnPointV = RedMidSpawnPoint.transform.position;
        RedBottomSpawnPointV = RedBottomSpawnPoint.transform.position;

        BlueTopSpawnPointV = BlueTopSpawnPoint.transform.position;
        BlueMidSpawnPointV = BlueMidSpawnPoint.transform.position;
        BlueBottomSpawnPointV = BlueBottomSpawnPoint.transform.position;

        //Spawn_MinionMelee(RedTopSpawnPointV);
        //Spawn_MinionMelee(RedMidSpawnPointV);
        //Spawn_MinionMelee(RedBottomSpawnPointV);
        //Spawn_MinionMelee(BlueTopSpawnPointV);
        //Spawn_MinionMelee(BlueMidSpawnPointV);
        //Spawn_MinionMelee(BlueBottomSpawnPointV);
        StartCoroutine(Wave());
    }

    private Vector2[] InputWayPoint(GameObject min)
    {//Blue, Red / Top, Mid, Bot
        Vector2[] nowVec = new Vector2[4];
        if (min.name.Contains("Top"))
        {
            nowVec[0] = new Vector2(90f, 90f);
            nowVec[3] = new Vector2(10f, 10f);
            nowVec[1] = new Vector2(15f, 90f);
            nowVec[2] = new Vector2(10f, 80f);

        }
        else if (min.name.Contains("Bot"))
        {
            nowVec[0] = new Vector2(90f, 90f);
            nowVec[3] = new Vector2(10f, 10f);
            nowVec[1] = new Vector2(90f, 15f);
            nowVec[2] = new Vector2(80f, 10f);
        }
        else if (min.name.Contains("Mid"))
        {
            nowVec[0] = new Vector2(90f, 90f);
            nowVec[3] = new Vector2(10f, 10f);
            nowVec[1] = new Vector2(65f, 65f);
            nowVec[2] = new Vector2(35f, 35f);
        }
        if (min.name.Contains("Blue"))
        {
            for (int i = 0; i < nowVec.Length / 2; ++i)
            {
                Vector2 v = nowVec[i];
                nowVec[i] = nowVec[nowVec.Length - i - 1];
                nowVec[nowVec.Length - i - 1] = v;
            }
        }
        return nowVec;
    }

    //int updatesCount = 0;
    //private void Update()
    //{//임시
    //    if (first)
    //    {
    //        first = false;
    //        //StartCoroutine(Wave());
    //        //Spawn_MinionMelee(RedTopSpawnPointV);

    //    }
    //    //if (updatesCount < 1 * 30)
    //    {
    //        if (updatesCount % (60*30) == 0)
    //        {
    //            Spawn_MinionMelee(RedTopSpawnPointV, "Red");
    //            Spawn_MinionMelee(RedMidSpawnPointV, "Red");
    //            Spawn_MinionMelee(RedBottomSpawnPointV, "Red");
    //            Spawn_MinionMelee(BlueTopSpawnPointV, "Blue");
    //            Spawn_MinionMelee(BlueMidSpawnPointV, "Blue");
    //            Spawn_MinionMelee(BlueBottomSpawnPointV, "Blue");
    //        }
    //    }
    //    ++updatesCount;
    //}

    void Spawn_MinionMelee(Vector3 position, string color)
    {
        GameObject MinionMelee = Minion_ObjectPool.current.GetPooledMelee(color);

        if (MinionMelee == null) return;
        MinionBehavior behav = MinionMelee.AddComponent<MinionBehavior>();

        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionMelee.name = "Minion_Red_Melee";

        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionMelee.name = "Minion_Blue_Melee";



        if (MinionMelee.transform.name.Contains("Red"))
            MinionMelee.layer = 14;
        else if (MinionMelee.name.Contains("Blue"))
            MinionMelee.layer = 15;



        if (position == RedTopSpawnPointV)
        {
            MinionMelee.name = "Minion_Red_Top_Melee";
            InitTarget(MinionMelee, TopNearRedPoint);
            SetRespownPosition(MinionMelee, RedTopRespownZone);
        }
        if (position == RedMidSpawnPointV)
        {
            MinionMelee.name = "Minion_Red_Mid_Melee";
            InitTarget(MinionMelee, MidCenterPoint);
            SetRespownPosition(MinionMelee, RedMidRespownZone);
        }
        if (position == RedBottomSpawnPointV)
        {
            MinionMelee.name = "Minion_Red_Bot_Melee";
            InitTarget(MinionMelee, BotNearRedPoint);
            SetRespownPosition(MinionMelee, RedBotRespownZone);
        }
        if (position == BlueTopSpawnPointV)
        {
            MinionMelee.name = "Minion_Blue_Top_Melee";
            InitTarget(MinionMelee, TopNearBluePoint);
            SetRespownPosition(MinionMelee, BlueTopRespownZone);
        }
        if (position == BlueMidSpawnPointV)
        {
            MinionMelee.name = "Minion_Blue_Mid_Melee";
            InitTarget(MinionMelee, MidCenterPoint);
            SetRespownPosition(MinionMelee, BlueMidRespownZone);
        }
        if (position == BlueBottomSpawnPointV)
        {
            MinionMelee.name = "Minion_Blue_Bot_Melee";
            InitTarget(MinionMelee, BotNearBluePoint);
            SetRespownPosition(MinionMelee, BlueBotRespownZone);
        }

        //MinionBehavior Behav = MinionMelee.GetComponent<MinionBehavior>();
        //Behav.gridWayPoints = InputWayPoint(MinionMelee);
        //Behav.SetGridWayPoints(InputWayPoint(MinionMelee));
        MinionMelee.GetComponent<MinionBehavior>().minAtk.nowTarget = null;
        //MinionMelee.GetComponent<AIDestinationSetter>().target = null;
        //MinionMelee.GetComponent<MinionAtk>().MoveTarget = null;

        //if (gridTerm == 0)
        //    gridTerm = GridManager.instance.gridTerm;
        //int tempX = (int)(position.x / gridTerm), tempZ = (int)(position.z / gridTerm);
        //bool fin = false;
        //if (GridManager.instance.gridObjArray[tempZ * GridManager.instance.maxX + tempX] != null)
        //{
        //    for (int i = 1; i < 5 && !fin; ++i)
        //    {//범위. 처음은 자기 주변으로 9칸, 두번째는 자기 주변 25칸
        //        for (int j = -i; j <= i && !fin; ++j)
        //        {//z축
        //            for (int k = -i; k <= i && !fin; ++k)
        //            {//x축
        //                if (GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)] == null)
        //                {
        //                    fin = true;
        //                }
        //                else if (!GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)].activeInHierarchy)
        //                {
        //                    fin = true;
        //                }
        //                if (fin)
        //                {
        //                    Vector3 tempVec = position;
        //                    tempVec.x += gridTerm * k;
        //                    tempVec.z += gridTerm * j;
        //                    MinionMelee.transform.position = tempVec;
        //                }
        //            }
        //        }
        //    }
        //    if (!fin)
        //    {
        //        MinionMelee.transform.position = position;
        //    }
        //}
        //else
        //{
        //    MinionMelee.transform.position = position;
        //}




        MinionMelee.SetActive(true);

    }

    void SetRespownPosition(GameObject obj, RespownZone rz)
    {
        for (int i = 0, len = rz.respownColliders.Length; i < len; ++i)
        {
            if (!rz.respownColliders[i].trigger)
            {
                obj.transform.position = rz.respownColliders[i].transform.position;
                break;
            }
        }
    }

    void Spawn_MinionMagician(Vector3 position, string color)
    {
        GameObject MinionMagician = Minion_ObjectPool.current.GetPooledCaster(color);

        if (MinionMagician == null) return;


        MinionBehavior Behav = MinionMagician.AddComponent<MinionBehavior>();

        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionMagician.name = "Minion_Red_Magician";

        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionMagician.name = "Minion_Blue_Magician";



        if (MinionMagician.transform.name.Contains("Red"))
            MinionMagician.layer = 14;
        else if (MinionMagician.name.Contains("Blue"))
            MinionMagician.layer = 15;




        if (position == RedTopSpawnPointV)
        {
            MinionMagician.name = "Minion_Red_Top_Magician";
            InitTarget(MinionMagician, TopNearRedPoint);
            SetRespownPosition(MinionMagician, RedTopRespownZone);
        }
        if (position == RedMidSpawnPointV)
        {
            MinionMagician.name = "Minion_Red_Mid_Magician";
            InitTarget(MinionMagician, MidCenterPoint);
            SetRespownPosition(MinionMagician, RedMidRespownZone);
        }
        if (position == RedBottomSpawnPointV)
        {
            MinionMagician.name = "Minion_Red_Bot_Magician";
            InitTarget(MinionMagician, BotNearRedPoint);
            SetRespownPosition(MinionMagician, RedBotRespownZone);
        }
        if (position == BlueTopSpawnPointV)
        {
            MinionMagician.name = "Minion_Blue_Top_Magician";
            InitTarget(MinionMagician, TopNearBluePoint);
            SetRespownPosition(MinionMagician, BlueTopRespownZone);
        }
        if (position == BlueMidSpawnPointV)
        {
            MinionMagician.name = "Minion_Blue_Mid_Magician";
            InitTarget(MinionMagician, MidCenterPoint);
            SetRespownPosition(MinionMagician, BlueMidRespownZone);
        }
        if (position == BlueBottomSpawnPointV)
        {
            MinionMagician.name = "Minion_Blue_Bot_Magician";
            InitTarget(MinionMagician, BotNearBluePoint);
            SetRespownPosition(MinionMagician, BlueBotRespownZone);
        }
        InputWayPoint(MinionMagician);

        Behav.gridWayPoints = InputWayPoint(MinionMagician);
        MinionMagician.GetComponent<MinionBehavior>().minAtk.nowTarget = null;
        //MinionMagician.GetComponent<AIDestinationSetter>().target = null;
        //MinionMagician.GetComponent<MinionAtk>().MoveTarget = null;


        //if (gridTerm == 0)
        //    gridTerm = GridManager.instance.gridTerm;
        //int tempX = (int)(position.x / gridTerm), tempZ = (int)(position.z / gridTerm);
        //bool fin = false;
        //if (GridManager.instance.gridObjArray[tempZ * GridManager.instance.maxX + tempX] != null)
        //{
        //    for (int i = 1; i < 5 && !fin; ++i)
        //    {//범위. 처음은 자기 주변으로 9칸, 두번째는 자기 주변 25칸
        //        for (int j = -i; j <= i && !fin; ++j)
        //        {//z축
        //            for (int k = -i; k <= i && !fin; ++k)
        //            {//x축
        //                if (GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)] == null)
        //                {
        //                    fin = true;
        //                }
        //                else if(!GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)].activeInHierarchy)
        //                {
        //                    fin = true;
        //                }
        //                if(fin)
        //                {
        //                    Vector3 tempVec = position;
        //                    tempVec.x += gridTerm * k;
        //                    tempVec.z += gridTerm * j;
        //                    MinionMagician.transform.position = tempVec;
        //                }
        //            }
        //        }
        //    }
        //    if (!fin)
        //    {
        //        MinionMagician.transform.position = position;
        //    }
        //}
        //else
        //{
        //    MinionMagician.transform.position = position;
        //}


        MinionMagician.SetActive(true);
    }

    void Spawn_MinionSiege(Vector3 position, string color)
    {
        GameObject MinionSiege = Minion_ObjectPool.current.GetPooledSiege(color);

        if (MinionSiege == null) return;


        MinionBehavior Behav = MinionSiege.AddComponent<MinionBehavior>();

        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionSiege.name = "Minion_Red_Siege";

        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionSiege.name = "Minion_Blue_Siege";


        //Behav.deadVec = position;

        if (MinionSiege.transform.name.Contains("Red"))
            MinionSiege.layer = 14;
        else if (MinionSiege.name.Contains("Blue"))
            MinionSiege.layer = 15;




        if (position == RedTopSpawnPointV)
        {
            MinionSiege.name = "Minion_Red_Top_Siege";
            InitTarget(MinionSiege, TopNearRedPoint);
            SetRespownPosition(MinionSiege, RedTopRespownZone);
        }
        if (position == RedMidSpawnPointV)
        {
            MinionSiege.name = "Minion_Red_Mid_Siege";
            InitTarget(MinionSiege, MidCenterPoint);
            SetRespownPosition(MinionSiege, RedMidRespownZone);
        }
        if (position == RedBottomSpawnPointV)
        {
            MinionSiege.name = "Minion_Red_Bot_Siege";
            InitTarget(MinionSiege, BotNearRedPoint);
            SetRespownPosition(MinionSiege, RedBotRespownZone);
        }
        if (position == BlueTopSpawnPointV)
        {
            MinionSiege.name = "Minion_Blue_Top_Siege";
            InitTarget(MinionSiege, TopNearBluePoint);
            SetRespownPosition(MinionSiege, BlueTopRespownZone);
        }
        if (position == BlueMidSpawnPointV)
        {
            MinionSiege.name = "Minion_Blue_Mid_Siege";
            InitTarget(MinionSiege, MidCenterPoint);
            SetRespownPosition(MinionSiege, BlueMidRespownZone);
        }
        if (position == BlueBottomSpawnPointV)
        {
            MinionSiege.name = "Minion_Blue_Bot_Siege";
            InitTarget(MinionSiege, BotNearBluePoint);
            SetRespownPosition(MinionSiege, BlueBotRespownZone);
        }
        InputWayPoint(MinionSiege);
        Behav.gridWayPoints = InputWayPoint(MinionSiege);
        MinionSiege.GetComponent<MinionBehavior>().minAtk.nowTarget = null;
        //MinionSiege.GetComponent<AIDestinationSetter>().target = null;
        //MinionSiege.GetComponent<MinionAtk>().MoveTarget = null;


        //if (gridTerm == 0)
        //    gridTerm = GridManager.instance.gridTerm;
        //int tempX = (int)(position.x / gridTerm), tempZ = (int)(position.z / gridTerm);
        //bool fin = false;
        //if (GridManager.instance.gridObjArray[tempZ * GridManager.instance.maxX + tempX] != null)
        //{
        //    for (int i = 1; i < 5 && !fin; ++i)
        //    {//범위. 처음은 자기 주변으로 9칸, 두번째는 자기 주변 25칸
        //        for (int j = -i; j <= i && !fin; ++j)
        //        {//z축
        //            for (int k = -i; k <= i && !fin; ++k)
        //            {//x축
        //                if (GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)] == null)
        //                {
        //                    fin = true;
        //                }
        //                else if (!GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)].activeInHierarchy)
        //                {
        //                    fin = true;
        //                }
        //                if (fin)
        //                {
        //                    Vector3 tempVec = position;
        //                    tempVec.x += gridTerm * k;
        //                    tempVec.z += gridTerm * j;
        //                    MinionSiege.transform.position = tempVec;
        //                }
        //            }
        //        }
        //    }
        //    if (!fin)
        //    {
        //        MinionSiege.transform.position = position;
        //    }
        //}
        //else
        //{
        //    MinionSiege.transform.position = position;
        //}


        MinionSiege.SetActive(true);
    }

    void Spawn_MinionSuper(Vector3 position, string color)
    {
        GameObject MinionSuper = Minion_ObjectPool.current.GetPooledSuper(color);

        if (MinionSuper == null) return;


        MinionBehavior Behav = MinionSuper.AddComponent<MinionBehavior>();

        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionSuper.name = "Minion_Red_Super";
            //MinionSuper.GetComponentInChildren<Renderer>().material.mainTexture = red;
        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionSuper.name = "Minion_Blue_Super";



        if (MinionSuper.transform.name.Contains("Red"))
            MinionSuper.layer = 14;
        else if (MinionSuper.name.Contains("Blue"))
            MinionSuper.layer = 15;



        if (position == RedTopSpawnPointV)
        {
            MinionSuper.name = "Minion_Red_Top_Super";
            InitTarget(MinionSuper, TopNearRedPoint);
            SetRespownPosition(MinionSuper, RedTopRespownZone);
        }
        if (position == RedMidSpawnPointV)
        {
            MinionSuper.name = "Minion_Red_Mid_Super";
            InitTarget(MinionSuper, MidCenterPoint);
            SetRespownPosition(MinionSuper, RedMidRespownZone);
        }
        if (position == RedBottomSpawnPointV)
        {
            MinionSuper.name = "Minion_Red_Bot_Super";
            InitTarget(MinionSuper, BotNearRedPoint);
            SetRespownPosition(MinionSuper, RedBotRespownZone);
        }
        if (position == BlueTopSpawnPointV)
        {
            MinionSuper.name = "Minion_Blue_Top_Super";
            InitTarget(MinionSuper, TopNearBluePoint);
            SetRespownPosition(MinionSuper, BlueTopRespownZone);
        }
        if (position == BlueMidSpawnPointV)
        {
            MinionSuper.name = "Minion_Blue_Mid_Super";
            InitTarget(MinionSuper, MidCenterPoint);
            SetRespownPosition(MinionSuper, BlueMidRespownZone);
        }
        if (position == BlueBottomSpawnPointV)
        {
            MinionSuper.name = "Minion_Blue_Bot_Super";
            InitTarget(MinionSuper, BotNearBluePoint);
            SetRespownPosition(MinionSuper, BlueBotRespownZone);
        }
        InputWayPoint(MinionSuper);
        Behav.gridWayPoints = InputWayPoint(MinionSuper);

        MinionSuper.GetComponent<MinionBehavior>().minAtk.nowTarget = null;
        //MinionSuper.GetComponent<AIDestinationSetter>().target = null;
        //MinionSuper.GetComponent<MinionAtk>().MoveTarget = null;



        //if (gridTerm == 0)
        //    gridTerm = GridManager.instance.gridTerm;
        //int tempX = (int)(position.x / gridTerm), tempZ = (int)(position.z / gridTerm);
        //bool fin = false;
        //if (GridManager.instance.gridObjArray[tempZ * GridManager.instance.maxX + tempX] != null)
        //{
        //    for (int i = 1; i < 5 && !fin; ++i)
        //    {//범위. 처음은 자기 주변으로 9칸, 두번째는 자기 주변 25칸
        //        for (int j = -i; j <= i && !fin; ++j)
        //        {//z축
        //            for (int k = -i; k <= i && !fin; ++k)
        //            {//x축
        //                if (GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)] == null)
        //                {
        //                    fin = true;
        //                }
        //                else if (!GridManager.instance.gridObjArray[(tempZ + j) * GridManager.instance.maxX + (tempX + k)].activeInHierarchy)
        //                {
        //                    fin = true;
        //                }
        //                if (fin)
        //                {
        //                    Vector3 tempVec = position;
        //                    tempVec.x += gridTerm * k;
        //                    tempVec.z += gridTerm * j;
        //                    MinionSuper.transform.position = tempVec;
        //                }
        //            }
        //        }
        //    }
        //    if (!fin)
        //    {
        //        MinionSuper.transform.position = position;
        //    }
        //}
        //else
        //{
        //    MinionSuper.transform.position = position;
        //}


        MinionSuper.SetActive(true);
    }

    private void InitTarget(GameObject minion, GameObject point)
    {
        MinionBehavior minB = minion.GetComponent<MinionBehavior>();
        MinionAtk minAtk = minB.minAtk;
        if(minAtk == null)
        {
            minB.minAtk = minB.transform.GetComponentInChildren<MinionAtk>();
            minAtk = minB.minAtk;
        }
        minion.GetComponent<AIDestinationSetter>().target = point.transform;
        minAtk.MoveTarget = point;
        minAtk.nowTarget = null;
        minion.GetComponent<MinionBehavior>().spawnPoint = point.transform.position;
    }

    IEnumerator Wave()
    {
        yield return new WaitForSeconds(FirstSpawnT);
        while (true)
        {
            WaveCount++;
            for (int i = 0; i <= 1; i++)
            {
                Spawn_MinionMelee(RedTopSpawnPointV, "Red");
                Spawn_MinionMelee(RedMidSpawnPointV, "Red");
                Spawn_MinionMelee(RedBottomSpawnPointV, "Red");
                Spawn_MinionMelee(BlueTopSpawnPointV, "Blue");
                Spawn_MinionMelee(BlueMidSpawnPointV, "Blue");
                Spawn_MinionMelee(BlueBottomSpawnPointV, "Blue");
                yield return new WaitForSeconds(1.0f);
            }
            
            if (RedTopSuppressorisBroken == true && RedMidSuppressorisBroken == true && RedBottomSuppressorisBroken == true)
            {
                Spawn_MinionSuper(RedTopSpawnPointV, "Red");
                Spawn_MinionSuper(RedMidSpawnPointV, "Red");
                Spawn_MinionSuper(RedBottomSpawnPointV, "Red");
            }
            if (RedTopSuppressorisBroken == true && WaveCount % 3 == 0)
                Spawn_MinionSuper(RedTopSpawnPointV, "Red");
            else if (WaveCount % 3 == 0)
                Spawn_MinionSiege(RedTopSpawnPointV, "Red");

            if (RedMidSuppressorisBroken == true && WaveCount % 3 == 0)
                Spawn_MinionSuper(RedMidSpawnPointV, "Red");
            else if (WaveCount % 3 == 0)
                Spawn_MinionSiege(RedMidSpawnPointV, "Red");

            if (RedBottomSuppressorisBroken == true && WaveCount % 3 == 0)
                Spawn_MinionSuper(RedBottomSpawnPointV, "Red");
            else if (WaveCount % 3 == 0)

                Spawn_MinionSiege(RedBottomSpawnPointV, "Red");

            if (BlueTopSuppressorisBroken == true && BlueMidSuppressorisBroken == true && BlueBottomSuppressorisBroken == true)
            {
                Spawn_MinionSuper(BlueTopSpawnPointV, "Blue");
                Spawn_MinionSuper(BlueMidSpawnPointV, "Blue");
                Spawn_MinionSuper(BlueBottomSpawnPointV, "Blue");
            }
            if (BlueTopSuppressorisBroken == true && WaveCount % 3 == 0)
                Spawn_MinionSuper(BlueTopSpawnPointV, "Blue");
            else if (WaveCount % 3 == 0)
                Spawn_MinionSiege(BlueTopSpawnPointV, "Blue");

            if (BlueMidSuppressorisBroken == true && WaveCount % 3 == 0)
                Spawn_MinionSuper(BlueMidSpawnPointV, "Blue");
            else if (WaveCount % 3 == 0)
                Spawn_MinionSiege(BlueMidSpawnPointV, "Blue");

            if (BlueBottomSuppressorisBroken == true && WaveCount % 3 == 0)
                Spawn_MinionSuper(BlueBottomSpawnPointV, "Blue");
            else if (WaveCount % 3 == 0)
                Spawn_MinionSiege(BlueBottomSpawnPointV, "Blue");

            if (WaveCount % 3 == 0)
                yield return new WaitForSeconds(1.0f);

            for (int i = 0; i < 3; i++)
            {
                Spawn_MinionMagician(RedTopSpawnPointV, "Red");
                Spawn_MinionMagician(RedMidSpawnPointV, "Red");
                Spawn_MinionMagician(RedBottomSpawnPointV, "Red");
                Spawn_MinionMagician(BlueTopSpawnPointV, "Blue");
                Spawn_MinionMagician(BlueMidSpawnPointV, "Blue");
                Spawn_MinionMagician(BlueBottomSpawnPointV, "Blue");
                yield return new WaitForSeconds(1.0f);
            }
            if (WaveCount % 3 == 0)
                yield return new WaitForSeconds(SpawnT);
            else
                yield return new WaitForSeconds(SpawnT);
        }
    }
}