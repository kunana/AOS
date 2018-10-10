using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 룸 씬에서 플레이어리스트 프리팹을 생성, 팀에따라 나누고, 플레이어가 나가면 삭제.
/// </summary>
public class PlayerLayoutGroup : Photon.PunBehaviour
{
    public GameObject playerListingPrefab;
    public GameObject[] Team1ViewPort;
    public GameObject[] Team2ViewPort;

    public GameObject[] Team1JoinButton;
    public GameObject[] Team2JoinButton;

    public List<PlayerListing> playerListings = new List<PlayerListing>();

    public int Team1_playerCount = 0;
    public int Team2_playerCount = 0;

    private bool[] Team1_Check = new bool[5] { false, false, false, false, false };
    private bool[] Team2_Check = new bool[5] { false, false, false, false, false };

    [HideInInspector]
    public GameObject CurrentPlayerListPrefab;

    private void Start()
    {
        OnJoinedRoom2();
        roomLayoutModify();
    }

    // 룸에 입장하면 최대 인원수에 맞춰서 안쓰는 레이아웃을 꺼버림
    public void roomLayoutModify()
    {
        // 2 -> 1~4
        // 4 -> 2~4
        // 6 -> 3~4
        // 8 -> 4
        // 10 -> 그대로
        for (int i = PhotonNetwork.room.MaxPlayers/2; i<=4; i++)
        {
            foreach(Transform tr in Team1ViewPort[i].transform)
            {
                tr.gameObject.SetActive(false);
            }
            foreach (Transform tr in Team2ViewPort[i].transform)
            {
                tr.gameObject.SetActive(false);
            }
        }
    }

    // 룸에 들어오면 기본세팅 1번 실행
    public void OnJoinedRoom2()
    {
        print("룸 플레이어 목록 불러옴");

        //룸안에 모든 플레이어를 불러옴
        PhotonPlayer[] photonPlayers = PhotonNetwork.playerList;
        for (int i = 0; i < photonPlayers.Length; i++)
        {
            if(photonPlayers[i].GetTeam().Equals(PunTeams.Team.red))
            {
                RedTeamJoin(photonPlayers[i]);
            }
            else if(photonPlayers[i].GetTeam().Equals(PunTeams.Team.blue))
            {
                BlueTeamJoin(photonPlayers[i]);
            }
            else
            {
                PlayerJoinedRoom(photonPlayers[i]);
            }
        }
        JoinButtonUpdate();
    }

    // 플레이어가 룸에 들어오면
    public void PlayerJoinedRoom(PhotonPlayer photonPlayer)
    {
        if (photonPlayer.Equals(null))
        {
            return;
        }
        
        //팀카운트에 따라 플레이어가 속한 뷰포트 및 팀 할당
        if (Team1_playerCount == Team2_playerCount)
        {
            RedTeamJoin(photonPlayer);
        }
        else if (Team1_playerCount > Team2_playerCount)
        {
            BlueTeamJoin(photonPlayer);
        }
        else if (Team1_playerCount < Team2_playerCount)
        {
            RedTeamJoin(photonPlayer);
        }

        // 참가버튼 재설정
        JoinButtonUpdate();
    }

    // 레드팀(1팀) 조인
    public void RedTeamJoin(PhotonPlayer photonPlayer)
    {
        //플레이어가 이미 들어왔는데, 프리팹이 다시 생성되는걸 방지
        PlayerLeftRoom(photonPlayer);

        //플레이어 프리팹 생성, 리스트에 추가
        CurrentPlayerListPrefab = Instantiate(playerListingPrefab);
        PlayerListing playerListing = CurrentPlayerListPrefab.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);
        playerListing.PhotonPlayer = photonPlayer;
        playerListings.Add(playerListing);

        // 플레이어 프리팹이 들어갈 자리의 자식들을 꺼버림 (선빼고)
        foreach (Transform tr in Team1ViewPort[Team1_playerCount].transform)
        {
            if (tr.name != "Line")
                tr.gameObject.SetActive(false);
        }

        // 프리펩 위치 설정
        playerListing.transform.SetParent(Team1ViewPort[Team1_playerCount].transform, false);
        playerListing.transform.localPosition = Vector3.zero;

        // bool 변수 체크. 스크립트에 팀과 번호 기록
        Team1_Check[Team1_playerCount] = true;
        playerListing.viewnum = Team1_playerCount;
        playerListing.Team = 'r';

