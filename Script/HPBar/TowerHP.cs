using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerHP : MonoBehaviour {

    private ProgressBar THPBar = null;
    public GameObject TmakeProgress = null;
    private Camera mainCamera = null;
    public Canvas myCanvas = null;
    private TowerBehaviour towerBehaviour;
    private FogOfWarEntity TheFogEntity;
    private Color myTeamColor = new Color(57 / 255f, 204 / 255f, 1, 1);
    private Color enemyTeamColor = new Color(1, 60 / 255f, 60 / 255f, 1);
    private string team;

    private void Awake()
    {
        mainCamera = Camera.main;
        myCanvas = GameObject.FindGameObjectWithTag("HpbarCanvas").GetComponent<Canvas>();
        TheFogEntity = GetComponent<FogOfWarEntity>();
        towerBehaviour = GetComponent<TowerBehaviour>();
        team = towerBehaviour.Team.ToLower();
    }

    void Start () {
        BasicSetting();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        if (TmakeProgress != null)
        {
            if (TheFogEntity.isCanTargeting)
            {
                //if (!TmakeProgress.activeInHierarchy)
                    TmakeProgress.SetActive(true);
            }
            else
            {
                //if (TmakeProgress.activeInHierarchy)
                    TmakeProgress.SetActive(false);
            }
            pos.y += 130f;
            TmakeProgress.transform.position = pos;
            TmakeProgress.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        RefreshHP();
    }

    public void RefreshHP()
    {
        if (towerBehaviour == null)
            return;
        THPBar.value = towerBehaviour.towerstat.Hp / towerBehaviour.towerstat.MaxHp;
    }

    public void InitProgressBar()
    {
        if (TmakeProgress != null)
            TmakeProgress.SetActive(false);
    }

    public void BasicSetting()
    {
      
        TmakeProgress = Pool_HP.current.GetPooledHPBar("TowerHPBar");
        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        TmakeProgress.transform.position = pos;

        THPBar = TmakeProgress.GetComponentInChildren<ProgressBar>();
        THPBar.value = 1;

        ProgressBarColorChange();
    }

    public void ProgressBarColorChange()
    {
        if (towerBehaviour == null)
            towerBehaviour = GetComponent<TowerBehaviour>();

        if (PhotonNetwork.player.GetTeam().ToString().Equals(towerBehaviour.Team.ToLower()))
            THPBar.Bar.GetComponent<Image>().color = myTeamColor;
        else
            THPBar.Bar.GetComponent<Image>().color = enemyTeamColor;
    }
}
