using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinionIcon : MonoBehaviour
{

    public SpriteRenderer Icon;
    public Sprite MinionRed;
    public Sprite MinionBlue;
    public Sprite TowerRed;
    public Sprite TowerBlue;
    private MinionBehavior MBehavior;
    private TowerBehaviour TBehavior;
    private SuppressorBehaviour SBehavior;
    private string myteam;

    private void OnEnable()
    {
        myteam = PhotonNetwork.player.GetTeam().ToString().ToLower();
        Icon = GetComponent<SpriteRenderer>();
        MinionRed = Resources.Load<Sprite>("Minimap/MIcon_Red_Minion");
        MinionBlue = Resources.Load<Sprite>("Minimap/MIcon_Blue_Minion") as Sprite;
        TowerRed = Resources.Load<Sprite>("Minimap/MIcon_Red_Tower") as Sprite;
        TowerBlue = Resources.Load<Sprite>("Minimap/MIcon_Blue_Tower") as Sprite;
       
        if (transform.parent.name.Contains("Minion"))
        {
            MBehavior = transform.parent.GetComponent<MinionBehavior>();
            if (myteam.Equals(MBehavior.team.ToString().ToLower()))
                Icon.sprite = MinionBlue;
            else
                Icon.sprite = MinionRed;
            transform.localPosition = new Vector3(0, 200, 0);
            transform.localRotation = Quaternion.Euler(90, 0, 0);
            transform.localScale = new Vector3(3, 3, 3);
        }
        else if (transform.parent.name.Contains("Tower"))
        {
            TBehavior = transform.parent.GetComponent<TowerBehaviour>();
            if (myteam.Equals(TBehavior.Team.ToString().ToLower()))
                Icon.sprite = TowerBlue;
            else
                Icon.sprite = TowerRed;
        }
        else if (transform.parent.name.Contains("Nexus") || transform.parent.name.Contains("Suppressor"))
        {
            SBehavior = transform.parent.GetComponent<SuppressorBehaviour>();
            if (myteam.Equals(SBehavior.Team.ToString().ToLower()))
                Icon.sprite = TowerBlue;
            else
                Icon.sprite = TowerRed;
        }



    }
}
