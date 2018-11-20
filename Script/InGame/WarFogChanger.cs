using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarFogChanger : Photon.PunBehaviour {

    FogOfWarManager WarFogManager;

    private void Start()
    {
        WarFogManager = this.GetComponent<FogOfWarManager>();
        if(PhotonNetwork.player.IsLocal)
        {
            if (PhotonNetwork.player.GetTeam().ToString().Contains("red"))
            {
                WarFogManager.ShowFaction(FogOfWar.Players.Player00);
            }
            else
            {
                WarFogManager.ShowFaction(FogOfWar.Players.Player01);
            }
        }
        

    }
}
