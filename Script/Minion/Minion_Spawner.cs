using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion_Spawner : Photon.PunBehaviour
{
    public int key = -1;
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

    public GameObject RedTopSuppressor;
    public GameObject RedMidSuppressor;
    public GameObject RedBottomSuppressor;
    public GameObject BlueTopSuppressor;
    public GameObject BlueMidSuppressor;
    public GameObject BlueBottomSuppressor;

    private int WaveCount = 0;
    private float FirstSpawnT = 65; // 기본 65
    private float SpawnT = 30; // 기본 30

    float gridTerm = 0;

    public RespownZone RedTopRespownZone;
    public RespownZone RedMidRespownZone;
    public RespownZone RedBotRespownZone;
    public RespownZone BlueTopRespownZone;
    public RespownZone BlueMidRespownZone;
    public RespownZone BlueBotRespownZone;

    private bool first = true;
    SystemMessage sysmsg;
    AudioSource audiosource;

    void Start()
    {
        if (!sysmsg)
            sysmsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        RedTopSpawnPointV = RedTopSpawnPoint.transform.position;
        RedMidSpawnPointV = RedMidSpawnPoint.transform.position;
        RedBottomSpawnPointV = RedBottomSpawnPoint.transform.position;

        BlueTopSpawnPointV = BlueTopSpawnPoint.transform.position;
        BlueMidSpawnPointV = BlueMidSpawnPoint.transform.position;
        BlueBottomSpawnPointV = BlueBottomSpawnPoint.transform.position;

        audiosource = gameObject.GetComponent<AudioSource>();
        audiosource.volume = 1f;
        audiosource.spatialBlend = 1;
        audiosource.minDistance = 1;
        audiosource.maxDistance = 40;
        audiosource.rolloffMode = AudioRolloffMode.Linear;

        if (PhotonNetwork.isMasterClient)
        {
            StartCoroutine(Wave());
        }
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
    void Spawn_MinionMelee(Vector3 position, string color)
    {
        GameObject MinionMelee = Minion_ObjectPool.current.GetPooledMelee(color);

        if (MinionMelee == null) return;
        MinionBehavior behav = MinionMelee.GetComponent<MinionBehavior>();
        KillManager.photonMinionList.Add(behav);
        behav.key = ++key;
        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionMelee.name = "Minion_Red_Melee";
        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionMelee.name = "Minion_Blue_Melee";

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

        MinionMelee.GetComponent<MinionBehavior>().minAtk.nowTarget = null;


        audiosource.PlayOneShot(SoundManager.instance.Minion_SpawnSound);
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


        MinionBehavior Behav = MinionMagician.GetComponent<MinionBehavior>();
        KillManager.photonMinionList.Add(Behav);
        Behav.key = ++key;
        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionMagician.name = "Minion_Red_Magician";

        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionMagician.name = "Minion_Blue_Magician";

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

        audiosource.PlayOneShot(SoundManager.instance.Minion_SpawnSound);
        MinionMagician.SetActive(true);
    }

    void Spawn_MinionSiege(Vector3 position, string color)
    {
        GameObject MinionSiege = Minion_ObjectPool.current.GetPooledSiege(color);

        if (MinionSiege == null) return;


        MinionBehavior Behav = MinionSiege.GetComponent<MinionBehavior>();
        KillManager.photonMinionList.Add(Behav);
        Behav.key = ++key;
        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionSiege.name = "Minion_Red_Siege";

        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionSiege.name = "Minion_Blue_Siege";

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

        audiosource.PlayOneShot(SoundManager.instance.Minion_SpawnSound);
        MinionSiege.SetActive(true);
    }

    void Spawn_MinionSuper(Vector3 position, string color)
    {
        GameObject MinionSuper = Minion_ObjectPool.current.GetPooledSuper(color);

        if (MinionSuper == null) return;


        MinionBehavior Behav = MinionSuper.GetComponent<MinionBehavior>();
        KillManager.photonMinionList.Add(Behav);
        Behav.key = ++key;
        if (position == RedTopSpawnPointV || position == RedMidSpawnPointV || position == RedBottomSpawnPointV)
        {
            MinionSuper.name = "Minion_Red_Super";
        }
        if (position == BlueTopSpawnPointV || position == BlueMidSpawnPointV || position == BlueBottomSpawnPointV)
            MinionSuper.name = "Minion_Blue_Super";

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

        audiosource.PlayOneShot(SoundManager.instance.Minion_SpawnSound);
        MinionSuper.SetActive(true);
    }

    private void InitTarget(GameObject minion, GameObject point)
    {
        MinionBehavior minB = minion.GetComponent<MinionBehavior>();
        MinionAtk minAtk = minB.minAtk;
        if (minAtk == null)
        {
            minB.minAtk = minB.transform.GetComponentInChildren<MinionAtk>();
            minAtk = minB.minAtk;
        }

        if (PhotonNetwork.isMasterClient)
            minion.GetComponent<AIDestinationSetter>().target = point.transform;

        minAtk.MoveTarget = point;
        minAtk.nowTarget = null;
        minion.GetComponent<MinionBehavior>().spawnPoint = point.transform.position;
    }

    [PunRPC]
    private void SpawnMelee()
    {
        Spawn_MinionMelee(RedTopSpawnPointV, "Red");
        Spawn_MinionMelee(RedMidSpawnPointV, "Red");
        Spawn_MinionMelee(RedBottomSpawnPointV, "Red");
        Spawn_MinionMelee(BlueTopSpawnPointV, "Blue");
        Spawn_MinionMelee(BlueMidSpawnPointV, "Blue");
        Spawn_MinionMelee(BlueBottomSpawnPointV, "Blue");
    }

    [PunRPC]
    private void SpawnMagician()
    {
        Spawn_MinionMagician(RedTopSpawnPointV, "Red");
        Spawn_MinionMagician(RedMidSpawnPointV, "Red");
        Spawn_MinionMagician(RedBottomSpawnPointV, "Red");
        Spawn_MinionMagician(BlueTopSpawnPointV, "Blue");
        Spawn_MinionMagician(BlueMidSpawnPointV, "Blue");
        Spawn_MinionMagician(BlueBottomSpawnPointV, "Blue");
    }

    [PunRPC]
    private void SpawnSuper(string line, string team)
    {
        if (team.Equals("Red"))
        {
            if (line.Equals("Top"))
                Spawn_MinionSuper(RedTopSpawnPointV, "Red");
            else if (line.Equals("Mid"))
                Spawn_MinionSuper(RedMidSpawnPointV, "Red");
            else if (line.Equals("Bot"))
                Spawn_MinionSuper(RedBottomSpawnPointV, "Red");
            else if (line.Equals("All"))
            {
                Spawn_MinionSuper(RedTopSpawnPointV, "Red");
                Spawn_MinionSuper(RedMidSpawnPointV, "Red");
                Spawn_MinionSuper(RedBottomSpawnPointV, "Red");
            }
        }
        else if (team.Equals("Blue"))
        {
            if (line.Equals("Top"))
                Spawn_MinionSuper(BlueTopSpawnPointV, "Blue");
            else if (line.Equals("Mid"))
                Spawn_MinionSuper(BlueMidSpawnPointV, "Blue");
            else if (line.Equals("Bot"))
                Spawn_MinionSuper(BlueBottomSpawnPointV, "Blue");
            else if (line.Equals("All"))
            {
                Spawn_MinionSuper(BlueTopSpawnPointV, "Blue");
                Spawn_MinionSuper(BlueMidSpawnPointV, "Blue");
                Spawn_MinionSuper(BlueBottomSpawnPointV, "Blue");
            }
        }
    }

    [PunRPC]
    private void SpawnSiege(string line, string team)
    {
        if (team.Equals("Red"))
        {
            if (line.Equals("Top"))
                Spawn_MinionSiege(RedTopSpawnPointV, "Red");
            else if (line.Equals("Mid"))
                Spawn_MinionSiege(RedMidSpawnPointV, "Red");
            else if (line.Equals("Bot"))
                Spawn_MinionSiege(RedBottomSpawnPointV, "Red");
        }
        else if (team.Equals("Blue"))
        {
            if (line.Equals("Top"))
                Spawn_MinionSiege(BlueTopSpawnPointV, "Blue");
            else if (line.Equals("Mid"))
                Spawn_MinionSiege(BlueMidSpawnPointV, "Blue");
            else if (line.Equals("Bot"))
                Spawn_MinionSiege(BlueBottomSpawnPointV, "Blue");
        }
    }

    IEnumerator Wave()
    {

        yield return new WaitForSeconds(FirstSpawnT - 7f);
        sysmsg.Annoucement(3, true);
        yield return new WaitForSeconds(4f);
        while (true)
        {
            WaveCount++;
            for (int i = 0; i < 2; i++)
            {
                this.photonView.RPC("SpawnMelee", PhotonTargets.AllViaServer, null);
                yield return new WaitForSeconds(1.0f);
            }

            if (BlueTopSuppressor.activeInHierarchy == false && BlueMidSuppressor.activeInHierarchy == false && BlueBottomSuppressor.activeInHierarchy == false)
            {
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "All", "Red");
            }
            if (BlueTopSuppressor.activeInHierarchy == false && WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "Top", "Red");
            else if (WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSiege", PhotonTargets.AllViaServer, "Top", "Red");

            if (BlueMidSuppressor.activeInHierarchy == false && WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "Mid", "Red");
            else if (WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSiege", PhotonTargets.AllViaServer, "Mid", "Red");

            if (BlueBottomSuppressor.activeInHierarchy == false && WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "Bot", "Red");
            else if (WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSiege", PhotonTargets.AllViaServer, "Bot", "Red");

            if (RedTopSuppressor.activeInHierarchy == false && RedMidSuppressor.activeInHierarchy == false && RedBottomSuppressor.activeInHierarchy == false)
            {
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "All", "Blue");
            }
            if (RedTopSuppressor.activeInHierarchy == false && WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "Top", "Blue");
            else if (WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSiege", PhotonTargets.AllViaServer, "Top", "Blue");

            if (RedMidSuppressor.activeInHierarchy == false && WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "Mid", "Blue");
            else if (WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSiege", PhotonTargets.AllViaServer, "Mid", "Blue");

            if (RedBottomSuppressor.activeInHierarchy == false && WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSuper", PhotonTargets.AllViaServer, "Bot", "Blue");
            else if (WaveCount % 3 == 0)
                this.photonView.RPC("SpawnSiege", PhotonTargets.AllViaServer, "Bot", "Blue");

            if (WaveCount % 3 == 0)
                yield return new WaitForSeconds(1.0f);

            for (int i = 0; i < 3; i++)
            {
                this.photonView.RPC("SpawnMagician", PhotonTargets.AllViaServer, null);
                yield return new WaitForSeconds(1.0f);
            }
            if (WaveCount % 3 == 0)
                yield return new WaitForSeconds(SpawnT);
            else
                yield return new WaitForSeconds(SpawnT);
        }
    }
}