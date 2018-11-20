using UnityEngine;
using UnityEngine.UI;

//room 씬 안 생성되는 플레이어 프리팹 안에 있는 텍스트에 플레이어 이름을 할당
public class PlayerListing : Photon.PunBehaviour {

    //프리팹에 포톤플레이어를 할당해주는 변수
    public PhotonPlayer PhotonPlayer {get;  set;}
    private Button PlayerPrefabButton;
    public Text PlayerName;
    //[HideInInspector]
    public char Team;
    //[HideInInspector]
    public int viewnum;

    private void Start()
    {
        if (!PhotonPlayer.IsMasterClient)
            ButtonAddListener();
        else
            MasterClientImage();
    }

    // 텍스트에 플레이어 닉네임 할당
    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
    {
        PlayerName.text = photonPlayer.NickName;
    }

    //생성된 버튼 클릭하면, CurrentRoomCanvas.cs 에 PlayerKick() 로 PhotonPlayer를 넘겨줌
    public void ButtonAddListener()
    {
        GameObject CurRoomCanvas = GetComponentInParent<CurrentRoomCanvas>().gameObject;
        CurrentRoomCanvas crCanvas = CurRoomCanvas.GetComponent<CurrentRoomCanvas>();

        PlayerPrefabButton = transform.Find("KickButton").GetComponent<Button>();
        PlayerPrefabButton.onClick.AddListener(() => crCanvas.PlayerKick(PhotonPlayer));
    }

    public void MasterClientImage()
    {
        // KickButton의 이미지를 방장 이미지로 바꿔주고 버튼리스너 안붙여줌
        transform.Find("KickButton").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/crown");
    }
}
