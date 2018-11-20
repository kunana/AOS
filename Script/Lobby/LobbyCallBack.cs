using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class LobbyCallBack : Photon.PunBehaviour
{
    public GameObject ErrorWindow;
    
    public override void OnConnectedToMaster()
    {
        Debug.Log("현재 지역:" + PhotonNetwork.networkingPeer.CloudRegion);

        if (PhotonNetwork.connected)
        {
            Debug.Log("Matser Sever Successfully Connected by Launcher.cs");
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("랜덤 입장 실패");
        ErrorWindow.SetActive(true);
        ErrorWindow.transform.GetChild(0).GetComponent<Text>().text = "현재 입장 가능한 방이 없습니다.";

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        //PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = this.maxPlayersPerRoom}, null);
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        print("룸 생성 실패" + codeAndMsg[1]);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("룸 생성 완료");
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogError("포톤 연결 끊김");
    }

    //Lobby 씬에서만 쓰는중. , Room 씬에서는 새로 override 함. PlayerLayoutGroup.cs 참조
    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 입장 완료");
        // 방으로 입장
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnJoinedLobby()
    {
        print("로비 입장 완료");
        if(SceneManager.GetActiveScene().name != "Lobby")
            SceneManager.LoadScene("Lobby");
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        print("룸 입장 실패" + codeAndMsg[1]);
        ErrorWindow.SetActive(true);

        if ((string)codeAndMsg[1] == "Game full")
            ErrorWindow.transform.GetChild(0).GetComponent<Text>().text = "방 인원이 가득찼습니다.";
        else if ((string)codeAndMsg[1] == "Game closed")
            ErrorWindow.transform.GetChild(0).GetComponent<Text>().text = "이미 시작된 게임입니다.";
    }
}
