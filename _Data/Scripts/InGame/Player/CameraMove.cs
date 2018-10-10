using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    private GameObject player = null;

    public float moveSpeed = 20;
    public float distance = 30;

    [Header("Barrier")]
    public float topBarrier = 0.97f;
    public float bottomBarrier = 0.03f;
    public float leftBarrier = 0.03f;
    public float rightBarrier = 0.97f;
    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        // 마우스의 위치따라 카메라를 이동
		if(Input.mousePosition.y >= Screen.height * topBarrier)
        {
            transform.Translate(Vector3.up * Time.deltaTime * moveSpeed);
        }
        if (Input.mousePosition.y <= Screen.height * bottomBarrier)
        {
            transform.Translate(Vector3.down * Time.deltaTime * moveSpeed);
        }
        if (Input.mousePosition.x >= Screen.width * rightBarrier)
        {
            transform.Translate(Vector3.right * Time.deltaTime * moveSpeed);
        }
        if (Input.mousePosition.x <= Screen.width * leftBarrier)
        {
            transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
        }

        // 스페이스바를 누르면 카메라의 x,z값이 플레이어를 따라감
        if(Input.GetKey(KeyCode.Space))
        {
            Vector3 playerPos = Vector3.zero;
            playerPos.x = player.transform.position.x;
            playerPos.z = player.transform.position.z;
            playerPos.y = transform.position.y;

            transform.position = playerPos;
        }        

        // 스크롤값이 들어왔을때만 카메라 y값 변경
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 100f;

            // 카메라의 높이 최소치 10, 최대치 30
            if (distance <= 10)
                distance = 10;
            else if (distance >= 30)
                distance = 30;

            Vector3 scrollpos = transform.position;
            scrollpos.y = distance;
            transform.position = scrollpos;
        }
    }
}
