using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionOpen : MonoBehaviour
{
    private bool OnOff = false;
    [SerializeField]
    private GameObject OptionWindows = null;
    [SerializeField]
    private GameObject ShopCanvas;
    [SerializeField]
    private GameObject BigPing;
    [SerializeField]
    private GameObject SmallPing;

    Vector3 InitialCamPos;
    PingSign pingsign;
    public float PingResetTime = 7f;

    void Start()
    {
        OptionWindows.SetActive(false);
        pingsign = BigPing.GetComponent<PingSign>();
        InitialCamPos = Camera.main.transform.position;
       
    }

    void Update()
    {
        
        //상점
        if (Input.GetKeyDown(KeyCode.Escape) && !ShopCanvas.gameObject.GetActive() && !OptionWindows.gameObject.GetActive())
        {
            OnOff = true;
            OptionWindows.SetActive(true);
        }
        //옵션
        else if (Input.GetKeyDown(KeyCode.Escape) && OptionWindows.gameObject.GetActive())
        {
            OnOff = false;
            OptionWindows.SetActive(false);
        }
        //Big 핑UI
        else if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0) 
            && InitialCamPos.Equals(Camera.main.transform.position) && EventSystem.current.IsPointerOverGameObject() == false 
            && !SmallPing.GetActive())
        {
            BigPing.SetActive(true);
        }

        InitialCamPos = Camera.main.transform.position;

        if(!pingsign.CanMakePing)
        {
            Invoke("PingReset", 7.0f);
        }

    }

    private void PingReset()
    {
        pingsign.CanMakePing = true;
        pingsign.MakeCount = 0;
    }
}   
