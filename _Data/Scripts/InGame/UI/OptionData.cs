using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System;

public class OptionData {
    private OptionData() { }
    private static OptionData _optionData = null;
    public static OptionData optionData
    {
        get
        {
            if(_optionData == null)
            {
                _optionData = new OptionData();
            }
            return _optionData;
        }
    }

    public int graphicQuality = 5;
    public float BgmVolume = 1.0f;
    public bool BgmOn = true;    


    
    //void Awake()
    //{
    //    //게임 실행 중 화면이 꺼지지 않게 함
    //    Screen.sleepTimeout = SleepTimeout.NeverSleep;
    //    //기본 게임 화면 해상도
    //    Screen.SetResolution(1920, 1080, true);
    //}


	// Use this for initialization
	void Start () {
        save();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void save()
    {
        string saveFilePath = Application.dataPath;
        string savePath = Path.Combine(saveFilePath, "OptionData.json");
        JObject root = new JObject();
        JObject optionData = new JObject();
        



    }
}
