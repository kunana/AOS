/*
 * 파일명     : BlackFog.cs
 * 작성자     : 황명우
 * 갱신일자   : 18.7.12
 * 소유자     : Player-FogOffArea
 * 요약       : 플레이어가 가진 Sphere Collider(Trigger가 켜져있어야 함)에 부딪힌 안개를 
 *              보이지 않게 가려 줌. 
 *              현재는 컬러로 했으나 레이어로 넣을 것들이 정리되고 나면
 *              안개를 특정 레이어로 넣었다 다시 빼는 식으로의 전환이 필요하며
 *              나중에는 보이다 안보이다 할 오브젝트의 레이어도 같이 건들여야 할 예정.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFog : MonoBehaviour
{
    //안개가 켜진 상태의 안개 색, 투명해진 안개 색
    Color fogOn = new Color(1, 1, 1, 0.6f), fogOff = new Color(1, 1, 1, 0);
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("blackFog"))
        {
            //레이어 정리 후 레이어 방식으로 변환 필요 -> 트렐로의 맵 안개 부분 설명 참조
            other.GetComponent<SpriteRenderer>().color = fogOff;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("blackFog"))
        {
            //레이어 정리 후 레이어 방식으로 변환 필요
            other.GetComponent<SpriteRenderer>().color = fogOn;
        }
    }
}
