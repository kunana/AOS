using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlistarR : MonoBehaviour
{

    public void OnParticleSystemStopped()
    {
        gameObject.SetActive(false);
        transform.position = Vector3.zero;

    }
}
