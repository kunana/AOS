using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class InGameTimer : Photon.PunBehaviour
{
    [SerializeField]
    private int roomStartTimestamp;
    private const string StartTimeKey = "#rt"; // the name of our "room time" custom property.
    float minutes;
    float seconds;

    public Text text;
    public float temp;

    float gold_IncreaseTime = 0;
    private void Start()
    {
        StartCoroutine("SetRoomStartTimestamp");
    }

    /// <summary>A common, synced timer as double (similar to Unity's Time.time) for a room, starting close to 0 and not wrapping around.</summary>
    /// <remarks>When IsoomTimeSet is false, RoomTimestamp and RoomTime will both be zero.</remarks>
    public float RoomTime
    {
        get
        {
            uint u = (uint)this.RoomTimestamp;
            float t = u;
            return t / 1000;
        }
    }

    /// <summary>A common, synced timer for a room, starting close to 0 and not wrapping around.</summary>
    /// <remarks>When IsoomTimeSet is false, RoomTimestamp and RoomTime will both be zero.</remarks>
    public int RoomTimestamp
    {
        get { return PhotonNetwork.inRoom ? PhotonNetwork.ServerTimestamp - this.roomStartTimestamp : 0; }
    }

    /// <summary>True if the client is in a room and if that room's start time is defined (by any player).</summary>
    /// <remarks>When IsoomTimeSet is false, RoomTimestamp and RoomTime will both be zero.</remarks>
    public bool IsRoomTimeSet
    {
        get { return PhotonNetwork.inRoom && PhotonNetwork.room.CustomProperties.ContainsKey(StartTimeKey); }
    }

    private void FixedUpdate()
    {
        minutes = Mathf.Floor(RoomTime / 60);
        seconds = Mathf.Floor(RoomTime % 60);

        if (text)
            text.text = string.Format("{0}:{1}", minutes.ToString("00"), seconds.ToString("00"));

        // 2분 이후부터 가만히 있어도 오르는 골드
        RisingGold(minutes);

        temp = RoomTime;
    }

    public void RisingGold(float minutes)
    {
        // 2분부터 1초당 2골드씩줌
        if (minutes >= 2.0f)
        {
            gold_IncreaseTime += Time.deltaTime;
            // 1초당 2골드하니 2골드씩올라서 0.5초당 1골드로함
            if (gold_IncreaseTime > 0.5f)
            {
                PlayerData.Instance.gold += 1;
                gold_IncreaseTime -= 0.5f;
            }
        }
    }


    internal IEnumerator SetRoomStartTimestamp()
    {
        if (IsRoomTimeSet || !PhotonNetwork.isMasterClient)
        {
            yield break;
        }


        // in some cases, when you enter a room, the server time is not available immediately.
        if (PhotonNetwork.ServerTimestamp == 0)
        {
            yield return null;
        }

        ExitGames.Client.Photon.Hashtable startTimeProp = new ExitGames.Client.Photon.Hashtable(); // only use ExitGames.Client.Photon.Hashtable for Photon
        startTimeProp[StartTimeKey] = PhotonNetwork.ServerTimestamp;

        PhotonNetwork.room.SetCustomProperties(startTimeProp); // implement OnPhotonCustomRoomPropertiesChanged(Hashtable propertiesThatChanged) to get this change everywhere
    }


    /// <remarks>
    /// In theory, the client which created the room might crash/close before it sets the start time.
    /// Just to make extremely sure this never happens, a new masterClient will check if it has to
    /// start a new round.
    /// </remarks>
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        StartCoroutine("SetRoomStartTimestamp");
    }

    public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(StartTimeKey))
            this.roomStartTimestamp = (int)propertiesThatChanged[StartTimeKey];
    }

}