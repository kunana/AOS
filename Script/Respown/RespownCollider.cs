using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespownCollider : MonoBehaviour
{
    List<GameObject> triggerList = new List<GameObject>();
    public bool trigger = false;
    private void OnTriggerEnter(Collider other)
    {//respown하는 애들은 respownchecker를 넣어준다. 우선은 미니언들만 리스폰 존을 만들어 둠.
        if (other.tag.Equals("RespownChecker"))
        {
            triggerList.Add(other.gameObject);
            if (!trigger)
                trigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("RespownChecker"))
        {
            if (triggerList.Contains(other.gameObject))
                triggerList.Remove(other.gameObject);
            if (triggerList.Count < 1)
                trigger = false;
        }
    }
}
