using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChampionIcon : MonoBehaviour
{
    SpriteRenderer ChampIcon;
    string IconName;

    private void Start()
    {
        string parent = gameObject.transform.parent.parent.name;
        if(parent.Contains("Ahri"))
        {
            IconName = "Ahri";
        }
        else if(parent.Contains("Ashe"))
        {
            IconName = "Ashe";
        }
        else if(parent.Contains("Alistar"))
        {
            IconName = "Alistar";
        }
        else if(parent.Contains("Mundo"))
        {
            IconName = "Mundo";
        }
        else if(parent.Contains("Garen"))
        {
            IconName = "Garen";
        }
        ChampIcon = this.transform.GetComponent<SpriteRenderer>();
        ChampIcon.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + IconName);
    }
}
