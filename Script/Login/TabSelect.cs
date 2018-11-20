using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabSelect : MonoBehaviour {

    private EventSystem eventSystem;
	// Use this for initialization
	void Start () {
        eventSystem = EventSystem.current;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = null;
            Selectable current = null;

            // current 갱신 (현재 선택되어있는 오브젝트가 active상태면 current로)
            if(eventSystem.currentSelectedGameObject != null)
            {
                if(eventSystem.currentSelectedGameObject.activeInHierarchy)
                {
                    current = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
                }
            }

            // current가 존재하면 next를 찾음
            if (current != null)
            {
                // shift 누르고 있으면 (shift + tab 이면) 뒤로
                if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    next = current.FindSelectableOnUp();
                    if (next == null)
                        next = current.FindSelectableOnLeft();
                }
                else
                {
                    next = current.FindSelectableOnDown();
                    if (next == null)
                        next = current.FindSelectableOnRight();
                }
            }
            // current가 null이면 선택가능한 전체에서 1번째를 잡음
            else
            {
                if (Selectable.allSelectables.Count > 0)
                    next = Selectable.allSelectables[0];
            }

            // next가 존재하면 선택(포커스)
            if (next != null)
                next.Select();
        }
	}
}
