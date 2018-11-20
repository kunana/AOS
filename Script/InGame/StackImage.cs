using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StackImage : MonoBehaviour
{
    public Dictionary<string, Image> ImageDic = new Dictionary<string, Image>();
    public Dictionary<string, TextMeshProUGUI> TextDic = new Dictionary<string, TextMeshProUGUI>();
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform t = transform.GetChild(i);
            ImageDic.Add(t.name, t.GetComponent<Image>());
            TextDic.Add(t.name, ImageDic[t.name].GetComponentInChildren<TextMeshProUGUI>());
        }
    }
}