        // 팀인원수 증가 및 팀 설정
        Team1_playerCount++;
        photonPlayer.SetTeam(PunTeams.Team.red);
    }

    // 블루팀(2팀) 조인
    public void BlueTeamJoin(PhotonPlayer photonPlayer)
    {
        //플레이어가 이미 들어왔는데, 프리팹이 다시 생성되는걸 방지
        PlayerLeftRoom(photonPlayer);

        //플레이어 프리팹 생성, 리스트에 추가
        CurrentPlayerListPrefab = Instantiate(playerListingPrefab);
        PlayerListing playerListing = CurrentPlayerListPrefab.GetComponent<PlayerListing>();
        playerListing.ApplyPhotonPlayer(photonPlayer);
        playerListing.PhotonPlayer = photonPlayer;
        playerListings.Add(playerListing);

        foreach (Transform tr in Team2ViewPort[Team2_playerCount].transform)
        {
            if (tr.name != "Line")
                tr.gameObject.SetActive(false);
        }

        playerListing.transform.SetParent(Team2ViewPort[Team2_playerCount].transform, false);
        playerListing.transform.localPosition = Vector3.zero;

        Team2_Check[Team2_playerCount] = true;
        playerListing.viewnum = Team2_playerCount;
        playerListing.Team = 'b';

        Team2_playerCount++;
        photonPlayer.SetTeam(PunTeams.Team.blue);
    }

    // 플레이어가 룸에서 나가면
    public void PlayerLeftRoom(PhotonPlayer photonPlayer)
    {
        // 나간 플레이어의 인덱스를 검색
        int index = playerListings.FindIndex(x => x.PhotonPlayer == photonPlayer);

        //똑같은 플레이어의 인덱스를 찾으면
        if (index != -1)
        {   //프리팹 삭제
            PlayerListing leftPlayer = playerListings[index];
            Destroy(playerListings[index].gameObject);
            playerListings.RemoveAt(index);

            // 팀카운트 조정
            if (photonPlayer.GetTeam().Equals(PunTeams.Team.red))
            {
                // 팀 인원수 감소
                Team1_playerCount--;

                // 삭제할 플레이어의 위치 받아옴.
                int viewnum = leftPlayer.viewnum;
                // 그 위치 다음에 유저가 있으면 1칸씩 당김
                for(int i= viewnum + 1; i < PhotonNetwork.room.MaxPlayers / 2; i++)
                {
                    foreach (PlayerListing playerPrefab in playerListings)
                    {
                        if(playerPrefab.Team == 'r' && playerPrefab.viewnum == i)
                        {
                            playerPrefab.transform.SetParent(Team1ViewPort[i - 1].transform);
                            playerPrefab.transform.localPosition = Vector3.zero;
                            playerPrefab.viewnum = i-1;
                            break;
                        }
                    }
                }

                // 마지막자리는 false로 만들고 꺼버림
                Team1_Check[Team1_playerCount] = false;
                foreach (Transform tr in Team1ViewPort[Team1_playerCount].transform)
                    tr.gameObject.SetActive(true);
            }

            if (photonPlayer.GetTeam().Equals(PunTeams.Team.blue))
            {
                Team2_playerCount--;

                int viewnum = leftPlayer.viewnum;
                for (int i = viewnum + 1; i < PhotonNetwork.room.MaxPlayers / 2; i++)
                {
                    foreach (PlayerListing playerPrefab in playerListings)
                    {
                        if (playerPrefab.Team == 'b' && playerPrefab.viewnum == i)
                        {
                            playerPrefab.transform.SetParent(Team2ViewPort[i - 1].transform);
                            playerPrefab.transform.localPosition = Vector3.zero;
                            playerPrefab.viewnum = i-1;
                            break;
                        }
                    }
                }

                Team2_Check[Team2_playerCount] = false;
                foreach (Transform tr in Team2ViewPort[Team2_playerCount].transform)
                    tr.gameObject.SetActive(true);
            }
        }
        JoinButtonUpdate();
    }

    public void JoinButtonUpdate()
    {
        // 참가버튼 다끄고
        for(int i=0; i<PhotonNetwork.room.MaxPlayers/2; i++)
        {
            Team1JoinButton[i].SetActive(false);
            Team2JoinButton[i].SetActive(false);
        }
        // 처음 사람이 없는구간의 참가버튼만 켬
        if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.blue))
        {
            for (int i = 0; i < PhotonNetwork.room.MaxPlayers / 2; i++)
            {
                if (Team1_Check[i] == false)
                {
                    Team1JoinButton[i].SetActive(true);
                    break;
                }
            }
        }
        if (PhotonNetwork.player.GetTeam().Equals(PunTeams.Team.red))
        {
            for (int i = 0; i < PhotonNetwork.room.MaxPlayers / 2; i++)
            {
                if (Team2_Check[i] == false)
                {
                    Team2JoinButton[i].SetActive(true);
                    break;
                }
            }
        }
    }

    public void TeamChange()
    {
        this.photonView.RPC("PlayerTeamSych", PhotonTargets.All, PhotonNetwork.player);
    }

    [PunRPC]
    public void PlayerTeamSych(PhotonPlayer player)
    {
        foreach (var playerPrefab in playerListings)
        {
            if (playerPrefab.PhotonPlayer.Equals(player))
            {
                if (player.GetTeam().Equals(PunTeams.Team.red) && Team2_playerCount < PhotonNetwork.room.MaxPlayers / 2)
                {
                    PlayerLeftRoom(player);
                    //블루팀(2팀) 입장
                    BlueTeamJoin(player);
                }
                else if (player.GetTeam().Equals(PunTeams.Team.blue) && Team1_playerCount < PhotonNetwork.room.MaxPlayers / 2)
                {
                    PlayerLeftRoom(player);
                    //레드팀(1팀) 입장
                    RedTeamJoin(player);
                }
                break;
            }
        }
        JoinButtonUpdate();
    }

    //[PunRPC]
    //public void PlayerTeamSych(PhotonPlayer player)
    //{
    //    foreach (var playerPrefab in playerListings)
    //    {
    //        if (playerPrefab.PhotonPlayer.Equals(player))
    //        {
    //            // 레드팀(1팀)이고 반대편이 가득차지 않았으면 팀을 변경
    //            if (player.GetTeam().Equals(PunTeams.Team.red) && Team2_playerCount < PhotonNetwork.room.MaxPlayers / 2)
    //            {
    //                // 블루팀으로 변경하고 블루팀 마지막자리로 위치이동
    //                player.SetTeam(PunTeams.Team.blue);
    //                playerPrefab.transform.SetParent(Team2ViewPort[Team2_playerCount].transform);
    //                playerPrefab.transform.localPosition = Vector3.zero;

    //                // 삭제와 마찬가지로 1칸씩 당겨줌
    //                int viewnum = playerPrefab.viewnum;
    //                if (viewnum < 4 && Team1_Check[viewnum + 1] == true)
    //                {
    //                    for (int i = viewnum + 1; i < PhotonNetwork.room.MaxPlayers / 2; i++)
    //                    {
    //                        foreach (PlayerListing pl in playerListings)
    //                        {
    //                            if (pl.Team == 'r' && pl.viewnum == i)
    //                            {
    //                                pl.transform.SetParent(Team1ViewPort[i - 1].transform);
    //                                pl.transform.localPosition = Vector3.zero;
    //                                pl.viewnum--;
    //                            }
    //                        }
    //                    }
    //                }

    //                // 1팀인원수 감소. 마지막자리 세팅
    //                Team1_playerCount--;
    //                Team1_Check[Team1_playerCount] = false;
    //                foreach (Transform tr in Team1ViewPort[Team1_playerCount].transform)
    //                    tr.gameObject.SetActive(true);

    //                // 2팀 인원수 증가. 
    //                Team2_Check[Team2_playerCount] = true;
    //                foreach (Transform tr in Team2ViewPort[Team2_playerCount].transform)
    //                {
    //                    if (tr.name.Contains("PlayerListing"))
    //                        continue;

    //                    if (tr.name != "Line")
    //                        tr.gameObject.SetActive(false);
    //                }
    //                Team2_playerCount++;

    //                playerPrefab.Team = 'b';
    //                playerPrefab.viewnum = Team2_playerCount - 1;
    //            }
    //            else if (player.GetTeam().Equals(PunTeams.Team.blue) && Team1_playerCount < PhotonNetwork.room.MaxPlayers / 2)
    //            {
    //                player.SetTeam(PunTeams.Team.red);
    //                playerPrefab.transform.SetParent(Team1ViewPort[Team1_playerCount].transform);
    //                playerPrefab.transform.localPosition = Vector3.zero;

    //                int viewnum = playerPrefab.viewnum;
    //                if (viewnum < 4 && Team2_Check[viewnum + 1] == true)
    //                {
    //                    for (int i = viewnum + 1; i < PhotonNetwork.room.MaxPlayers / 2; i++)
    //                    {
    //                        foreach (PlayerListing pl in playerListings)
    //                        {
    //                            if (pl.Team == 'b' && pl.viewnum == i)
    //                            {
    //                                pl.transform.SetParent(Team2ViewPort[i - 1].transform);
    //                                pl.transform.localPosition = Vector3.zero;
    //                                pl.viewnum--;
    //                            }
    //                        }
    //                    }
    //                }
    //                Team2_playerCount--;
    //                Team2_Check[Team2_playerCount] = false;
    //                foreach (Transform tr in Team2ViewPort[Team2_playerCount].transform)
    //                    tr.gameObject.SetActive(true);

    //                Team1_Check[Team1_playerCount] = true;
    //                foreach (Transform tr in Team1ViewPort[Team1_playerCount].transform)
    //                {
    //                    if (tr.name.Contains("PlayerListing"))
    //                        continue;

    //                    if (tr.name != "Line")
    //                        tr.gameObject.SetActive(false);
    //                }
    //                Team1_playerCount++;

    //                playerPrefab.Team = 'r';
    //                playerPrefab.viewnum = Team1_playerCount - 1;
    //            }
    //            break;
    //        }
    //    }
    //    JoinButtonUpdate();
    //}
}
