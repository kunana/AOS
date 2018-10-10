using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonDestroy : Photon.MonoBehaviour {

    public static GameObject LocalPlayer;

    private void Awake()
    {
      LocalPlayer = this.gameObject;
      DontDestroyOnLoad(LocalPlayer);
    }
}
