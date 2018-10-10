using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//미니맵 클릭시 레이를 쏴서 메인 카메라를 해당 위치에 이동 시킴.
//미니맵 Raw 이미지가 있는곳
public class MinimapClick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    //탑다운, Otho 카메라 (미니맵 카메라 할당)
    public Camera miniMapCam;
    public GameObject TargetObj;
    private Vector2 localCursor;
    public GameObject SmallPing;
    public PingSignSmall SPing;
    public bool isClicking;

    Texture tex; //1024-1024
    Rect r; // 70,70/140/140
    float coordX;
    float coordY;
    float recalcX;
    float recalcY;
    Vector3 RayToWorldPos;

    public bool IsPointerOver
    {
        get
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }

    private void Awake()
    {
        SPing = SmallPing.GetComponent<PingSignSmall>();
        tex = GetComponent<RawImage>().texture;
        r = GetComponent<RawImage>().rectTransform.rect;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (SmallPing.GetActive()) // 미니맵 핑이 활성화 되었을때만
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.position, eventData.pressEventCamera, out localCursor))
            {
                //RawImage 텍스처와 로컬 커서의 크기를 사용. 텍스처의 0과 width - height 사이의 X, Y 좌표 클램프
                coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
                coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);

                //텍스처 너비와 높이를 기준으로 coordX, coordY를 %(0.0-1.0)로 변환
                recalcX = coordX / tex.width;
                recalcY = coordY / tex.height;
                localCursor = new Vector2(recalcX, recalcY);

                //핑 UI 종료점 생성
                MinimapCamMove(3);
            }
        }
    }

    private void Update()
    {
        if (isClicking)
        {
            if (!Input.GetKey(KeyCode.LeftAlt))
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, Input.mousePosition, null, out localCursor))
                {
                    coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
                    coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);
                    recalcX = coordX / tex.width;
                    recalcY = coordY / tex.height;
                    localCursor = new Vector2(recalcX, recalcY);
                    MinimapCamMove(0);
                }
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localCursor))
        {
            isClicking = true;
            coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
            coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);

            recalcX = coordX / tex.width;
            recalcY = coordY / tex.height;
            localCursor = new Vector2(recalcX, recalcY);

            //핑 UI 시작점 생성
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                SmallPing.SetActive(true);
                MinimapCamMove(2);
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
                isClicking = true;


        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isClicking = false;
        if (!SmallPing.GetActive())
        {
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform, eventData.pressPosition, eventData.pressEventCamera, out localCursor))
            {
                coordX = Mathf.Clamp(0, (((localCursor.x - r.x) * tex.width) / r.width), tex.width);
                coordY = Mathf.Clamp(0, (((localCursor.y - r.y) * tex.height) / r.height), tex.height);

                recalcX = coordX / tex.width;
                recalcY = coordY / tex.height;
                localCursor = new Vector2(recalcX, recalcY);

                ////미니맵 클릭시
                //if (eventData.button == PointerEventData.InputButton.Left) // 왼 클릭시 메인 카메라 이동
                //    MinimapCamMove(0);
                //else 
                if (eventData.button == PointerEventData.InputButton.Right)//오른 클릭시 플레이어 이동
                    MinimapCamMove(1);
            }
        }
    }

    private void MinimapCamMove(int num)
    {
        Ray miniMapRay = miniMapCam.ScreenPointToRay(new Vector2(localCursor.x * miniMapCam.pixelWidth, localCursor.y * miniMapCam.pixelHeight));
        RaycastHit miniMapHit;
        RayToWorldPos = Vector3.zero;

        if (Physics.Raycast(miniMapRay, out miniMapHit, Mathf.Infinity))
        {
            if (num.Equals(0)) //카메라이동
                Camera.main.transform.position = new Vector3(miniMapHit.point.x, Camera.main.transform.position.y, miniMapHit.point.z);
            else if (num.Equals(1)) // 영웅 이동
            {
                Vector3 h = miniMapHit.point;
                h.y = 1;
                TargetObj.transform.position = h;
            }
            else if (num.Equals(2)) // 미니맵핑 시작점
            {
                SPing.setLine("Start", new Vector3(miniMapHit.point.x, Camera.main.transform.position.y + 100, miniMapHit.point.z));
                SPing.setLine("End", new Vector3(miniMapHit.point.x, Camera.main.transform.position.y + 100, miniMapHit.point.z));
                SPing.StartPos = new Vector2(miniMapHit.point.x, miniMapHit.point.z); // 각도계산용
                SPing.InitialCoordinate = new Vector3(miniMapHit.point.x, 1, miniMapHit.point.z);
            }
            else if (num.Equals(3)) // 미니맵핑 종료점
            {
                RayToWorldPos = new Vector3(miniMapHit.point.x, Camera.main.transform.position.y + 100, miniMapHit.point.z);
                SPing.setLine("End", new Vector3(miniMapHit.point.x, Camera.main.transform.position.y + 100, miniMapHit.point.z));
                SPing.Endpos = new Vector2(miniMapHit.point.x, miniMapHit.point.z); // 각도계산용
            }
        }
    }


}




