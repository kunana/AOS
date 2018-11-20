using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

public class KTYOPTION : MonoBehaviour
{
    #region Declare
    [Header("Json 파일경로")]
    private string SaveFilePath = string.Empty;

    [Header("비디오,볼륨,인터페이스 게임 오브젝트")]
    public GameObject GraphicInMenu = null;
    public GameObject VolumeInMenu = null;
    public GameObject InterfaceInMenu = null;

    [Header("비디오,볼륨,인터페이스 전환버튼")]
    public GameObject Interface_BTN = null;
    public GameObject Graphic_BTN = null;
    public GameObject Volume_BTN = null;

    [Header("게임종료, 항복, 확인, 취소 버튼")]
    public GameObject Confirm_Btn = null;
    public GameObject ExitGame_Btn = null;
    public GameObject Surrender_Btn = null;
    public GameObject Cancel_Btn = null;
    public GameObject Close_Btn = null;

    [Header("비디오 옵션 설정 버튼")]
    public Slider GraphicQuality = null;
    public Dropdown dropResolution = null;
    public Dropdown dropWindow = null;
    public GameObject DefaultGraphicButton = null;
    public Text QulityViewText = null;

    private int resolutionNum = 0;
    private int windowMode = 1;
    private int currentGQ = 5;


    [Header("볼륨 설정 버튼")]
    public Slider MasterVolume;
    public Slider SFXVolume;
    public Slider BGMVolume;
    public GameObject volumeDefaultButton;
    public Text MasterVolumeText;
    public Text SFXVolumeText;
    public Text BGMVolumeText;
    public GameObject mvMute = null;
    public GameObject sfxMute = null;
    public GameObject bgmMute = null;
    //오디오
    public AudioSource BGMAudio = null;
    public AudioSource SFXAudio = null;
    public AudioSource MasterAudio = null;

    public GameObject ButtonLight = null;
    #endregion

    #region Editor_Hierarchy
    private void Awake()
    {
        //오디오 소스 찾기
        if (BGMAudio.Equals(null) || SFXAudio.Equals(null) || MasterAudio.Equals(null))
            print("오디오 소스가 없습니다.");

        Button_AddListener();
        Cursor_Confined();

        // 저장된 파일주소가 없으면 주소 생성하고 기본해상도로 설정
        if (SaveFilePath.Equals(string.Empty))
        {
            SaveFilePath = Path.Combine(Application.streamingAssetsPath, "Option.Json");
        }

        // 해당 주소에 파일이 없으면 기본세팅하고 파일 저장
        if (!File.Exists(SaveFilePath))
        {
            GraphicSetting_Reset();
            VolumeSetting_Reset();
            SaveOptionJson();
        }
        // 파일이 있으면 불러옴
        else
        {
            LoadOptionJson();
        }
    }

    public void Cursor_Confined()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void Button_AddListener()
    {
        //옵션화면 전환
        Graphic_BTN.GetComponent<Button>().onClick.AddListener(() => OptionGraphic());
        Volume_BTN.GetComponent<Button>().onClick.AddListener(() => OptionVolume());
        Interface_BTN.GetComponent<Button>().onClick.AddListener(() => OptionInterface());

        //상시 노출 버튼
        ExitGame_Btn.GetComponent<Button>().onClick.AddListener(() => GameExit());
        Surrender_Btn.GetComponent<Button>().onClick.AddListener(() => Surrender());
        Confirm_Btn.GetComponent<Button>().onClick.AddListener(() => OK_Button());
        Cancel_Btn.GetComponent<Button>().onClick.AddListener(() => Cancel_Button());
        Close_Btn.GetComponent<Button>().onClick.AddListener(() => CloseOptionWindow());

        //볼륨뮤트
        mvMute.GetComponent<Button>().onClick.AddListener(() => VolumeMute(0));
        bgmMute.GetComponent<Button>().onClick.AddListener(() => VolumeMute(1));
        sfxMute.GetComponent<Button>().onClick.AddListener(() => VolumeMute(2));

        //비디오 슬라이더
        GraphicQuality.onValueChanged.AddListener(delegate { QualityChange(); });

        //해상도 드롭다운
        dropResolution.onValueChanged.AddListener((int value) =>
        {
            resolutionNum = value;
        });
        dropWindow.onValueChanged.AddListener((int value) =>
        {
            windowMode = value;
        });

        // 볼륨 슬라이더
        MasterVolume.onValueChanged.AddListener((float value) =>
        {
            MasterAudio.volume = value;
        });
        SFXVolume.onValueChanged.AddListener((float value) =>
        {
            SFXAudio.volume = value;
        });
        BGMVolume.onValueChanged.AddListener((float value) =>
        {
            BGMAudio.volume = value;
        });

        //소리 기본값으로 변경
        Button DftVolumeButton = volumeDefaultButton.GetComponent<Button>();
        DftVolumeButton.onClick.AddListener(() => VolumeSetting_Reset());
    }

