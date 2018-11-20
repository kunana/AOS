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
    //public Pathfinding.RVO.RVOController rvocontroller;
    FogOfWarEntity WarFog;
    public Rigidbody rigidbody;
    PlayerMouse playermouse;
    Skills playerskill;
    GameObject forminimap;
    GameObject astar_target;
    GameObject splatmanager;

    private PhotonView photonview;
    public GameObject PlayerObj;

    private void Start()
    {
        photonview = this.transform.GetChild(0).GetComponent<PhotonView>();

        PlayerObj.SetActive(true);
        //rvocontroller = GetComponentInChildren<Pathfinding.RVO.RVOController>();
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

        rigidbody.isKinematic = true;
        WarFog.enabled = false;

        if (photonView.isMine)
            PlayerObj.name = gameObject.name + "_" + PhotonNetwork.player.NickName;
        else
            PlayerObj.name = gameObject.name + photonView.owner.NickName;

        SceneManager.activeSceneChanged += SceneLoaded;

        DontDestroyOnLoad(this);
    }

    private void SceneLoaded_Send()
    {
        if (this == null)
            return;

        if (photonview.isMine)
        {
            byte evcode = 33;
            string Team = PhotonNetwork.player.GetTeam().ToString();
            int viewid = photonview.viewID;

            object datas = new object[] { viewid, Team };
            RaiseEventOptions op = new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient, CachingOption = EventCaching.AddToRoomCache };
            PhotonNetwork.RaiseEvent(evcode, datas, true, op);
            PhotonNetwork.SendOutgoingCommands();
        }
    }

    private void SceneLoaded(Scene current, Scene next)
    {
        if (this == null)
            return;

        string currentName = current.name;

        if (currentName == null)
        {
            // Scene1 has been removed
            currentName = "Replaced";
        }
        //Debug.Log("Scenes: " + currentName + ", " + next.name);

        if (next.name.Equals("InGame"))
            SceneLoaded_Send();
    }

    private void OnLevelWasLoaded(int level)
    {
        PlayerSetting();
        if(SceneManager.GetSceneByBuildIndex(level).name.Contains("Room") || SceneManager.GetSceneByBuildIndex(level).name.Contains("Lobby"))
        {
            GameObject.Destroy(this.gameObject,2f);
        }
    }

    private void PlayerSetting()
    {
        if (SceneManager.GetActiveScene().name.Equals("InGame"))
        {
            PlayerObj.transform.gameObject.SetActive(true);
            StructureSetting.instance.player = PlayerObj.gameObject;
            //PlayerObj.transform.gameObject.SetActive(false);
            if (GetComponent<PhotonView>().owner.GetTeam().ToString().Equals("red"))
                WarFog.faction = FogOfWar.Players.Player00;
            else
                WarFog.faction = FogOfWar.Players.Player01;
            WarFog.enabled = true;

            switch (transform.GetChild(0).GetComponent<ChampionData>().ChampionName)
            {
                case "Alistar":
                    playerskill = GetComponentInChildren<AlistarSkill>();
                    break;
            }
            //rvocontroller.radius = 1;
            if (photonView.isMine)
            {
                seeker.enabled = true;
                aipath.enabled = true;
                funnelmodifier.enabled = true;
                aidestinatonsetter.enabled = true;
                //rvocontroller.enabled = true;

                //if (PhotonNetwork.player.IsLocal) // 지금 안가려짐
                //{
                //    if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
                //        WarFog.faction = FogOfWar.Players.Player00;
                //    else
                //        WarFog.faction = FogOfWar.Players.Player01;
                //}
                return;
            }
            else if (!photonView.isMine)
            {
                playermouse.enabled = false;
                astar_target.SetActive(false);
                splatmanager.SetActive(false);
            }

        }
    }

}