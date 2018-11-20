using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultCanvas : Photon.PunBehaviour {

    public Text winloseText;
    public Image myChampionImage;
    public Text myGradeText;

    public GameObject[] BlueTeamResultInfo;
    public GameObject[] RedTeamResultInfo;

    private void Awake()
    {
        TabCharacterInfo characterInfo = null;
        ResultManager.ResultData rd = null;

        winloseText.text = ResultManager.Instance.result;

        // 데이터가 저장되있지않으면 해당줄 액티브 꺼버림
        // 저장되있으면 저장데이터 불러와서 보여줌
        for (int i = 0; i < 5; i++)
        {
            if (string.IsNullOrEmpty(ResultManager.Instance.blueTeamResults[i].championName))
            {
                BlueTeamResultInfo[i].SetActive(false);
            }
            else
            {
                characterInfo = BlueTeamResultInfo[i].GetComponent<TabCharacterInfo>();
                rd = ResultManager.Instance.blueTeamResults[i];

                DataApply(characterInfo, rd);

                if (rd.me)
                {
                    myChampionImage.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + rd.championName + "_Big");
                    myGradeText.text = GradeCalculate(rd.kill, rd.death, rd.assist);
                }
            }

            if (string.IsNullOrEmpty(ResultManager.Instance.redTeamResults[i].championName))
                RedTeamResultInfo[i].SetActive(false);
            else
            {
                characterInfo = RedTeamResultInfo[i].GetComponent<TabCharacterInfo>();
                rd = ResultManager.Instance.redTeamResults[i];

                DataApply(characterInfo, rd);

                if (rd.me)
                {
                    myChampionImage.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + rd.championName + "_Big");
                    myGradeText.text = GradeCalculate(rd.kill, rd.death, rd.assist);
                }
            }
        }
        //PhotonNetwork.LeaveRoom();
    }

    public string GradeCalculate(int kill, int death, int assist)
    {
        float kda = (float)(kill + assist) / (float)death;
        string grade;

        if (kda >= 3.0f)
            grade = "S+";
        else if (kda >= 2.7f)
            grade = "S";
        else if (kda >= 2.4f)
            grade = "S-";
        else if (kda >= 2.1f)
            grade = "A+";
        else if (kda >= 1.8f)
            grade = "A";
        else if (kda >= 1.5f)
            grade = "A-";
        else if (kda >= 1.2f)
            grade = "B+";
        else if (kda >= 0.9f)
            grade = "B";
        else if (kda >= 0.6f)
            grade = "B-";
        else
            grade = "C";

        return grade;
    }

    public void DataApply(TabCharacterInfo characterInfo, ResultManager.ResultData rd)
    {
        characterInfo.level_Text.text = rd.level.ToString();
        characterInfo.championIcon_Image.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + rd.championName + "_Big");

        characterInfo.nickname_Text.text = rd.nickName;

        characterInfo.kda_Text.text = rd.kill.ToString() + "/" + rd.death.ToString() + "/" + rd.assist.ToString();
        characterInfo.cs_Text.text = rd.cs.ToString();

        for (int i = 0; i < 6; i++)
        {
            if (rd.items[i] != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[rd.items[i]];
                // 원본의 주소를 가져오므로 변경해서는 myItem을 변경해서는 안됨.
                characterInfo.items[i].gameObject.GetComponent<ItemInfo>().myItem = it;
                characterInfo.items[i].sprite = Resources.Load<Sprite>("Item_Image/" + it.icon_name);
                characterInfo.items[i].color = Color.white;
            }
            else
            {
                characterInfo.items[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                characterInfo.items[i].sprite = null;
                characterInfo.items[i].color = new Color(1, 1, 1, 0);
            }
        }

        if (rd.accessoryItem != 0)
        {
            ShopItem.Item it = ShopItem.Instance.itemlist[rd.accessoryItem];
            characterInfo.accessoryItem.gameObject.GetComponent<ItemInfo>().myItem = it;
            characterInfo.accessoryItem.sprite = Resources.Load<Sprite>("Item_Image/" + it.icon_name);
            characterInfo.accessoryItem.color = Color.white;
        }
        else
        {
            characterInfo.accessoryItem.gameObject.GetComponent<ItemInfo>().myItem = null;
            characterInfo.accessoryItem.sprite = null;
            characterInfo.accessoryItem.color = new Color(1, 1, 1, 0);
        }
    }

    public void LeaveButton()
    {
        ResultManager.Instance.ListReset();
        PhotonNetwork.player.SetTeam(PunTeams.Team.none);

        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Lobby");
    }
}
