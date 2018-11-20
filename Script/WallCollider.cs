using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Minion"))
        {
            MinionAtk mA = other.GetComponent<MinionBehavior>().minAtk;
            if (!mA.isPushing)
                return;
            mA.PushWall();
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            ChampionAtk cA = other.GetComponent<ChampionBehavior>().myChampAtk;
            if (!cA.isPushing)
                return;
            cA.PushWall();
        }
        else if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            MonsterAtk mA = other.GetComponent<MonsterBehaviour>().monAtk;
            if (!mA.isPushing)
                return;
            mA.PushWall();
        }
    }
}
