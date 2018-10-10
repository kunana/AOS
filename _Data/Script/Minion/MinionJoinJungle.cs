using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionJoinJungle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
            if (other.tag.Equals("Minion"))
            {
                other.GetComponent<MinionBehavior>().minAtk.RemoveNowTarget();
                //other.GetComponent<MinionAtk>().RemoveNowTarget();
            }
    }
}
