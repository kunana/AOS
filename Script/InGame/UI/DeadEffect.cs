using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public class DeadEffect : MonoBehaviour {

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TurnOn()
    {
        spriteRenderer.enabled = true;
    }

    public void TurnOff()
    {
        spriteRenderer.enabled = false;
    }
}
