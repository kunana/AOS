using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCreator : Photon.PunBehaviour
{

    private static PlayerCreator _instance;
    public static PlayerCreator instance
    {
        get
        {
            if (_instance == null)
                _instance = (PlayerCreator)FindObjectOfType(typeof(PlayerCreator));
            return _instance;
        }
    }

    public List<PhotonPlayer> Redplayer = new List<PhotonPlayer>();
    public List<PhotonPlayer> Blueplayer = new List<PhotonPlayer>();

    private string TeamColor = string.Empty;
    private byte TeamGroup = 0;
    private Vector3 blueTeamPos = new Vector3(260, 5, 260);
    private Vector3 redTeamPos = new Vector3(10, 5, 10);
    private Vector3 temp = new Vector3(5, 0, 0);

    GameObject player;

    private void Awake()
    {
        int ran = Random.Range(0, 10);
        temp = new Vector3(ran, 0, ran);
    }
    private void OnLevelWasLoaded(int level)
    {
       
        if (SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
        {
            player.transform.GetChild(0).gameObject.SetActive(true);
            StructureSetting.instance.player = player.transform.GetChild(0).gameObject;
            player.transform.GetChild(0).gameObject.SetActive(false);
            if (PhotonNetwork.player.GetTeam().ToString().Equals("red")) //레드 팀일때.
            {
                player.transform.GetChild(1).localPosition = redTeamPos + temp;
            }
            else
            {

                player.transform.GetChild(1).localPosition = blueTeamPos + temp;
            }
        }
    }

    public void MakeChampion()
    {
        
        print(PhotonNetwork.player.GetTeam().ToString());
        if (PhotonNetwork.player.GetTeam().ToString().Equals("red")) //레드 팀일때.
        {
            MakeChampObj(redTeamPos + temp);
           
            
        }
        else if (PhotonNetwork.player.GetTeam().ToString().Equals("blue"))
        {
            
            MakeChampObj(blueTeamPos + temp);
            
        }
    }

    private void MakeChampObj(Vector3 pos)
    {
        if (PlayerData.Instance.championName.Equals("Ahri"))
        {
            player = PhotonNetwork.Instantiate("Champion/Ahri", pos + temp, Quaternion.identity, TeamGroup);
            player.transform.GetChild(0).tag = "Player";
            player.name = "Ahri";

        }
        else if (PlayerData.Instance.championName.Equals("Alistar"))
        {
            player = PhotonNetwork.Instantiate("Champion/Alistar", pos + temp, Quaternion.identity, TeamGroup);
            player.transform.GetChild(0).tag = "Player";
            player.name = "Alistar";
        }
        else if (PlayerData.Instance.championName.Equals("Ashe"))
        {
            player = PhotonNetwork.Instantiate("Champion/Ashe", pos + temp, Quaternion.identity, TeamGroup);
            player.transform.GetChild(0).tag = "Player";
            player.name = "Ashe";
        }
        else if (PlayerData.Instance.championName.Equals("Mundo"))
        {
            player = PhotonNetwork.Instantiate("Champion/Mundo", pos + temp, Quaternion.identity, TeamGroup);
            player.transform.GetChild(0).tag = "Player";
        }
        else if (PlayerData.Instance.championName.Equals("Garen"))
        {
            player = PhotonNetwork.Instantiate("Champion/Garen", pos + temp, Quaternion.identity, TeamGroup);
            player.transform.GetChild(0).tag = "Player";
            player.name = "Garen";
        }


    }   



}
