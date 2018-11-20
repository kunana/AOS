using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurrenderUI : MonoBehaviour
{

    public GameObject Surrender_UI;
    public GameObject TextObj;
    private Text resultText;
    public GameObject YesBtn;
    public GameObject NoBtn;
    public Image Timer;

    public GameObject[] StatusBoxes = new GameObject[5];

    private int SurrenderCount = 0;
    private int YesCount = 0;
    private int TeamMemberCount = 0;
    bool isAgree;
    bool SendOnce = false;
    public float LimitTime = 30f;
    private string Team;

    bool initial = false;
    Color Green = new Color(0.3f, 0.9f, 0.4f);
    Color Red = new Color(0.6f, 0.07f, 0.08f);
    Color Grey = new Color(0.33f, 0.33f, 0.33f);

    RaiseEventOptions op;
    byte evcode_UIopen;
    byte evcode_Select_Surrender;

    private InGameManager inGameManager;

    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            StatusBoxes[i] = Surrender_UI.transform.GetChild(i).gameObject;
        }

        PhotonNetwork.OnEventCall += Received_Surrender;
        op = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
        };
        resultText = TextObj.GetComponent<Text>();
        inGameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();
    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= Received_Surrender;
    }

    private void Received_Surrender(byte eventCode, object content, int senderId)
    {
        if (this == null)
            return;

        PhotonPlayer sender = PhotonPlayer.Find(senderId);
        if (eventCode.Equals(evcode_UIopen))
        {
            if (SurrenderCount != 0)
            {
                YesCount = 0;
                SurrenderCount = 0;
            }
            if (sender.GetTeam().ToString() != PhotonNetwork.player.GetTeam().ToString())
                return;
            Surrender_UI.gameObject.SetActive(true);
            SoundManager.instance.PlaySound(SoundManager.instance.UI_Open);

            if (sender == PhotonNetwork.player)
            {
                YesBtn.SetActive(false);
                NoBtn.SetActive(false);
                TextObj.SetActive(true);
                StatusBoxes[SurrenderCount].GetComponent<Image>().color = Green;
                resultText.text = "동의 하셨습니다";
            }
            else
            {
                StatusBoxes[SurrenderCount].GetComponent<Image>().color = Green;
                YesBtn.SetActive(true);
                NoBtn.SetActive(true);
            }
            YesCount++;
            SurrenderCount++;

        }
        else if (eventCode.Equals(evcode_Select_Surrender))
        {
            object[] received_datas = content as object[];
            bool yesorno = (bool)received_datas[0];
            if (yesorno)//예스 클릭
            {
                StatusBoxes[SurrenderCount].GetComponent<Image>().color = Green;

                if (sender == PhotonNetwork.player)
                {
                    TextObj.SetActive(true);
                    resultText.text = "동의 하셨습니다";
                    YesBtn.SetActive(false);
                    NoBtn.SetActive(false);
                }
                YesCount++;
            }
            else if (!yesorno)
            {
                StatusBoxes[SurrenderCount].GetComponent<Image>().color = Red;
                if (sender == PhotonNetwork.player)
                {
                    TextObj.SetActive(true);
                    resultText.text = "거부 하셨습니다";
                    YesBtn.SetActive(false);
                    NoBtn.SetActive(false);
                }
            }
            SurrenderCount++;
        }
    }

    private void Update()
    {
        if (!initial) //초기 설정
        {
            if (!inGameManager.runOnce)
                return;

            initial = true;
            if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
            {
                evcode_Select_Surrender = 133;
                evcode_UIopen = 136;
                Team = "red";
                TeamMemberCount = inGameManager.redTeamPlayer.Count;
            }
            else if (PhotonNetwork.player.GetTeam().ToString().Equals("blue"))
            {
                evcode_Select_Surrender = 143;
                evcode_UIopen = 146;
                Team = "blue";
                TeamMemberCount = inGameManager.blueTeamPlayer.Count;
            }
        }

        if (Surrender_UI.activeInHierarchy && initial)
        {
            if (!SendOnce)
            {
                Timer.fillAmount -= 1.0f / LimitTime * Time.deltaTime;

                if (YesCount.Equals(TeamMemberCount)) // 만장일치
                {
                    voteEnd();
                    return;
                }
                // 과반수가 넘어가면 
                if ((float)YesCount > (float)TeamMemberCount / 2.0f)
                {
                    voteEnd();
                    return;
                }
                // 과반수가 안넘어가면 타이머가 다될때까지 기다린 후 다되면 리셋
                if (Timer.fillAmount.Equals(0))
                {
                    if ((float)YesCount > (float)TeamMemberCount / 2.0f)
                    {
                        voteEnd();
                        return;
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        StatusBoxes[i].GetComponent<Image>().color = Grey;
                    }
                    YesBtn.SetActive(false);
                    NoBtn.SetActive(false);
                    TextObj.SetActive(false);
                    Timer.fillAmount = 1;
                    LimitTime = 10;
                    YesCount = 0;
                    SurrenderCount = 0;
                    Surrender_UI.SetActive(false);
                }
            }
        }
    }

    private void voteEnd()
    {
        YesBtn.SetActive(false);
        NoBtn.SetActive(false);
        resultText.text = "과반수 동의로 항복합니다";
        if (!SendOnce)
        {
            SendOnce = true;
            if (PhotonNetwork.player.GetTeam().ToString().Equals("red") && PhotonNetwork.player.GetTeam().ToString().Equals(Team))
            {
                if (inGameManager.redTeamPlayer[0].GetPhotonView().owner.Equals(PhotonNetwork.player))
                    inGameManager.GameEnded("red");
            }
            else if (PhotonNetwork.player.GetTeam().ToString().Equals("blue"))
            {
                if (inGameManager.blueTeamPlayer[0].GetPhotonView().owner.Equals(PhotonNetwork.player))
                    inGameManager.GameEnded("blue");
            }
        }
    }

    public void UI_Open()
    {
        //if 시간 확인.
        if (!Surrender_UI.activeInHierarchy)
            PhotonNetwork.RaiseEvent(evcode_UIopen, null, true, op);
        else
        {
            return;
        }
    }
    public void UI_Exit()
    {
        //if 시간 확인.
        //if (Surrender_UI.activeInHierarchy)
        //    PhotonNetwork.RaiseEvent(evcode_UIopen, null, false, op);
    }

    public void Btn_Yes()
    {
        isAgree = true;
        object[] datas = new object[] { isAgree };
        PhotonNetwork.RaiseEvent(evcode_Select_Surrender, datas, true, op);
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }
    public void Btn_No()
    {
        isAgree = false;
        object[] datas = new object[] { isAgree };
        PhotonNetwork.RaiseEvent(evcode_Select_Surrender, datas, true, op);
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }
}