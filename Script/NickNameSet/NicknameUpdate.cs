using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;

public class NicknameUpdate : MonoBehaviour {

    public InputField Input_nickname;
    public Text ErrorText;

    // Use this for initialization
    void Start () {
        Input_nickname.ActivateInputField();
    }

    void Update()
    {

    }

    public void EnterCheck()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            AcceptButton();
        }
    }

    public void AcceptButton()
    {
        if (Input_nickname.text.Contains("BGA") || Input_nickname.text.Contains("bga"))
        {
            ErrorText.text = "BGA라는 단어는 직원용 계정에 한정되므로 포함될 수 없습니다.";
            Input_nickname.ActivateInputField();
            return;
        }

        // playfab 서버 접속되었는지 확인하여 되면 실행. 아니면 에러메세지 출력
        var request = new UpdateUserTitleDisplayNameRequest { DisplayName = Input_nickname.text };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, DisplayNameUpdateSuccess, DisplayNameUpdateFailure);
    }

    private void DisplayNameUpdateSuccess(UpdateUserTitleDisplayNameResult result2)
    {
        //print("DisplayName 업데이트에 성공");
        PlayerPrefs.SetString("Nickname", Input_nickname.text);
        SceneManager.LoadScene("Lobby");
    }

    private void DisplayNameUpdateFailure(PlayFabError error)
    {
        //print("DisplayName 업데이트에 실패");
        print(error.GenerateErrorReport());

        if (error.ErrorMessage == "Invalid input parameters")
            ErrorText.text = "소환사 이름을 다시 확인해주세요.";
        else if (error.ErrorMessage == "Name not available")
            ErrorText.text = "소환사 이름이 중복되었습니다.";

        Input_nickname.ActivateInputField();
    }
}
