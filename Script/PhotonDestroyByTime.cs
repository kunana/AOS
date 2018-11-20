using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonDestroyByTime : MonoBehaviour
{

    public float destroyTime = 5.0f;
    private bool check = true;

    // Update is called once per frame
    void Update()
    {
        if (!check)
            return;

        destroyTime -= Time.deltaTime;

        if (destroyTime < 0)
        {
            if (GetComponent<PhotonView>().owner.Equals(PhotonNetwork.player))
                PhotonNetwork.Destroy(this.gameObject);
            else
                check = false;
        }
    }
}