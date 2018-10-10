using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SelectionManager : Photon.PunBehaviour {

    public Text TitleText;
    public Text TimerText;

    public GameObject completeButton;

    private float Timer = 90.0f;
    private bool SelectFinish = false;

    private SelectionLayoutGroup slg;
    // Use this for initialization
    void Start () {
        slg = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<SelectionLayoutGroup>();
        // 타이머연동 RaiseEvent 등록
        PhotonNetwork.OnEventCall += TimerShare;
    }
	
	// Update is called once per frame
	void Update () {

        // 마스터 클라이언트의 타이머를 다른 클라이언트들에 적용
        if (PhotonNetwork.isMasterClient)
        {
            Timer -= Time.deltaTime;
            if (Timer <= 0)
                Timer = 0;

            TimerText.text = Mathf.FloorToInt(Timer).ToString();

            PhotonNetwork.RaiseEvent((byte)1, Timer, true, new RaiseEventOptions()
            {
                CachingOption = EventCaching.DoNotCache,
                Receivers = ReceiverGroup.Others
            });
            //this.photonView.RPC("TimerShare", PhotonTargets.AllViaServer, Timer);
        }

        // 선택완료가 아닐때 타이머가 0이되면 전투준비로 바꾸고 다시 5초의 대기시간 줌
        if (!SelectFinish)
        {
            if (Timer <= 0)
            {
                SelectFinish = true;
                TitleText.text = "전투 준비!";
                Timer = 5.5f;

                // 유저중 한명이라도 선택완료가 아니면 해당유저 강퇴하고 룸으로 이동
                if (PhotonNetwork.isMasterClient)
                {
                    foreach (SelectListing Prefab in slg.selectListings)
                    {
                        if (!Prefab.isSelect)
                        {
                            PhotonNetwork.room.IsOpen = true;
                            PhotonNetwork.LoadLevelAsync("Room");
                        }
                    }
                }
            }
        }
        // 이후 다시 5초가 지나서 0초가되면 게임시작
        else
        {
            if (Timer <= 0)
            {
                //게임시작
                if (PhotonNetwork.isMasterClient)
                {
                    PhotonNetwork.LoadLevel("InGame");
                }
            }
        }
    }

    public void SelectCompleteButton()
    {
        foreach (SelectListing Prefab in slg.selectListings)
        {
            if (Prefab.PhotonPlayer == PhotonNetwork.player)
            {
                if (Prefab.Selected_ChampName.Equals(string.Empty))
                    return;

                TitleText.text = "전투 준비!";
                completeButton.GetComponent<Button>().interactable = false;
                completeButton.GetComponent<Image>().color = Color.gray;
                PlayerData.Instance.championName = Prefab.Selected_ChampName;

                Prefab.isSelect = true;
                Prefab.TimerText.SetActive(false);
                Prefab.ChampNameText.text = Prefab.Selected_ChampName;

                this.photonView.RPC("selectComplete", PhotonTargets.AllViaServer);
                break;
            }
        }
    }

    public void LeaveButton()
    {
        PhotonNetwork.LeaveRoom(true);
    }

    [PunRPC]
    public void selectComplete(PhotonMessageInfo info) // 버튼의 스위치 온/오프
    {
        //같은팀이면 챔피언명 뜸
        foreach(SelectListing Prefab in slg.selectListings)
        {
            if (Prefab.PhotonPlayer == info.sender)
            {
                if (PhotonNetwork.player.GetTeam().Equals(info.sender.GetTeam()))
                {
                    Prefab.ChampNameText.text = Prefab.Selected_ChampName;
                }
                else
                {
                    Prefab.ChampNameText.text = "준비 완료";
                }
                Prefab.isSelect = true;
                break;
            }
        }

        if (PhotonNetwork.isMasterClient)
        {
            // 유저중 한명이라도 선택완료가 아니면 리턴
            foreach (SelectListing Prefab in slg.selectListings)
            {
                if (!Prefab.isSelect)
                    return;
            }
            // 모두 선택완료라면 Timer를 0으로 만들어서 바로 시작대기로 만듬
            Timer = 0;
        }
    }

    //[PunRPC]
    //public void TimerShare(float masterTimer, PhotonMessageInfo info)
    //{
    //    if(!PhotonNetwork.isMasterClient)
    //    {
    //        Timer = masterTimer;
    //        TimerText.text = Mathf.FloorToInt(Timer).ToString();
    //    }
    //}

    // RaiseEvent
    public void TimerShare(byte eventcode, object content, int senderid)
    {
        if(eventcode == 1)
        {
            Timer = (float)content;

            if (TimerText != null)
                TimerText.text = Mathf.FloorToInt(Timer).ToString();
        }
    }
}
