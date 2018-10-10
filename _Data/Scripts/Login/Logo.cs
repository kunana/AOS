using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Logo : MonoBehaviour
{
    RectTransform HOSlogo;
    public Image GameLogo;
    float time = 0.5f;

    private void Awake()
    {
        HOSlogo = GetComponent<RectTransform>();
    }
    private void Update()
    {
        HOSlogo.Rotate(Vector3.back);
    }

}