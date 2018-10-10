using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using System.IO;

public class KTYOPTION : MonoBehaviour
{

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
    private int windowMode = 0;
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

    private void Awake()
    {
        //오디오 소스 찾기
        if (BGMAudio.Equals(null) || SFXAudio.Equals(null) || MasterAudio.Equals(null))
            print("오디오 소스가 없습니다.");
       
    }

    private void Start()
    {
        if (SaveFilePath.Equals(string.Empty))
            SaveFilePath = Path.Combine(Application.streamingAssetsPath, "Option.Json");
        else if (!File.Exists(SaveFilePath))
        {
            SaveOptionJson();
        }

        GraphicInMenu.SetActive(true);
        VolumeInMenu.SetActive(false);
        InterfaceInMenu.SetActive(false);

        LoadOptionJson();

        Button_AddListener();
        VolumeAdjust();
        VideoAdjust();
    }

    private void Update()
    {
        //텍스트 업데이트
        MasterVolumeText.text = (MasterVolume.value * 100).ToString("N0");
        SFXVolumeText.text = (SFXVolume.value * 100).ToString("N0");
        BGMVolumeText.text = (BGMVolume.value * 100).ToString("N0");
    }

    public void Button_AddListener()
    {
        //옵션화면 전환
        Graphic_BTN.GetComponent<Button>().onClick.AddListener(() => OptionScreen());
        Volume_BTN.GetComponent<Button>().onClick.AddListener(() => OptionSound());
        Interface_BTN.GetComponent<Button>().onClick.AddListener(() => OptionInterface());
        //상시 노출 버튼
        ExitGame_Btn.GetComponent<Button>().onClick.AddListener(() => GameExit());
        Confirm_Btn.GetComponent<Button>().onClick.AddListener(() => SaveOptionJson());
        Cancel_Btn.GetComponent<Button>().onClick.AddListener(() => LoadOptionJson());
        Surrender_Btn.GetComponent<Button>().onClick.AddListener(() => Surrender());
        Close_Btn.GetComponent<Button>().onClick.AddListener(() => CloseOptionWindow());
        //볼륨뮤트
        mvMute.GetComponent<Button>().onClick.AddListener(() => VolumeMute(0));
        bgmMute.GetComponent<Button>().onClick.AddListener(() => VolumeMute(1));
        sfxMute.GetComponent<Button>().onClick.AddListener(() => VolumeMute(2));
        //비디오 슬라이더
        GraphicQuality.onValueChanged.AddListener(delegate { QualityChange(); });
    }

    //뮤트
    private void VolumeMute(int num)
    {
        //마스터 볼륨
        if (num.Equals(0))
        {
            MasterAudio.mute = !MasterAudio.mute;
        }
        else if (num.Equals(1))
        {
            BGMAudio.mute = !BGMAudio.mute;
        }
        else if (num.Equals(2))
        {
            SFXAudio.mute = !SFXAudio.mute;
        }
    }


    //볼륨 슬라이더
    private void VolumeAdjust()
    {
        MasterVolume.value = MasterAudio.volume;
        SFXVolume.value = SFXAudio.volume;
        BGMVolume.value = BGMAudio.volume;

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
        DftVolumeButton.onClick.AddListener(() =>
        {
            MasterAudio.volume = 1.0f;
            SFXAudio.volume = 1.0f;
            BGMAudio.volume = 1.0f;

            SFXVolume.value = 1.0f;
            MasterVolume.value = 1.0f;
            BGMVolume.value = 1.0f;
        });
    }

    private void QualityChange()
    {   
        if (GraphicQuality.value == 5)
        {
            currentGQ = 5;
            QulityViewText.text = "품질 우선";
        }
        else if (GraphicQuality.value == 4)
        {
            currentGQ = 4;
            QulityViewText.text = "매우 높은 품질";
        }
        else if (GraphicQuality.value == 3)
        {
            currentGQ = 3;
            QulityViewText.text = "높은 품질";
        }
        else if (GraphicQuality.value == 2)
        {
            currentGQ = 2;
            QulityViewText.text = "중간";
        }
        else if (GraphicQuality.value == 1)
        {
            currentGQ = 1;
            QulityViewText.text = "높은 성능";
        }
        else if (GraphicQuality.value == 0)
        {
            currentGQ = 0;
            QulityViewText.text = "성능 우선";
        }
        QualitySettings.SetQualityLevel(currentGQ);
    }

