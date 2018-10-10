using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectListing : Photon.MonoBehaviour
{
    //프리팹에 포톤플레이어를 할당해주는 변수
    public PhotonPlayer PhotonPlayer { get; set; }

    public Text ChampNameText;
    public Text PlayerNameText;
    public Image Champ_Image;
    public GameObject TimerText;

    public bool isSelect = false;
    [HideInInspector]
    public string Selected_ChampName = string.Empty;
    private SelectionManager selectionManager;

    private SelectionLayoutGroup slg;
    private void Start()
    {
        if (PhotonPlayer.IsLocal)
        {
            GetComponent<Image>().color = new Color(233f/255f, 192f/255f, 49f/255f, 1);
            TimerText.SetActive(true);
        }

        gameObject.name = PhotonPlayer.NickName;
        ChampNameText.text = "선택중...";
        selectionManager = GameObject.FindGameObjectWithTag("SelectionManager").GetComponent<SelectionManager>();
        slg = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<SelectionLayoutGroup>();
    }

    private void Update()
    {
        if(!isSelect)
            TimerText.GetComponent<Text>().text = selectionManager.TimerText.text;
    }

    // 텍스트에 플레이어 닉네임 할당
    public void ApplyPhotonPlayer(PhotonPlayer photonPlayer)
    {
        PlayerNameText.text = photonPlayer.NickName;
    }

    //현재 플레이어 리스트에 챔피언을 할당.
    public void ApplyChampion(string name)
    {
        // 선택된 챔피언이 없다면 입력
        // 이미지는 버튼에서 바꿔줌
        if (Selected_ChampName.Equals(string.Empty))
        {
            Selected_ChampName = name;
        }
        // 선택된 챔피언이 변경되었으면
        else if(!Selected_ChampName.Equals(name))
        {
            // 챔피언리스트에서 이전에 선택된 챔피언을 찾아 RPC를 보내 다른사람이 선택가능하게 함.
            GameObject.FindGameObjectWithTag("ChampList").transform.Find(Selected_ChampName)
                .GetComponent<ChampionButton>().SendRPC("switchRPC");

            // 이후 챔피언 바꿔줌
            Selected_ChampName = name;
        }
    }

    public void SendRPC(string method)
    {
        this.photonView.RPC(method, PhotonTargets.AllViaServer);
    }
}
