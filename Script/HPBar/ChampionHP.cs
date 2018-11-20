using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChampionHP : Photon.MonoBehaviour {

    private ProgressBar cHpBar = null;
    private ProgressBar cMpBar = null;
    public GameObject CmakeProgress = null;
    public GameObject RealBar = null;
    private Camera mainCamera = null;
    public Canvas myCanvas = null;
    private ChampionData championData;
    private ChampionBehavior championBehavior;
    private FogOfWarEntity TheFogEntity;
    private Color myTeamColor = new Color(57 / 255f, 204 / 255f, 1, 1);
    private Color enemyTeamColor = new Color(1, 60 / 255f, 60 / 255f, 1);
    private Text Leveltext;
    private bool isload =false;
    private string team;
    Vector3 pos;

    private void OnLevelWasLoaded(int level)
    {
        if(SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
        {
            Invoke("BasicSet", 5f);
        }
    }
    private void BasicSet()
    {
        mainCamera = Camera.main;
        GameObject CanvasObject = GameObject.FindGameObjectWithTag("HpbarCanvas");
        myCanvas = CanvasObject.GetComponent<Canvas>();
        TheFogEntity = GetComponent<FogOfWarEntity>();
        championData = GetComponent<ChampionData>();
        championBehavior = GetComponent<ChampionBehavior>();
        if(!isload)
        {
            BasicSetting();
            isload = true;
        }
    }

    void Update()
    {
        if(isload && CmakeProgress != null)
        {
            pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
            
                if (championBehavior.mesh.enabled == true)
                {
                   CmakeProgress.SetActive(true);
                }
                else if(championBehavior.mesh.enabled == false)
                {
                   CmakeProgress.SetActive(false);
                }
                pos.y += 100f;
                CmakeProgress.transform.position = pos;
                //CmakeProgress.transform.localScale = new Vector3(1f, 1f, 1f);
            
            RefreshHP();
            //if(!photonView.isMine)
            //print("HP "+ PhotonNetwork.player.NickName + " " + CmakeProgress.gameObject.GetActive());
        }
        
    }

    public void RefreshHP()
    {
        if (championData == null || championBehavior == null)
            return;
        cHpBar.value = championData.totalstat.Hp / championData.totalstat.MaxHp;
        cMpBar.value = championData.totalstat.Mp / championData.totalstat.MaxMp;
        Leveltext.text = championData.totalstat.Level.ToString();
    }

    public void InitProgressBar()
    {
        if (CmakeProgress != null)
            CmakeProgress.SetActive(false);
        CmakeProgress = null;
        Leveltext = null;
        cHpBar = null;
        cMpBar = null;
    }

    public void BasicSetting()
    {
       
        CmakeProgress = Pool_HP.current.GetPooledHPBar("ChampionHPBar");
        pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        CmakeProgress.transform.position = pos;
        Leveltext = CmakeProgress.transform.GetChild(2).GetComponentInChildren<Text>();
        //cHpBar = CmakeProgress.GetComponentInChildren<ProgressBar>();
        cHpBar = CmakeProgress.transform.GetChild(0).GetComponent<ProgressBar>();
        cHpBar.value = 1;
        cMpBar = CmakeProgress.transform.GetChild(1).GetComponent<ProgressBar>();
        cMpBar.value = 1;
        ProgressBarColorChange();
    }

    public void ProgressBarColorChange()
    {
        if (championData == null)
            championData = GetComponent<ChampionData>();

        if (PhotonNetwork.player.GetTeam().ToString().Equals(championBehavior.Team.ToLower()))
            cHpBar.Bar.GetComponent<Image>().color = myTeamColor;
        else
            cHpBar.Bar.GetComponent<Image>().color = enemyTeamColor;
    }
}
