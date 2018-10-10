using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetProjectile : MonoBehaviour
{

    public void ActiveFalse(float t)
    {
        Invoke("ActiveFalse", t);
    }

    private void ActiveFalse()
    {
        gameObject.SetActive(false);
    }
}