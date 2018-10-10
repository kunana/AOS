using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 방생성, 랜덤룸 입장,
/// </summary>
public class CreateRoom : Photon.PunBehaviour, IPunCallbacks
{
    private byte maxPlayersPerRoom = 10;
    private string roomname = "";
    public Text RoomnamePlaceholder;

    private void Start()
    {
        RoomnamePlaceholder.text = PhotonNetwork.playerName + "님의 게임";
        maxPlayersPerRoom = (byte)((transform.Find("TeamCountDropdown").GetComponent<Dropdown>().value + 1) * 2);
    }

    public void maxPlayerChange(int value)
    {
        maxPlayersPerRoom = (byte)((value+1) * 2);
    }

    public void roomnameChange(string value)
    {
        roomname = value;
    }

    public void RoomCreateButton()
    {
        RoomOptions roomoption = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = maxPlayersPerRoom,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "MasterName", PhotonNetwork.playerName } },
            CustomRoomPropertiesForLobby = new string[] { "MasterName" }
        };

        if (roomname == "")
        {
            // 방이름이 없을경우 ~~님의 게임으로 자동설정.
            roomname = PhotonNetwork.playerName + "님의 게임";
            if (PhotonNetwork.CreateRoom(roomname, roomoption, TypedLobby.Default))
            {
                Debug.Log("룸 생성 성공. RoomName : " + roomname + " MaxPlayers : " + maxPlayersPerRoom.ToString());
            }
            else
            {
                Debug.Log("룸 생성실패");
            }
        }
        else
        {
            if (PhotonNetwork.CreateRoom(roomname, roomoption, TypedLobby.Default))
            {
                Debug.Log("룸 생성 성공. RoomName : " + roomname + " MaxPlayers : " + maxPlayersPerRoom.ToString());
            }
            else
            {
                Debug.Log("룸 생성실패");
            }
        }
    }

    public void CancelButton()
    {
        gameObject.SetActive(false);
    }
}
