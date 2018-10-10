using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleUnit3D : MonoBehaviour
{
	public FogOfWar.Players faction = 0;
	public float visionRange = 2f;

	private void Start ()
	{
		Revealer3D revealer = new Revealer3D(visionRange, faction, this.transform);
		FogOfWar3D.RegisterRevealer(revealer);
	}

	private void OnDestroy()
	{
		FogOfWar3D.UnregisterRevealer(transform.GetInstanceID());
	}

	public void UpdateVisionRange()
	{
		FogOfWar3D.UpdateVisionRange(transform.GetInstanceID(), visionRange);
	}
}
