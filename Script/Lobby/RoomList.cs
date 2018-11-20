using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RoomList : MonoBehaviour, IPointerClickHandler
{
    public GameObject SelectedImage;
    public Text RoomNameText;
    public Text RoomMasterText;
    public Text PlayerCountText;
    public Text RoomStatusText;

    public string RoomName { get; private set; }
    public bool Updated { get; set; }

    private bool one_click = false;
    private float double_click_checktime = 0;
    private float check_delay = 0.4f;

    private void Update()
    {
        if (one_click)
        {
            if ((Time.time - double_click_checktime) > check_delay)
                one_click = false;
        }
    }

    public void RoomClick()
    {
        LobbyManager lobbyManager = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<LobbyManager>();

        if (lobbyManager.selectedRoomObject != gameObject)
        {
            // 기존에 선택된게 있으면 끄기
            if (lobbyManager.selectedRoomObject != null)
                lobbyManager.selectedRoomObject.GetComponent<RoomList>().SelectedImage.SetActive(false);

            SelectedImage.SetActive(true);
            lobbyManager.selectedRoomName = RoomNameText.text;
            lobbyManager.selectedRoomObject = gameObject;
        }
        else
        {
            SelectedImage.SetActive(false);
            lobbyManager.selectedRoomName = "";
            lobbyManager.selectedRoomObject = null;
        }

        if (SoundManager.instance.gameObject.activeInHierarchy)
            SoundManager.instance.Button_UI_Sound();
    }

    //룸 텍스트 갱신
    public void SetRoomText(RoomInfo room)
    {
        RoomName = room.Name;
        RoomNameText.text = room.Name;

        ExitGames.Client.Photon.Hashtable cp = room.CustomProperties;
        RoomMasterText.text = (string)cp["MasterName"];
        PlayerCountText.text = room.PlayerCount.ToString() + "/" + room.MaxPlayers.ToString();
        if (room.IsOpen)
            RoomStatusText.text = "대기 중";
        else
            RoomStatusText.text = "게임 중";
    }

    // 더블클릭 체크
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 첫클릭하면 변수 true, 첫클릭한 시간체크
            if (!one_click)
            {
                one_click = true;
                double_click_checktime = Time.time;
                RoomClick();
            }
            // 더블클릭이면
            else
            {
                one_click = false;
                GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<LobbyManager>().JoinButton();
                if (SoundManager.instance.gameObject.activeInHierarchy)
                    SoundManager.instance.Button_UI_Sound();
            }
        }
    }
}