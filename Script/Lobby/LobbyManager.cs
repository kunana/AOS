using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    [HideInInspector]
    public bool isConnecting;
    [HideInInspector]
    public string _gameVersion = "1";

    public Text playerName;
    public GameObject RoomCreateWindow;

    [HideInInspector]
    public string selectedRoomName = "";
    [HideInInspector]
    public GameObject selectedRoomObject;

    WaitForSeconds connectCheckInterval = new WaitForSeconds(5.0f);

    // Use this for initialization
    void Awake()
    {
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.automaticallySyncScene = true;
    }

    private void OnEnable()
    {
        Connect();
        NicknameSet();
        StartCoroutine("ConnectCheck");
    }

    IEnumerator ConnectCheck()
    {
        while (true)
        {
            yield return connectCheckInterval;
            if (!PhotonNetwork.connected)
            {
                Connect();
            }
        }
    }

    public void Connect()
    {
        isConnecting = true;

        //마스터 서버와 연결되면 로비로 들어감
        if (PhotonNetwork.connected)
        {
            Debug.Log("포톤서버 연결되있음");
            PhotonNetwork.JoinLobby();
        }
        else
        {
            print("포톤서버에 연결시도");
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public void NicknameSet()
    {
        if (PlayerPrefs.HasKey("Nickname"))
        {
            playerName.text = PlayerPrefs.GetString("Nickname");
            PhotonNetwork.playerName = PlayerPrefs.GetString("Nickname");
        }
    }

    public void CreateRoomButton()
    {
        if (RoomCreateWindow != null)
            RoomCreateWindow.SetActive(true);
        if (SoundManager.instance != null)
            SoundManager.instance.Button_Lobby_Sound();
    }

    public void QuickJoinButton()
    {
        PhotonNetwork.JoinRandomRoom();
        if (SoundManager.instance != null)
            SoundManager.instance.Button_Lobby_Sound();
    }

    public void JoinButton()
    {
        if (selectedRoomName != "")
            PhotonNetwork.JoinRoom(selectedRoomName);
        if (SoundManager.instance != null)
            SoundManager.instance.Button_Click_Sound();
    }

    public void ErrorOKButton()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.Button_Click_Sound();
    }
}