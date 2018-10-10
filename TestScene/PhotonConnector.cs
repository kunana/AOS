using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonConnector : Photon.PunBehaviour
{
    [HideInInspector]
    public bool isConnecting;
    [HideInInspector]
    public string _gameVersion = "1";

    public Text messageText;
    private string nickname;
    private string team;


    void Awake()
    {
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.automaticallySyncScene = true;
    }

    public void Connect(string nickname, string team)
    {
        this.nickname = nickname;
        this.team = team;

        //마스터 서버와 연결되면 로비로 들어감
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.playerName = this.nickname;
            if (this.team.Equals("red"))
            {
                PhotonNetwork.player.SetTeam(PunTeams.Team.red);
            }
            else if (this.team.Equals("blue"))
            {
                PhotonNetwork.player.SetTeam(PunTeams.Team.blue);
            }

            Debug.Log("포톤서버 연결되있음");
            messageText.text = "포톤서버 연결되있음\n닉네임 : " + PhotonNetwork.playerName + "\n챔피언 : " + PlayerData.Instance.championName + "\n팀 : " + PhotonNetwork.player.GetTeam().ToString();
            PhotonNetwork.JoinLobby();
        }
        else
        {
            print("포톤서버에 연결시도 중");
            messageText.text = "포톤서버에 연결시도 중";
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }
    


    public override void OnJoinedLobby()
    {
        PhotonNetwork.playerName = this.nickname;
        if (this.team.Equals("red"))
            PhotonNetwork.player.SetTeam(PunTeams.Team.red);
        else if (this.team.Equals("blue"))
            PhotonNetwork.player.SetTeam(PunTeams.Team.blue);

        print("로비 입장 완료");
        messageText.text = "로비 입장 완료";
        RoomOptions op = new RoomOptions
        {
            MaxPlayers = 10,
        };
        PhotonNetwork.JoinOrCreateRoom("MYrooom", op, null);
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        print("랜덤 입장 실패");
        messageText.text = "랜덤 입장 실패";
        RoomOptions op = new RoomOptions
        {
            MaxPlayers = 10,
        };
        PhotonNetwork.JoinOrCreateRoom("MYrooom", op, null);
    }

    public override void OnJoinedRoom()
    {
        print("룸 입장 완료");
        messageText.text = "" +
            "룸 입장 완료\n닉네임 : " + PhotonNetwork.room + PhotonNetwork.playerName + "\n챔피언 : " + PlayerData.Instance.championName + "\n팀 : " + PhotonNetwork.player.GetTeam().ToString();
        isConnecting = true;

        if (PhotonNetwork.player.IsLocal)
        {
            PlayerCreator.instance.MakeChampion();
        }
    }

    

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (newPlayer.GetTeam().ToString().Equals("red"))
            {
                PlayerCreator.instance.Redplayer.Add(newPlayer);
            }
            else
            {
                PlayerCreator.instance.Blueplayer.Add(newPlayer);
            }
        }
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        print("룸 입장 실패");
        messageText.text = "룸 입장 실패";
        print(codeAndMsg[0]);
    }
}
