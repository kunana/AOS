using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//Going 핑이 생성될때 챔피언의 위치따라 Z축 회전이 바뀜
public class TextLookat : MonoBehaviour {

    PhotonPlayer Player; // 온라인용
    private GameObject _player; // 로컬
    Vector3 dir;

    private void Update()
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        var dir = transform.position - _player.transform.position;
        dir.Normalize();
        Vector2 dir2 = dir;
        transform.DOLookAt(_player.transform.position, 0.1f); 
    }
}
