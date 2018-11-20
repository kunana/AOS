using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerMouse : MonoBehaviour
{
    public GameObject myTarget;
    public ChampionAtk myChampAtk;
    public ChampionBehavior myChampBehav;
    public bool atkCommand = false;
    public bool wardCommand = false;

    Vector3 v;
    Ray r;
    RaycastHit[] hits;
    Vector3 dest;
    public MouseFxPooling fxpool;
    private MinimapClick MinimapClick;
    string playerTeam;

    private PlayerData playerData;
    private string myChampName;
    private float soundtimer = 5.0f;

    private void Start()
    {
        myChampBehav = GetComponent<ChampionBehavior>();
        playerTeam = PhotonNetwork.player.GetTeam().ToString();
        if (playerTeam.Equals("red"))
            playerTeam = "Red";
        else if (playerTeam.Equals("blue"))
            playerTeam = "Blue";
        else
            print("PlayerMouse.cs :: 26 :: Player has not Team T_T");

        playerData = PlayerData.Instance;
        myChampName = PlayerData.Instance.championName;
    }
    private void OnLevelWasLoaded(int level)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
        {
            MinimapClick = GameObject.FindGameObjectWithTag("MinimapClick").GetComponent<MinimapClick>();
            fxpool = GameObject.FindGameObjectWithTag("MouseFxPool").GetComponent<MouseFxPooling>();
        }

    }
    private void Update()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Equals("InGame"))
        {
            if (PlayerData.Instance.isDead)
                return;
            if (Input.GetKeyDown(KeyCode.A))
            {
                atkCommand = !atkCommand;
                wardCommand = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                if (PlayerData.Instance.accessoryItem.Equals(1))
                {
                    wardCommand = !wardCommand;
                    if (wardCommand)
                    {
                        if (myChampAtk.wardAmount < 1)
                        {
                            wardCommand = false;
                            atkCommand = false;
                        }
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                myChampAtk.Stop();
            }
            if (Input.GetMouseButtonDown(1))
            {
                soundtimer -= Time.deltaTime;
                if (soundtimer <= 0)
                {
                    ChampionSound.instance.WalkSound(myChampName);
                    soundtimer = 5.0f;
                }
                //우선 이동만. 나중엔 공격인지 뭔지 그런 것 판단도 해야 할 것.
                if (atkCommand)
                    atkCommand = false;
                if (wardCommand)
                    if (myChampAtk.wardAmount > 0)
                        wardCommand = false;

                Vector3 h = Vector3.zero;
                GameObject target = null;
                bool touchGround = false;
                bool touchEnemy = false;

                v = Input.mousePosition;
                r = Camera.main.ScreenPointToRay(v);
                hits = Physics.RaycastAll(r);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag.Equals("Terrain"))
                    {
                        h = hit.point;
                        h.y = 1;
                        touchGround = true;
                        if (!fxpool && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "InGame")
                            fxpool = GameObject.FindGameObjectWithTag("MouseFxPool").GetComponent<MouseFxPooling>();
                        fxpool.GetPool("Default", h);
                    }
                    else if (hit.collider.tag.Equals("Minion"))
                    {
                        if (!hit.collider.name.Contains(playerTeam))
                            if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                touchEnemy = true;

                    }
                    else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                    {
                        if (!hit.collider.gameObject.GetComponent<ChampionBehavior>().Team.Equals(playerTeam))
                            if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                touchEnemy = true;
                    }
                    else if (hit.collider.tag.Equals("Tower"))
                    {
                        TowerBehaviour t = hit.collider.gameObject.GetComponent<TowerBehaviour>();
                        if (t.isCanAtkMe)
                            if (!t.Team.Equals(playerTeam))
                                //if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                touchEnemy = true;
                    }
                    else if (hit.collider.tag.Equals("Suppressor") || hit.collider.tag.Equals("Nexus"))
                    {
                        SuppressorBehaviour s = hit.collider.gameObject.GetComponent<SuppressorBehaviour>();
                        if (s.isCanAtkMe)
                            if (!s.Team.Equals(playerTeam))
                                //if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                touchEnemy = true;
                    }
                    else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
                    {
                        if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                            touchEnemy = true;
                    }
                    if (touchEnemy)
                    {
                        target = hit.collider.gameObject;
                        break;
                    }
                }

                // 상점 켜져있으면 뒤로 움직이지않게
                var shop = GameObject.FindGameObjectWithTag("ShopCanvas");
                if (shop != null)
                {
                    GraphicRaycaster ShopGR = shop.GetComponent<GraphicRaycaster>();
                    PointerEventData ped = new PointerEventData(null);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> results = new List<RaycastResult>();
                    ShopGR.Raycast(ped, results);
                    foreach (RaycastResult result in results)
                    {
                        if (result.gameObject.transform.GetComponentInParent<GraphicRaycaster>().Equals(ShopGR))
                        {
                            touchEnemy = false;
                            touchGround = false;
                            break;
                        }
                    }
                }

                // 옵션 켜져있으면 움직이지않게
                var optionCanvas = GameObject.FindGameObjectWithTag("OptionCanvas");
                if (optionCanvas != null)
                {
                    GraphicRaycaster OptionGR = optionCanvas.GetComponent<GraphicRaycaster>();
                    PointerEventData ped = new PointerEventData(null);
                    ped.position = Input.mousePosition;
                    List<RaycastResult> results = new List<RaycastResult>();
                    OptionGR.Raycast(ped, results);
                    foreach (RaycastResult result in results)
                    {
                        if (result.gameObject.transform.GetComponentInParent<GraphicRaycaster>().Equals(OptionGR))
                        {
                            touchEnemy = false;
                            touchGround = false;
                            break;
                        }
                    }
                }

                if (touchEnemy)
                {
                    myTarget.transform.position = transform.position;
                    myChampAtk.willAtkAround = false;
                    if (!myChampAtk.isTargetting)
                    {
                        myChampAtk.isTargetting = true;
                        myChampAtk.AtkTargetObj = target;
                    }
                }
                else if (touchGround)
                {
                    //if (MinimapClick.IsPointerOver)
                    //    return;
                    myTarget.transform.position = h;
                    myChampAtk.willAtkAround = false;
                    if (myChampAtk.isTargetting)
                    {
                        myChampAtk.isTargetting = false;
                        myChampAtk.AtkTargetObj = null;
                        myChampAtk.TheAIPath.canMove = true;
                        myChampAtk.TheAIPath.canSearch = true;
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (atkCommand)
                {
                    atkCommand = false;
                    Vector3 h = Vector3.zero;
                    GameObject target = null;
                    bool touchGround = false;
                    bool touchEnemy = false;
                    v = Input.mousePosition;
                    r = Camera.main.ScreenPointToRay(v);
                    hits = Physics.RaycastAll(r);
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.tag.Equals("Terrain"))
                        {
                            h = hit.point;
                            h.y = 1;
                            touchGround = true;
                            fxpool.GetPool("Force", h);
                        }
                        else if (hit.collider.tag.Equals("Minion"))
                        {
                            if (!hit.collider.name.Contains(playerTeam))
                                if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                    touchEnemy = true;
                            fxpool.GetPool("Force", h, hit.transform.gameObject);
                        }
                        else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                        {
                            if (!hit.collider.gameObject.GetComponent<ChampionBehavior>().Team.Equals(playerTeam))
                                if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                    touchEnemy = true;
                            fxpool.GetPool("Force", h, hit.transform.gameObject);
                        }
                        else if (hit.collider.tag.Equals("Tower"))
                        {
                            TowerBehaviour t = hit.collider.gameObject.GetComponent<TowerBehaviour>();
                            if (t.isCanAtkMe)
                                if (!t.Team.Equals(playerTeam))
                                    //if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                    touchEnemy = true;
                        }
                        else if (hit.collider.tag.Equals("Suppressor") || hit.collider.tag.Equals("Nexus"))
                        {
                            SuppressorBehaviour s = hit.collider.gameObject.GetComponent<SuppressorBehaviour>();
                            if (s.isCanAtkMe)
                                if (!s.Team.Equals(playerTeam))
                                    //if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                    touchEnemy = true;
                        }
                        else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
                        {
                            //if (hit.collider.GetComponentInChildren<MonsterBehaviour>().monAtk.isAtking)//정글이 누굴 공격중이다
                            if (hit.collider.GetComponent<FogOfWarEntity>().isCanTargeting)
                                touchEnemy = true;
                            fxpool.GetPool("Force", h, hit.transform.gameObject);
                        }
                        if (touchEnemy)
                        {
                            target = hit.collider.gameObject;
                            break;
                        }
                    }

                    if (touchEnemy)
                    {
                        //myTarget.transform.position = transform.position;
                        myChampAtk.willAtkAround = false;
                        if (!myChampAtk.isTargetting)
                        {
                            myChampAtk.isTargetting = true;
                            myChampAtk.isWarding = false;
                            myChampAtk.AtkTargetObj = target;
                        }
                    }
                    else if (touchGround)
                    {//여기 고치기
                        myTarget.transform.position = h;
                        myChampAtk.willAtkAround = true;
                        if (myChampAtk.isTargetting)
                        {
                            myChampAtk.isTargetting = false;
                            myChampAtk.isWarding = false;
                            myChampAtk.AtkTargetObj = null;
                        }
                    }
                }
                else if (wardCommand)
                {
                    Vector3 h = Vector3.zero;
                    v = Input.mousePosition;
                    r = Camera.main.ScreenPointToRay(v);
                    hits = Physics.RaycastAll(r);
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.tag.Equals("Terrain"))
                        {
                            h = hit.point;
                            h.y = 1;
                            bool isNotCollision = true;
                            Collider[] Cols = Physics.OverlapSphere(h, 1f);
                            foreach (Collider a in Cols)
                            {
                                if (a.tag.Equals("WarCollider"))
                                {
                                    isNotCollision = false;
                                    break;
                                }
                            }
                            if (isNotCollision)
                            {
                                wardCommand = false;
                                myChampAtk.WantBuildWard(h);
                            }
                        }
                    }
                }
            }
        }
    }
}