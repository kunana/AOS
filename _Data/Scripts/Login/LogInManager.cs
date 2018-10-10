using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class LogInManager : MonoBehaviour
{
    public Toggle videoCheck = null;
    public Toggle soundCheck = null;
    public VideoPlayer video = null;
    public AudioSource audio = null;

    public InputField InputID;
    public InputField InputPass;
    public Text ErrorText;

    public GameObject RegisterWindow;
    public InputField RegisterInputID;
    public InputField RegisterInputPass;

    private string username;
    private string password;

    // Use this for initialization
    private void Awake()
    {
        // B668 근우
        PlayFabSettings.TitleId = "5C01";
        video.GetComponent<VideoPlayer>().Play();
    }

    public void ID_value_Changed()
    {
        username = InputID.text.ToString();

        // 엔터치면 로그인함수 실행
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Login();
        }
    }

    public void PW_value_Changed()
    {
        password = InputPass.text.ToString();

        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
             Login();
        }
    }

    public void RIDPW_value_Changed()
    {
        // 엔터치면 회원가입
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            Register();
        }
    }

    public void Login()
    {
        var request = new LoginWithPlayFabRequest { Username = username, Password = password };
        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);
        ErrorText.text = "<color=#ffffff>로그인 중...</color>";
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("로그인 성공");
        PlayerPrefs.SetString("Username", username);

        // 계정정보 받아옴
        var request = new GetAccountInfoRequest { Username = username };
        PlayFabClientAPI.GetAccountInfo(request, GetAccountSuccess, GetAccountFailure);
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("로그인 실패");
        Debug.LogWarning(error.GenerateErrorReport());
        ErrorText.text = "계정이름과 비밀번호가 일치하지 않습니다. 다시 시도해 주세요.";
    }

    private void GetAccountSuccess(GetAccountInfoResult result)
    {
        print("Accout를 정상적으로 받아옴");

        string nickname = result.AccountInfo.TitleInfo.DisplayName;
        if(nickname == null)
        {
            SceneManager.LoadScene("NicknameSet");
        }
        else
        {
            PlayerPrefs.SetString("Nickname", nickname);
            SceneManager.LoadScene("Lobby");
        }
    }

    private void GetAccountFailure(PlayFabError error)
    {
        print("Accout를 받아오지 못함");
        print(error.GenerateErrorReport());
        ErrorText.text = "계정정보를 받아오지 못했습니다.";
    }

    public void RegisterButton()
    {
        RegisterWindow.SetActive(true);
    }

    public void Register()
    {
        var request = new RegisterPlayFabUserRequest { Username = RegisterInputID.text, Password = RegisterInputPass.text, RequireBothUsernameAndEmail = false };
        PlayFabClientAPI.RegisterPlayFabUser(request, RegisterSuccess, RegisterFailure);
    }

    public void Cancel()
    {
        RegisterInputID.text = "";
        RegisterInputPass.text = "";
        RegisterWindow.SetActive(false);
    }

    private void RegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("가입 성공");
        ErrorText.text = "계정 생성에 성공했습니다.";
        Cancel();
    }

    private void RegisterFailure(PlayFabError error)
    {
        Debug.LogWarning("가입 실패");
        Debug.LogWarning(error.GenerateErrorReport());
        ErrorText.text = "계정 생성에 실패했습니다. 계정이름은 3자, 비밀번호는 6자리 이상으로 설정해주세요.";
        print(error.ErrorMessage);
    }

    public void VideoToggle()
    {
        if (videoCheck.GetComponent<Toggle>().isOn)
        {
            video.GetComponent<VideoPlayer>().playbackSpeed = 0;
        }
        else
        {
            video.GetComponent<VideoPlayer>().playbackSpeed = 1;
        }
    }

    public void AudioToggle()
    {
        if (soundCheck.GetComponent<Toggle>().isOn)
        {
            audio.GetComponent<AudioSource>().mute = true;
        }
        else
        {
            audio.GetComponent<AudioSource>().mute = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
