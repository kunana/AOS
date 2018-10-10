using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RightClickPurchase : MonoBehaviour, IPointerClickHandler
{
    private bool one_click = false;
    private float double_click_checktime = 0;
    private float check_delay = 0.4f;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (one_click)
        {
            if ((Time.time - double_click_checktime) > check_delay)
                one_click = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().ItemPurchase();
        }

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 첫클릭하면 변수 true, 첫클릭한 시간체크
            if (!one_click)
            {
                one_click = true;
                double_click_checktime = Time.time;
            }
            // 더블클릭이면
            else
            {
                one_click = false;

                GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().ItemPurchase();
            }
        }    
    }
}
