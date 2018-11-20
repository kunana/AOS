using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarFogForEffect : MonoBehaviour
{

    FogOfWarEntity warFog;
    int childCount = 0;
    ParticleSystem ps;
    public FogOfWarEntity senderEntity;
    public GameObject[] mychild;
    private void Awake()
    {
        childCount = transform.childCount;
        ps = GetComponent<ParticleSystem>();
        warFog = GetComponent<FogOfWarEntity>();
        if (!warFog)
        {
            this.gameObject.AddComponent<FogOfWarEntity>();
            if (warFog.gameObjects.Length == 0)
            {
                warFog.gameObjects = new GameObject[childCount];
                for (int i = 1; i < childCount; i++)
                {
                    warFog.gameObjects[i] = transform.GetChild(i).gameObject;
                }
            }
            if (PhotonNetwork.player.GetTeam().ToString().ToLower().Equals("red"))
                warFog.faction = FogOfWar.Players.Player00;
            else if (PhotonNetwork.player.GetTeam().ToString().ToLower().Equals("blue"))
                warFog.faction = FogOfWar.Players.Player01;
        }
        else
        {
            if (PhotonNetwork.player.GetTeam().ToString().ToLower().Equals("red"))
                warFog.faction = FogOfWar.Players.Player00;
            else if (PhotonNetwork.player.GetTeam().ToString().ToLower().Equals("blue"))
                warFog.faction = FogOfWar.Players.Player01;
        }

    }
    
    public void stopParticle()
    {
        if(this.gameObject.name.Contains("Ignite") | this.gameObject.name.Contains("Teleport"))
        ps.Stop();
    }

}