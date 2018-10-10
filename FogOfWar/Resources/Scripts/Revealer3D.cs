using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Revealer3D
{
	public Transform sceneReference;
	public float visionRange = 1f;
	public FogOfWar.Players faction = FogOfWar.Players.Player00;

	public Revealer3D(float visionRange, FogOfWar.Players faction, Transform reference)
	{
		this.sceneReference = reference;
		this.visionRange = visionRange;
		this.faction = faction;
	}
}
