using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProjectile : MonoBehaviour
{
    private void OnEnable()
    {
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
        transform.position = transform.parent.position;
    }
}