    private void Start()
    {
        LoadOptionJson();

        // 기본으로 그래픽 설정 열어줌
        GraphicInMenu.SetActive(true);
        VolumeInMenu.SetActive(false);
        InterfaceInMenu.SetActive(false);
    }

    private void Update()
    {
        //텍스트 업데이트
        MasterVolumeText.text = (MasterVolume.value * 100).ToString("N0");
        SFXVolumeText.text = (SFXVolume.value * 100).ToString("N0");
        BGMVolumeText.text = (BGMVolume.value * 100).ToString("N0");
    }

    // 창닫기
    public void CloseOptionWindow()
    {
        // 저장된거 불러와서 원래대로
        if (File.Exists(SaveFilePath))
            LoadOptionJson();
        this.gameObject.SetActive(false);
        SoundManager.instance.PlaySound(SoundManager.instance.UI_Close);
    }

    // 그래픽옵션만 보이기
    public void OptionGraphic()
    {
        GraphicInMenu.SetActive(true);
        VolumeInMenu.SetActive(false);
        InterfaceInMenu.SetActive(false);
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }

    // 음향옵션만 보이기
    public void OptionVolume()
    {
        GraphicInMenu.SetActive(false);
        VolumeInMenu.SetActive(true);
        InterfaceInMenu.SetActive(false);
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }

    // 인터페이스옵션만 보이기
    public void OptionInterface()
    {
        GraphicInMenu.SetActive(false);
        VolumeInMenu.SetActive(false);
        InterfaceInMenu.SetActive(true);
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }

    private void OK_Button()
    {
        GraphicAdjust();
        VolumeAdjust();
        SaveOptionJson();
        SoundManager.instance.PlaySound(SoundManager.instance.UI_Close);
        this.gameObject.SetActive(false);
    }

    private void Cancel_Button()
    {
        CloseOptionWindow();
    }

