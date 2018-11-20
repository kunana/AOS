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

        // ��â�� Result�����͸� ResultManager�� ����
        mytabUI.ResultManagerSave(resultText);

        // ������ ����Ʈ ��������.
        PlayerData.Instance.ItemReset();
        PlayerData.Instance.GoldReset();

        SceneManager.LoadScene("Result");
    }
}
