using System.Collections;
using UnityEngine;

public class SuppressorRevive : MonoBehaviour
{
    GameObject mySon;
    SystemMessage SysMsg;
    SupHP suphp;
    private void Awake()
    {
        mySon = transform.GetChild(0).gameObject;
        SysMsg = GameObject.FindGameObjectWithTag("SystemMsg").GetComponent<SystemMessage>();
        if (!suphp)
            suphp = GetComponentInChildren<SupHP>();
    }
    public void WillRevive()
    {
        StartCoroutine("Revive");
    }
    IEnumerator Revive()
    {
        yield return new WaitForSeconds(300f);
        SysMsg.Annoucement(9, true);
        mySon.SetActive(true);
        suphp.respawn();
    }

}
