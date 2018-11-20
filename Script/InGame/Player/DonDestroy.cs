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

    private void OnLevelWasLoaded(int level)
    {
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Equals("Result"))
        {
            Destroy(gameObject);
        }
    }
}
