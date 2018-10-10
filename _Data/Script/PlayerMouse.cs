using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouse : Photon.MonoBehaviour
{
    public GameObject myTarget;
    public ChampionAtk myChampAtk;
    public bool atkCommand = false;
    // Use this for initialization
    Vector3 v;
    Ray r;
    RaycastHit[] hits;
    Vector3 dest;
    private void Update()
    {
        
        if (base.photonView.isMine)
        {
            if (Input.GetKeyDown(KeyCode.A))
                atkCommand = !atkCommand;
            if (Input.GetMouseButtonDown(1))
            {//우선 이동만. 나중엔 공격인지 뭔지 그런 것 판단도 해야 할 것.
                if (atkCommand)
                    atkCommand = false;
                Vector3 h = Vector3.zero;
                GameObject target = null;
                bool touchGround = false;
                bool touchEnemy = false;
                v = Input.mousePosition;
                r = Camera.main.ScreenPointToRay(v);
                hits = Physics.RaycastAll(r);
                foreach (RaycastHit hit in hits)
                {
                    if (hit.collider.tag.Equals("Terrain"))
                    {
                        h = hit.point;
                        h.y = 1;
                        touchGround = true;
                    }
                    else if (hit.collider.tag.Equals("Minion") || hit.collider.tag.Equals("Player")
                        || hit.collider.tag.Equals("Tower"))
                    {
                        touchEnemy = true;
                        target = hit.collider.gameObject;
                    }
                }

                if (touchEnemy)
                {
                    myTarget.transform.position = transform.position;
                    myChampAtk.willAtkAround = false;
                    if (!myChampAtk.isTargetting)
                    {
                        myChampAtk.isTargetting = true;
                        myChampAtk.AtkTargetObj = target;
                    }
                }
                else if (touchGround)
                {
                    myTarget.transform.position = h;
                    myChampAtk.willAtkAround = false;
                    if (myChampAtk.isTargetting)
                    {
                        myChampAtk.isTargetting = false;
                        myChampAtk.AtkTargetObj = null;
                    }
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                if (atkCommand)
                {
                    atkCommand = false;
                    Vector3 h = Vector3.zero;
                    GameObject target = null;
                    bool touchGround = false;
                    bool touchEnemy = false;
                    v = Input.mousePosition;
                    r = Camera.main.ScreenPointToRay(v);
                    hits = Physics.RaycastAll(r);
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.collider.tag.Equals("Terrain"))
                        {
                            h = hit.point;
                            h.y = 1;
                            touchGround = true;
                        }
                        else if (hit.collider.tag.Equals("Minion") || hit.collider.tag.Equals("Player")
                            || hit.collider.tag.Equals("Tower"))
                        {
                            touchEnemy = true;
                            target = hit.collider.gameObject;
                        }
                    }

                    if (touchEnemy)
                    {
                        myTarget.transform.position = transform.position;
                        myChampAtk.willAtkAround = false;
                        if (!myChampAtk.isTargetting)
                        {
                            myChampAtk.isTargetting = true;
                            myChampAtk.AtkTargetObj = target;
                        }
                    }
                    else if (touchGround)
                    {//여기 고치기
                        myTarget.transform.position = h;
                        myChampAtk.willAtkAround = true;
                        if (myChampAtk.isTargetting)
                        {
                            myChampAtk.isTargetting = false;
                            myChampAtk.AtkTargetObj = null;
                        }
                    }
                }
            }
        }
    }
}