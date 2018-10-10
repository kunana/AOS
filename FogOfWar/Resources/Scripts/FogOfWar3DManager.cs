using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FogOfWar3DManager : MonoBehaviour
{
#if UNITY_EDITOR
	public bool drawRevealers = false;
#endif

	public List<Faction3D> factions = new List<Faction3D>();
	public List<Revealer3D> revealers = new List<Revealer3D>();

	public int factionCount = 1;
	public int maxRevealers = 16;

	public FogOfWar.Players currentlyRevealing = FogOfWar.Players.Player00;
	
	public Color coveredColor = new Color(.2f,.2f,.2f,1f);

	private Vector4[] positionRange;

	public FogOfWar.FogEffect fogEffect = FogOfWar.FogEffect.None;
	public float animatedFogSpeed;
	public float animatedFogIntensity;
	public float animatedFogTiling;
	public Texture2D fogNoise;

	public bool manageVisibility = false;
	public bool useThreads = false;

	private Vector3 pos;
	public Shader sh;

	private void OnEnable()
	{
		if (FogOfWar3D.fogManager == null)
		{
			Debug.Log("Initialized fog of war manager");
			FogOfWar3D.RegisterFogOfWarManager(this);
		}

		if (factions.Count < 1)
		{
			AddFaction();
		}

		positionRange = new Vector4[maxRevealers];

		FogOfWar.fogAlignment = FogOfWar.FogAlignment.DDDMode;

		ClearArea();
		SetUpShaderKeywords();
	}

	private void OnDisable()
	{
		ClearArea();
		Shader.SetGlobalColor("_CoveredColor", Color.white);
	}

	private void OnGUI()
	{
		//GUI.Label(new Rect(Screen.width / 2f, 0f, 50f, 50f), (1f / Time.deltaTime).ToString("f2"));
		GUI.Label(new Rect(Screen.width / 2f, 0f, 50f, 50f), (sh.isSupported).ToString());
	}

	private void Update()
	{
		int num = revealers.Count;
		int idx = 0;

		for (int i = 0; i < num; i++)
		{
			if (idx < maxRevealers)
			{ 
				if (revealers[i].faction == currentlyRevealing)
				{
					pos = revealers[i].sceneReference.position;
					positionRange[idx] = new Vector4(pos.x, pos.y, pos.z, revealers[i].visionRange);
					Shader.SetGlobalVector("Revealer" + idx.ToString(), positionRange[idx]);
					idx++;
				}
			}
		}

		for (int i = idx; i < maxRevealers; i++)
		{
			Shader.SetGlobalVector("Revealer" + idx.ToString(), Vector4.zero);
			positionRange[i] = Vector4.zero;
		}
		
		//Shader.SetGlobalVectorArray("_Revealers", positionRange);
	}

	public void ClearArea()
	{
		for (int i = 0; i < maxRevealers; i++)
		{
			positionRange[i] = new Vector4(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity, 0f);
			Shader.SetGlobalVector("Revealer" + i.ToString(), positionRange[i]);
		}
	}

	public void AddFaction()
	{
		Faction3D newFaction = new Faction3D("Faction " + factions.Count.ToString(), factions.Count, (FogOfWar.Players) Mathf.Pow(2, factions.Count));
		factions.Add(newFaction);
	}

	public void RemoveFaction()
	{
		if (factions.Count >= 2)
		{
			factions.RemoveAt(factions.Count - 1);
		}
		else {
			Debug.LogWarning("Cant remove last faction in list!");
		}
	}

	public void RemoveFaction(int index)
	{
		if (factions.Count > index)
		{
			factions.RemoveAt(index);
		}
	}

	public void RevealFaction(FogOfWar.Players faction)
	{
		currentlyRevealing = faction;
		FogOfWar3D.currentlyRevealed = faction;
	}

	public void RevealFaction(int faction)
	{
		FogOfWar.Players f = (FogOfWar.Players)Mathf.Pow(2, faction);
		currentlyRevealing = f;
		FogOfWar3D.currentlyRevealed = f;
	}

	public bool IsRevealingFaction(FogOfWar.Players faction)
	{
		if (currentlyRevealing == faction)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void UpdateMaxRevealers(int num)
	{
		maxRevealers = num;
		positionRange = new Vector4[maxRevealers];
	}

	public void UpdateMaxRevealers()
	{
		positionRange = new Vector4[maxRevealers];
	}

	public FogOfWar.Players GetCurrentlyRevealedFaction()
	{
		return currentlyRevealing;
	}

	public void RegisterRevealer(Revealer3D revealer)
	{
		revealers.Add(revealer);
	}

	public void UnregisterRevealer(int ID)
	{
		int count = revealers.Count;

		for (int i = 0; i < count; i++)
		{
			if (revealers[i].sceneReference.GetInstanceID() == ID)
			{
				revealers.RemoveAt(i);
				return;
			}
		}

		Debug.LogWarning("Could not unregister revealer!");
	}

	public void UpdateVisionRange(int ID, float visionRange)
	{
		int count = revealers.Count;

		for (int i = 0; i < count; i++)
		{
			if (revealers[i].sceneReference.GetInstanceID() == ID)
			{
				revealers[i].visionRange = visionRange;
				return;
			}
		}

		Debug.LogWarning("Could not update vision range!");
	}

	public void SetUpShaderKeywords()
	{
		Shader.DisableKeyword("HorizontalMode");
		Shader.DisableKeyword("VerticalMode");
		Shader.EnableKeyword("DDDMode");
		Shader.SetGlobalColor("_CoveredColor", coveredColor);
		Shader.SetGlobalTexture("FogNoise", fogNoise);

		switch (fogEffect)
		{
			case FogOfWar.FogEffect.None:
				Shader.DisableKeyword("FoWColor");
				Shader.DisableKeyword("FoWAnimatedFog");
				break;

			case FogOfWar.FogEffect.Color:
				Shader.DisableKeyword("FoWAnimatedFog");
				Shader.EnableKeyword("FoWColor");
				Shader.SetGlobalColor("FogColor", coveredColor);
				break;

			case FogOfWar.FogEffect.AnimatedFog:
				Shader.DisableKeyword("FoWColor");
				Shader.EnableKeyword("FoWAnimatedFog");
				Shader.SetGlobalTexture("FogNoise", fogNoise as Texture2D);
				Shader.SetGlobalColor("FogColor", coveredColor);
				Shader.SetGlobalFloat("FogSpeed", animatedFogSpeed);
				Shader.SetGlobalFloat("FogTiling", animatedFogTiling);
				Shader.SetGlobalFloat("FogIntensity", animatedFogIntensity);
				break;
		}
	}
}
