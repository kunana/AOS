using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public enum Index { Null = 0, Welcome, MinionWait, MinionSpawn, SuperMinion, Kill, Killed, Execution, T_Destroy, T_Destroyed, Inhibitor_Destroy, Inhibitor_Destroyed, Inhibitor_Respawn, E_Inhibitor_Respawn };
public class SystemMessage : MonoBehaviour
{

    [HideInInspector]
    public Index index = Index.Null;
    private string[] Message = new string[]
    {"", "아카데미 협곡에 오신 것을 환영합니다", "미니언 생성까지 30초 남았습니다", "미니언이 생성되었습니다",
        "이제 슈퍼 미니언이 생성됩니다!", "적을 처치하였습니다", "처치 당했습니다","처형 당했습니다", "포탑이 파괴되었습니다",
        "적 포탑이 파괴되었습니다", "적 억제기가 파괴되었습니다", "억제기가 파괴되었습니다",
    "억제기가 곧 재생성 됩니다", "적 억제기가 곧 재생성 됩니다"};

    public Sprite[] Iconlist = new Sprite[] { };

    public Text Text;
    Color red = new Color(255f, 60f, 60f, 0f);
    Color blue = new Color(57f, 204f, 255f, 0f);

    public GameObject Win_Lose_Image;
    public Text WinText;
    public Text LoseText;

    public GameObject ChampIconL;
    public GameObject ChampIconR;

    public Image ChampLeft;
    public Image ChampRight;

    public GameObject WinLoseUI;
    public Text Game_Won;
    public Text Game_Lose;

    private GameObject ExitButton;
    private bool soundonce = false;

    RaiseEventOptions op;
    byte evcode;
    private byte TeamCode = 0;
    object[] curdata = new object[3];

    private void Start()
    {

        if (PhotonNetwork.player.GetTeam().ToString().Equals("red"))
            TeamCode = 131;
        else
            TeamCode = 141;

        ExitButton = Win_Lose_Image.transform.GetChild(0).gameObject;

        PhotonNetwork.OnEventCall += SysMessageReceived;
        op = new RaiseEventOptions { Receivers = ReceiverGroup.All };

    }

    private void OnDestroy()
    {
        PhotonNetwork.OnEventCall -= SysMessageReceived;
    }

    private void SysMessageReceived(byte eventCode, object content, int senderId)
    {
        if (eventCode == TeamCode)
        {
            Sequence se = DOTween.Sequence();
            se.Append(Text.DOFade(1.0f, 3f))
               .Append(Text.DOColor(Color.white, 0f))
               .Append(Text.DOFade(0.0f, 3f));
            TextChanger((int)content);


        }
        else if (eventCode == 150)
        {
            Sequence se = DOTween.Sequence();
            se.Append(Text.DOFade(1.0f, 1.5f))
               .Append(Text.DOColor(Color.white, 0f))
               .Append(Text.DOFade(0.0f, 3f));
            TextChanger((int)content);
        }
        else if (eventCode == 160)
        {
            object[] Receiveddatas = content as object[];
            KillMsg((string)Receiveddatas[0], (string)Receiveddatas[1], (string)Receiveddatas[2]);
        }
        return;
    }

    /// <param name="num">0Null, 1Welcome, 2MinionWait, 3MinionSpawn, 4SuperMinion, 5Kill, 6Killed, 7Excution, 8T_Destroy, 
    /// 9 T_Destroyed, 10Inhibitor_Destroy, 11Inhibitor_Destroyed, 12Inhibitor_Respawn </param>
    /// <param name="isAnnouce">전체 방송인지 팀 전송인지.</param>
    public void Annoucement(int num, bool isAnnouce, string team = null)
    {
        if (isAnnouce)
            evcode = 150;
        else
        {
            if (team.Equals("red") || team.Equals("Red"))
                evcode = 131;
            else if (team.Equals("blue") || team.Equals("Blue"))
                evcode = 141;
        }
        PhotonNetwork.RaiseEvent(evcode, num, true, op);
        PhotonNetwork.SendOutgoingCommands();
    }

