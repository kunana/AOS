using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ChampionButton : Photon.MonoBehaviour
{
    public bool isSelect = false;
    public Image IconImage;
    private Button myButton;

    private GameObject SelectRoom;
    private SelectionLayoutGroup slg;

    private void Start()
    {
        myButton = GetComponent<Button>();
        SelectRoom = GameObject.FindGameObjectWithTag("SelectRoom");
        slg = GameObject.FindGameObjectWithTag("RedTeamLayout").GetComponent<SelectionLayoutGroup>();
    }

    public void Onclick_Button()
    {
        if (SoundManager.instance != null)
            SoundManager.instance.Button_Click_Sound();

        // 챔피언 고르면 누른 유저의 프리팹을 찾아 이름과 아이콘 세팅
        foreach (SelectListing Prefab in slg.selectListings)
        {
            if(Prefab.PhotonPlayer == PhotonNetwork.player)
            {
                // 유저가 선택완료면 챔피언을 변경하지않음.
                if (Prefab.isSelect)
                    return;

                Prefab.ApplyChampion(gameObject.name);
                Prefab.Champ_Image.sprite = IconImage.sprite;
                break;
            }
        }

        //RPC
        this.photonView.RPC("switchRPC", PhotonTargets.AllViaServer);
        this.photonView.RPC("Sync", PhotonTargets.AllViaServer);
    }

    public void SendRPC(string method)
    {
        this.photonView.RPC(method, PhotonTargets.AllViaServer);
    }

    [PunRPC]
    public void switchRPC(PhotonMessageInfo info) // 버튼의 스위치 온/오프
    {
        //RPC 를 보낸 sender와 팀을 비교해서 다른 팀이면 RPC를 받지않음
        if (PhotonNetwork.player.GetTeam().Equals(info.sender.GetTeam()))
        {
            // 내가 선택하면 다른사람들은 RPC 받아서 비활성화
            Switch();
        }
    }

    [PunRPC]
    public void Sync(PhotonMessageInfo info) // 챔피언 이름 할당, 이미지 할당
    {
        // 같은편만 받음
        if (PhotonNetwork.player.GetTeam().Equals(info.sender.GetTeam()))
        {            
            foreach (SelectListing Prefab in slg.selectListings)
            {
                if (Prefab.PhotonPlayer == info.sender)
                {
                    Prefab.Selected_ChampName = gameObject.name;
                    Prefab.Champ_Image.sprite = IconImage.sprite;
                    break;
                }
            }
        }
    }

    public void Switch()
    {
        isSelect = !isSelect;
        if (isSelect)
        {
            myButton.interactable = false;
            IconImage.color = Color.gray;
        }
        else
        {
            myButton.interactable = true;
            IconImage.color = Color.white;
        }
    }
}
