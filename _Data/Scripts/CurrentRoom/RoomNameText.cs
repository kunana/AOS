using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//룸 씬안에 텍스트 설정
public class RoomNameText : Photon.PunBehaviour
{

    public GameObject MainTextObj;
    public GameObject SubTextObj;
    private Text MainText;
    [HideInInspector]
    public Text SubText;
    private float Timer = 88.0f;

    //메인텍스트 할당, 없으면 생성
    private void Awake()
    {
        if (MainTextObj.Equals(null) || SubTextObj.Equals(null))
        {
            MainTextObj = GameObject.FindGameObjectWithTag("Title Text");
            SubTextObj = GameObject.FindGameObjectWithTag("Sub Text");
        }

        MainText = MainTextObj.GetComponent<Text>();
        SubText = SubTextObj.GetComponent<Text>();
        if (MainTextObj.Equals(null))
        {
            Debug.Log("No GameObject Name as TitleTextObj, We Made New One");
            MainTextObj = new GameObject();
            MainTextObj.AddComponent<Text>();
            MainText = MainTextObj.GetComponent<Text>();

        }
        else if (SubTextObj.Equals(null))
        {
            Debug.Log("No GameObject Name as SubTextObj, We Made New One");
            SubTextObj = new GameObject();
            SubTextObj.AddComponent<Text>();
            SubText = SubTextObj.GetComponent<Text>();
        }
    }
    //씬에 따라서 텍스트 내용 변환
    void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("Room"))
        {
            MainText.text = PhotonNetwork.room.Name;
            SubText.text = PhotonNetwork.room.IsVisible ? "공개 게임" : "비공개 게임";
        }
        else if (SceneManager.GetActiveScene().name.Equals("Selection"))
        {

            MainText.text = "챔피언을 선택해주세요!";
            SubText.text = Timer.ToString();
        }

    }
    //챔피언 선택 씬일때 텍스트 변환
    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("Selection"))
        {
            Timer -= Time.deltaTime;
            SubText.text = Mathf.Round(Timer).ToString();
            if (Timer <= 0)
            {
                Timer = 5;
                MainText.text = "게임이 곧 시작 됩니다!";
                SubText.SendMessageUpwards("OnClick_StartGame", null, options: SendMessageOptions.RequireReceiver);
                if(Timer <= 0)
                {
                    Timer = 0;
                    //게임 스타트
                }
            }
        }
    }
}

