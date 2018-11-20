using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabCharacterInfo : MonoBehaviour
{

    public Image spell_up_Image;
    public Image spell_down_Image;
    public Image championIcon_Image;
    public Image ultimate_Image;
    public Text level_Text;
    public Text kda_Text;
    public Text cs_Text;
    public Image[] items;
    public Image accessoryItem;

    // Result씬용
    public Text nickname_Text;

    [Space]
    public string nickname;
    public GameObject IDView;
    public Text IDViewText;
    private bool mouseOver = false;

    private Image myBackground;

    private void Awake()
    {
        myBackground = GetComponent<Image>();
    }

    private void OnEnable()
    {
        mouseOver = false;
        if (PhotonNetwork.playerName.Equals(nickname))
        {
            myBackground.color = new Color(42f / 255f, 110f / 255f, 178f / 255f, 50f / 255f);
        }
    }

    private void Update()
    {
        if (mouseOver)
        {
            InfoMouseOver();
        }
    }

    public void InfoMouseEnter()
    {
        IDView.SetActive(true);
        IDViewText.text = nickname.ToString();
        mouseOver = true;
    }

    public void InfoMouseOver()
    {
        IDView.transform.position = Input.mousePosition + new Vector3(1f, 1f);
    }

    public void InfoMouseExit()
    {
        IDView.SetActive(false);
        mouseOver = false;
    }
}