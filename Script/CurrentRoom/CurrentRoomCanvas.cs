using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Room 씬 안에 시작, 나가기버튼 ,강퇴버튼
public class CurrentRoomCanvas : Photon.PunBehaviour
{
    public Text RoomNameText;
    public Text RoomSettingText;

    public GameObject ConfirmBox;
    public Text ConfirmText;
    private PhotonPlayer SelectedPlayer;

    public GameObject startButton;

    private void Start()
    {
        ConfirmBox.SetActive(false);

        RoomNameText.text = PhotonNetwork.room.Name;
        int teamcount = PhotonNetwork.room.MaxPlayers / 2;
        RoomSettingText.text = "방장 - " + (string)PhotonNetwork.room.CustomProperties["MasterName"]
            + "\n" + teamcount.ToString() + "대" + teamcount.ToString() + " 게임";

        StartButtonActive();
    }

    public void StartButtonActive()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            startButton.GetComponent<Button>().interactable = false;
            startButton.GetComponent<Image>().color = Color.gray;
        }
        else
        {
            startButton.GetComponent<Button>().interactable = true;
            startButton.GetComponent<Image>().color = new Color(16f / 255f, 22f / 255f, 30f / 255f, 1);
        }
    }

    public void StartButton()
    {
        if (PhotonNetwork.isMasterClient)
        {
            //게임 시작과 동시에 참가 못하게 비공개, 게임 닫힘 상태로 만듬
            //PhotonNetwork.room.IsVisible = false;
            PhotonNetwork.room.IsOpen = false;
            Debug.Log("캐릭터 선택으로 이동");

            //챔피언 선택 씬 로드
            PhotonNetwork.LoadLevelAsync("Selection");
        }

        if (SoundManager.instance != null)
            SoundManager.instance.Button_Click_Sound();
    }

    public void LeaveRoomButton()
    {
        PhotonNetwork.LeaveRoom();
        Debug.Log("방에서 나감");

        if (SoundManager.instance != null)
            SoundManager.instance.Button_Click_Sound();
    }

    /// <summary>
    /// Room 씬에서 Player 프리팹을 클릭하면 PlayerListing.cs 에서 Photonplayer를 넘겨줌
    /// 강퇴메세지 확인 창이 생성. 예, 아니오로 구분
    /// </summary>
    public void PlayerKick(PhotonPlayer other)
    {
        SelectedPlayer = other;
        if (PhotonNetwork.isMasterClient)
        {
            ConfirmBox.SetActive(true);
            ConfirmText.text = other.NickName + "님을 추방하시겠습니까?";

            if (SoundManager.instance != null)
                SoundManager.instance.Button_Click_Sound();
        }
    }

    public void Kick_OK()
    {
        ConfirmBox.SetActive(false);
        PhotonNetwork.CloseConnection(SelectedPlayer);
        Debug.Log(SelectedPlayer.NickName + " 강퇴함");

        if (SoundManager.instance != null)
            SoundManager.instance.Button_Click_Sound();
    }

    public void Kick_Cancel()
    {
        ConfirmBox.SetActive(false);
        SelectedPlayer = null;

        if (SoundManager.instance != null)
            SoundManager.instance.Button_Click_Sound();
    }
}
