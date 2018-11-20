using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIWinLose : MonoBehaviour {

    public TabUI mytabUI;
    public GameObject win_Text;
    public GameObject lose_Text;

    public void ExitButton()
    {
        string resultText = "";
        if(win_Text.activeSelf)
            resultText = win_Text.GetComponent<Text>().text;
        else if(lose_Text.activeSelf)
            resultText = lose_Text.GetComponent<Text>().text;

        // 탭창의 Result데이터를 ResultManager에 저장
        mytabUI.ResultManagerSave(resultText);

        // 아이템 리스트 리셋해줌.
        PlayerData.Instance.ItemReset();
        PlayerData.Instance.GoldReset();

        SceneManager.LoadScene("Result");
    }
}