    public void Surrender()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.Button_Click);
    }

    public void GameExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    #endregion

    #region Graphic

    // 기본값 복원 버튼  -  그래픽설정 초기화함
    public void GraphicSetting_Reset()
    {
        resolutionNum = 0;
        dropResolution.value = resolutionNum;

        windowMode = 1;
        dropWindow.value = windowMode;

        currentGQ = 5;
        GraphicQuality.value = 5;
        QulityViewText.text = "매우 높음";

        GraphicAdjust();
    }

    // 그래픽 퀄리티 변경시
    private void QualityChange()
    {
        if (GraphicQuality.value == 5)
        {
            currentGQ = 5;
            QulityViewText.text = "매우 높음";
        }
        else if (GraphicQuality.value == 4)
        {
            currentGQ = 4;
            QulityViewText.text = "다소 높음";
        }
        else if (GraphicQuality.value == 3)
        {
            currentGQ = 3;
            QulityViewText.text = "높음";
        }
        else if (GraphicQuality.value == 2)
        {
            currentGQ = 2;
            QulityViewText.text = "중간";
        }
        else if (GraphicQuality.value == 1)
        {
            currentGQ = 1;
            QulityViewText.text = "낮음";
        }
        else if (GraphicQuality.value == 0)
        {
            currentGQ = 0;
            QulityViewText.text = "매우 낮음";
        }
        QualitySettings.SetQualityLevel(currentGQ);
        Cursor.lockState = CursorLockMode.None;
        Invoke("Cursor_Confined", 0.05f);
    }

    // 그래픽 설정 싱크
    private void GraphicAdjust()
    {
        //드롭다운 해상도 선택, 윈도우 창 모드 온 오프
        bool fullwindow = true;
        if (windowMode == 0)
            fullwindow = false;
        else
            fullwindow = true;

        int resolution_width = 1920;
        int resolution_height = 1080;
        switch (resolutionNum)
        {
            case 0:
                resolution_width = 1920;
                resolution_height = 1080;
                break;
            case 1:
                resolution_width = 1280;
                resolution_height = 720;
                break;
            case 2:
                resolution_width = 960;
                resolution_height = 540;
                break;
            default:
                resolution_width = 1920;
                resolution_height = 1080;
                break;
        }
        Screen.SetResolution(resolution_width, resolution_height, fullwindow);

        QualityChange();
    }
    #endregion

    #region Volume
    public void VolumeSetting_Reset()
    {
        MasterAudio.volume = 1.0f;
        MasterVolume.value = 1.0f;
        MasterAudio.mute = false;
        MuteIconChange(mvMute, MasterAudio.mute);

        SFXAudio.volume = 1.0f;
        SFXVolume.value = 1.0f;
        SFXAudio.mute = false;
        MuteIconChange(sfxMute, SFXAudio.mute);

        BGMAudio.volume = 1.0f;
        BGMVolume.value = 1.0f;
        BGMAudio.mute = false;
        MuteIconChange(bgmMute, BGMAudio.mute);
    }

    //뮤트
    private void VolumeMute(int num)
    {
        switch (num)
        {
            case 0:
                MasterAudio.mute = !MasterAudio.mute;
                MuteIconChange(mvMute, MasterAudio.mute);
                break;
            case 1:
                BGMAudio.mute = !BGMAudio.mute;
                MuteIconChange(bgmMute, BGMAudio.mute);
                break;
            case 2:
                SFXAudio.mute = !SFXAudio.mute;
                MuteIconChange(sfxMute, SFXAudio.mute);
                break;
            default:
                break;
        }
    }

    //뮤트 색상, 아이콘변경
    private void MuteIconChange(GameObject MuteObject, bool condition)
    {
        if(!condition)
        {
            MuteObject.GetComponent<Image>().color = new Color(25f / 255f, 136f / 255f, 255f / 255f);
            MuteObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/volume-on");
        }
        else
        {
            MuteObject.GetComponent<Image>().color = new Color(255f / 255f, 78f / 255f, 78f / 255f);
            MuteObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/volume-off");
        }
    }

    //볼륨 슬라이더
    private void VolumeAdjust()
    {
        MasterVolume.value = MasterAudio.volume;
        SFXVolume.value = SFXAudio.volume;
        BGMVolume.value = BGMAudio.volume;
    }
    #endregion

    #region Json
    public void SaveOptionJson()
    {
        JObject root = new JObject();
        JObject option = new JObject();

        //option.Add("Quality", QualitySettings.GetQualityLevel().ToString());
        option.Add("Quality", currentGQ);
        option.Add("Resolution", resolutionNum);
        option.Add("WindowMode", windowMode);

        option.Add("MasterVolume", MasterAudio.volume);
        option.Add("SFXVolume", SFXAudio.volume);
        option.Add("BGMVolume", BGMAudio.volume);

        option.Add("MasterMute", MasterAudio.mute);
        option.Add("SFXMute", SFXAudio.mute);
        option.Add("BGMMute", BGMAudio.mute);
        root.Add("Option", option);
        File.WriteAllText(SaveFilePath, root.ToString());
    }

    public void LoadOptionJson()
    {
        try
        {
            if (File.Exists(SaveFilePath))
            {
                StreamReader read = File.OpenText(SaveFilePath);
                string text = read.ReadToEnd();
                JObject root = JObject.Parse(text);
                JObject option = root["Option"] as JObject;

                string strQuality = option["Quality"].ToString();
                string strResolution = option["Resolution"].ToString();
                string strWindowMode = option["WindowMode"].ToString();

                string strMasterVolume = option["MasterVolume"].ToString();
                string strSFXVolume = option["SFXVolume"].ToString();
                string strBGMVolume = option["BGMVolume"].ToString();

                string strMasterMute = option["MasterMute"].ToString();
                string strSFXMute = option["SFXMute"].ToString();
                string strBGMMute = option["BGMMute"].ToString();

                // 그래픽
                currentGQ = int.Parse(strQuality);
                GraphicQuality.value = currentGQ;

                resolutionNum = int.Parse(strResolution);
                dropResolution.value = resolutionNum;

                windowMode = int.Parse(strWindowMode);
                dropWindow.value = windowMode;
                GraphicAdjust();

                // 음향
                MasterAudio.volume = float.Parse(strMasterVolume);
                MasterVolume.value = MasterAudio.volume;

                SFXAudio.volume = float.Parse(strSFXVolume);
                SFXVolume.value = SFXAudio.volume;

                BGMAudio.volume = float.Parse(strBGMVolume);
                BGMVolume.value = BGMAudio.volume;

                MasterAudio.mute = bool.Parse(strMasterMute);
                MuteIconChange(mvMute, MasterAudio.mute);
                SFXAudio.mute = bool.Parse(strSFXMute);
                MuteIconChange(sfxMute, SFXAudio.mute);
                BGMAudio.mute = bool.Parse(strBGMMute);
                MuteIconChange(bgmMute, BGMAudio.mute);

                read.Close();
            }
            else
            {
                //print("파일 경로 찾을수 없음");
            }
        }
        catch(Exception e)
        {
            print(e.Message);
        }
    }
    #endregion
}