    /// <param name="KillerImg">가해자 오브젝트의 이미지(string)</param>
    /// <param name="KilledImg">피해자 오브젝트 이미지(string)</param>
    /// <param name="Team">가해자의 팀(string) 타워면 "ex"</param>
    public void sendKillmsg(string KillerImg, string KilledImg, string Team)
    {
        evcode = 160;
        object[] datas = new object[] { KillerImg, KilledImg, Team };

        if (isDatasame(curdata, datas))
            return;
        curdata = datas;
        PhotonNetwork.RaiseEvent(evcode, datas, true, op);
        PhotonNetwork.SendOutgoingCommands();
        Invoke("Reset_data", 10f);
    }
    public void Reset_data()
    {
        for (int i = 0; i < curdata.Length; i++)
        {
            curdata[i] = null;
        }
    }

    private bool isDatasame(object[] data1, object[] data2)
    {
        bool issame = false;
        for (int i = 0; i < 3; i++)
        {
            if ((string)data1[i] == (string)data2[i])
            {
                issame = true;
            }
            else
            {
                issame = false;
                return issame;
            }
        }
        return issame;
    }

    private void KillMsg(string KillerImg, string KilledImg, string type)
    {

        if (type.ToLower().Equals("ex")) // 처형이면
        {
            TextChanger(7);
            Sequence se = DOTween.Sequence();
            se.Append(Text.DOFade(1.0f, 1.5f))
               .Append(Text.DOColor(Color.red, 0f))
               .Append(Text.DOFade(0.0f, 3f));
        }
        else if (type.ToLower() == PhotonNetwork.player.GetTeam().ToString().ToLower()) //가해자가 같은팀이면
        {
            TextChanger(5);
            Sequence se = DOTween.Sequence();
            se.Append(Text.DOFade(1.0f, 1.5f))
               .Append(Text.DOFade(0.0f, 3f));
        }
        else if (type.ToLower() != PhotonNetwork.player.GetTeam().ToString().ToLower()) // 가해자가 다른팀이면
        {
            TextChanger(6);
            Sequence se = DOTween.Sequence();
            se.Append(Text.DOFade(1.0f, 1.5f))
               .Append(Text.DOFade(0.0f, 3f));
        }

        ChampIconL.SetActive(true);
        ChampIconR.SetActive(true);
        sprtieChanger(KillerImg, KilledImg);
        Invoke("ActiveOff", 3f);

    }
    private void ActiveOff()
    {
        ChampIconL.SetActive(false);
        ChampIconR.SetActive(false);
    }
    /// <summary>
    /// 스프라이트 배열순서 0 = 아리 1 = 애쉬 2 = 가렌 3 = 문도 4 = 알리스타 5 = 타워 6 = 미니언 7 = 몬스터
    /// </summary>
    private void sprtieChanger(string KillerImg, string KilledImg)
    {
        if (KillerImg.ToLower().Contains("ahri"))
        {
            ChampLeft.sprite = Iconlist[0];
        }
        else if (KillerImg.ToLower().Contains("ashe"))
        {
            ChampLeft.sprite = Iconlist[1];
        }
        else if (KillerImg.ToLower().Contains("garen"))
        {
            ChampLeft.sprite = Iconlist[2];
        }
        else if (KillerImg.ToLower().Contains("mundo"))
        {
            ChampLeft.sprite = Iconlist[3];
        }
        else if (KillerImg.ToLower().Contains("alistar"))
        {
            ChampLeft.sprite = Iconlist[4];
        }
        else if (KillerImg.ToLower().Contains("tower"))
        {
            ChampLeft.sprite = Iconlist[5];
        }
        else if (KillerImg.ToLower().Contains("minion"))
        {
            ChampLeft.sprite = Iconlist[6];
        }
        else if (KillerImg.ToLower().Contains("monster"))
        {
            ChampLeft.sprite = Iconlist[7];
        }

        if (KilledImg.ToLower().Contains("ahri"))
        {
            ChampRight.sprite = Iconlist[0];
        }
        else if (KilledImg.ToLower().Contains("ashe"))
        {
            ChampRight.sprite = Iconlist[1];
        }
        else if (KilledImg.ToLower().Contains("garen"))
        {
            ChampRight.sprite = Iconlist[2];
        }
        else if (KilledImg.ToLower().Contains("mundo"))
        {
            ChampRight.sprite = Iconlist[3];
        }
        else if (KilledImg.ToLower().Contains("alistar"))
        {
            ChampRight.sprite = Iconlist[4];
        }
        else if (KilledImg.ToLower().Contains("tower"))
        {
            ChampRight.sprite = Iconlist[5];
        }
        else if (KilledImg.ToLower().Contains("minion"))
        {
            ChampRight.sprite = Iconlist[6];
        }
        else if (KilledImg.ToLower().Contains("monster"))
        {
            ChampRight.sprite = Iconlist[7];
        }
    }

