using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatFunction : Photon.MonoBehaviour
{

    //채팅을 저장할 리스트, 텍스트박스, 인풋박스
    public List<string> ChatList = new List<string>();
    public Text chatBox;
    public InputField chatInput;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void chatValueChanged()
    {
        //엔터키로 전송
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Send();
            chatInput.ActivateInputField();
        }
    }

    private void Send()
    {
        string currentMsg = chatInput.text;
        SendRPC(PhotonTargets.All, currentMsg);
        chatInput.text = string.Empty;
    }

    //RPC를 사용하여 메세지를 주고받음 
    //RPC함수 'SendMSG 함수'를 가지고 있으면 모두 호출함
    public void SendRPC(PhotonTargets _target, string _msg)
    {
        photonView.RPC("SendMsg", _target, _msg);
    }

    public void SendSystemMessage(PhotonTargets _target, string _msg)
    {
        photonView.RPC("SendSystemMsg", _target, _msg);
    }

    [PunRPC]
    private void SendMsg(string _msg, PhotonMessageInfo _info)
    {
        string sendPlayer = _info.sender.ToString().Split("\'".ToCharArray())[1];
        AddChatToChatBox(string.Format("{0}: {1}", sendPlayer, _msg));
    }

    [PunRPC]
    private void SendSystemMsg(string msg)
    {
        string systemMsg = "<color=#2E2E2E>" + msg + "</color>";
        AddChatToChatBox(systemMsg);
    }

    //메세지를 받아서 메세지박스에 출력 및 리스트에 저장
    private void AddChatToChatBox(string _msg)
    {
        string chat = chatBox.text;
        chat += string.Format("\n{0}", _msg);
        chatBox.text = chat;
        ChatList.Add(_msg);
    }


}
