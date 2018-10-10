using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Faction3D
{
	public string name = "";
	public int id = 0;
	public FogOfWar.Players revealFactions;

	public Faction3D(string name, int id, FogOfWar.Players revealFactions)
	{
		this.name = name;
		this.id = id;
		this.revealFactions = revealFactions;
	}
	/*
	public void RegisterRevealer(Transform scenereference, float visionRange, FogOfWar.Players faction)
	{
		//Debug.Log(scenereference.GetInstanceID());
		Revealer3D revealer = new Revealer3D(visionRange, faction, scenereference);
		revealers.Add(revealer);
	}

	public void UnregisterRevealer(Revealer3D revealer)
	{
		int num = revealers.Count;
		for (int i = 0; i < num; i++)
		{
			if(revealers[i].sceneReference.GetInstanceID() == revealer.sceneReference.GetInstanceID())
			{
				revealers.RemoveAt(i);
			}
		}
	}
	*/
}
