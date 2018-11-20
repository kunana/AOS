using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChampionIcon : MonoBehaviour
{
    SpriteRenderer ChampIcon;
    string IconName;
    public SpriteRenderer Background;
    public ChampionBehavior ch;
    public Material red;
    public Material blue;


    private void Start()
    {
        string parent = gameObject.transform.parent.parent.name;
        ch = transform.parent.parent.gameObject.GetComponentInChildren<ChampionBehavior>();
        if (parent.Contains("Ahri"))
        {
            IconName = "Ahri";
        }
        else if (parent.Contains("Ashe"))
        {
            IconName = "Ashe";
        }
        else if (parent.Contains("Alistar"))
        {
            IconName = "Alistar";
        }
        else if (parent.Contains("Mundo"))
        {
            IconName = "Mundo";
        }
        else if (parent.Contains("Garen"))
        {
            IconName = "Garen";
        }
        ChampIcon = this.transform.GetComponent<SpriteRenderer>();
        ChampIcon.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + IconName);
    }

    void ChanageColor()
    {
        if (ch)
        {
            if (ch.Team.ToLower().Equals("red"))
            {
                Background.material = red;
            }
            else if (ch.Team.ToLower().Equals("blue"))
            {
                Background.material = blue;
            }
        }
        else if (!ch)
        {
            ch = transform.parent.parent.gameObject.GetComponentInChildren<ChampionBehavior>();
            if (ch.Team.ToLower().Equals("red"))
            {
                Background.material = red;
            }
            else if (ch.Team.ToLower().Equals("blue"))
            {
                Background.material = blue;
            }
        }
    }

    void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame") && Background)
        {
            Invoke("ChanageColor", 10f);
        }
    }
}
