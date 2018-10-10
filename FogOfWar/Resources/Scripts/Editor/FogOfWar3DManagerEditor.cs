using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

[ExecuteInEditMode]
[CustomEditor(typeof(FogOfWar3DManager))]   
public class FogOfWar3DManagerEditor : Editor
{
	SerializedProperty drawRevealers;
	SerializedProperty maxRevealers;
	SerializedProperty coveredColor;
	SerializedProperty animatedFogSpeed;
	SerializedProperty animatedFogIntensity;
	SerializedProperty animatedFogTiling;
	SerializedProperty fogNoise;

	private void OnEnable()
	{
		drawRevealers = serializedObject.FindProperty("drawRevealers");
		maxRevealers = serializedObject.FindProperty("maxRevealers");
		coveredColor = serializedObject.FindProperty("coveredColor");
		fogNoise = serializedObject.FindProperty("fogNoise");
		animatedFogSpeed = serializedObject.FindProperty("animatedFogSpeed");
		animatedFogIntensity = serializedObject.FindProperty("animatedFogIntensity");
		animatedFogTiling = serializedObject.FindProperty("animatedFogTiling");
		maxRevealers.intValue = LoadNumRevealers();
		serializedObject.ApplyModifiedProperties();
	}

	public override void OnInspectorGUI()
	{
		var fogOfWarManager3D = (FogOfWar3DManager)target;
		serializedObject.Update();

		EditorGUILayout.LabelField("Fog of War 3D Manager", EditorStyles.toolbarButton);
		
		EditorGUILayout.BeginVertical("Box");
		{
			maxRevealers.intValue = Mathf.Clamp(EditorGUILayout.IntField(new GUIContent("Max Revealers", "The max amount of revealers per faction"), maxRevealers.intValue), 16, 1023);

			if (GUILayout.Button(new GUIContent("Update", "This triggers recompilation of UFoW shaders and can take several minutes to complete."), EditorStyles.toolbarButton))
			{
				fogOfWarManager3D.UpdateMaxRevealers();
				UpdateMaxRevealers();
			}

			EditorGUILayout.HelpBox("Updating this value changes an include file globally and all Fog of War related shaders need to be recompiled. This may take a while!", MessageType.Warning);

			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
			{
				EditorGUILayout.HelpBox("Using too many revealers will not work on low end devices. To find the correct number of simultanious revealers, first set to a low level and test on device. Keep this value as low as possible for performance!", MessageType.Warning);
			}

			EditorGUILayout.LabelField("Fog of War styles", EditorStyles.toolbarButton);
			GUILayout.Space(-2f);

			EditorGUILayout.BeginVertical("Box");
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.PrefixLabel("Fog Effect");
					fogOfWarManager3D.fogEffect = (FogOfWar.FogEffect)EditorGUILayout.EnumPopup(fogOfWarManager3D.fogEffect);
				}
				EditorGUILayout.EndHorizontal();

				if (fogOfWarManager3D.fogEffect == FogOfWar.FogEffect.None)
				{
					coveredColor.colorValue = EditorGUILayout.ColorField("Fog Color", coveredColor.colorValue);
				}

				if (fogOfWarManager3D.fogEffect == FogOfWar.FogEffect.Color)
				{
					coveredColor.colorValue = EditorGUILayout.ColorField("Fog Color", coveredColor.colorValue);
				}

				if (fogOfWarManager3D.fogEffect == FogOfWar.FogEffect.AnimatedFog)
				{
					EditorGUILayout.PrefixLabel("Fog Noise");
					fogNoise.objectReferenceValue = EditorGUILayout.ObjectField(fogNoise.objectReferenceValue, typeof(Texture2D), false) as Texture2D;
					coveredColor.colorValue = EditorGUILayout.ColorField("Fog Color", coveredColor.colorValue);
					animatedFogSpeed.floatValue = EditorGUILayout.FloatField("Speed", animatedFogSpeed.floatValue);
					animatedFogIntensity.floatValue = EditorGUILayout.FloatField("Intensity", animatedFogIntensity.floatValue);
					animatedFogTiling.floatValue = EditorGUILayout.FloatField("Tiling", animatedFogTiling.floatValue);
				}
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndVertical();

		//EditorGUILayout.LabelField("Factions", EditorStyles.toolbarButton);
		EditorGUILayout.BeginHorizontal("Toolbar");
		{
			EditorGUILayout.LabelField("Factions");
			if (GUILayout.Button("+", EditorStyles.toolbarButton, GUILayout.MaxWidth(Screen.width / 8f)))
				fogOfWarManager3D.AddFaction();

			if (GUILayout.Button("-", EditorStyles.toolbarButton, GUILayout.MaxWidth(Screen.width / 8f)))
				fogOfWarManager3D.RemoveFaction();

		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(-4f);

		EditorGUILayout.BeginVertical("Box");
		{
			EditorGUILayout.BeginHorizontal();
			{
				EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.MaxWidth(Screen.width / 10f));
				EditorGUILayout.LabelField("Name", EditorStyles.boldLabel, GUILayout.MaxWidth(Screen.width / 3f));
				EditorGUILayout.LabelField("Num", EditorStyles.boldLabel, GUILayout.MaxWidth(Screen.width / 6f));
				EditorGUILayout.LabelField("Status", EditorStyles.boldLabel, GUILayout.MaxWidth(Screen.width / 5f));
				EditorGUILayout.LabelField("", EditorStyles.boldLabel, GUILayout.MaxWidth(Screen.width / 5f));
			}
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < fogOfWarManager3D.factions.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField(i.ToString(), GUILayout.MaxWidth(Screen.width/10f));
					fogOfWarManager3D.factions[i].name = EditorGUILayout.TextField(fogOfWarManager3D.factions[i].name, GUILayout.MaxWidth(Screen.width / 3f));

					EditorGUILayout.LabelField(fogOfWarManager3D.revealers.Count.ToString(), GUILayout.MaxWidth(Screen.width / 6f));

					GUILayout.FlexibleSpace();

					if (fogOfWarManager3D.IsRevealingFaction(fogOfWarManager3D.factions[i].revealFactions))
					{
						GUI.color = Color.green;
						if (GUILayout.Button("Revealing", EditorStyles.toolbarButton, GUILayout.MaxWidth(Screen.width / 5f)))
						{
						}	
						GUI.color = Color.white;
					}
					else {
						if (GUILayout.Button("Reveal", EditorStyles.toolbarButton, GUILayout.MaxWidth(Screen.width / 5f)))
						{
							fogOfWarManager3D.RevealFaction(fogOfWarManager3D.factions[i].revealFactions);
						}
					}

					GUI.color = Color.red;

					if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.MaxWidth(Screen.width / 15f)))
						fogOfWarManager3D.RemoveFaction(i);

