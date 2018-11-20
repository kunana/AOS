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
    private GameObject TabUI = null;
    [SerializeField]
    private GameObject ShopCanvas;
    [SerializeField]
    private GameObject BigPing;
    [SerializeField]
    private GameObject SmallPing;

    Vector3 InitialCamPos;
    PingSign pingsign;
    public float PingResetTime = 7f;
    public bool IsPointerOverGameObject = false;
    public bool InitialCamPosbool = false;
    public bool keyinpuy = false;

    void Start()
    {
        OptionWindows.SetActive(false);
        TabUI.SetActive(false);
        pingsign = BigPing.GetComponent<PingSign>();
        InitialCamPos = Camera.main.transform.position;
       
    }

    void Update()
    {
        
        //상점
        if (Input.GetKeyDown(KeyCode.Escape) && !ShopCanvas.gameObject.GetActive() && !OptionWindows.gameObject.GetActive())
        {
            OnOff = true;
            SoundManager.instance.PlaySound(SoundManager.instance.UI_Open);
            OptionWindows.SetActive(true);
        }
        //옵션
        else if (Input.GetKeyDown(KeyCode.Escape) && OptionWindows.gameObject.GetActive())
        {
            OnOff = false;
            SoundManager.instance.PlaySound(SoundManager.instance.UI_Close);
            OptionWindows.GetComponent<KTYOPTION>().CloseOptionWindow();
        }
        //Big 핑UI
        else if (Input.GetKey(KeyCode.LeftAlt) && Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false 
            && !SmallPing.GetActive())
        {
            BigPing.SetActive(true);
        }
        
    }

    IEnumerator PingSignOff()
    {
        yield return new WaitForSeconds(5f);
        BigPing.SetActive(false);
    }
}   
