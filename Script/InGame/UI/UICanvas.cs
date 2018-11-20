using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{

    public GameObject Item;
    public GameObject Skill;
    public GameObject Stat;
    public GameObject Icon;
    public GameObject Recall;
    public GameObject Tooltip;
    public GameObject ItemTooltip;
    public GameObject RightTop;

    [Space]
    public GameObject TabUI;
    public GameObject EnemyUI;
    public GameObject ChatUI;

    private ChatFunction chatfunction;
    Vector3 v;
    Ray r;
    RaycastHit[] hits;

    private float refreshTime = -5;
    private float refreshPeriod = 0.5f;

    // 임시변수 나중에 지워라
    public GameObject WinLoseUI;

    // Use this for initialization
    void Start()
    {
        chatfunction = ChatUI.transform.parent.GetComponent<ChatFunction>();
    }

    // Update is called once per frame
    void Update()
    {   
        if(!chatfunction.chatInput.IsActive())
        {
            // 임시로 종료창띄우기
            if (Input.GetKeyDown(KeyCode.Slash))
            {
                WinLoseUI.SetActive(true);
            }

            // P 상점
            if (Input.GetKeyDown(KeyCode.P))
            {
                Item.GetComponent<ItemUI>().ShopButton();
            }

            // Tab 스코어보드
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TabUI.SetActive(true);
            }
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                TabUI.SetActive(false);
            }
        }

        // Enter 채팅창
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // 꺼져있으면 켜기
            if (!ChatUI.activeSelf)
            {
                ChatUI.SetActive(true);
                ChatUI.GetComponent<InputField>().ActivateInputField();
                chatfunction.RevealScroll();
            }
            else
            {
                chatfunction.Send();
                ChatUI.SetActive(false);
                chatfunction.HideScroll();
            }
        }

        // 적클릭. ray로 쏘기
        if (Input.GetMouseButtonDown(0))
        {
            v = Input.mousePosition;
            r = Camera.main.ScreenPointToRay(v);
            hits = Physics.RaycastAll(r);

            bool find = false;
            foreach (RaycastHit hit in hits)
            {
                //// 정글 태그추가하기. 내캐릭터는 제외하게 예외처리하기.
                //if (hit.collider.tag.Equals("Minion") || hit.collider.tag.Equals("Tower")
                //    || hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                //{

                //    EnemyUI.SetActive(true);
                //    find = true;
                //    EnemyUI.GetComponent<UIEnemy>().ApplyObject(hit.collider.gameObject);
                //}
                if (hit.collider.tag.Equals("Minion") || hit.collider.tag.Equals("Tower"))
                {
                    if (hit.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                        find = true;
                }
                else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Champion")))
                {
                    if (!hit.collider.tag.Equals("Player"))
                        if (hit.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                            find = true;
                }
                else if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("Monster")))
                {
                    if (hit.transform.GetComponent<FogOfWarEntity>().isCanTargeting)
                        find = true;
                }
                if (find)
                {
                    EnemyUI.SetActive(true);
                    EnemyUI.GetComponent<UIEnemy>().ApplyObject(hit.collider.gameObject);
                }
            }
            if (!find)
            {
                EnemyUI.SetActive(false);
                EnemyUI.GetComponent<UIEnemy>().selectedObject = null;
            }
        }


        // 주기적으로 상대 정보업데이트하기
        refreshTime += Time.deltaTime;
        if(refreshTime >= refreshPeriod)
        {
            TabUI.GetComponent<TabUI>().TabRefresh();
            refreshTime -= refreshPeriod;
        }

        // 애니메이션 테스트용 나중에 지우기
        //Animator animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
        //if (Input.GetKeyDown(KeyCode.Z))
        //    animator.SetBool("Q", true);
        //if (Input.GetKeyUp(KeyCode.Z))
        //    animator.SetBool("Q", false);

        //if (Input.GetKeyDown(KeyCode.X))
        //    animator.SetBool("W", true);
        //if (Input.GetKeyUp(KeyCode.X))
        //    animator.SetBool("W", false);

        //if (Input.GetKeyDown(KeyCode.C))
        //    animator.SetBool("E", true);
        //if (Input.GetKeyUp(KeyCode.C))
        //    animator.SetBool("E", false);

        //if (Input.GetKeyDown(KeyCode.V))
        //    animator.SetBool("R", true);
        //if (Input.GetKeyUp(KeyCode.V))
        //    animator.SetBool("R", false);
    }
}
