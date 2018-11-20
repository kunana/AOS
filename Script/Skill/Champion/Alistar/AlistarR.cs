using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlistarR : MonoBehaviour
{
    Transform skillcontainer;
    void Awake()
    {
        skillcontainer = transform.parent;
    }
    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;
        transform.parent = skillcontainer;
    }
}
