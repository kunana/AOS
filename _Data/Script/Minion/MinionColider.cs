using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MinionColider : MonoBehaviour
{
    private enum MinionType { Melee, Magic, Siege, Super }
    private enum Coliders { vaild, attack }
    [SerializeField]
    private Coliders colName;
    [SerializeField]
    private MinionType minionType;
    private float vaildRange = 15;
    private bool VaildTriggerOn = false;
    //밀리&슈퍼 / 매직,시즈
    private float[] AttackRange = { 3, 5 };
    private MinionBehavior mi;
    SphereCollider col;

    private void Start()
    {
        col = GetComponent<SphereCollider>();
        mi = GetComponentInParent<MinionBehavior>();
        setMinionCol();
    }
    private void setMinionCol()
    {
        //0 = 밀리 1 = 원거리
        if (transform.parent.name.Contains("Super"))
        {
            minionType = MinionType.Super;
            setcol(0);
        }
        else if (transform.parent.name.Contains("Melee"))
        {
            minionType = MinionType.Melee;
            setcol(0);
        }
        else if (transform.parent.name.Contains("Magic"))
        {
            minionType = MinionType.Magic;
            setcol(1);
        }
        else if (transform.parent.name.Contains("Siege"))
        {
            minionType = MinionType.Siege;
            setcol(1);
        }
    }
    private void setcol(int attackType)
    {
        if (transform.name.Contains("Vaild"))
        {
            colName = Coliders.vaild;
            col.radius = vaildRange;
        }
        else if (transform.name.Contains("Attack"))
        {
            colName = Coliders.attack;
            col.radius = AttackRange[attackType];
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        //14 = Red 15 = Blue
        //var otherParent = other.transform.parent;

        if (colName.Equals(Coliders.vaild) && other.CompareTag("Minion"))
        {   
            if (this.transform.parent.name.Contains("Minion") && this.transform.parent.gameObject.layer != other.gameObject.layer)
            {
                VaildTriggerOn = true;
                var dist = Vector3.Distance(transform.parent.position, other.transform.position);
                Transform tr = null;
                float maxDistance = 10000f;
                if (maxDistance > dist && other.gameObject.activeInHierarchy)
                {
                    tr = other.transform;
                    maxDistance = dist;
                    mi.CurTarget = tr;
                }
            }
        }
    }
    //private void OnTriggerStay(Collider other)
    //{
    //    var minionbehavior = this.GetComponentInParent<MinionBehavior>();
    //    if(minionbehavior.CurTarget == null)
    //    {
    //        if (colName.Equals(Coliders.attack) && other.CompareTag("Minion"))
    //        {
    //            if (this.transform.parent.name.Contains("Minion") && this.transform.parent.gameObject.layer != other.gameObject.layer)
    //            {
    //                VaildTriggerOn = true;
    //                var dist = Vector3.Distance(transform.parent.position, other.transform.position);
    //                Transform tr = null;
    //                float maxDistance = 10000f;
    //                if (maxDistance > dist && other.gameObject.activeInHierarchy)
    //                {
    //                    tr = other.transform;
    //                    maxDistance = dist;
    //                    mi.CurTarget = tr;
    //                }
    //            }
    //        }
    //    }
       
    //}

    private void OnTriggerExit(Collider other)
    {
        var otherParent = other.transform.parent;

        //if (colName.Equals(Coliders.vaild))
        //{
        //    if (mi.FindTargetList(other.transform))
        //        mi.delTargetList(other.transform);
        //}

        if (colName.Equals(Coliders.attack))
        {
            if (otherParent != null)
            {
                if (otherParent.name.Contains("Minion"))
                {
                    mi.canAttack = false;
                }
            }
        }
    }
}
