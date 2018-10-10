using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICanvas : MonoBehaviour {

    public GameObject Item;
    public GameObject Skill;
    public GameObject Stat;
    public GameObject Icon;
    public GameObject Recall;
    public GameObject Tooltip;
    public GameObject ItemTooltip;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
        {
            Item.GetComponent<ItemUI>().ShopButton();
        }
	}
}
