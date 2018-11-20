using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCreator : Photon.PunBehaviour
{
    GameObject player;

    public void MakeChampion()
    {
        if (PhotonNetwork.player.GetTeam().ToString().Equals("red")) //레드 팀일때.
            MakeChampObj();
        else if (PhotonNetwork.player.GetTeam().ToString().Equals("blue"))
            MakeChampObj();
    }

    private void MakeChampObj()
    {
        if (PlayerData.Instance.championName.Equals("Ahri"))
        {
            player = PhotonNetwork.Instantiate("Champion/Ahri", Vector3.zero, Quaternion.identity, 0);
            player.name = "Ahri";
        }
        else if (PlayerData.Instance.championName.Equals("Alistar"))
        {
            player = PhotonNetwork.Instantiate("Champion/Alistar", Vector3.zero, Quaternion.identity, 0);
            player.name = "Alistar";
        }
        else if (PlayerData.Instance.championName.Equals("Ashe"))
        {
            player = PhotonNetwork.Instantiate("Champion/Ashe", Vector3.zero, Quaternion.identity, 0);
            player.name = "Ashe";
        }
        else if (PlayerData.Instance.championName.Equals("Mundo"))
        {
            player = PhotonNetwork.Instantiate("Champion/Mundo", Vector3.zero, Quaternion.identity, 0);
            player.name = "Mundo";
        }
        else if (PlayerData.Instance.championName.Equals("Garen"))
        {
            player = PhotonNetwork.Instantiate("Champion/Garen", Vector3.zero, Quaternion.identity, 0);
            player.name = "Garen";
        }
        player.transform.GetChild(0).tag = "Player";
    }
}
