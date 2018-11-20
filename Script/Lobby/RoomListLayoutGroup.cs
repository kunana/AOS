using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 방리스트 새로고침, 방리스트 생성 
public class RoomListLayoutGroup : Photon.PunBehaviour,IPunCallbacks
{ 
    public GameObject roomListingPrefab;
    private List<RoomList> roomListingButtons = new List<RoomList>();

    void Start()
    {
        RefreshRoomList();
        StartCoroutine(Roomupdate());
    }

    IEnumerator Roomupdate()
    {
        OnReceivedRoomListUpdate();
        yield return new WaitForSeconds(10.0f);
    }

    public void RefreshRoomList()
    {   
        OnReceivedRoomListUpdate();
    }

    // 로비로 들어왔을때 현재 생성된 룸을 받아줌
    public override void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (RoomInfo room in rooms)
        {
            RoomReceived(room);
        }

        RemoveOldRooms();
    }

    //룸옵션 IsVisible 이 true, 룸에 플레이어가 맥스플레이어보다 작을때, 방리스트 프리팹 생성
    private void RoomReceived(RoomInfo room)
    {
        // 생성된 방 중에 리스트에 있는 방이 있는지 체크
        int index = roomListingButtons.FindIndex(x => x.RoomName == room.Name);

        // 리스트에 방이 없다면 화면에 생성하고 리스트에 등록
        if (index == -1)
        {
            //room.PlayerCount < room.MaxPlayers (열린방보기)
            if (room.IsVisible)
            {
                GameObject roomListingObj = Instantiate(roomListingPrefab);
                roomListingObj.transform.SetParent(transform, false);

                RoomList roomListing = roomListingObj.GetComponent<RoomList>();
                roomListingButtons.Add(roomListing);

                index = (roomListingButtons.Count - 1);
            }
        }

        // 리스트에 방이 있다면 해당 값(포인터)를 받아 방 이름을 갱신, 업데이트를 체크해줌
        if (index != -1)
        {
            RoomList roomListing = roomListingButtons[index];
            roomListing.SetRoomText(room);
            roomListing.Updated = true;
        }
    }

    private void RemoveOldRooms()
    {
        List<RoomList> removeRooms = new List<RoomList>();

        // 업데이트가 안된 방은 삭제해줌
        foreach (RoomList roomListing in roomListingButtons)
        {
            if (!roomListing.Updated)
                removeRooms.Add(roomListing);
            else
                roomListing.Updated = false;
        }

        foreach (RoomList roomListing in removeRooms)
        {
            GameObject roomListingObj = roomListing.gameObject;
            roomListingButtons.Remove(roomListing);
            Destroy(roomListingObj);
        }
    }
}