    private void VideoAdjust()
    {
        //껐다 켰을때 그래픽 슬라이더 위치 변경
        GraphicQuality.value = currentGQ;

        //해상도 드롭다운
        dropResolution.onValueChanged.AddListener((int value) =>
        {
            resolutionNum = value;
        });
        dropWindow.onValueChanged.AddListener((int value) =>
        {
            windowMode = value;
        });

        //드롭다운 해상도 선택, 윈도우 창 모드 온 오프
        if (windowMode == 0 && resolutionNum == 0)
        {
            Screen.SetResolution(1920, 1080, false);
        }
        else if (windowMode == 1 && resolutionNum == 0)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else if (windowMode == 0 && resolutionNum == 1)
        {
            Screen.SetResolution(1280, 720, false);
        }
        else if (windowMode == 1 && resolutionNum == 1)
        {
            Screen.SetResolution(1280, 720, true);
        }
        else if (windowMode == 0 && resolutionNum == 2)
        {
            Screen.SetResolution(800, 600, false);
        }
        else if (windowMode == 1 && resolutionNum == 2)
        {
            Screen.SetResolution(800, 600, true);
        }

        //그래픽 기본값으로 변경
        Button DftGraphicButton = DefaultGraphicButton.GetComponent<Button>();
        DftGraphicButton.onClick.AddListener(() =>
        {
            resolutionNum = 0;
            windowMode = 0;
            currentGQ = 5;
            QulityViewText.text = "매우높음";
        });

        //확인 버튼 눌렀을 때 슬라이더 값에 따라 Json에 그래픽 퀄 값 저장
        Button ConfirmBtn = Confirm_Btn.GetComponent<Button>();
        ConfirmBtn.onClick.AddListener(() =>
        {
            if (GraphicQuality.value == 5)
                currentGQ = 5;
            else if (GraphicQuality.value == 4)
                currentGQ = 4;
            else if (GraphicQuality.value == 3)
                currentGQ = 3;
            else if (GraphicQuality.value == 2)
                currentGQ = 2;
            else if (GraphicQuality.value == 1)
                currentGQ = 1;
            else if (GraphicQuality.value == 0)
                currentGQ = 0;
            SaveOptionJson();
            CloseOptionWindow();
        });
    }

    private void SaveOptionJson()
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
        root.Add("Option", option);
        File.WriteAllText(SaveFilePath, root.ToString());
    }

    private void LoadOptionJson()
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
            currentGQ = int.Parse(strQuality);
            resolutionNum = int.Parse(strResolution);
            windowMode = int.Parse(strWindowMode);
            MasterAudio.volume = float.Parse(strMasterVolume);
            SFXAudio.volume = float.Parse(strSFXVolume);
            BGMAudio.volume = float.Parse(strBGMVolume);
            GraphicQuality.value = currentGQ;
            MasterVolume.value = MasterAudio.volume;
            SFXVolume.value = SFXAudio.volume;
            BGMVolume.value = BGMAudio.volume;
        }
        else
        {
            print("파일 경로 찾을수 없음");
        }
    }

    public void CloseOptionWindow()
    {
        this.gameObject.SetActive(false);
    }
    public void OptionScreen()
    {
        GraphicInMenu.SetActive(true);
        VolumeInMenu.SetActive(false);
        InterfaceInMenu.SetActive(false);
        VideoAdjust();
    }
    public void OptionSound()
    {
        GraphicInMenu.SetActive(false);
        VolumeInMenu.SetActive(true);
        InterfaceInMenu.SetActive(false);
        VolumeAdjust();
    }
    public void OptionInterface()
    {
        GraphicInMenu.SetActive(false);
        VolumeInMenu.SetActive(false);
        InterfaceInMenu.SetActive(true);
        VideoAdjust();
    }

    public void Surrender()
    {

    }

    public void GameExit()
    {

    }
}
