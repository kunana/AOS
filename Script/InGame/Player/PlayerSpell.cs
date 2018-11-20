using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using Werewolf.SpellIndicators;

public class PlayerSpell : Photon.PunBehaviour
{
    ChampionData ChampData;
    ChampionBehavior mybehav;
    public GameObject Ignite_OBJ = null;
    public GameObject Flash_OBJ = null;
    public GameObject Ghost_OBJ = null;
    public GameObject Smite_OBJ = null;
    public GameObject Heal_OBJ = null;
    public GameObject Teleport_OBJ = null;
    public GameObject LevelUp = null;
    public GameObject Recall = null;
    public GameObject ManaPotion = null;
    public GameObject HealPotion = null;
    public GameObject Death = null;
    // 정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막 (0~8)
    // 스펠 ID
    //           점멸 유체화 회복 강타 순간이동 점화       (0~8)

    Dictionary<string, List<GameObject>> Spelllist = new Dictionary<string, List<GameObject>>();
    GameObject SpellContainer;
    public int Poolamount = 12;
    private int Spell_D = 0;
    private int Spell_F = 0;

    //점멸
    private class CollisionListData
    {
        public float distance = 0;
        public GameObject obj = null;
    }
    private int SortByDistanceFar(CollisionListData p1, CollisionListData p2)
    {
        return p2.distance.CompareTo(p1.distance);
    }
    private int SortByDistanceClose(CollisionListData p1, CollisionListData p2)
    {
        return p1.distance.CompareTo(p2.distance);
    }
    private int layerMask;
    private float CurFlashdist = 10f;
    private float MaxFalshMax = 20f;
    public float adjustFlashDistance = 3f;
    RaycastHit hit;
    Ray CamRay;
    Vector3 mousepos;
    Vector3 CamRayHitPOs;
    Vector3 temp = Vector3.zero;
    Vector3 dir;
    Vector3 startRayPos;
    CollisionListData colData;
    List<CollisionListData> obj;
    //점멸

    //테스트 후 다 private 으로 바꿀것
    public bool isIgniteClick = false;
    public bool isSmiteClick = false;
    public bool isTeleportClick = false;
    public bool isTeleportDestSet = false;
    private bool isGhost = false;
    public bool isAttack = false;
    public bool isIgnite = false;
    public bool ImOnFire = false;
    public bool Once = false;
    public bool IgniteTargetset = false;
    public bool SmiteTargetset = false;

    private float curhp;
    private float TeleportTime = 4f;
    private float HealRange = 10f;
    private float IgniteRange = 25f;
    private float SmiteRange = 25f;
    private float Healfloat = 0f;
    private float SmiteDam = 0f;
    private float IgnteDam = 0f;
    private float IgnteTime = 0;
    private int Playerlevel = 0;
    private int Attackerlevel = 0;
    private float GhostAcceleration = 10.0f;
    private float curSpeed = 0;
    private float MaxSpeed = 0;

    Vector3 offset = Vector3.zero;
    Vector3 TeleportDestPos = Vector3.zero;

    GameObject Player;
    GameObject AstarTarget;
    GameObject Target;
    GameObject IgniteAtker;
    GameObject IgniteAtked;
    string team;
    PhotonView photonview;
    Pathfinding.AIPath aiPath;
    SplatManager splatmanager;
    [SerializeField]
    Splat splat;
    MinimapClick minimap;
    public AOSMouseCursor cursor;
    PingPooling Ping;
    RTS_Cam.RTS_Camera cam;
    Pathfinding.AIPath AIPath;
    ChatFunction chat;
    RaiseEventOptions op;
    List<WarFogForEffect> fog = new List<WarFogForEffect>();
    private byte sendcode = 152;
    private byte sendIgnitecode = 153;
    private byte sendHealcode = 154;
    public bool isInGameScene = false;
    private object[] datas;
    private string senderteam;
    private PhotonPlayer sender;

    private FogOfWarEntity tempfogEntity;
    private WarFogForEffect tempFogEffect;

    private void Start()
    {
        mybehav = GetComponent<ChampionBehavior>();
        Spell_D = PlayerData.Instance.spell_D;
        Spell_F = PlayerData.Instance.spell_F;
        ChampData = GetComponent<ChampionData>();
        Player = GameObject.FindGameObjectWithTag("Player");
        AstarTarget = GameObject.FindGameObjectWithTag("PlayerA*Target");
        if (!Player)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
            if (!Player)
                return;
        }

