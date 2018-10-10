using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignName : MonoBehaviour {

    Text ChampName;

    private void Start()
    {
        ChampName = this.GetComponent<Text>();
        AssignText();
    }
    public void AssignText()
    {
        if(PhotonNetwork.player.IsLocal)
        ChampName.text = PlayerData.Instance.championName;
    }
}
