using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

//Going 핑이 생성될때 챔피언의 위치따라 Z축 회전이 바뀜
public class TextLookat : MonoBehaviour
{
    public Transform TextObj;
    public TMPro.TextMeshPro ChampName;

    private Vector3 SenderPlayerPos; // 온라인용
    private Vector3 LocalPos;

    public bool isLocal = true;
    public string MyChampName = string.Empty;
    private byte MyEventGroup = 0;

    private void OnEnable()
    {
        TextObj = transform.GetChild(1).transform;
        ChampName = TextObj.GetComponent<TextMeshPro>();
        LocalPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        PhotonNetwork.OnEventCall += FindSender;

        if (MyEventGroup.Equals(0)) // 이벤트 코드 초기화
        {
            if (PhotonNetwork.player.IsLocal)
            {
                if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
                {
                    MyEventGroup = 10;
                }
                else
                {
                    MyEventGroup = 20;
                }
            }
        }
    }

    private void FindSender(byte eventCode, object content, int senderId)
    {
        if (eventCode.Equals(MyEventGroup)) //PingPooling.cs 
        {   //핑 프리팹 이름, 월드 좌표, 샌더챔피언 이름, 샌더의 포지션
            object[] datas = content as object[];
            PhotonPlayer sender = PhotonPlayer.Find(senderId);
            if (datas.Length.Equals(4))
            {
                string temp = (string)datas[2];
                MyChampName = temp;
                SenderPlayerPos = (Vector3)datas[3];
                isLocal = false;
            }
        }
    }

    private void Update()
    {
        ChampName.text = MyChampName;

        if (!isLocal)
            transform.DOLookAt(SenderPlayerPos, 0.5f, AxisConstraint.Y); // Y축만 회전되게 고정
        else
            transform.DOLookAt(LocalPos, 0.5f, AxisConstraint.Y);
    }
}