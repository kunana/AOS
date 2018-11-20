using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Selection 씬에서 각 플레이어 프리팹을 팀별로, 뷰포트에 생성해줌
public class SelectionLayoutGroup : Photon.PunBehaviour
{
    public List<SelectListing> selectListings;

    //플레이어 리스트 프리팹
    public GameObject ChampSelectingPrefab;
    public GameObject ChampSelectingPrefab_Enemy;
    public GameObject MyTeamViewPort;
    public GameObject EnemyTeamViewPort;

    //현재 로컬플레이어의 프리팹 과 팀
    public GameObject CurrentSelectingPrefab;
    public PhotonPlayer LocalPlayer;

    private void Start()
    {
        selectListings = new List<SelectListing>();
        selectListings.Clear();

        //로컬플레이어 프리팹 먼저 생성
        LocalPlayer = PhotonNetwork.player;
        if (LocalPlayer.IsLocal)
        {
            // 프리팹 생성후 리스트 등록. 세팅
            CurrentSelectingPrefab = Instantiate(ChampSelectingPrefab, MyTeamViewPort.transform, false);
            SelectListing selectListing = CurrentSelectingPrefab.GetComponent<SelectListing>();
            selectListings.Add(selectListing);

            selectListing.PhotonPlayer = LocalPlayer;
            selectListing.ApplyPhotonPlayer(LocalPlayer);
        }
        //다른플레이어 리스트를 받아와서 플레이어 리스트 만큼 프리팹을 만듬
        PhotonPlayer[] photonPlayers = PhotonNetwork.otherPlayers;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            PlayerJoinedRoom(photonPlayers[i]);
        }
    }

    // 나 제외
    public void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer.Equals(null))
        {
            return;
        }
        GameObject SelectingPrefab;

        //로컬플레이어가 아니면 로컬플레이어가 속한 팀에따라 좌우를 나눔
        if (LocalPlayer.GetTeam() != photonPlayer.GetTeam())
        {
            SelectingPrefab = Instantiate(ChampSelectingPrefab_Enemy);
            SelectingPrefab.transform.SetParent(EnemyTeamViewPort.transform, false);
        }
        else
        {
            SelectingPrefab = Instantiate(ChampSelectingPrefab);
            SelectingPrefab.transform.SetParent(MyTeamViewPort.transform, false);
        }

        SelectListing selectListing = SelectingPrefab.GetComponent<SelectListing>();
        selectListings.Add(selectListing);

        selectListing.PhotonPlayer = photonPlayer;
        selectListing.ApplyPhotonPlayer(photonPlayer);
    }
}
