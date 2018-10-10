using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TestSceneStart : MonoBehaviour
{

    public PhotonConnector pc;
    public InputField nickname_inputfield;
    public Dropdown champion_dropdown;
    public Dropdown team_dropdown;

    private string nickname;
    private string team;

    private bool load = false;
    AsyncOperation async;

    void Start()
    {
        PlayerData.Instance.championName = "Ahri";
        team = "red";


    }

    public void Team_Update(int value)
    {
        switch (value)
        {
            case 0:
                team = "red";
                break;
            case 1:
                team = "blue";
                break;
            default:
                break;
        }
    }

    public void ChampionName_Update(int value)
    {
        if (PhotonNetwork.player.IsLocal)
        {
            switch (value)
            {
                case 0:
                    PlayerData.Instance.championName = "Ahri";
                    break;
                case 1:
                    PlayerData.Instance.championName = "Alistar";
                    break;
                case 2:
                    PlayerData.Instance.championName = "Ashe";
                    break;
                case 3:
                    PlayerData.Instance.championName = "Garen";
                    break;
                case 4:
                    PlayerData.Instance.championName = "Mundo";
                    break;
                default:
                    break;
            }
        }
    }

    public void Nickname_Update(string value)
    {
        nickname = value;
    }

    public void PhotonConnectButton()
    {
        if (!string.IsNullOrEmpty(nickname))
            pc.Connect(nickname, team);
    }

    public void StartButtonClick()
    {
        if (pc.isConnecting && PhotonNetwork.isMasterClient)
        {
            SceneManager.LoadScene("InGame");
            //if (!load)
            //    StartCoroutine(LoadNewScene());

        }
        else
            print("아직 접속안됨");
    }

    IEnumerator LoadNewScene()
    {

        // This line waits for 3 seconds before executing the next line in the coroutine.
        // This line is only necessary for this demo. The scenes are so simple that they load too fast to read the "Loading..." text.
        yield return new WaitForSeconds(3);

        // Start an asynchronous operation to load the scene that was passed to the LoadNewScene coroutine.
        AsyncOperation async = SceneManager.LoadSceneAsync("InGame");

        // While the asynchronous operation to load the new scene is not yet complete, continue waiting until it's done.
        while (!async.isDone)
        {
            yield return null;
        }
    }
}
