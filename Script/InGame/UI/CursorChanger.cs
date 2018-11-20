using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum _Property
//{
//   RedTeam,
//   BlueTeam,
//   Monster,
//   Shop,
//}

////마우스 커서 변환. 커서가 변할만한 모든 오브젝트에 추가.
//public class CursorChanger : MonoBehaviour {
    
//    //현재 게임오브젝트의 팀 속성
//    public _Property property;
//    //로컬클라이언트의 커서스크립트
//    MouseCursor cursor;
//    string localPlayerTeam;

//    private void Awake()
//    {   
//        if(this.gameObject.tag.Contains("RedTeam"))
//        {
//            property = _Property.RedTeam;
//        }
//        else if(this.gameObject.tag.Contains("BlueTeam"))
//        {
//            property = _Property.BlueTeam;
//        }
//        else if (this.gameObject.tag.Contains("Shop"))
//        {
//            property = _Property.Shop;
//        }
//        else
//        {
//            property = _Property.Monster;
//        }

//        cursor = Camera.main.GetComponent<MouseCursor>();
//    }

//    private void Start()
//    {
//        //포톤 서버 연동시 사용
//        //localPlayerTeam = cursor.localPlayer.GetTeam().ToString();
//    }

//    private void OnMouseOver()
//    {   
//        //오프라인용 서버. 연동되면 삭제.
//        if (this.property == _Property.RedTeam)
//        {
//            cursor.SetCursor(2, Vector2.zero, false);
//        }
//        else if (this.property == _Property.BlueTeam)
//        {
//            cursor.SetCursor(1, Vector2.zero, false);
//        }
//        else if (this.property == _Property.Monster)
//        {
//            cursor.SetCursor(2, Vector2.zero, false);
//        }
//        else if(this.property == _Property.Shop)
//        {
//            cursor.SetCursor(5, Vector2.zero, false);
//        }

//        //포톤 서버 연동시 사용
//        //if(localPlayerTeam != this.property.ToString() || this.property.Equals(_Property.Monster)) //로컬플레이어와 팀이 다르면
//        //{
//        //    cursor.SetCursor(2, Vector2.zero, false);
//        //}
//        //else if(localPlayerTeam.Equals(this.property.ToString())) //로컬플레이어와 팀이 같으면
//        //{
//        //    cursor.SetCursor(1, Vector2.zero,false);
//        //}
//        //else if(this.property.Equals(_Property.Shop))
//        //{
//        //    cursor.SetCursor(5, Vector2.zero,false);
//        //}
//    }
//    private void OnMouseExit()
//    {
//        cursor.SetCursor(0, Vector2.zero, false);
//    }
//}
