using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CSText : MonoBehaviour {

    TMPro.TextMeshPro textmesh;
    public CsTextPool CsPool;
    public Material mesh;
    private Vector3 curpos = Vector3.zero;
    public string cstext;
    public bool once;
    void Awake()
    {
        textmesh = GetComponent<TMPro.TextMeshPro>();
        mesh = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        cstext = textmesh.text;
    }
    void OnEnable()
    {
        Invoke("TextOff", 2.5f);
    }
    void Update()
    {   
        if(!once)
        {
            once = true;
            textmesh.text = cstext;
            transform.DOMoveY(transform.position.y + 3.0f, 0.2f);
            textmesh.DOFade(0f, 1.5f);
            mesh.DOFade(0f, 1.5f);
        }
        //cstext = " ";
    }

    private void TextOff()
    {
        once = false;
        textmesh.text = " ";
        transform.position = curpos;
        textmesh.DOFade(1f, 0.1f);
        mesh.DOFade(1f, 0.1f);
        gameObject.SetActive(false);
    }
}
