using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsheEHawkWard : MonoBehaviour
{
    public GameObject particle;
    private void OnEnable()
    {
        Invoke("ActiveFalse", 5f);
    }

    public void ActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
