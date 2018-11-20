using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabUI : MonoBehaviour
{

    public GameObject[] BlueTeamInfo;
    public GameObject[] RedTeamInfo;

    private ResultManager.ResultData[] blueTeamRD = new ResultManager.ResultData[5];
    private ResultManager.ResultData[] redTeamRD = new ResultManager.ResultData[5];

    private InGameManager inGameManager;

    //
    private int blueTeamTotalKill = 0;
    private int redTeamTotalKill = 0;
    [Space]
    public Text blueTeamKill;
    public Text redTeamKill;
    [Space]
    public Text blueTeamTowerKill;
    public Text redTeamTowerKill;
    [Space]
    public Text blueTeamDragonKill;
    public Text redTeamDragonKill;

    [Space]
    public GameObject[] explanations;

    private void Awake()
    {
        // 자식들에게 번호를 먹여줌
        for (int i = 0; i < BlueTeamInfo.Length; i++)
        {
            BlueTeamInfo[i].transform.SetSiblingIndex(i);
        }
        for (int i = 0; i < RedTeamInfo.Length; i++)
        {
            RedTeamInfo[i].transform.SetSiblingIndex(i);
        }

        // 방 인원수에 맞게끔만 뜨게하고 나머지는 꺼버림
        float f = (float)PhotonNetwork.room.PlayerCount / 2f;
        // 0.5를 반올림했는데 0이나와서 0.01을 더해줌(이해불가)
        int n = Mathf.RoundToInt(f + 0.01f);
        for (int i = n; i < 5; i++)
        {
            BlueTeamInfo[i].SetActive(false);
            RedTeamInfo[i].SetActive(false);
        }

        // ResultData를 초기화
        for (int i = 0; i < 5; i++)
        {
            blueTeamRD[i] = new ResultManager.ResultData();
            redTeamRD[i] = new ResultManager.ResultData();
        }

        inGameManager = GameObject.FindGameObjectWithTag("InGameManager").GetComponent<InGameManager>();
    }

    private void OnEnable()
    {
        // 예
        if (inGameManager == null)
            return;

        // 모든 플레이어가 로딩이 다 됐으면
        if (inGameManager.runOnce)
            TabRefresh();

        // 새창열때 설명 열린거있으면 다꺼줌
        foreach (GameObject go in explanations)
        {
            go.SetActive(false);
        }
    }

    public void TabRefresh()
    {
        TeamRefresh("blue");
        TeamRefresh("red");
        killTowerDragonUpdate();
    }

    public void killTowerDragonUpdate()
    {
        blueTeamTotalKill = 0;
        redTeamTotalKill = 0;
        foreach (var blueTeamChampion in inGameManager.blueTeamPlayer)
        {
            if (blueTeamChampion != null)
                blueTeamTotalKill += blueTeamChampion.GetComponent<ChampionData>().kill;
        }
        foreach (var redTeamChampion in inGameManager.redTeamPlayer)
        {
            if (redTeamChampion != null)
                redTeamTotalKill += redTeamChampion.GetComponent<ChampionData>().kill;
        }

        blueTeamKill.text = blueTeamTotalKill.ToString();
        redTeamKill.text = redTeamTotalKill.ToString();
        blueTeamTowerKill.text = inGameManager.blueTeamTowerKill.ToString();
        redTeamTowerKill.text = inGameManager.redTeamTowerKill.ToString();
        blueTeamDragonKill.text = inGameManager.blueTeamDragonKill.ToString();
        redTeamDragonKill.text = inGameManager.redTeamDragonKill.ToString();
    }

    public void ResultDataSave(string team, int index, ChampionData cd)
    {
        if (team.Equals("red"))
        {
            redTeamRD[index].championName = cd.ChampionName;
            redTeamRD[index].nickName = cd.GetComponent<PhotonView>().owner.NickName;
            redTeamRD[index].level = cd.totalstat.Level;
            redTeamRD[index].kill = cd.kill;
            redTeamRD[index].death = cd.death;
            redTeamRD[index].assist = cd.assist;
            redTeamRD[index].cs = cd.cs;
            redTeamRD[index].items[0] = cd.item[0];
            redTeamRD[index].items[1] = cd.item[1];
            redTeamRD[index].items[2] = cd.item[2];
            redTeamRD[index].items[3] = cd.item[3];
            redTeamRD[index].items[4] = cd.item[4];
            redTeamRD[index].items[5] = cd.item[5];
            redTeamRD[index].accessoryItem = cd.accessoryItem;

            if (cd.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
            {
                redTeamRD[index].me = true;
            }
        }
        else if (team.Equals("blue"))
        {
            blueTeamRD[index].championName = cd.ChampionName;
            blueTeamRD[index].nickName = cd.GetComponent<PhotonView>().owner.NickName;
            blueTeamRD[index].level = cd.totalstat.Level;
            blueTeamRD[index].kill = cd.kill;
            blueTeamRD[index].death = cd.death;
            blueTeamRD[index].assist = cd.assist;
            blueTeamRD[index].cs = cd.cs;
            blueTeamRD[index].items[0] = cd.item[0];
            blueTeamRD[index].items[1] = cd.item[1];
            blueTeamRD[index].items[2] = cd.item[2];
            blueTeamRD[index].items[3] = cd.item[3];
            blueTeamRD[index].items[4] = cd.item[4];
            blueTeamRD[index].items[5] = cd.item[5];
            blueTeamRD[index].accessoryItem = cd.accessoryItem;

            if (cd.GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
            {
                blueTeamRD[index].me = true;
            }
        }
    }

    public void ResultManagerSave(string result)
    {
        ResultManager.Instance.result = result;

        for (int i = 0; i < 5; i++)
        {
            if (blueTeamRD[i] != null)
                ResultManager.Instance.ResultInput(blueTeamRD[i], "blue");
            if (redTeamRD[i] != null)
                ResultManager.Instance.ResultInput(redTeamRD[i], "red");
        }
    }

    public void TeamRefresh(string team)
    {
        int repeatCount = 0;
        if (team.Equals("blue"))
        {
            repeatCount = inGameManager.blueTeamPlayer.Count;
        }
        else if (team.Equals("red"))
        {
            repeatCount = inGameManager.redTeamPlayer.Count;
        }

        for (int j = 0; j < repeatCount; j++)
        {
            ChampionData cd = null;
            TabCharacterInfo characterInfo = null;

            if (team.Equals("blue"))
            {
                if (inGameManager.blueTeamPlayer[j].gameObject != null)
                {
                    cd = inGameManager.blueTeamPlayer[j].GetComponent<ChampionData>();
                    characterInfo = BlueTeamInfo[j].GetComponent<TabCharacterInfo>();
                    BlueTeamInfo[j].SetActive(true);
                }
            }
            else if (team.Equals("red"))
            {
                if (inGameManager.redTeamPlayer[j].gameObject != null)
                {
                    cd = inGameManager.redTeamPlayer[j].GetComponent<ChampionData>();
                    characterInfo = RedTeamInfo[j].GetComponent<TabCharacterInfo>();
                    RedTeamInfo[j].SetActive(true);
                }
            }
            else
            {
                return;
            }

            if (cd == null)
                return;

            // 결과창에 넘길 RD에 저장
            ResultDataSave(team, j, cd);

            // 실제 탭 갱신. 아이콘 텍스트 스펠이미지 등...
            // cd에서 invoke로 너무 늦게 찾아줘서 그전에 누르면 터지기에 없으면 바로 불러주게함.
            if (cd.UIIcon == null)
            {
                cd.FindUICanvas();
            }

            // UIICon, UISkill은 내거니까 다른애들것도 제대로 뜨려면 여기서 가지고 오는게 아니라 리소스에서 불러줘야함
            if (characterInfo.championIcon_Image.sprite == null)
                characterInfo.championIcon_Image.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + cd.ChampionName);

            if (characterInfo.spell_up_Image.sprite == null)
            {
                characterInfo.spell_up_Image.sprite = Resources.Load<Sprite>("Spell/" + cd.spell_D);
                characterInfo.spell_up_Image.color = Color.white;
            }
            if (characterInfo.spell_down_Image.sprite == null)
            {
                characterInfo.spell_down_Image.sprite = Resources.Load<Sprite>("Spell/" + cd.spell_F);
                characterInfo.spell_down_Image.color = Color.white;
            }

            // cd에서 spell을 받아오기전에 불러서 0,0으로 들어오면 null로 바꿔서 다음 업데이트때 새로 받게함
            if (cd.spell_D == cd.spell_F)
            {
                characterInfo.championIcon_Image.sprite = null;
                characterInfo.spell_up_Image.sprite = null;
                characterInfo.spell_down_Image.sprite = null;
            }

            // 스펠쿨 + 궁극기쿨도 받아오기
            // 아군 적군 부활시간도 받아오기

            // 킬뎃은 시야 안보여도 갱신되기때문에 위로뺌
            characterInfo.kda_Text.text = cd.kill.ToString() + "/" + cd.death.ToString() + "/" + cd.assist.ToString();

            // 마우스 오버하면 아이디나옴
            characterInfo.nickname = cd.GetComponent<PhotonView>().owner.NickName;

            // 시야에서 안보이는애면 ResultData만 저장하고 아이콘, 스펠만 업데이트하고 레벨,kda,cs,아이템을 업데이트하지않음.
            if (!cd.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                continue;

            // 레벨, CS 받아오기.
            characterInfo.level_Text.text = cd.totalstat.Level.ToString();
            characterInfo.cs_Text.text = cd.cs.ToString();

            // 아이템 적용
            for (int i = 0; i < 6; i++)
            {
                if (cd.item[i] != 0)
                {
                    ShopItem.Item it = ShopItem.Instance.itemlist[cd.item[i]];
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

            if (cd.accessoryItem != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[cd.accessoryItem];
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

        // 유저수만큼만 갱신하고 뒤에 혹시 안꺼진게 있으면 꺼줌
        for (int i = repeatCount; i < 5; i++)
        {
            if (team.Equals("blue"))
            {
                if (BlueTeamInfo[i].activeSelf)
                    BlueTeamInfo[i].SetActive(false);
            }
            else if (team.Equals("red"))
            {
                if (RedTeamInfo[i].activeSelf)
                    RedTeamInfo[i].SetActive(false);
            }
        }
    }
}