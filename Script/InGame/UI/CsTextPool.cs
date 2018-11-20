using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsTextPool : MonoBehaviour
{
    public int amount = 30;
    public GameObject csText;
    public List<GameObject> textPool = new List<GameObject>();

    void Awake()
    {
        MakeCSText();
    }

    private void MakeCSText()
    {
        for (int i = 0; i < amount; i++)
        {
            var fx = Instantiate(csText, transform);
            textPool.Add(fx);
            fx.transform.position = Vector3.zero;
            fx.gameObject.SetActive(false);
        }
    }

    public void getCsText(Vector3 pos, string text)
    {
        pos.y += 3f;
        GameObject fx = null;
        if (textPool.Count <= 0)
            MakeCSText();

        fx = textPool[0];
        textPool.RemoveAt(0);
        textPool.Add(fx);
        fx.transform.position = pos;
        
        fx.SetActive(true);
        fx.GetComponent<CSText>().cstext = text;
        SoundManager.instance.PlaySound(SoundManager.instance.Gold);
        // SoundManager.instance.PlaySound(SoundManager.instance.Help);
    }

}
