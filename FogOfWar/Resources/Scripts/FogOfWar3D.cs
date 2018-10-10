using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class FogOfWar3D
{
	public static FogOfWar3DManager fogManager;
	public static FogOfWar.Players currentlyRevealed = FogOfWar.Players.Player00;

	public delegate void FactionChange();
	public static event FactionChange OnFactionChange;

	public static void RegisterFogOfWarManager(FogOfWar3DManager manager)
	{
		fogManager = manager;
	}

	public static void RegisterRevealer(Revealer3D revealer)
	{
		fogManager.RegisterRevealer(revealer);
	}

	public static void UnregisterRevealer(int ID)
	{
		fogManager.UnregisterRevealer(ID);
	}

	public static void UpdateVisionRange(int ID, float visionRange)
	{
		fogManager.UpdateVisionRange(ID, visionRange);
	}
}