					GUI.color = Color.white;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.LabelField("Debug", EditorStyles.toolbarButton);
		GUILayout.Space(-3f);
		EditorGUILayout.BeginVertical("Box");
		{
			
			drawRevealers.boolValue = EditorGUILayout.Toggle("Draw ranges", drawRevealers.boolValue);
		}
		EditorGUILayout.EndVertical();

		serializedObject.ApplyModifiedProperties();
	}

	private void OnSceneGUI()
	{
		var fogOfWarManager3D = (FogOfWar3DManager)target;

		if (fogOfWarManager3D.drawRevealers)
		{
			FogOfWar.Players currentFaction = fogOfWarManager3D.GetCurrentlyRevealedFaction();
			
			for (int i = 0; i < fogOfWarManager3D.revealers.Count; i++)
			{
				if (currentFaction == fogOfWarManager3D.revealers[i].faction)
				{
					Handles.color = Color.green;
					Handles.DrawWireDisc(fogOfWarManager3D.revealers[i].sceneReference.position, Vector3.up, fogOfWarManager3D.revealers[i].visionRange);
					Handles.color = Color.red;
					Handles.DrawWireDisc(fogOfWarManager3D.revealers[i].sceneReference.position, Vector3.left, fogOfWarManager3D.revealers[i].visionRange);
					Handles.color = Color.blue;
					Handles.DrawWireDisc(fogOfWarManager3D.revealers[i].sceneReference.position, Vector3.forward, fogOfWarManager3D.revealers[i].visionRange);
				}
				
				//Handles.SphereHandleCap(0, currentFaction.revealers[i].position, Quaternion.identity, currentFaction.revealers[i].visionRange, EventType.Repaint);
				//Handles.color = Color.white;
			}
		}
	}

	private void UpdateMaxRevealers()
	{
		string blankPath = Application.dataPath + "/FogOfWar/Resources/Shaders/FogOfWar/FogOfWarMath.blank";
		string path = Application.dataPath + "/FogOfWar/Resources/Shaders/FogOfWar/FogOfWarMath.cginc";
		string data = "";

		if (File.Exists(blankPath))
		{
			data = File.ReadAllText(blankPath);
			data = UpdateRevealersInFile(data, maxRevealers.intValue);

			if (File.Exists(path))
			{
				File.WriteAllText(path, data);

				PlayerPrefs.SetInt("UFoWMaxRevealers", maxRevealers.intValue);
				PlayerPrefs.Save();
			}
				
		} else {
			blankPath = EditorUtility.OpenFilePanel(
				"Please specify FogOfWarMath.cginc",
				"",
				"cginc");
		}
		/*
		if (File.Exists(path))
		{
			path = File.ReadAllText(path);
		}
		else {
			Debug.LogError("Could not find FogOfWarMath.cginc");
		}
		*/
	}

	public int LoadNumRevealers()
	{
		int num = PlayerPrefs.GetInt("UFoWMaxRevealers");
		if (num == 0)
		{
			num = 32;
			PlayerPrefs.SetInt("UFoWMaxRevealers", num);
			PlayerPrefs.Save();
		}

		return num;
	}

	public string UpdateRevealersInFile(string file, int num)
	{
		string constants = "";
		string calculations = "";
		for (int i = 0; i < num; i++)
		{
			constants += "float4 Revealer"+i.ToString()+";"+'\u000A' + '\u0009';
			calculations += "col.rgb += clamp((1 - (distance(Revealer"+i.ToString()+ ".xyz, WorldPos) / Revealer"+i.ToString()+".w)) * 5, 0, 1);" + '\u000A' + '\u0009' + '\u0009';
		}

		file = file.Replace(">>INSERT REVEALERS<<", constants);
		file = file.Replace(">>CALCULATE FOG<<", calculations);
		return file;
	}
	
	public int ParseMaxRevealers(string file)
	{
		int len = file.Length;
		for (int i = 0; i < len - 13; i++)
		{
			if (file[i] == 'R' && file[i + 1] == 'E' && file[i + 2] == 'V' && file[i + 3] == 'E' && file[i + 4] == 'A' && file[i + 5] == 'L' && file[i + 6] == 'E' && file[i + 7] == 'R' && file[i + 8] == '_' && file[i + 9] == 'C' && file[i + 10] == 'O' && file[i + 11] == 'U' && file[i + 12] == 'N' && file[i + 13] == 'T')
			{
				int idx = i;
				while (true)
				{
					if (file[idx] == ' ')
					{
						int idx2 = idx+1;
						string number = "";

						while (true)
						{
							if (file[idx2] != ' ' && file[idx2] != '\n')
							{
								number += file[idx2];
							} else {
								return int.Parse(number);
							}
							
							idx2++;
						}
					}
					idx++;
				}
			}
		}

		return 128;//Return default if error
	}
	
}
