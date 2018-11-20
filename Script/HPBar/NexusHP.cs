using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NexusHP : MonoBehaviour {

    private ProgressBar NHPBar = null;
    public GameObject NmakeProgress = null;
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
        GameObject CanvasObject = GameObject.FindGameObjectWithTag("HpbarCanvas");
        myCanvas = CanvasObject.GetComponent<Canvas>();
        TheFogEntity = GetComponent<FogOfWarEntity>();
        suppressorBehaviour = GetComponent<SuppressorBehaviour>();
        team = suppressorBehaviour.Team.ToLower();
    }

    // Use this for initialization
    void Start () {
        BasicSetting();
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        if (NmakeProgress != null)
        {
            if (TheFogEntity.isCanTargeting)
            {
                if (!NmakeProgress.activeInHierarchy)
                    NmakeProgress.SetActive(true);
            }
            else
            {
                if (NmakeProgress.activeInHierarchy)
                    NmakeProgress.SetActive(false);
            }
            pos.y += 150f;
            NmakeProgress.transform.position = pos;
            NmakeProgress.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        RefreshHP();
    }

    public void RefreshHP()
    {
        if (suppressorBehaviour == null)
            return;
        NHPBar.value = suppressorBehaviour.towerstat.Hp / suppressorBehaviour.towerstat.MaxHp;

        if (NHPBar.value <= 0)
            NmakeProgress.SetActive(false);
    }

    public void InitProgressBar()
    {
        if (NmakeProgress != null)
            NmakeProgress.SetActive(false);
    }

    public void BasicSetting()
    {
       
            NmakeProgress = Pool_HP.current.GetPooledHPBar("NexusHPBar");
        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        NmakeProgress.transform.position = pos;

        NHPBar = NmakeProgress.GetComponentInChildren<ProgressBar>();
        NHPBar.value = 1;

        ProgressBarColorChange();
    }

    public void ProgressBarColorChange()
    {
        if (suppressorBehaviour == null)
            suppressorBehaviour = GetComponent<SuppressorBehaviour>();

        if (PhotonNetwork.player.GetTeam().ToString().Equals(suppressorBehaviour.Team.ToLower()))
            NHPBar.Bar.GetComponent<Image>().color = myTeamColor;
        else
            NHPBar.Bar.GetComponent<Image>().color = enemyTeamColor;
    }
}
