using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SelectionCallBack : Photon.PunBehaviour {

    // 한명이 나가면 룸으로 이동
    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        PhotonNetwork.RemoveRPCs(PhotonNetwork.player);

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevelAsync("Room");
            PhotonNetwork.room.IsOpen = true;
        }
        
    }

    public override void OnLeftRoom()
    {
        //PhotonNetwork.RemoveRPCs(PhotonNetwork.player);
        PhotonNetwork.player.SetTeam(PunTeams.Team.none);
        
        SceneManager.LoadScene("Lobby");
    }
}
