using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScriptDisabler : Photon.PunBehaviour
{
    Pathfinding.Seeker seeker;
    Pathfinding.AIPath aipath;
    Pathfinding.FunnelModifier funnelmodifier;
    Pathfinding.AIDestinationSetter aidestinatonsetter;
    Pathfinding.RVO.RVOController rvocontroller;
    FogOfWarEntity WarFog;
    Rigidbody rigidbody;

    PlayerMouse playermouse;
    Skills playerskill;

    public GameObject PlayerObj;
    GameObject forminimap;
    GameObject astar_target;
    GameObject splatmanager;

    private void OnLevelWasLoaded(int level)
    {
        rigidbody.isKinematic = false;
        if (SceneManager.GetActiveScene().name.Equals("InGame"))
        {   
            switch (transform.GetChild(0).GetComponent<ChampionData>().ChampionName)
            {
                case "Alistar":
                    playerskill = GetComponentInChildren<AlistarSkill>();
                    break;
                default:
                    break;
            }
            if (!PhotonNetwork.player.IsLocal)
                AllDisable();

            if (photonView.isMine)
            {
                seeker.enabled = true;
                aipath.enabled = true;
                funnelmodifier.enabled = true;
                aidestinatonsetter.enabled = true;
                rvocontroller.enabled = true;
                
                if (PhotonNetwork.player.IsLocal)
                {
                    if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
                    {
                        WarFog.faction = FogOfWar.Players.Player00;
                    }
                    else
                    {
                        WarFog.faction = FogOfWar.Players.Player01;
                    }
                }
                return;
            }
        }
    }
    private void Awake()
    {
        PlayerObj.SetActive(true);
        rvocontroller = GetComponentInChildren<Pathfinding.RVO.RVOController>();
        seeker = GetComponentInChildren<Pathfinding.Seeker>();
        aipath = GetComponentInChildren<Pathfinding.AIPath>();
        funnelmodifier = GetComponentInChildren<Pathfinding.FunnelModifier>();
        aidestinatonsetter = GetComponentInChildren<Pathfinding.AIDestinationSetter>();
        playermouse = GetComponentInChildren<PlayerMouse>();
        rigidbody = GetComponentInChildren<Rigidbody>();
        WarFog = PlayerObj.GetComponent<FogOfWarEntity>();

        forminimap = transform.GetChild(2).gameObject;
        astar_target = transform.GetChild(1).gameObject;
        splatmanager = transform.GetChild(0).GetChild(0).gameObject;
    }

    public void AllDisable()
    {
        playermouse.enabled = false;
        astar_target.SetActive(false);
        splatmanager.SetActive(false);
    }

}
