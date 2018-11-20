using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class AOSMouseCursor : Photon.MonoBehaviour
{
    //[Space(10),Header("마우스 커서")]
    public Texture2D[] MouseTexture = null;  //0 == Default 1 == Ally 2 == Enemy 3 = ForceAtk 4 = Ping 5 = Shop
    public CursorMode mode = CursorMode.ForceSoftware;
    public int PreCursor = -1;
    public int CurrentCursor = -2;
    public bool setCenter;
    private bool forceAtk = false;

    MouseFxPooling fxPool;
    public Transform Target;
    private ChatFunction chatfunction;

    void OnLevelWasLoaded(int level)
    {
        if (!SceneManager.GetSceneByBuildIndex(level).name.Contains("InGame"))
            SetCursor(0, Vector2.zero);
        else
        {
            fxPool = GameObject.FindGameObjectWithTag("MouseFxPool").GetComponent<MouseFxPooling>();
            Target = GameObject.FindGameObjectWithTag("PlayerA*Target").transform;
            if (Target == null)
            {
                Target = GameObject.FindGameObjectWithTag("PlayerA*Target").transform;
            }
            chatfunction = GameObject.FindGameObjectWithTag("ChatManager").GetComponentInChildren<ChatFunction>();
        }
    }

    public void Start()
    {
        SetCursor(PreCursor, Vector2.zero);
        //DontDestroyOnLoad(this.gameObject);
        //포톤 사용시 주석해제
        //if (base.photonView.isMine && PhotonNetwork.player.IsLocal)
        //    localPlayer = PhotonNetwork.player;
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name.Equals("InGame") && EventSystem.current.IsPointerOverGameObject() == false) // 인게임에서 A키를 사용한 강제어택
        {
            if (chatfunction.chatInput.IsActive())
                return;
            //마우스 커서-------------------------------------------------------------------------------
            if (Input.GetKeyDown(KeyCode.A))
            {
                if(!forceAtk)
                {
                    forceAtk = true;
                    SetCursor(3, Vector2.zero);
                }
                else
                {
                    forceAtk = false;
                    SetCursor(PreCursor, Vector2.zero);
                }
            }
            else if (Input.GetMouseButtonDown(0) && forceAtk)
            {
                SetCursor(0, Vector2.zero);
                forceAtk = false;
                fxPool.GetPool("Force", Target.position);
            }
        }
    }

    /// <summary>
    /// 0 == Default 1 == Ally 2 == Enemy 3 = ForceAtk 4 = Ping 5 = Shop 
    /// </summary>
    /// <param name="type">마우스 커서의 텍스쳐</param>
    /// <param name="hotSpot">마우스 포인트 클릭 좌표, Defalut 는 Vector2.zero </param>
    /// <param name="setCenter">//중심을 사용하지 않을 경우 Adjust Hot Spot으로 입력 받은 좌표사용</param>
    public void SetCursor(int type, Vector2 coordinate) // 마우스 커서 설정
    {
        // 같은 커서로 바꾸라고하면 안함
        if (CurrentCursor == type)
            return;

        //새로 커서가 바뀌면 마지막커서를 갱신
        PreCursor = CurrentCursor;
        //마우스클릭의 위치가 커서 텍스쳐의 중점인가?
        if (type.Equals(3))
        {
            coordinate.x = MouseTexture[type].width / 2;
            coordinate.y = MouseTexture[type].height / 2;
            Cursor.SetCursor(MouseTexture[type], coordinate, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(MouseTexture[type], coordinate, CursorMode.Auto);
        }
        // 커서 바꾸고나면 현재커서를 저장.
        CurrentCursor = type;
        //print(PreCursor + " , " + CurrentCursor);
    }
}

