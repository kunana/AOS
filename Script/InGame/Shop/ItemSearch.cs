using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSearch : MonoBehaviour {
    public GameObject SearchView;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Search(string input)
    {
        if (input.Length == 0)
            SearchView.gameObject.SetActive(false);
        else
        {
            SearchView.gameObject.SetActive(true);

            GameObject.FindGameObjectWithTag("ShopCanvas").GetComponent<Shop>().ItemSearch(input);
        }
    }

    public void SearchClose()
    {
        GetComponent<InputField>().text = "";
    }
}