        team = PhotonNetwork.player.GetTeam().ToString().ToLower();
        photonview = GetComponent<PhotonView>();
        aiPath = GetComponent<Pathfinding.AIPath>();
        splatmanager = GetComponentInChildren<SplatManager>();
        AIPath = GetComponent<Pathfinding.AIPath>();

        curSpeed = aiPath.maxSpeed;

        layerMask = (-1) - ((1 << LayerMask.NameToLayer("WallCollider")));
        op = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
        };
    }

    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
        {
            PhotonNetwork.OnEventCall += SpellEffectSync;

            cursor = GameObject.FindGameObjectWithTag("MouseCursor").GetComponent<AOSMouseCursor>();
            Ping = GameObject.FindGameObjectWithTag("PingPool").GetComponent<PingPooling>();
            cam = Camera.main.GetComponent<RTS_Cam.RTS_Camera>();
            minimap = GameObject.FindGameObjectWithTag("MinimapClick").GetComponent<MinimapClick>();
            chat = GameObject.FindGameObjectWithTag("ChatManager").GetComponentInChildren<ChatFunction>();
            SpellContainer = GameObject.FindGameObjectWithTag("SpellPooling");
            makepool();
            isInGameScene = true;
        }
    }

    private void makepool()
    {
        if (!isInGameScene)
        {
            SpellPooling(Ignite_OBJ, "Ignite");
            SpellPooling(Teleport_OBJ, "Teleport");
            SpellPooling(Flash_OBJ, "Flash");
            SpellPooling(Smite_OBJ, "Smite");
            SpellPooling(Heal_OBJ, "Heal");
            SpellPooling(LevelUp, "LevelUp");
            SpellPooling(HealPotion, "HealPotion");
            SpellPooling(ManaPotion, "ManaPotion");
            SpellPooling(Recall, "Recall");
            SpellPooling(Ghost_OBJ, "Ghost");
            SpellPooling(Death, "Death");
        }

    }
    //타겟팅 점화, 강타,
    private void Update()
    {
        if (isInGameScene)
        {
            if (chat.chatInput.IsActive())
                return;

            if (isIgniteClick)
            {
                if (isSmiteClick || isTeleportClick || Input.GetMouseButton(1))
                {
                    cursor.SetCursor(0, Vector2.zero);
                    isIgniteClick = false;
                    splatmanager.Cancel();
                    return;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    splatmanager.Cancel();
                    isIgniteClick = false;
                    Ignite();
                    cursor.SetCursor(0, Vector2.zero);
                }
            }
            if (isSmiteClick)
            {
                if (isIgniteClick || isTeleportClick || Input.GetMouseButton(1))
                {
                    cursor.SetCursor(0, Vector2.zero);
                    isSmiteClick = false;
                    splatmanager.Cancel();
                    return;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    splatmanager.Cancel();
                    isSmiteClick = false;
                    Smite();
                    cursor.SetCursor(0, Vector2.zero);
                }
            }
            if (isTeleportClick)
            {
                if (isIgniteClick || isSmiteClick || Input.GetMouseButton(1))
                {
                    cursor.SetCursor(0, Vector2.zero);
                    isTeleportClick = false;
                    return;
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    TeleportDestinationSet();
                    isTeleportClick = false;
                    cursor.SetCursor(0, Vector2.zero);
                }
            }

        }
        if (isIgnite)
        {
            ImOnFire = true;
            IgnteTime += Time.deltaTime;
            if (IgnteTime >= 1.0f)
            {
                IgnteTime = 0;
                IgnteDam += Mathf.FloorToInt(55 + (25 * Attackerlevel)) / 5;
                if (mybehav.HitMe((float)Mathf.FloorToInt(55 + (25 * Attackerlevel)) / 5, "FD", IgniteAtker, IgniteAtker.gameObject.name))
                {
                    if (ImOnFire)
                    {
                        ImOnFire = false;
                        IgnteTime = 0;
                        IgnteDam = 0;
                        ChampionData chdata = IgniteAtker.GetComponent<ChampionData>();
                        chdata.Kill_CS_Gold_Exp("", 0, this.transform.position);
                    }
                }
            }
            if (IgnteDam >= Mathf.FloorToInt(55 + (25 * Attackerlevel)))
            {
                IgnteDam = 0;
                IgnteTime = 0;
                isIgnite = false;
            }
        }

        if (isTeleportDestSet)//목표 설정후에
        {
            if (!Once)
            {
                curhp = ChampData.totalstat.Hp;
                Once = true;
                SendEffect("Teleport", Player.transform.position, team, photonview.viewID);
                Ping.GetFxPool("Going", TeleportDestPos, false);
                if (Target.GetComponent<PhotonView>() != null)
                    SendEffect("Teleport", Target.transform.position, team, photonview.viewID);
                else
                    SendEffect("Teleport", Target.transform.position, team);
                AIPath.isStopped = true;
                if (Target.transform.name.Contains("Minion"))
                    Target.GetComponent<Pathfinding.AIPath>().isStopped = true;
            }
            TeleportTime -= Time.deltaTime;
            if (curhp > ChampData.totalstat.Hp)
            {
                if (fog.Count > 0)
                    for (int i = 0; i < fog.Count; i++)
                    {
                        fog[i].stopParticle();
                    }
                fog.Clear();
                cursor.SetCursor(cursor.PreCursor, Vector2.zero);
                isTeleportDestSet = false;
                TeleportTime = 4.5f;
                Once = false;
                cam.ResetTarget();
                AIPath.isStopped = false;
                if (Target.transform.name.Contains("Minion"))
                    Target.GetComponent<Pathfinding.AIPath>().isStopped = false;
                return;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                if (fog.Count > 0)
                    for (int i = 0; i < fog.Count; i++)
                    {
                        fog[i].stopParticle();
                    }
                fog.Clear();
                cursor.SetCursor(cursor.PreCursor, Vector2.zero);
                if (Spell_D == 6)
                    ChampData.current_Cooldown_D = 1;
                else if (Spell_F == 6)
                    ChampData.current_Cooldown_F = 1;
                isTeleportDestSet = false;
                TeleportTime = 4.5f;
                Once = false;
                cam.ResetTarget();
                cursor.SetCursor(0, Vector3.zero);
                AIPath.isStopped = false;
                if (Target.transform.name.Contains("Minion"))
                    Target.GetComponent<Pathfinding.AIPath>().isStopped = false;
                return;
            }
            else if (TeleportTime <= 0)
            {
                cam.SetTarget(Player.transform);
                TeleportTime = 4.5f;
                TeleportDestPos.y = Player.transform.position.y;
                Player.transform.position = TeleportDestPos;
                //AstarTarget.transform.position = Player.transform.position;
                isTeleportDestSet = false;
                Once = false;
                Invoke("TeleportEnd", 0.5f);
            }
        }

        if (isGhost) //유체화 켜졌을때
        {
            if (!aiPath)
                return;
            GhostAcceleration -= Time.deltaTime;

            if (GhostAcceleration >= 8.0f)
            {
                MaxSpeed = aiPath.maxSpeed + (aiPath.maxSpeed * (0.27f + (0.01f * (float)Playerlevel)));
                aiPath.maxSpeed += (aiPath.maxSpeed * (0.27f + (0.01f * Playerlevel))) * Time.deltaTime;
                if (aiPath.maxSpeed >= MaxSpeed)
                {
                    aiPath.maxSpeed = MaxSpeed;
                }
                //27% + (1% * level)
            }
            if (GhostAcceleration <= 0)
            {
                Ghost(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChampData.current_Cooldown_D = 1;
            ChampData.current_Cooldown_F = 1;
        }

    }

    private void SpellEffectSync(byte eventCode, object content, int senderId)
    {
        if (eventCode.Equals(sendcode) || eventCode.Equals(sendIgnitecode) || eventCode.Equals(sendHealcode))
        {
            object[] receivedData = content as object[];
            GameObject temp;
            sender = PhotonPlayer.Find(senderId);
            senderteam = sender.GetTeam().ToString().ToLower();
            if (eventCode.Equals(sendcode))
            {
                if (receivedData.Length.Equals(3))
                    GetSpell((string)receivedData[0], (Vector3)receivedData[1], (string)receivedData[2]);
                else if (receivedData.Length.Equals(4))
                {
                    if ((int)receivedData[3] != 0)
                    {
                        int viewid = (int)receivedData[3];
                        temp = PhotonView.Find(viewid).gameObject;
                        GetSpell((string)receivedData[0], (Vector3)receivedData[1], (string)receivedData[2], viewid);
                    }
                    else
                    {
                        GetSpell((string)receivedData[0], (Vector3)receivedData[1], (string)receivedData[2]);
                    }

                }
            }
            else if (eventCode.Equals(sendIgnitecode)) //점화 동기화
            {
                IgniteAtked = PhotonView.Find((int)receivedData[0]).gameObject;
                if (IgniteAtked.GetComponent<PhotonView>().owner == PhotonNetwork.player)
                {
                    isIgnite = true;
                    Attackerlevel = (int)receivedData[1];
                    IgniteAtker = PhotonView.Find((int)receivedData[2]).gameObject;
                }
            }
            else if (eventCode.Equals(sendHealcode))//힐 동기화
            {
                if (PhotonView.Find((int)receivedData[0]).gameObject.GetComponent<PhotonView>().owner == photonview.owner)
                {
                    SyncHeal((float)receivedData[1], (string)receivedData[2]);
                }
            }
        }
    }

    public void SendEffect(string spellname, Vector3 pos, string team, int _viewid = 0)
    {
        datas = new object[] { spellname, pos, team, _viewid, };
        PhotonNetwork.RaiseEvent(sendcode, datas, true, op);
    }
    private void SpellPooling(GameObject prefab, string name, int amount = 10)
    {
        if (!SpellContainer)
        {
            SpellContainer = new GameObject();
            SpellContainer.transform.parent = GameObject.FindGameObjectWithTag("PingPool").transform.parent;
            SpellContainer.tag = "SpellPooling";
            SpellContainer.name = "SpellContainer";
        }

        if (!Spelllist.ContainsKey(name))
        {
            List<GameObject> list = new List<GameObject>();
            Spelllist.Add(name, list);
        }
        List<GameObject> tempList = new List<GameObject>();
        for (int i = 0; i < amount; ++i)
        {
            GameObject obj = Instantiate(prefab, SpellContainer.transform);
            obj.SetActive(false);
            tempList.Add(obj);
        }
        Spelllist[name].InsertRange(0, tempList);
    }

    private void GetSpell(string spellname, Vector3 pos, string team, int viewid = 0)
    {
        GameObject obj = Spelllist[spellname][0];
        GameObject tempObj;
        if (obj == null)
            return;
        if (Spelllist[spellname].Count == 0)
        {
            SpellPooling(obj, spellname);
        }
        if (obj.name.Contains("Teleport") || obj.name.Contains("Recall"))
            fog.Add(obj.GetComponent<WarFogForEffect>());
        if (viewid != 0)
        {
            tempObj = PhotonView.Find(viewid).gameObject;
            if (tempObj != null)
            {
                if (obj.GetComponent<WarFogForEffect>() != null && tempObj.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                {
                    tempFogEffect = obj.GetComponent<WarFogForEffect>();
                    tempfogEntity = tempObj.GetComponent<FogOfWarEntity>();
                    tempFogEffect.senderEntity = tempfogEntity;
                }
            }
            if (spellname.Contains("Heal") || spellname.Contains("Ignite") || spellname.Contains("Ghost"))
            {
                obj.transform.SetParent(tempObj.transform);
                obj.transform.Rotate(Vector3.zero);
            }

        }
        if (!spellname.Contains("Ignite"))
        {
            if (team.Contains("red"))
                obj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player00;
            else if (team.Contains("blue"))
                obj.GetComponent<FogOfWarEntity>().faction = FogOfWar.Players.Player01;
        }
        Spelllist[spellname].RemoveAt(0);
        Spelllist[spellname].Add(obj);
        obj.transform.position = pos;
        obj.SetActive(true);
        obj.GetComponent<ParticleSystem>().Play();
    }

    private Vector3 cursorWorldPosOnNCP
    {
        get
        {
            return Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x,
                Input.mousePosition.y,
                Camera.main.nearClipPlane));
        }
    }

    public void Call_SpellD()
    {//점멸 아이콘 마우스 오버일때만 범위 나타내기
        switch (Spell_D)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                break;
            // 탈진
            case 1:
                break;
            // 점멸
            case 2:
                Flash();
                break;
            // 유체화
            case 3:
                Ghost(true);
                break;
            // 회복
            case 4:
                Heal();
                break;
            // 강타
            case 5:
                splatmanager.Point.Select();
                splatmanager.Point.SetRange(12);
                splatmanager.RangeIndicator.SetScale(SmiteRange, 5f);
                isSmiteClick = true;
                break;
            // 순간이동
            case 6:
                isTeleportClick = true;
                break;
            // 점화
            case 7:
                splatmanager.Point.Select();
                splatmanager.Point.SetRange(12);
                splatmanager.RangeIndicator.SetScale(IgniteRange, 5f);
                isIgniteClick = true;
                break;
            // 방어막
            case 8:
                break;
        }
    }

    public void Call_SpellF()
    {//점멸 아이콘 마우스 오버일때만 범위 나타내기
        switch (Spell_F)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                break;
            // 탈진
            case 1:
                break;
            // 점멸
            case 2:
                Flash();
                break;
            // 유체화
            case 3:
                Ghost(true);
                break;
            // 회복
            case 4:
                Heal();
                break;
            // 강타
            case 5:
                splatmanager.Point.Select();
                splatmanager.Point.SetRange(12);
                splatmanager.RangeIndicator.SetScale(SmiteRange, 5f);
                isSmiteClick = true;
                break;
            // 순간이동
            case 6:
                isTeleportClick = true;
                break;
            // 점화
            case 7:

                splatmanager.Point.Select();
                splatmanager.Point.SetRange(12);
                splatmanager.RangeIndicator.SetScale(IgniteRange, 5f);
                isIgniteClick = true;
                break;
            // 방어막
            case 8:
                break;
        }
    }

    private void Smite()
    {   //20 1-4 30 5-9 40 10- 14 50 15-18
        //370 + (20 * level) 500 Range model Edge range range)

        mousepos = Input.mousePosition;
        CamRay = Camera.main.ScreenPointToRay(mousepos);
        bool ishit = Physics.Raycast(CamRay, out hit, Mathf.Infinity, layerMask); // 카메라 기준 레이, 월콜라이더 무시
        if (ishit)
        {
            Vector3 temp = hit.point;
            temp.y = Player.transform.position.y;

            float dist = Vector3.Distance(temp, Player.transform.position);
            if (dist <= SmiteRange)
            {
                Playerlevel = ChampData.totalstat.Level;
                if (Playerlevel <= 4 && Playerlevel >= 1)
                    SmiteDam = 370 + (20 * Playerlevel);
                else if (Playerlevel <= 9 && Playerlevel >= 5)
                    SmiteDam = 370 + (30 * Playerlevel);
                else if (Playerlevel <= 14 && Playerlevel >= 10)
                    SmiteDam = 370 + (40 * Playerlevel);
                else if (Playerlevel <= 18 && Playerlevel >= 15)
                    SmiteDam = 370 + (50 * Playerlevel);

                if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
                {
                    if (Vector3.Distance(Player.transform.position, hit.transform.position) <= SmiteRange)
                    {
                        SmiteTargetset = true;
                        //동기화 할것.
                        hit.collider.gameObject.GetComponent<MonsterBehaviour>().HitMe(SmiteDam, "FD", Player.gameObject);
                        SendEffect("Smite", hit.transform.position, team);
                        ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Smite);
                        ChampData.totalstat.Hp += ChampData.totalstat.MaxHp * 0.1f;
                    }
                }
                else if (hit.collider.gameObject.name.Contains("Minion"))
                {
                    MinionBehavior M_behav = hit.collider.gameObject.GetComponent<MinionBehavior>();
                    if (M_behav.team.ToString().ToLower() != (team)) // 다른팀이면
                    {

                        if (Vector3.Distance(Player.transform.position, hit.transform.position) <= SmiteRange)
                        {
                            SmiteTargetset = true;
                            //동기화 할것.
                            hit.collider.gameObject.GetComponent<MinionBehavior>().HitMe(SmiteDam, "FD");
                            SendEffect("Smite", hit.transform.position, team);
                            ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Smite);
                            ChampData.totalstat.Hp += ChampData.totalstat.MaxHp * 0.1f;
                        }
                        //동기화 해당 플레이어 프리팹의 코루틴 ignite 불러줄것
                    }
                }
                else
                {
                    SmiteTargetset = false;
                    //cursor.SetCursor(cursor.PreCursor, Vector2.zero);
                }
            }
        }
    }

    private void Flash()
    {
        if (!Player)
            Player = this.gameObject;
        if (Player != null)
        {
            //effectForviewid = gameObject.GetComponent<PhotonView>().viewID;
            ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Flash);
            SendEffect("Flash", Player.transform.position, team, photonview.viewID);
            mousepos = Input.mousePosition;
            CamRay = Camera.main.ScreenPointToRay(mousepos);
            CamRayHitPOs = Vector3.zero;

            bool ishit = Physics.Raycast(CamRay, out hit, Mathf.Infinity, layerMask); // 카메라 기준 레이
            if (ishit) //카메라 기준 레이 저장
            {

                temp = hit.point;
                temp.y = Player.transform.position.y;
            }

            CurFlashdist = Vector3.Distance(Player.transform.position, temp); // 마우스 와 플레이어 오브젝트 거리 계산
            if (CurFlashdist > MaxFalshMax) // 최대 점멸 거리 보다 긴가?
                CurFlashdist = MaxFalshMax;

            dir = (temp - Player.transform.position).normalized;  // 방향 계산
            startRayPos = Player.transform.position - dir * adjustFlashDistance; // 레이 시작점 설정

            RaycastHit[] hits = Physics.RaycastAll(startRayPos, dir, CurFlashdist); // 레이 시작점에서 끝점까지 레이
            bool noHit = true;
            if (obj != null)
                obj.Clear();
            else
                obj = new List<CollisionListData>();
            temp = startRayPos + dir * CurFlashdist;
            temp.y = 0.5f;
            foreach (RaycastHit h in hits)
            {
                if (h.collider.name.Contains("MapLine"))
                {
                    Vector3 dir = h.point - transform.position;
                    transform.position = h.point - dir * 0.5f;
                    return;
                }
                else if (!h.collider.name.Contains("Terrain")) // 터레인이 아니면
                {
                    //sqrmagnitude
                    noHit = false;
                    Vector3 offset = startRayPos - h.collider.gameObject.transform.position;
                    colData = new CollisionListData(); // 장애물 게임 오브젝트와 거리 저장
                    colData.obj = h.collider.gameObject;
                    colData.distance = temp.sqrMagnitude;
                    obj.Add(colData);
                }
            }

            if (noHit) // 아무것도 안맞으면 가라
            {
                if (mybehav.isDead)
                    return;
                Player.transform.position = temp;
                AstarTarget.transform.position = temp;
                SendEffect("Flash", temp, team, photonview.viewID);
            }
            else // 무언가 맞으면
            {
                obj.Sort(SortByDistanceFar); // 장애물 거리가 먼 순서대로 정렬
                Vector3 goal = Player.transform.position + dir * CurFlashdist; //점멸 목표지점 저장
                Collider[] goalCheckCollider = Physics.OverlapSphere(goal, 0.25f); // 목표지점에 무언가 있는지 검사
                bool bAllTerrain = true;
                foreach (Collider c in goalCheckCollider)
                {
                    if (!c.name.Contains("Terrain")) // 무언가 있으면 
                        bAllTerrain = false;
                }

                if (bAllTerrain) // 무언가 없으면 가라
                {
                    if (mybehav.isDead)
                        return;
                    Player.transform.position = temp;
                    AstarTarget.transform.position = temp;
                    SendEffect("Flash", temp, team, photonview.viewID);
                }
                else // 무언가 있을때
                {
                    foreach (CollisionListData o in obj) // 저장한 장애물 데이터에서
                    {
                        foreach (RaycastHit h in hits) //최초 플레이어기준 레이 충돌체
                        {
                            if (h.collider.name == o.obj.name) // 최초 검사한 레이와 비교
                            {
                                Vector3 now = h.point - dir * 0.5f; // 장애물에서 조금 떨어진곳으로
                                Collider[] FinalGoalCheck = Physics.OverlapSphere(now, 0.25f); // 마지막 확인
                                bAllTerrain = true;
                                foreach (Collider c in FinalGoalCheck)
                                {
                                    if (c.name == "Terrain")
                                        bAllTerrain = false;
                                }
                                if (bAllTerrain)
                                {
                                    if (mybehav.isDead)
                                        return;
                                    now.y = Player.transform.position.y;
                                    Player.transform.position = now;
                                    AstarTarget.transform.position = Player.transform.position;
                                    SendEffect("Flash", temp, team, photonview.viewID);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void TeleportDestinationSet()
    {   //미니맵 클릭 텔레포트는 MinimapClick.cs

        if (!minimap.IsPointerOver)//미니맵클릭이 아니면
        {
            Vector3 mousepos = Input.mousePosition;
            Ray Ray = Camera.main.ScreenPointToRay(mousepos);
            bool ishit = Physics.Raycast(Ray, out hit, Mathf.Infinity, layerMask); // 카메라 기준 레이
            if (ishit)
            {
                temp = hit.point;
                temp.y = Player.transform.position.y;
                FindClosestObject(temp);
            }
        }
    }

    public void FindClosestObject(Vector3 pos)
    {
        Collider[] ObjectCheck = Physics.OverlapSphere(pos, 10f); // 목표지점에 무언가 있는지 검사
        if (obj == null)
            obj = new List<CollisionListData>();
        else
            obj.Clear();
        //sqrMagnitude 
        foreach (Collider col in ObjectCheck)
        {
            if (col.transform.name.Contains("Terrain"))
            {
                continue;
            }
            else if (col.transform.name.Contains("Tower"))
            {
                if (col.gameObject.GetComponent<TowerBehaviour>().Team.ToLower().Equals(team))
                {
                    offset = col.gameObject.transform.position - Player.transform.position;
                    colData = new CollisionListData(); // 장애물 게임 오브젝트와 거리 저장
                    colData.obj = col.gameObject;
                    colData.distance = offset.sqrMagnitude;
                    obj.Add(colData);
                }
            }
            else if (col.transform.name.Contains("Minion"))
            {
                if (col.gameObject.GetComponent<MinionBehavior>().team.ToString().ToLower().Equals(team))
                {
                    offset = col.gameObject.transform.position - Player.transform.position;
                    colData = new CollisionListData(); // 장애물 게임 오브젝트와 거리 저장
                    colData.obj = col.gameObject;
                    colData.distance = offset.sqrMagnitude;
                    obj.Add(colData);
                }
            }
            else if (col.transform.gameObject.layer.Equals(13))
            {
                if (col.gameObject.GetComponent<ChampionBehavior>().Team.ToLower().Equals(team) && col.gameObject != this.gameObject)
                {
                    offset = col.gameObject.transform.position - Player.transform.position;
                    colData = new CollisionListData(); // 장애물 게임 오브젝트와 거리 저장
                    colData.obj = col.gameObject;
                    colData.distance = offset.sqrMagnitude;
                    obj.Add(colData);
                }
            }
            else if (col.transform.name.Contains("Ward"))
            {
                if (col.gameObject.GetComponent<Ward>().team.ToLower().Equals(team))
                {
                    offset = col.gameObject.transform.position - Player.transform.position;
                    colData = new CollisionListData(); // 장애물 게임 오브젝트와 거리 저장
                    colData.obj = col.gameObject;
                    colData.distance = offset.sqrMagnitude;
                    obj.Add(colData);
                }
            }
            else if (col.transform.name.Contains("Nexus") || col.transform.name.Contains("Suppressor"))
            {
                if (col.gameObject.GetComponent<SuppressorBehaviour>().Team.ToLower().Equals(team))
                {
                    offset = col.gameObject.transform.position - Player.transform.position;
                    colData = new CollisionListData(); // 장애물 게임 오브젝트와 거리 저장
                    colData.obj = col.gameObject;
                    colData.distance = offset.sqrMagnitude;
                    obj.Add(colData);
                }
            }
        }
        if (!obj.Count.Equals(0))
        {
            ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Teleport);
            //cursor.SetCursor(cursor.PreCursor, Vector2.zero);
            obj.Sort(SortByDistanceClose);
            dir = (obj[0].obj.transform.position - Player.transform.position).normalized;
            TeleportDestPos = obj[0].obj.transform.position + dir * +5.0f;
            TeleportDestPos.y = 0.5f;
            Target = obj[0].obj.gameObject;
            isTeleportDestSet = true;
            return;
        }
        else if (obj.Count.Equals(0))
        {
            //cursor.SetCursor(cursor.PreCursor, Vector2.zero);
        }
    }

    private void Heal()
    {
        //75 + (15 * level) Self, 850 for ally range
        mousepos = Input.mousePosition;
        CamRay = Camera.main.ScreenPointToRay(mousepos);
        int Target = 0;
        Healfloat = 0;
        curhp = aiPath.maxSpeed;

        bool ishit = Physics.Raycast(CamRay, out hit, Mathf.Infinity, layerMask); // 카메라 기준 레이
        if (ishit)
        {
            temp = hit.point;
            temp.y = Player.transform.position.y;

            Collider[] ObjectCheck = Physics.OverlapSphere(Player.transform.position, HealRange);
            Playerlevel = ChampData.totalstat.Level;

            if (obj == null)
                obj = new List<CollisionListData>();
            else
                obj.Clear();
            //sqrMagnitude 
            foreach (Collider col in ObjectCheck)
            {
                if (col.transform.gameObject.layer.Equals(13) && col.gameObject != this.gameObject)
                {
                    if (col.gameObject.GetComponent<ChampionBehavior>().Team.ToLower().Equals(team))
                    {
                        offset = col.gameObject.transform.position - temp;
                        colData = new CollisionListData(); // 장애물 게임 오브젝트와 거리 저장
                        colData.obj = col.gameObject;
                        colData.distance = offset.sqrMagnitude;
                        obj.Add(colData);
                    }
                }
            }
            if (!obj.Count.Equals(0)) // 주위에 챔피언이 하나라도 있으면
            {
                obj.Sort(SortByDistanceClose);
                if (obj[0].distance <= 1.5f) // 마우스커서위치에서 가까우면.
                {
                    Healfloat = 75 + (15 * Playerlevel);
                    Target = obj[0].obj.gameObject.GetComponent<PhotonView>().viewID;
                    datas = new object[] { Target, Healfloat, team };
                    PhotonNetwork.RaiseEvent(sendHealcode, datas, true, op);

                    ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Ingnite);
                    SendEffect("Heal", transform.position, team, photonview.viewID);
                    GetComponent<ChampionData>().totalstat.Hp += Healfloat;
                    aiPath.maxSpeed += aiPath.maxSpeed * 0.3f;
                    Invoke("PlayerSpeedReset", 2f);
                    return;
                }
                else // 가깝지 않으면 HP 가장 작은 자
                {
                    float minhp = 100000f;
                    for (int i = 0; i < obj.Count; i++)
                    {
                        float curhp = obj[i].obj.gameObject.GetComponent<ChampionData>().totalstat.Hp;
                        if (minhp > curhp)
                        {
                            minhp = curhp;
                            Target = obj[i].obj.gameObject.GetComponent<PhotonView>().viewID;
                        }
                    }
                    Healfloat = 75 + (15 * Playerlevel);
                    datas = new object[] { Target, Healfloat, team };
                    PhotonNetwork.RaiseEvent(sendHealcode, datas, true, op);

                    SoundManager.instance.ChampSound(SoundManager.instance.Ingnite);
                    SendEffect("Heal", transform.position, team, photonview.viewID);
                    GetComponent<ChampionData>().totalstat.Hp += Healfloat;
                    aiPath.maxSpeed += aiPath.maxSpeed * 0.3f;
                    Invoke("PlayerSpeedReset", 2f);
                    return;
                }

            }
            else // 주위에 챔피언이 하나라도 없으면
            {
                ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Ingnite);
                SendEffect("Heal", transform.position, team, photonview.viewID);
                Healfloat = 75 + (15 * Playerlevel);
                GetComponent<ChampionData>().totalstat.Hp += Healfloat;
                aiPath.maxSpeed += aiPath.maxSpeed * 0.3f;
                Invoke("PlayerSpeedReset", 2f);
            }
        }
    }
    public void SyncHeal(float heal, string team)
    {
        ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Ingnite);
        SendEffect("Heal", transform.position, team, photonview.viewID);
        ChampData.totalstat.Hp += heal;
        aiPath.maxSpeed += aiPath.maxSpeed * 0.3f;
        Invoke("PlayerSpeedReset", 2f);
    }

    private void PlayerSpeedReset()
    {
        aiPath.maxSpeed = curSpeed;
    }

    private void TeleportEnd()
    {
        cam.ResetTarget();
        AIPath.isStopped = false;
        if (Target.transform.name.Contains("Minion"))
            Target.GetComponent<Pathfinding.AIPath>().isStopped = false;
    }

    private void Ignite()
    {
        //55 + (25 * level) // 600 범위
        mousepos = Input.mousePosition;
        CamRay = Camera.main.ScreenPointToRay(mousepos);
        int Target = 0;
        bool ishit = Physics.Raycast(CamRay, out hit, Mathf.Infinity, layerMask); // 카메라 기준 레이, 월콜라이더 무시
        if (ishit)
        {
            if (hit.collider.gameObject.layer.Equals(Player.gameObject.layer))
            {
                if (hit.collider.gameObject.GetComponent<PhotonView>().owner.GetTeam().ToString().ToLower() != (team)) // 다른팀이면
                {
                    if (Vector3.Distance(Player.transform.position, hit.transform.position) <= IgniteRange)
                    {
                        Playerlevel = ChampData.totalstat.Level;
                        //cursor.SetCursor(cursor.PreCursor, Vector2.zero);
                        IgniteTargetset = true;
                        Target = hit.collider.gameObject.GetComponent<PhotonView>().viewID;
                        datas = new object[] { Target, Playerlevel, photonview.viewID };
                        PhotonNetwork.RaiseEvent(sendIgnitecode, datas, true, op);
                        ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Ingnite);
                        SendEffect("Ignite", hit.transform.position, team, Target);
                    }
                }
            }
            else
            {
                IgniteTargetset = false;
                //cursor.SetCursor(cursor.PreCursor, Vector2.zero);
            }
        }
    }

    private void Ghost(bool isghost)
    {
        if (isghost)
        {
            ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Ghost);
            SendEffect("Ghost", transform.position, team, photonview.viewID);
            Playerlevel = ChampData.totalstat.Level;
            //GetComponent<Pathfinding.RVO.RVOController>().enabled = false;
            isGhost = true;
        }
        else
        {
            isGhost = false;
            aiPath.maxSpeed = curSpeed;
            //GetComponent<Pathfinding.RVO.RVOController>().enabled = true;
            GhostAcceleration = 10.0f;
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= SpellEffectSync;
    }
}