using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    public Image Bar = null;
    [Range(0, 1.0f)]
    public float value = 1;
    public Text ProgressText;
    [HideInInspector]
    public string text = "";

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Bar.enabled)
        Bar.fillAmount = value;

        if(ProgressText != null)
        {
            ProgressText.text = this.text;
        }
	}
}
