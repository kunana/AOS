/*
 * 파일명     : BlackFog.cs
 * 작성자     : 황명우
 * 갱신일자   : 18.7.12
 * 소유자     : GridManager
 * 요약       : 맵 전체를 덮는 전장의 안개를 그리드마다 하나씩 만들어주는 함수
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackFog : MonoBehaviour
{
    [Tooltip("GridBall.cs의 density값과 일치")]
    public float density = 1f;

    [Tooltip("각 그리드가 가질 전장의 안개 프리팹 오브젝트")]
    public GameObject blackFogObj = null; // Assets폴더의 blackFog 프리팹
    [HideInInspector]
    public GameObject[] blackFog; //각 그리드의 전장의 안개 오브젝트

    private void Start()
    {
        MakeBlackFog();
    }

    //각 그리드마다 검은 안개를 덮어준다.
    private void MakeBlackFog()
    {
        blackFog = new GameObject[(int)(40 * 41 * density * density) + 1];
        for (int z = 0; z <= 40 * density; ++z)
            for (int x = 0; x <= 40 * density; ++x)
            {
                Vector3 point = new Vector3((int)x, 1, (int)z);
                int num = (z * (40 * (int)density) + x);
                blackFog[num] = Instantiate(blackFogObj, point, Quaternion.identity, transform);
                blackFog[num].transform.Rotate(90, 0, 0);
                blackFog[num].name = "blackFogX" + x.ToString() + "Z" + z.ToString();
            }
    }
}
