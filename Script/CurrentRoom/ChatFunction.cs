using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class ChatFunction : Photon.MonoBehaviour
{
    //채팅을 저장할 리스트, 텍스트박스, 인풋박스
    public List<string> ChatList = new List<string>();
    public Text chatBox;
    public ScrollRect chatScroll;
    public Scrollbar scrollbar;
    public InputField chatInput;
    public Text SendTypeDisplay;
    public InGameTimer IngameTimer;
    public bool isTeamSend = false;
    public string team = "red";

    string red = "<color=#9D0F29>";
    string blue = "<color=#1BA1CF>";
    string endColor = "</color>";
    string Mychamp;
    string isteam = "";
    float chatBoxAlpha;
    bool isInGame = false;
    SelectionManager selection;

    private void Awake()
    {
        //DontDestroyOnLoad(this.gameObject);        
    }
    private void OnLevelWasLoaded(int level)
    {
        if (SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
        {
            SendTypeDisplay.text = "[전체]";
            IngameTimer = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameTimer>();
            isInGame = true;
            chatBox.fontSize = 22;

        }
        else if (SceneManager.GetSceneByBuildIndex(level).name.Equals("Selection"))
        {
            isInGame = false;
            selection = GameObject.FindGameObjectWithTag("SelectionManager").GetComponent<SelectionManager>();
        }
        else
        {
            isInGame = false;
        }
        Mychamp = PlayerData.Instance.championName;
        chatInput.text = string.Empty;

    }

    public void chatValueChanged()
    {
        //엔터키로 전송
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Send();
            chatInput.ActivateInputField();
            if (SoundManager.instance.gameObject.activeInHierarchy)
                SoundManager.instance.Chat_Sound();
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("InGame"))
        {
            if (chatInput.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    isTeamSend = !isTeamSend;
                    SendTypeChange();
                }
            }
        }
        else if (SceneManager.GetActiveScene().name.Equals("Result"))
        {
            isTeamSend = false;
        }
    }

    private void SendTypeChange()
    {
        if (!isTeamSend)
            SendTypeDisplay.text = "[전체]";
        else
            SendTypeDisplay.text = "[팀에게만]";
    }

    public void HideScroll()
    {
        chatScroll.verticalScrollbar = null;
        scrollbar.gameObject.SetActive(false);
    }

    public void RevealScroll()
    {
        scrollbar.gameObject.SetActive(true);
        chatScroll.verticalScrollbar = scrollbar;
    }

    public void Send()
    {
        if (SceneManager.GetActiveScene().name.Equals("Selection"))
        {
            if (selection.Timer <= 3.0f)
            {
                return;
            }
        }
        string currentMsg = chatInput.text;
        if (string.IsNullOrEmpty(currentMsg))
            return;

        if (!isTeamSend)
            SendRPC(PhotonTargets.All, currentMsg, false, Mychamp);
        else
            SendRPC(PhotonTargets.All, currentMsg, true, Mychamp);
        chatInput.text = string.Empty;
    }

    //RPC를 사용하여 메세지를 주고받음 
    //RPC함수 'SendMSG 함수'를 가지고 있으면 모두 호출함
    public void SendRPC(PhotonTargets _target, string _msg, bool isTeamChat, string championName)
    {
        photonView.RPC("SendMsg", _target, _msg, isTeamChat, championName);
    }

    [PunRPC]
    private void SendMsg(string _msg, bool isteamchat, string championName, PhotonMessageInfo _info)
    {
        string sendPlayer = _info.sender.ToString().Split("\'".ToCharArray())[1];

        if (isInGame) // 인게임에서만
        {
            if(championName.Contains("Ashe") || championName.Contains("ashe"))
            {
                championName = "제애쉬";
            }
            else if (championName.Contains("Mundo") || championName.Contains("mundo"))
            {
                championName = "거문도";
            }
            else if (championName.Contains("Alistar") || championName.Contains("alistar"))
            {
                championName = "하영수";
            }

            if (isteamchat) // 팀챗 활성화면 같은 팀만 받기
            {
                isteam = "(팀에게만)";
                if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString())) // 팀따라 색주기.
                    sendPlayer = blue + isteam + sendPlayer + " (" + championName.ToString() + ")" + endColor;
                else
                    sendPlayer = red + isteam + sendPlayer + " (" + championName.ToString() + ")" + endColor;

                if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString()))
                    AddChatToChatBox(string.Format("[{0}] {1} : {2}", IngameTimer.text.text, sendPlayer, _msg)); // 타이머, <컬러>sender, 챔프 : 내용
                else
                    return;
            }
            else
            {
                isteam = "(전체에게)";
                if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString())) // 팀따라 색주기.
                    sendPlayer = blue + isteam + sendPlayer + " (" + championName.ToString() + ")" + endColor;
                else
                    sendPlayer = red + isteam + sendPlayer + " (" + championName.ToString() + ")" + endColor;
                AddChatToChatBox(string.Format("[{0}] {1} : {2}", IngameTimer.text.text, sendPlayer, _msg)); // 타이머, <컬러>sender, 챔프 : 내용
            }
        }
        else
        {
            if (PhotonNetwork.player.GetTeam().ToString().Equals(_info.sender.GetTeam().ToString())) // 팀따라 색주기.
                sendPlayer = blue + sendPlayer + endColor;
            else
                sendPlayer = red + sendPlayer + endColor;
            AddChatToChatBox(string.Format("{0} : {1}", sendPlayer, _msg));
        }
    }

    public void SendSystemMessage(PhotonTargets _target, string _msg)
    {
        photonView.RPC("SendSystemMsg", _target, _msg);
    }

    [PunRPC]
    private void SendSystemMsg(string msg)
    {
        string systemMsg = "<color=#ffe963>" + msg + "</color>";
        AddChatToChatBox(systemMsg);
    }

    //메세지를 받아서 메세지박스에 출력 및 리스트에 저장
    private void AddChatToChatBox(string _msg)
    {
        if(isInGame)
        if (SoundManager.instance != null)
            SoundManager.instance.Chat_Sound();
        string chat = chatBox.text;
        chat += string.Format("\n{0}", _msg);
        chatBox.text = chat;
        ChatList.Add(_msg);
    }
}
