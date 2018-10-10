using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFxPooling : MonoBehaviour
{

    //[Space(10),Header("마우스 커서")]
    public GameObject[] Mousefx = new GameObject[3];  //0 = default, 1= forceAttack  
    public int FXmakeCount = 10;

    public bool MakeOnce; // 이펙트 갯수의 제한 유무
    public float DelayT = 0.3f; // 이펙트 갯수의 제한 유무
    public int curD = 0; // 이펙트 갯수의 제한 유무
    public int curA = 0; // 이펙트 갯수의 제한 유무

    List<GameObject> MouseFxPoolDefault = new List<GameObject>();
    List<GameObject> MouseFxPoolForce = new List<GameObject>();
    private GameObject Temp;

    private void Awake()
    {
        MakeFxDefault();
        MakeFxForceAtk();
    }
    private void MakeFxDefault()
    {
        for (int i = 0; i < FXmakeCount; i++)
        {
            var Fx = Instantiate(Mousefx[0], transform.position, Quaternion.identity, transform);
            MouseFxPoolDefault.Add(Fx);
            Fx.SetActive(false);
        }
    }
    private void MakeFxForceAtk()
    {
        for (int i = 0; i < FXmakeCount; i++)
        {
            var Fx = Instantiate(Mousefx[1], transform.position, Quaternion.identity, transform);
            MouseFxPoolForce.Add(Fx);
            Fx.SetActive(false);
        }
    }

    public void GetPool(string name, Vector3 pos)
    {
        if (name.Equals("Default"))
        {
            if (curD == FXmakeCount)
                curD = 0;
            Temp = MouseFxPoolDefault[curD];
            curD++;
            pos.y += 1;
            Temp.transform.position = pos;
            Temp.SetActive(true);
        }
        else if (name.Equals("Force"))
        {
            if (curA == FXmakeCount)
                curA = 0;
            Temp = MouseFxPoolForce[curA];
            curA++;
            pos.y += 1;
            Temp.transform.position = pos;
            Temp.SetActive(true);
        }
    }
}
