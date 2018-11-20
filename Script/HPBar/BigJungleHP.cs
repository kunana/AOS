using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BigJungleHP : MonoBehaviour {

    private ProgressBar JHPBar = null;
    public GameObject JmakeProgress = null;
    private Camera mainCamera = null;
    public Canvas myCanvas = null;
    private MonsterBehaviour monsterBehaviour;
    private FogOfWarEntity TheFogEntity;
    private Text HPText;

    private void Awake()
    {
        mainCamera = Camera.main;
        GameObject CanvasObject = GameObject.FindGameObjectWithTag("HpbarCanvas");
        myCanvas = CanvasObject.GetComponent<Canvas>();
        TheFogEntity = GetComponent<FogOfWarEntity>();
        monsterBehaviour = GetComponent<MonsterBehaviour>();
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
        if (JmakeProgress != null)
        {
            if (TheFogEntity.isCanTargeting)
            {
               // if (!JmakeProgress.activeInHierarchy)
                    JmakeProgress.SetActive(true);
            }
            else
            {
               // if (JmakeProgress.activeInHierarchy)
                    JmakeProgress.SetActive(false);
            }
            pos.y += 130f;
            JmakeProgress.transform.position = pos;
            JmakeProgress.transform.localScale = new Vector3(1f, 1f, 1f);

            RefreshHP();
        }

    }

    public void RefreshHP()
    {
        if (JmakeProgress == null || monsterBehaviour == null || monsterBehaviour.stat == null)
            return;
        JHPBar.value = monsterBehaviour.stat.Hp / monsterBehaviour.stat.MaxHp;
        HPText = JmakeProgress.transform.Find("Level_Image").GetComponentInChildren<Text>();
        HPText.text = Mathf.FloorToInt(monsterBehaviour.stat.Hp).ToString();
    }

    public void InitProgressBar()
    {
        if (JmakeProgress != null)
            JmakeProgress.SetActive(false);
        JmakeProgress = null;
        JHPBar = null;
    }

    public void BasicSetting()
    {
        JmakeProgress = Pool_HP.current.GetPooledHPBar("BigJungleHPBar");
        Vector3 pos = CanvasExt.WorldToCanvas(myCanvas, transform.position, mainCamera);
        JmakeProgress.transform.position = pos;

        JHPBar = JmakeProgress.GetComponentInChildren<ProgressBar>();
        JHPBar.value = 1;
    }
}
