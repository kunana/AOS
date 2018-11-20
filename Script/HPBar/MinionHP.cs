using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class CanvasExt
{
    public static Vector2 WorldToCanvas(this Canvas canvas,
        Vector3 world_pos, Camera cam = null)
    {
        if (cam == null)
            cam = Camera.main;
        return cam.WorldToScreenPoint(world_pos);
        //var viewport_pos = cam.WorldToViewportPoint(world_pos);
        //var canvas_rect = canvas.GetComponent<RectTransform>();
        //return new Vector2((viewport_pos.x * canvas_rect.sizeDelta.x),
        //(viewport_pos.y * canvas_rect.sizeDelta.y));
    }
}

public class MinionHP : MonoBehaviour
{
    private ProgressBar mHpBar = null;
    public GameObject makeProgress = null;
    public GameObject RealBar = null;
    private Camera mainCamera = null;
    public Canvas myCanvas = null;
    private MinionBehavior minionBehavior;
    private FogOfWarEntity TheFogEntityRed;
    private FogOfWarEntity TheFogEntityBlue;
    private FogOfWarEntity fog;
    private Color myTeamColor = new Color(57 / 255f, 204 / 255f, 1, 1);
    private Color enemyTeamColor = new Color(1, 60 / 255f, 60 / 255f, 1);
    public GameObject ThisMinion;
    private string team;
    Vector3 pos;
    private void Awake()
    {
        mainCamera = Camera.main;
        GameObject CanvasObject = GameObject.FindGameObjectWithTag("HpbarCanvas");
        myCanvas = CanvasObject.GetComponent<Canvas>();
        fog = GetComponent<FogOfWarEntity>();
        minionBehavior = GetComponent<MinionBehavior>();
        team = minionBehavior.team.ToString().ToLower();
        ThisMinion = this.gameObject;
        RealBar = transform.GetChild(0).gameObject;
    }

    void Start()
    {
        //Vector3 canvasPos = CanvasExt.WorldToCanvas(myCanvas, transform.position);        
        //makeProgress.transform.position = myCanvas.transform.position;
        BasicSetting();
    }

    // Update is called once per frame
    void Update()
    {
        if (makeProgress == null)
            return;

        if (this.gameObject.activeInHierarchy)
        {
            pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
            if (RealBar != null)
            {
                if (minionBehavior.mesh.enabled == true)
                {
                    RealBar.SetActive(true);
                }
                else if (minionBehavior.mesh.enabled == false)
                {
                    RealBar.SetActive(false);
                }
                pos.y += 40f;
                makeProgress.transform.position = pos;
                makeProgress.transform.localScale = new Vector3(1f, 1f, 1f);
                RefreshHP();
            }
            else
            {
                hpbarOn();
            }
        }
    }

    public void RefreshHP()
    {
        if (minionBehavior == null)
            return;
        if (mHpBar == null)
        {
            hpbarOn();
        }
        mHpBar.value = minionBehavior.stat.Hp / minionBehavior.stat.MaxHp;
    }

    public void InitProgressBar()
    {
        if (makeProgress != null)
        {
            makeProgress.SetActive(false);
            makeProgress = null;
            RealBar = null;
            mHpBar = null;
        }
    }

    // minionBehavior에서 onEnable에서 팀설정 이후에 불러줌
    public void BasicSetting()
    {
        makeProgress = Pool_HP.current.GetPooledHPBar("MinionHPBar");
        minionBehavior.hpbar = makeProgress;
        //RealBar = makeProgress.transform.GetChild(0).gameObject;
        makeProgress.SetActive(true);
        Invoke("hpbarOn", 0.5f);
        pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        makeProgress.transform.position = pos;
    }

    private void hpbarOn()
    {
        RealBar = makeProgress.transform.GetChild(0).gameObject;
        RealBar.SetActive(true);
        mHpBar = makeProgress.GetComponent<ProgressBar>();
        mHpBar.value = 1;
        ProgressBarColorChange();
    }

    public void ProgressBarColorChange()
    {
        if (minionBehavior == null)
            minionBehavior = GetComponent<MinionBehavior>();

        if (PhotonNetwork.player.GetTeam().ToString().Equals(minionBehavior.team.ToString()))
            mHpBar.Bar.GetComponent<Image>().color = myTeamColor;
        else
            mHpBar.Bar.GetComponent<Image>().color = enemyTeamColor;
    }
}
