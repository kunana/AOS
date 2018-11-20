using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupHP : MonoBehaviour
{

    private ProgressBar THPBar = null;
    public GameObject TmakeProgress = null;
    private Camera mainCamera = null;
    public Canvas myCanvas = null;
    private SuppressorBehaviour suppressorBehaviour;
    private FogOfWarEntity TheFogEntity;
    private Color myTeamColor = new Color(57 / 255f, 204 / 255f, 1, 1);
    private Color enemyTeamColor = new Color(1, 60 / 255f, 60 / 255f, 1);
    private string team;

    private void Awake()
    {
        mainCamera = Camera.main;
        myCanvas = GameObject.FindGameObjectWithTag("HpbarCanvas").GetComponent<Canvas>();
        TheFogEntity = GetComponent<FogOfWarEntity>();
        suppressorBehaviour = GetComponent<SuppressorBehaviour>();
        team = suppressorBehaviour.Team.ToLower();

    }

    // Use this for initialization
    void Start()
    {
        BasicSetting();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        if (TmakeProgress != null)
        {
            if (TheFogEntity.isCanTargeting)
            {
                if (!TmakeProgress.activeInHierarchy)
                    TmakeProgress.SetActive(true);
            }
            else
            {
                if (TmakeProgress.activeInHierarchy)
                    TmakeProgress.SetActive(false);
            }
            pos.y += 100f;
            TmakeProgress.transform.position = pos;
            TmakeProgress.transform.localScale = new Vector3(1f, 1f, 1f);
            if (TmakeProgress.activeInHierarchy)
            RefreshHP();
        }

    }

    public void RefreshHP()
    {
        if (suppressorBehaviour == null)
            return;
        THPBar.value = suppressorBehaviour.towerstat.Hp / suppressorBehaviour.towerstat.MaxHp;
    }

    public void HpbarOff()
    {
        if (TmakeProgress != null)
            TmakeProgress.SetActive(false);
    }

    public void BasicSetting()
    {

        TmakeProgress = Pool_HP.current.GetPooledHPBar("TowerHPBar");
        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        
        THPBar = TmakeProgress.GetComponentInChildren<ProgressBar>();
        THPBar.value = 1;

        ProgressBarColorChange();
    }

    public void respawn()
    {
        TmakeProgress.SetActive(true);
        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        TmakeProgress.transform.position = pos;

        THPBar = TmakeProgress.GetComponentInChildren<ProgressBar>();
        THPBar.value = 1;

        ProgressBarColorChange();
    }

    public void ProgressBarColorChange()
    {
        if (suppressorBehaviour == null)
            suppressorBehaviour = GetComponent<SuppressorBehaviour>();

        if (PhotonNetwork.player.GetTeam().ToString().Equals(suppressorBehaviour.Team.ToLower()))
            THPBar.Bar.GetComponent<Image>().color = myTeamColor;
        else
            THPBar.Bar.GetComponent<Image>().color = enemyTeamColor;
    }
}
