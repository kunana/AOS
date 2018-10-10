using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour {

    // 0은 1차타워, 1은 2차타워, 2는 억제기타워
    [Header("Prefab")]
    public GameObject Tower;
    public GameObject Inhibitor;

    [Space]
    [Header("Check")]
    public GameObject[] Top_Tower;
    public GameObject[] Mid_Tower;
    public GameObject[] Bot_Tower;

    public GameObject Top_Inhibitor;
    public GameObject Mid_Inhibitor;
    public GameObject Bot_Inhibitor;

    public GameObject[] Nexus_Tower;
    public GameObject Nexus;

    enum Tower_State
    {
        tower1,
        tower2,
        tower_Inhibitor
    }

    // 기본은 1차타워를 공격할 수 있는 상태
    Tower_State top_state = Tower_State.tower1;
    Tower_State mid_state = Tower_State.tower1;
    Tower_State bot_state = Tower_State.tower1;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // 타워가 부숴지면 함수가 불림
    public void topState_change()
    {
        // 1차타워가 부숴지면 2차타워 공격가능한 상태로 변경
        if (top_state == Tower_State.tower1)
            top_state = Tower_State.tower2;
        // 2차타워가 부숴지면 억제기타워 공격가능한 상태로 변경
        else if (top_state == Tower_State.tower2)
            top_state = Tower_State.tower_Inhibitor;
    }

    public void midState_change()
    {
        if (mid_state == Tower_State.tower1)
            mid_state = Tower_State.tower2;
        else if (mid_state == Tower_State.tower2)
            mid_state = Tower_State.tower_Inhibitor;
    }

    public void botState_change()
    {
        if (bot_state == Tower_State.tower1)
            bot_state = Tower_State.tower2;
        else if (bot_state == Tower_State.tower2)
            bot_state = Tower_State.tower_Inhibitor;
    }

    // 억제기 부숴지면 억제기 변수 null로 변경후 Invoke로 5분뒤 revive함수 호출. 위치보냄
    public void Inhibitor_revive(string name, Vector3 pos)
    {
        if (name == "top")
        {
            Top_Inhibitor = Instantiate(Inhibitor, transform);
            Top_Inhibitor.transform.position = pos;
        }
        else if (name == "mid")
        {
            Mid_Inhibitor = Instantiate(Inhibitor, transform);
            Mid_Inhibitor.transform.position = pos;
        }
        else if (name == "bot")
        {
            Bot_Inhibitor = Instantiate(Inhibitor, transform);
            Bot_Inhibitor.transform.position = pos;
        }
    }

    public bool nexus_tower_attack()
    {
        // 억제기가 하나라도 깨져있다면 넥서스타워 공격가능
        if (Top_Inhibitor == null || Mid_Inhibitor == null || Bot_Inhibitor == null)
            return true;

        return false;
    }

    public bool nexus_attack()
    {
        // 억제기타워가 다 부숴지고 억제기가 하나라도 깨져있으면 넥서스 공격가능
        if (Nexus_Tower[0] == null && Nexus_Tower[1] == null && nexus_tower_attack())
            return true;

        return false;
    }
}