    public void GameEndUI(bool islose)//졌으면 true
    {
        SoundManager.instance.PlaySound(SoundManager.instance.Nexus_DestroyUI);
        Win_Lose_Image.SetActive(true);//배경 이미지 활성화
        if (islose)//졌으면
        {
            LoseText.gameObject.SetActive(true);
            LoseText.DOFade(1.0f, 3.0f);
            Invoke("lose", 3f);
        }
        else// 이겼으면
        {
            WinText.gameObject.SetActive(true);
            WinText.DOFade(1.0f, 3.0f);
            Invoke("Win", 3f);
        }

        ExitButton.SetActive(true);
    }

    private void Win()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.Victory);
    }
    private void lose()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.Defeat);
    }

    public void TextChanger(int num)
    {
        if (this == null)
            return;

        switch (num)
        {
            case (int)Index.Null:
                Text.text = Message[0];
                break;
            case (int)Index.Welcome:
                Text.text = Message[1];
                if(!soundonce)
                {
                    soundonce = true;
                ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Welcome);
                }
                break;
            case (int)Index.MinionWait:
                Text.text = Message[2];
                SoundManager.instance.PlaySound(SoundManager.instance.Minion_30Second_Remain);
                break;
            case (int)Index.MinionSpawn:
                Text.text = Message[3];
                SoundManager.instance.PlaySound(SoundManager.instance.Minion_Maked);
                break;
            case (int)Index.SuperMinion:
                Text.text = Message[4];
                break;
            case (int)Index.Kill:
                Text.DOColor(Color.blue, 0f);
                Text.text = Message[5];
                SoundManager.instance.PlaySound(SoundManager.instance.Champion_Kill);
                break;
            case (int)Index.Killed:
                Text.DOColor(Color.red, 0f);
                Text.text = Message[6];
                SoundManager.instance.PlaySound(SoundManager.instance.Champion_Killed);
                break;
            case (int)Index.Execution:
                Text.DOColor(Color.red, 0f);
                Text.text = Message[7];
                SoundManager.instance.PlaySound(SoundManager.instance.Champion_Executed);
                break;
            case (int)Index.T_Destroy:
                Text.DOColor(Color.blue, 0f);
                Text.text = Message[8];
                SoundManager.instance.PlaySound(SoundManager.instance.Destroy_Tower);
                break;
            case (int)Index.T_Destroyed:
                Text.DOColor(Color.red, 0f);
                Text.text = Message[9];
                SoundManager.instance.PlaySound(SoundManager.instance.Destroy_Tower);
                break;
            case (int)Index.Inhibitor_Destroy:
                Text.DOColor(Color.blue, 0f);
                Text.text = Message[10];
                SoundManager.instance.PlaySound(SoundManager.instance.Destroy_Suppressor);
                break;
            case (int)Index.Inhibitor_Destroyed:
                Text.DOColor(Color.red, 0f);
                Text.text = Message[11];
                SoundManager.instance.PlaySound(SoundManager.instance.Destroyed_Suppressor);
                break;
            case (int)Index.Inhibitor_Respawn:
                Text.DOColor(Color.blue, 0f);
                Text.text = Message[12];
                SoundManager.instance.PlaySound(SoundManager.instance.Revive_Suppressor);
                break;
            case (int)Index.E_Inhibitor_Respawn:
                Text.DOColor(Color.red, 0f);
                Text.text = Message[13];
                SoundManager.instance.PlaySound(SoundManager.instance.EnemyRevive_Suppressor);
                break;
        }
    }
}