using UnityEngine;
using System.Collections;

[System.Serializable]
public class Revealer
{
    public float visionRange = 6f;
    public FogOfWar.Players faction = FogOfWar.Players.Player00;
    public int upVision = 0;
    public GameObject sceneReference;

    public Revealer(float visionRange, FogOfWar.Players faction, int upVision, GameObject sceneReference)
    {
        this.visionRange = visionRange;
		this.faction = faction;
		this.upVision = upVision;
		this.sceneReference = sceneReference;
    }
}