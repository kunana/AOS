using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureSetting : MonoBehaviour
{
    private static StructureSetting _instance;
    public static StructureSetting instance
    {
        get
        {
            if (_instance == null)
                _instance = (StructureSetting)FindObjectOfType(typeof(StructureSetting));
            return _instance;
        }
    }
    public GameObject cattail;
    public GameObject tower;
    public GameObject monster;
    public GameObject player;
    public GameObject outlineTrees; // 네비게이션 메쉬를 덜 깔끔하게 하는 것으로 판단 됨.
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).gameObject;
        if (!cattail.activeInHierarchy)
            ActiveTrue();

    }
    public void ActiveTrue()
    {   
        cattail.SetActive(true);
        tower.SetActive(true);
        monster.SetActive(true);
        player.SetActive(true);
        outlineTrees.SetActive(true);
    }
}