using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSort : MonoBehaviour {

    [HideInInspector]
    public int selectedID = 0;
    private GameObject SelectObject = null;

	// Use this for initialization
	void Start () {
        select(selectedID, transform.Find("All").gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void select(int id, GameObject go)
    {
        // 이전에 선택되있던 오브젝트의 색상 원래대로. 배경 끔
        if (SelectObject != null)
        {
            Color basicColor = new Color(43f/255f, 96f/255f, 93f/255f, 60f/255f);
            SelectObject.GetComponent<Image>().color = basicColor;

            Color basicTextColor;
            // 색구분
            if (selectedID == 0 || selectedID == 1 || selectedID == 4 || selectedID == 9 || selectedID == 14 || selectedID == 19)
                basicTextColor = new Color(125f / 255f, 116f / 255f, 82f / 255f, 1); 
            else
                basicTextColor = new Color(44f / 255f, 100f / 255f, 95f / 255f, 1);
            SelectObject.transform.GetChild(0).GetComponent<Text>().color = basicTextColor;

            SelectObject.GetComponent<Image>().enabled = false;
        }

        selectedID = id;
        SelectObject = go;

        Color selectedColor = new Color(114f / 255f, 164f / 255f, 147f / 255f, 60f / 255f);
        SelectObject.GetComponent<Image>().color = selectedColor;

        Color selectedTextColor;
        if (selectedID == 0 || selectedID == 1 || selectedID == 4 || selectedID == 9 || selectedID == 14 || selectedID == 19)
            selectedTextColor = new Color(219f / 255f, 204f / 255f, 95f / 144f, 1);
        else
            selectedTextColor = new Color(78f / 255f, 200f / 255f, 189f / 255f, 1);
        SelectObject.transform.GetChild(0).GetComponent<Text>().color = selectedTextColor;

        SelectObject.GetComponent<Image>().enabled = true;


        // 클릭한 부분의 ID를 받아 정렬하는 아이템 생성함수로 넘겨줌
        GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().ItemCreate(selectedID);
    }
}
