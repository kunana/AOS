using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class RoomCallBack : Photon.PunBehaviour
{
    private GameObject PlayerLayout;
    private PlayerLayoutGroup plLayoutGroup;

    public ChatFunction ChatManager;

    private void Start()
    {
        PlayerLayout = GameObject.FindGameObjectWithTag("RedTeamLayout");
        plLayoutGroup = PlayerLayout.GetComponent<PlayerLayoutGroup>();
        if (PlayerLayout.Equals(null) || plLayoutGroup.Equals(null))
        {
            Debug.Log("(<Color=Red>Missing</Color> PlayerLayoutGroup GameObject or Component");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("현재 지역:" + PhotonNetwork.networkingPeer.CloudRegion);

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.LogError("OnDisconnectedFromPhoton() called by CallbackManager.cs");
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log(newPlayer + "들어옴");
        plLayoutGroup.PlayerJoinedRoom(newPlayer);

        if (PhotonNetwork.isMasterClient)
            ChatManager.SendSystemMessage(PhotonTargets.All, newPlayer.NickName + "님이 룸에 참가했습니다.");

        if (SoundManager.instance != null)
            SoundManager.instance.Enter_User_Sound();
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log(otherPlayer + "나감");
        plLayoutGroup.PlayerLeftRoom(otherPlayer);

        if (SoundManager.instance != null)
            SoundManager.instance.ExitRoom_Sound();

        // 사람이 나가면 방장체크하여 룸설정 바꿔줌
        if (PhotonNetwork.isMasterClient)
        {
            // 채팅창에 시스템메세지 출력
            ChatManager.SendSystemMessage(PhotonTargets.All, otherPlayer.NickName + "님이 룸에서 나갔습니다.");

            // 룸 설정에서 마스터 변경
            ExitGames.Client.Photon.Hashtable cp = PhotonNetwork.room.CustomProperties;
            cp["MasterName"] = PhotonNetwork.playerName;
            PhotonNetwork.room.SetCustomProperties(cp);

            // 해당 플레이어 프리팹 찾아서 방장으로 바꿔주고 강퇴클릭 리스너 없앰
            PlayerLayoutGroup plg = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<PlayerLayoutGroup>();
            foreach(PlayerListing pl in plg.playerListings)
            {
                if(pl.PlayerName.text == PhotonNetwork.playerName)
                {
                    pl.transform.Find("KickButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/crown");
                    pl.transform.Find("KickButton").GetComponent<Button>().onClick.RemoveAllListeners();
                    break;
                }
            }

            // 스타트 버튼 활성화
            GameObject.FindGameObjectWithTag("CurrentRoom").GetComponent<CurrentRoomCanvas>().StartButtonActive();
        }

        if (PhotonNetwork.room.PlayerCount == 1)
        {
            if(PhotonNetwork.isMasterClient)
            {
                PhotonPlayer Leftplayer = PhotonNetwork.playerList[0];
                PhotonNetwork.SetMasterClient(Leftplayer);
                Debug.Log(Leftplayer + "이(가) 방장이 되었습니다.");
            }
        }
    }

    public override void OnLeftRoom()
    {
        if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.red))
        {
            plLayoutGroup.Team1_playerCount--;
        }
        else if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.blue))
        {
            plLayoutGroup.Team2_playerCount--;
        }
        PhotonNetwork.player.SetTeam(PunTeams.Team.none);
    }

    public override void OnJoinedLobby()
    {
        if(SceneManager.GetActiveScene().name != "Lobby")
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
