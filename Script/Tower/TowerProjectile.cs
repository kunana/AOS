using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    public Transform target;
    private bool hasParent = false;
    private void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            transform.position += dir.normalized * Time.deltaTime * 40f;

            // 포탄이 타겟에게 거의 다도달하면 액티브꺼줌
            if (dir.magnitude < 0.5f)
            {
                CancelInvoke();
                ActiveFalse();
            }
        }
    }

    private void OnEnable()
    {
        if (!hasParent)
        {
            if (transform.parent == null)
                hasParent = false;
            else
                hasParent = true;
        }
        if (hasParent)
            transform.position = transform.parent.position;
    }
    public void ActiveFalse(float t)
    {
        Invoke("ActiveFalse", t);
    }

    private void ActiveFalse()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (!hasParent)
        {
            if (transform.parent == null)
                hasParent = false;
            else
                hasParent = true;
        }
        if (hasParent)
            transform.position = transform.parent.position;
        target = null;
    }
}