using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpTooltip : MonoBehaviour {

    public GameObject tooltip;

    private ChampionData cd;
    private bool mouseover = false;

    // Use this for initialization
    void Start () {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player == null)
        {
            StructureSetting.instance.ActiveTrue();
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        cd = Player.GetComponent<ChampionData>();
    }
	
	// Update is called once per frame
	void Update () {
		if(mouseover)
        {
            tooltip.transform.Find("Text").GetComponent<Text>().text = cd.mystat.Exp.ToString() + " / " + cd.mystat.RequireExp.ToString() + " 경험치";
            tooltip.transform.position = Input.mousePosition + new Vector3(-tooltip.GetComponent<RectTransform>().sizeDelta.x*0.75f, tooltip.GetComponent<RectTransform>().sizeDelta.y);
        }
	}

    public void PointerEnter()
    {
        if(tooltip != null)
        {
            tooltip.SetActive(true);
            mouseover = true;
        }
    }

    public void PointerExit()
    {
        if (tooltip != null)
        {
            tooltip.SetActive(false);
            mouseover = false;
        }
    }
}
