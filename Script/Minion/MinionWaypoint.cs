using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class MinionWaypoint : MonoBehaviour
{
    public GameObject RedPoint;
    public GameObject BluePoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (other.tag.Equals("Minion"))
        {
            MinionAtk min = other.GetComponent<MinionBehavior>().minAtk;
            //MinionAtk min = other.GetComponent<MinionAtk>();
            if (other.name.Contains("Blue"))
            {
                if (min.nowTarget == null)
                {
                    min.nowTarget = RedPoint;
                    other.GetComponent<AIDestinationSetter>().target = RedPoint.transform;
                }
                else if (min.nowTarget.tag.Equals("WayPoint"))
                {
                    min.nowTarget = RedPoint;
                    other.GetComponent<AIDestinationSetter>().target = RedPoint.transform;
                }
                min.MoveTarget = RedPoint;
            }
            else if (other.name.Contains("Red"))
            {
                if (min.nowTarget == null)
                {
                    min.nowTarget = BluePoint;
                    other.GetComponent<AIDestinationSetter>().target = BluePoint.transform;
                }
                else if (min.nowTarget.tag.Equals("WayPoint"))
                {
                    min.nowTarget = BluePoint;
                    other.GetComponent<AIDestinationSetter>().target = BluePoint.transform;
                }
                //other.GetComponent<MinionAtk>().MoveTarget = BluePoint;
                min.MoveTarget = BluePoint;
            }
        }
    }
}