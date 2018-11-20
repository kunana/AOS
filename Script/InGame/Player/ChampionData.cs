using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChampionData : Photon.MonoBehaviour, IPunObservable
{
    public Skills playerSkill;
    // 기본스탯, 스킬
    public StatClass.Stat mystat = new StatClass.Stat();
    public SkillClass.Skill myskill = new SkillClass.Skill();
    public ShopItem.Item itemstat = new ShopItem.Item();
    public StatClass.Stat totalstat = null;

    // 챔피언명
    [HideInInspector]
    public string ChampionName = "Ashe";

    // 스킬레벨
    public int skill_Q = 0;
    public int skill_W = 0;
    public int skill_E = 0;
    public int skill_R = 0;
    public float Cooldown_Passive = 0;
    public float Cooldown_Q = 0;
    public float Cooldown_W = 0;
    public float Cooldown_E = 0;
    public float Cooldown_R = 0;
    public float current_Cooldown_Passive = 0;
    public float current_Cooldown_Q = 0;
    public float current_Cooldown_W = 0;
    public float current_Cooldown_E = 0;
    public float current_Cooldown_R = 0;
    // 스킬 쿨타임도중 스킬레벨업하여 쿨감소 방지하기위해
    public float temp_Cooldown_Q = 0;
    public float temp_Cooldown_W = 0;
    public float temp_Cooldown_E = 0;
    public float temp_Cooldown_R = 0;
    public float mana_Q = 0;
    public float mana_W = 0;
    public float mana_E = 0;
    public float mana_R = 0;

    // 스펠쿨타임. ID는 PlayerData에서 받아옴.(왜냐면 캐릭터 선택창에서 받아와야하니까)
    public int spell_D = 0;
    public int spell_F = 0;
    public float Cooldown_D = 0;
    public float Cooldown_F = 0;
    public float current_Cooldown_D = 0;
    public float current_Cooldown_F = 0;

    //룬정보
    public int mainRune = 0;
    public int subRune1 = 0;
    public int subRune2 = 0;
    public int subRune3 = 0;
    public int assistSubRune1 = 0;
    public int assistSubRune2 = 0;

    //아이템
    public int[] item = null;
    public int accessoryItem = 0;

    // 귀환시간
    private float RecallTime = 8.0f;
    private float currentRecallTime = 8.0f;
    [HideInInspector]
    public bool RecallStart = false;
    [HideInInspector]
    public Vector3 RedPos;
    [HideInInspector]
    public Vector3 BluePos;

    //킬데스어시cs
    public int kill = 0;
    public int death = 0;
    public int assist = 0;
    public int cs = 0;

    // 오브젝트 받아옴
    public GameObject UIRecall;
    public UIStat UIStat;
    public UIIcon UIIcon;
    private UISkill UISkill;
    public UIRightTop UIRightTop;

    // 경험치 테스트용
    private float testTime = 0;
    private float regenTime = 0;

    // 컨트롤로 스킬찍게 체크
    private bool ctrlcheck = false;

    //UI캔버스 찾았는지 체크
    private bool Find = false;

    //리콜시 못움직이게 현재위치 저장
    Vector3 CurPos = Vector3.zero;
    //리콜시 체력 저장
    float CurHp = 0;
    //채팅시 스펠 못하게
    private ChatFunction chatfunction;
    private PlayerData playerData;

    // 애니메이션을 위해 astar Target을 받아옴
    Pathfinding.AIDestinationSetter myAIDestinationSetter;
    public Animator myAnimator;
    private PlayerSpell playerSpell;

    //스킬 사용에 체력을 쓰는 애들 용
    public bool isNoMP = false;

    //스킬 쓰면 일정 시간 데미지, 물방, 마방, 이속 올라가는 거 반영
    public float skillPlusAtkDam = 0;
    public float skillPlusAtkDef = 0;
    public float skillPlusAbilDef = 0;
    public float skillPlusSpeed = 0;
    public AIPath TheAIPath;
    public bool test = false;
    PhotonView view;
    public bool canSkill = true;

    // 포션체크
    private bool hpPotion = false;
    private bool mpPotion = false;
    private float hpPotionTimeCheck = 15.0f;
    private float mpPotionTimeCheck = 15.0f;
    private float hpPotionCycle = 0.5f;
    private float mpPotionCycle = 0.5f;

    private CsTextPool csTextPool;//CSText
    private AudioSource audio;//CSText
    bool effectOnce = false;
    bool effectOnce2 = false;
    int testnum = 0;
    bool recallComplete = false;

    private StackImage TheStackImage = null;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        if (photonView.isMine)
        {
            ChampionName = PlayerData.Instance.championName;
            if (ChampionName.Contains("Mundo"))
                isNoMP = true;
            setSpell();
            setStatSkill(ChampionName);

            item = PlayerData.Instance.item;
        }
        else
        {
            item = new int[6];
        }
        totalstat = mystat.ClassCopy();

        myAIDestinationSetter = GetComponent<Pathfinding.AIDestinationSetter>();
        myAnimator = GetComponent<Animator>();
        TheAIPath = GetComponent<AIPath>();
        playerSpell = GetComponent<PlayerSpell>();
        playerData = PlayerData.Instance;
        audio = GetComponent<AudioSource>();

        int ran = Random.Range(1, 9);
        RedPos = new Vector3(4 + ran, 0.5f, 10f);
        BluePos = new Vector3(262 + ran, 0.5f, 270f);
    }

    private void OnLevelWasLoaded(int level)
    {
        PhotonNetwork.isMessageQueueRunning = true;
        if (UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(level).name.Equals("InGame"))
            Invoke("FindUICanvas", 3f);
    }

    public void FindUICanvas()
    {
        UICanvas UIcanvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<UICanvas>();
        UIStat = UIcanvas.Stat.GetComponent<UIStat>();
        UIIcon = UIcanvas.Icon.GetComponent<UIIcon>();
        UISkill = UIcanvas.Skill.GetComponent<UISkill>();
        UIRecall = UIcanvas.Recall;
        UIRightTop = UIcanvas.RightTop.GetComponent<UIRightTop>();
        Find = true;

        chatfunction = GameObject.FindGameObjectWithTag("ChatManager").GetComponentInChildren<ChatFunction>();
        csTextPool = GameObject.FindGameObjectWithTag("CSText").GetComponent<CsTextPool>();
    }

    // Update is called once per frame
    void Update()
    {
        //    if (transform.position.y < -1)
        //    {
        //        Vector3 a = transform.position;
        //        a.y = 1;
        //        transform.position = a;
        //    }

        if (!photonView.isMine)
            return;

        if (Find)
        {
            // Ctrl눌렀는지 체크
            CtrlCheck();

            // 스킬
            SkillCheck();

            //스펠
            SpellCheck();

            // Recall
            RecallCheck();

            // 체력, 마나재생
            HealthManaRegen();

            // 아이템
            ItemCheck();

            ////경험치 증가 테스트
            //testTime += Time.deltaTime;
            //if (testTime >= 1.0f && mystat.Level < 18)
            //{
            //    mystat.Exp += 150;
            //    testTime = 0;
            //}

            if (totalstat.Level == 18)
                return;

            if (mystat.Exp > mystat.RequireExp)
                LevelUp();
        }
        if (recallComplete)
        {
            TheAIPath.canMove = true;
            if (PhotonNetwork.player.GetTeam().ToString().Contains("red"))
            {
                transform.position = RedPos;
                playerSkill.TheChampionAtk.AStarTargetObj.transform.position = RedPos;
            }
            else if (PhotonNetwork.player.GetTeam().ToString().Contains("blue"))
            {
                transform.position = BluePos;
                playerSkill.TheChampionAtk.AStarTargetObj.transform.position = BluePos;
            }
            recallComplete = false;
        }
    }

    public void HealthManaRegen()
    {
        if (playerData.isDead || totalstat.Hp <= 0)
            return;

        regenTime += Time.deltaTime;
        if (regenTime >= 0.5f)
        {
            regenTime -= 0.5f;
            if (totalstat.Hp < totalstat.MaxHp)
                totalstat.Hp += totalstat.Health_Regen * 0.1f;
            if (totalstat.Mp < totalstat.MaxMp)
                totalstat.Mp += totalstat.Mana_Regen * 0.1f;

            if (totalstat.Hp > totalstat.MaxHp)
                totalstat.Hp = totalstat.MaxHp;
            if (totalstat.Mp > totalstat.MaxMp)
                totalstat.Mp = totalstat.MaxMp;
        }
    }

    public void ItemCheck()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PotionCheck(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PotionCheck(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            PotionCheck(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            PotionCheck(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            PotionCheck(5);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            PotionCheck(6);
        }

        if (hpPotion)
        {
            if (!effectOnce)
            {
                effectOnce = true;
                playerSpell.SendEffect("HealPotion", transform.position, PhotonNetwork.player.GetTeam().ToString().ToLower(), view.viewID);
                if (TheStackImage == null)
                {
                    TheStackImage = GameObject.FindGameObjectWithTag("StackImage").GetComponent<StackImage>();
                }
                if (!TheStackImage.ImageDic["HPPotion"].gameObject.activeInHierarchy)
                {
                    TheStackImage.ImageDic["HPPotion"].gameObject.SetActive(true);
                }
            }
            // 15초동안 150회복. 초당 10회복. 0.5초당 5회복
            hpPotionCycle -= Time.deltaTime;

            if (hpPotionCycle < 0)
            {
                if (totalstat.Hp < totalstat.MaxHp)
                    totalstat.Hp += 5;
                if (totalstat.Hp > totalstat.MaxHp)
                    totalstat.Hp = totalstat.MaxHp;
                hpPotionCycle = 0.5f;
            }

            // 15초가 다 지나면 포션사용가능
            hpPotionTimeCheck -= Time.deltaTime;
            TheStackImage.TextDic["HPPotion"].text = Mathf.FloorToInt(hpPotionTimeCheck).ToString();
            if (hpPotionTimeCheck < 0)
            {
                effectOnce = false;
                hpPotionTimeCheck = 15.0f;
                hpPotion = false;
                TheStackImage.TextDic["HPPotion"].text = "";
                TheStackImage.ImageDic["HPPotion"].gameObject.SetActive(false);
            }
        }

        if (mpPotion)
        {
            if (!effectOnce2)
            {
                effectOnce2 = true;
                playerSpell.SendEffect("HealPotion", transform.position, PhotonNetwork.player.GetTeam().ToString().ToLower(), view.viewID);
                if (TheStackImage == null)
                {
                    TheStackImage = GameObject.FindGameObjectWithTag("StackImage").GetComponent<StackImage>();
                }
                if (!TheStackImage.ImageDic["MPPotion"].gameObject.activeInHierarchy)
                {
                    TheStackImage.ImageDic["MPPotion"].gameObject.SetActive(true);
                }
            }
            // 15초동안 100회복. 초당 6.666회복. 0.5초당 3.3333회복
            mpPotionCycle -= Time.deltaTime;

            if (mpPotionCycle < 0)
            {

                if (totalstat.Mp < totalstat.MaxMp)
                    totalstat.Mp += 5;
                if (totalstat.Mp > totalstat.MaxMp)
                    totalstat.Mp = totalstat.MaxMp;
                mpPotionCycle = 0.5f;
            }

            // 15초가 다 지나면 포션사용가능
            mpPotionTimeCheck -= Time.deltaTime;
            TheStackImage.TextDic["MPPotion"].text = Mathf.FloorToInt(mpPotionTimeCheck).ToString();
            if (mpPotionTimeCheck < 0)
            {
                effectOnce2 = false;
                mpPotionTimeCheck = 15.0f;
                mpPotion = false;
                TheStackImage.TextDic["MPPotion"].text = "";
                TheStackImage.ImageDic["MPPotion"].gameObject.SetActive(false);
            }
        }
    }

    public void PotionCheck(int itemSlotNum)
    {
        // 일단은 bool값 검사해서 사용중이면 못사용하게함.
        // 체력포션이면
        if (item[itemSlotNum - 1] == 2 && !hpPotion)
        {
            PlayerData.Instance.item[itemSlotNum - 1] = 0;
            PlayerData.Instance.ItemUpdate();
            PlayerData.Instance.ItemUndoListReset();
            hpPotion = true;
        }
        // 마나포션이면
        else if (item[itemSlotNum - 1] == 3 && !mpPotion)
        {
            PlayerData.Instance.item[itemSlotNum - 1] = 0;
            PlayerData.Instance.ItemUpdate();
            PlayerData.Instance.ItemUndoListReset();
            mpPotion = true;
        }
    }

    public void CtrlCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ctrlcheck = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ctrlcheck = false;
        }
    }

    public void SkillCheck()
    {
        if (ctrlcheck && Input.GetKeyDown(KeyCode.Q))
        {
            if (UISkill.getSkillPoint() >= 1 && UISkill.SkillUpButton[0].activeSelf)
                UISkill.skillUp("Q");
        }
        if (ctrlcheck && Input.GetKeyDown(KeyCode.W))
        {
            if (UISkill.getSkillPoint() >= 1 && UISkill.SkillUpButton[1].activeSelf)
                UISkill.skillUp("W");
        }
        if (ctrlcheck && Input.GetKeyDown(KeyCode.E))
        {
            if (UISkill.getSkillPoint() >= 1 && UISkill.SkillUpButton[2].activeSelf)
                UISkill.skillUp("E");
        }
        if (ctrlcheck && Input.GetKeyDown(KeyCode.R))
        {
            if (UISkill.getSkillPoint() >= 1 && UISkill.SkillUpButton[3].activeSelf)
                UISkill.skillUp("R");
        }

        if (!playerSkill || !chatfunction)
            return;

        // 현재쿨타임이 0일때만(쿨타임이 안돌아갈때만) 스킬 써짐
        if (canSkill && !playerData.isDead)
        {
            if (!playerSkill.isSkillIng && !chatfunction.chatInput.IsActive())
            {
                if (Input.GetKeyDown(KeyCode.Q) && !ctrlcheck)
                {
                    if (current_Cooldown_Q == 0 && skill_Q >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalstat.Hp - 2 > mana_Q)
                            {
                                playerSkill.QCasting();
                            }
                        }
                        else if (totalstat.Mp >= mana_Q)
                        {
                            playerSkill.QCasting();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.W) && !ctrlcheck)
                {
                    if (current_Cooldown_W == 0 && skill_W >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalstat.Hp - 2 > mana_W)
                            {
                                playerSkill.WCasting();
                            }
                        }
                        else if (totalstat.Mp >= mana_W)
                        {
                            playerSkill.WCasting();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.E) && !ctrlcheck)
                {
                    if (current_Cooldown_E == 0 && skill_E >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalstat.Hp - 2 > mana_E)
                            {
                                playerSkill.ECasting();
                            }
                        }
                        else if (totalstat.Mp >= mana_E)
                        {
                            playerSkill.ECasting();
                        }
                    }
                }
                if (Input.GetKeyDown(KeyCode.R) && !ctrlcheck)
                {
                    if (current_Cooldown_R == 0 && skill_R >= 1)
                    {
                        if (isNoMP)
                        {
                            if (totalstat.Hp - 2 > mana_R)
                            {
                                playerSkill.RCasting();
                            }
                        }
                        else if (totalstat.Mp >= mana_R)
                        {
                            playerSkill.RCasting();
                        }
                    }
                }
            }
        }
        // 스킬쿨이 돌고있으면 시간마다 점점 쿨감소
        if (current_Cooldown_Q != 0)
        {
            current_Cooldown_Q -= Time.deltaTime;
            if (current_Cooldown_Q < 0)
            {
                current_Cooldown_Q = 0;
                UISkill.SkillDisabledImage[1].SetActive(false);
                UISkill.SkillCooldownImage[1].fillAmount = 0;
                UISkill.SkillCooldownText[1].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.SkillCooldownImage[1].fillAmount = current_Cooldown_Q / temp_Cooldown_Q;
                if (current_Cooldown_Q > 1.0f)
                    UISkill.SkillCooldownText[1].text = Mathf.FloorToInt(current_Cooldown_Q).ToString();
                else
                    UISkill.SkillCooldownText[1].text = current_Cooldown_Q.ToString("N1");
            }
        }
        if (current_Cooldown_W != 0)
        {
            current_Cooldown_W -= Time.deltaTime;
            if (current_Cooldown_W < 0)
            {
                current_Cooldown_W = 0;
                UISkill.SkillDisabledImage[2].SetActive(false);
                UISkill.SkillCooldownImage[2].fillAmount = 0;
                UISkill.SkillCooldownText[2].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.SkillCooldownImage[2].fillAmount = current_Cooldown_W / temp_Cooldown_W;
                if (current_Cooldown_W > 1.0f)
                    UISkill.SkillCooldownText[2].text = Mathf.FloorToInt(current_Cooldown_W).ToString();
                else
                    UISkill.SkillCooldownText[2].text = current_Cooldown_W.ToString("N1");
            }
        }
        if (current_Cooldown_E != 0)
        {
            current_Cooldown_E -= Time.deltaTime;
            if (current_Cooldown_E < 0)
            {
                current_Cooldown_E = 0;
                UISkill.SkillDisabledImage[3].SetActive(false);
                UISkill.SkillCooldownImage[3].fillAmount = 0;
                UISkill.SkillCooldownText[3].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.SkillCooldownImage[3].fillAmount = current_Cooldown_E / temp_Cooldown_E;
                if (current_Cooldown_E > 1.0f)
                    UISkill.SkillCooldownText[3].text = Mathf.FloorToInt(current_Cooldown_E).ToString();
                else
                    UISkill.SkillCooldownText[3].text = current_Cooldown_E.ToString("N1");
            }
        }
        if (current_Cooldown_R != 0)
        {
            current_Cooldown_R -= Time.deltaTime;
            if (current_Cooldown_R < 0)
            {
                current_Cooldown_R = 0;
                UISkill.SkillDisabledImage[4].SetActive(false);
                UISkill.SkillCooldownImage[4].fillAmount = 0;
                UISkill.SkillCooldownText[4].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.SkillCooldownImage[4].fillAmount = current_Cooldown_R / temp_Cooldown_R;
                if (current_Cooldown_R > 1.0f)
                    UISkill.SkillCooldownText[4].text = Mathf.FloorToInt(current_Cooldown_R).ToString();
                else
                    UISkill.SkillCooldownText[4].text = current_Cooldown_R.ToString("N1");
            }
        }

    }
    public void UsedQ()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_Q = Cooldown_Q;
        current_Cooldown_Q = Cooldown_Q;
        if (isNoMP)
            totalstat.Hp -= mana_Q;
        else
            totalstat.Mp -= mana_Q;
        if (Cooldown_Q != 0)
            UISkill.SkillDisabledImage[1].SetActive(true);
        ChampionSound.instance.Skill(playerData.championName, 0, audio);
    }

    public void UsedW()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_W = Cooldown_W;
        current_Cooldown_W = Cooldown_W;
        if (isNoMP)
            totalstat.Hp -= mana_W;
        else
            totalstat.Mp -= mana_W;

        if (Cooldown_W != 0)
            UISkill.SkillDisabledImage[2].SetActive(true);
        ChampionSound.instance.Skill(playerData.championName, 1, audio);
    }

    public void UsedE()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_E = Cooldown_E;
        current_Cooldown_E = Cooldown_E;
        if (isNoMP)
            totalstat.Hp -= mana_E;
        else
            totalstat.Mp -= mana_E;

        if (Cooldown_W != 0)
            UISkill.SkillDisabledImage[3].SetActive(true);
        ChampionSound.instance.Skill(playerData.championName, 2, audio);

    }

    public void UsedR()
    {
        if (RecallStart)
            RecallCancel();

        temp_Cooldown_R = Cooldown_R;
        current_Cooldown_R = Cooldown_R;
        if (isNoMP)
            totalstat.Hp -= mana_R;
        else
            totalstat.Mp -= mana_R;

        if (Cooldown_R != 0)
            UISkill.SkillDisabledImage[4].SetActive(true);

    }


    public void SpellCheck()
    {
        if (chatfunction.chatInput.IsActive())
            return;

        if (Input.GetKeyDown(KeyCode.D) && !RecallStart && !playerSkill.TheChampionAtk.isStun && !playerData.isDead)
        {
            if (current_Cooldown_D == 0)
            {
                if (RecallStart)
                    RecallCancel();

                playerSpell.Call_SpellD();

                if (spell_D != 5 && spell_D != 6 && spell_D != 7)
                {
                    current_Cooldown_D = Cooldown_D;
                    if (Cooldown_D != 0)
                        UISkill.SpellDisabledImage[0].SetActive(true);
                }
                else
                {
                    playerSpell.cursor.SetCursor(3, Vector2.zero);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.F) && !RecallStart && !playerSkill.TheChampionAtk.isStun && !playerData.isDead)
        {
            if (current_Cooldown_F == 0)
            {
                if (RecallStart)
                    RecallCancel();

                playerSpell.Call_SpellF();

                if (spell_F != 5 && spell_F != 6 && spell_F != 7)
                {
                    current_Cooldown_F = Cooldown_F;
                    if (Cooldown_F != 0)
                        UISkill.SpellDisabledImage[1].SetActive(true);
                }
                else
                {
                    playerSpell.cursor.SetCursor(3, Vector2.zero);
                }
            }
        }
        if (spell_D == 5 || spell_D == 6 || spell_D == 7)
        {
            if ((spell_D == 5 && playerSpell.SmiteTargetset))
            {
                playerSpell.SmiteTargetset = false;
                current_Cooldown_D = Cooldown_D;
                if (Cooldown_D != 0)
                    UISkill.SpellDisabledImage[0].SetActive(true);
            }
            else if ((spell_D == 6 && playerSpell.Once))
            {
                current_Cooldown_D = Cooldown_D;
                if (Cooldown_D != 0)
                    UISkill.SpellDisabledImage[0].SetActive(true);
            }
            else if ((spell_D == 7 && playerSpell.IgniteTargetset))
            {
                playerSpell.IgniteTargetset = false;
                current_Cooldown_D = Cooldown_D;
                if (Cooldown_D != 0)
                    UISkill.SpellDisabledImage[0].SetActive(true);
            }
        }
        if (spell_F == (5) || spell_F == (6) || spell_F == (7))
        {
            if ((spell_F == 5 && playerSpell.SmiteTargetset))
            {
                playerSpell.SmiteTargetset = false;
                current_Cooldown_F = Cooldown_F;
                if (Cooldown_F != 0)
                    UISkill.SpellDisabledImage[1].SetActive(true);
            }
            else if ((spell_F == 6 && playerSpell.Once))
            {
                current_Cooldown_F = Cooldown_F;
                if (Cooldown_F != 0)
                    UISkill.SpellDisabledImage[1].SetActive(true);
            }
            else if ((spell_F == 7 && playerSpell.IgniteTargetset))
            {
                playerSpell.IgniteTargetset = false;
                current_Cooldown_F = Cooldown_F;
                if (Cooldown_F != 0)
                    UISkill.SpellDisabledImage[1].SetActive(true);
            }
        }
        if (current_Cooldown_D != 0)
        {
            current_Cooldown_D -= Time.deltaTime;
            if (current_Cooldown_D < 0)
            {
                current_Cooldown_D = 0;
                UISkill.SpellDisabledImage[0].SetActive(false);
                UISkill.SpellCooldownImage[0].fillAmount = 0;
                UISkill.SpellCooldownText[0].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.SpellCooldownImage[0].fillAmount = current_Cooldown_D / Cooldown_D;
                if (current_Cooldown_D > 1.0f)
                    UISkill.SpellCooldownText[0].text = Mathf.FloorToInt(current_Cooldown_D).ToString();
                else
                    UISkill.SpellCooldownText[0].text = current_Cooldown_D.ToString("N1");
            }
        }
        if (current_Cooldown_F != 0)
        {
            current_Cooldown_F -= Time.deltaTime;
            if (current_Cooldown_F < 0)
            {
                current_Cooldown_F = 0;
                UISkill.SpellDisabledImage[1].SetActive(false);
                UISkill.SpellCooldownImage[1].fillAmount = 0;
                UISkill.SpellCooldownText[1].text = "";
            }
            else
            {
                // 이미지와 텍스트 갱신
                UISkill.SpellCooldownImage[1].fillAmount = current_Cooldown_F / Cooldown_F;
                if (current_Cooldown_F > 1.0f)
                    UISkill.SpellCooldownText[1].text = Mathf.FloorToInt(current_Cooldown_F).ToString();
                else
                    UISkill.SpellCooldownText[1].text = current_Cooldown_F.ToString("N1");
            }
        }
    }

    public void RecallCheck()
    {
        if (!RecallStart)
            if (chatfunction.chatInput.IsActive())
                return;

        if (Input.GetKeyDown(KeyCode.B) && !playerData.isDead && !playerSkill.TheChampionAtk.isStun)
        {
            ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Recall);
            //이펙트 부르기
            Recall();
        }
        if (RecallStart)
        {
            playerSpell.SendEffect("Recall", transform.position, PhotonNetwork.player.GetTeam().ToString());
            currentRecallTime -= Time.deltaTime;
            UIRecall.GetComponent<RecallUI>().RecallProgressBar.value = currentRecallTime / RecallTime;
            UIRecall.GetComponent<RecallUI>().RemainTime.text = currentRecallTime.ToString("N1");

            //리콜시 제자리에서 리콜하도록
            transform.position = CurPos;


            if (currentRecallTime <= 0)
            {
                RecallStart = false;
                UIRecall.SetActive(false);
                RecallComplete();
            }

            RecallCancelCheck();
        }
    }

    public void setStatSkill(string championName)
    {
        mystat = StatClass.instance.characterData[championName].ClassCopy();
        myskill = SkillClass.instance.skillData[championName].ClassCopy();
        Cooldown_Passive = myskill.passiveCooldown;
    }

    public void setSpell()
    {
        spell_D = PlayerData.Instance.spell_D;
        spell_F = PlayerData.Instance.spell_F;
        //스펠 쿨세팅
        switch (spell_D)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                Cooldown_D = 210;
                break;
            // 탈진
            case 1:
                Cooldown_D = 210;
                break;
            // 점멸
            case 2:
                Cooldown_D = 300;
                break;
            // 유체화
            case 3:
                Cooldown_D = 180;
                break;
            // 회복
            case 4:
                Cooldown_D = 240;
                break;
            // 강타
            case 5:
                Cooldown_D = 15;
                break;
            // 순간이동
            case 6:
                Cooldown_D = 360;
                break;
            // 점화
            case 7:
                Cooldown_D = 210;
                break;
            // 방어막
            case 8:
                Cooldown_D = 180;
                break;
            default:
                break;
        }
        switch (spell_F)
        {
            //정화 탈진 점멸 유체화 회복 강타 순간이동 점화 방어막
            // 정화
            case 0:
                Cooldown_F = 210;
                break;
            // 탈진
            case 1:
                Cooldown_F = 210;
                break;
            // 점멸
            case 2:
                Cooldown_F = 300;
                break;
            // 유체화
            case 3:
                Cooldown_F = 180;
                break;
            // 회복
            case 4:
                Cooldown_F = 240;
                break;
            // 강타
            case 5:
                Cooldown_F = 15;
                break;
            // 순간이동
            case 6:
                Cooldown_F = 360;
                break;
            // 점화
            case 7:
                Cooldown_F = 210;
                break;
            // 방어막
            case 8:
                Cooldown_F = 180;
                break;
            default:
                break;
        }
    }

    public void LevelUp()
    {
        if (mystat.Level >= 18)
            return;

        mystat.Level++;
        mystat.Exp -= mystat.RequireExp;
        mystat.Hp += mystat.UP_HP;
        mystat.MaxHp += mystat.UP_HP;
        mystat.Mp += mystat.UP_MP;
        mystat.MaxMp += mystat.UP_MP;
        mystat.Health_Regen += mystat.UP_HPRegen;
        mystat.Mana_Regen += mystat.UP_MPRegen;
        mystat.Attack_Damage += mystat.UP_AttackDamage;
        mystat.Attack_Def += mystat.UP_Def;
        mystat.Ability_Def += mystat.UP_MagicDef;

        if (mystat.Level <= 17)
            mystat.RequireExp = StatClass.instance.RequireExp[mystat.Level - 1];
        else if (mystat.Level == 18)
        {
            mystat.RequireExp = 0;
            mystat.Exp = 0;
        }
        // 공속은 계산법이 복잡해서 일단 UI상에서만 계산하여 표시

        totalstat.Hp += mystat.UP_HP;
        totalstat.Mp += mystat.UP_MP;
        TotalStatUpdate();

        if (photonView.isMine)
        {
            ChampionSound.instance.PlayPlayerFx(SoundManager.instance.LevelUp);
            UIStat.Refresh();
            UIIcon.LevelUp();
            UISkill.LevelUp();
        }
    }

    public void Recall()
    {
        if (RecallStart || playerData.isDead || playerSkill.TheChampionAtk.isStun)
            return;

        CurPos = transform.position;
        CurHp = totalstat.Hp;

        RecallStart = true;
        UIRecall.SetActive(true);

        currentRecallTime = RecallTime;
    }

    public void RecallComplete()
    {
        ChampionSound.instance.PlayPlayerFx(SoundManager.instance.Recall_Complete);

        TheAIPath.canMove = true;
        if (PhotonNetwork.player.GetTeam().ToString().Contains("red"))
        {
            transform.position = RedPos;
            playerSkill.TheChampionAtk.AStarTargetObj.transform.position = RedPos;
        }
        else if (PhotonNetwork.player.GetTeam().ToString().Contains("blue"))
        {
            transform.position = BluePos;
            playerSkill.TheChampionAtk.AStarTargetObj.transform.position = BluePos;
        }
        mystat.Hp = mystat.MaxHp;
        mystat.Mp = mystat.MaxMp;

        recallComplete = true;

        Camera.main.transform.position = new Vector3(transform.position.x, Camera.main.transform.position.y, transform.position.z)
            + Camera.main.GetComponent<RTS_Cam.RTS_Camera>().targetOffset;
    }

    public void RecallCancelCheck()
    {
        // 마우스 클릭했을때, 공격받았을때,
        if (Input.GetMouseButtonDown(1) || CurHp > totalstat.Hp)
        {
            ChampionSound.instance.PlayerFx.Stop();
            RecallCancel();
        }
    }

    public void RecallCancel()
    {
        //리콜 이펙트 중단시킬것.
        RecallStart = false;
        UIRecall.SetActive(false);
    }

    public void ItemUpdate(int[] item, int accessoryitem)
    {
        this.item = item;
        this.accessoryItem = accessoryitem;

        if (UIStat == null)
            FindUICanvas();

        ItemStatUpdate();

        if (UIStat != null)
            UIStat.Refresh();
    }

    public void ItemStatUpdate()
    {
        // 장착한 item의 stat합계를 reset
        ItemStatReset(itemstat);

        // itemlist 돌면서 0번이 아니면 itemstat에 stat을 더해줌.
        foreach (int i in item)
        {
            if (i == 0)
                continue;

            ShopItem.Item tempitem = ShopItem.Instance.itemlist[i];

            itemstat.attack_damage += tempitem.attack_damage;
            itemstat.attack_speed += tempitem.attack_speed;
            itemstat.critical_percent += tempitem.critical_percent;
            itemstat.life_steal += tempitem.life_steal;

            itemstat.ability_power += tempitem.ability_power;
            if (isNoMP)
                itemstat.mana = 0;
            else
                itemstat.mana += tempitem.mana;
            itemstat.mana_regen += tempitem.mana_regen;
            itemstat.cooldown_reduce += tempitem.cooldown_reduce;

            itemstat.armor += tempitem.armor;
            itemstat.magic_resist += tempitem.magic_resist;
            itemstat.health += tempitem.health;
            itemstat.health_regen += tempitem.health_regen;

            itemstat.movement_speed += tempitem.movement_speed / 50f;
        }

        TotalStatUpdate();
    }

    public void ItemStatReset(ShopItem.Item itemstat)
    {
        itemstat.attack_damage = 0;
        itemstat.attack_speed = 0;
        itemstat.critical_percent = 0;
        itemstat.life_steal = 0;

        itemstat.ability_power = 0;
        itemstat.mana = 0;
        itemstat.mana_regen = 0;
        itemstat.cooldown_reduce = 0;

        itemstat.armor = 0;
        itemstat.magic_resist = 0;
        itemstat.health = 0;
        itemstat.health_regen = 0;

        itemstat.movement_speed = 0;
    }

    // 경험치는 여기서 관리안함.
    public void TotalStatUpdate()
    {
        totalstat.Level = mystat.Level;

        TotalStatDamDefUpdate();
        TotalStatSpeedUpdate();

        //totalstat.Attack_Damage = mystat.Attack_Damage + itemstat.attack_damage;
        totalstat.Attack_Speed = mystat.Attack_Speed + itemstat.attack_speed;
        totalstat.Critical_Percentage = mystat.Critical_Percentage + itemstat.critical_percent;

        // 흡혈 스탯이 없었던가?
        //totalstat.life = mystat.life + itemstat.attack_damage;

        totalstat.Ability_Power = mystat.Ability_Power + itemstat.ability_power;
        totalstat.MaxMp = mystat.MaxMp + itemstat.mana;
        totalstat.Mana_Regen = mystat.Mana_Regen * (1 + itemstat.mana_regen / 100f);
        totalstat.CoolTime_Decrease = mystat.CoolTime_Decrease + itemstat.cooldown_reduce;

        //totalstat.Attack_Def = mystat.Attack_Def + itemstat.armor;
        //totalstat.Ability_Def = mystat.Ability_Def + itemstat.magic_resist;
        totalstat.MaxHp = mystat.MaxHp + itemstat.health;
        totalstat.Health_Regen = mystat.Health_Regen * (1 + itemstat.health_regen / 100f);

        //totalstat.Move_Speed = mystat.Move_Speed + itemstat.movement_speed;
        // 체젠 마젠은 %니까 100%증가면 2배의속도가 되게 (1 + x/100)을 곱해줌
    }

    public void TotalStatDamDefUpdate()
    {
        totalstat.Attack_Damage = mystat.Attack_Damage + itemstat.attack_damage + skillPlusAtkDam;
        totalstat.Attack_Def = mystat.Attack_Def + itemstat.armor + skillPlusAtkDef;
        totalstat.Ability_Def = mystat.Ability_Def + itemstat.magic_resist + skillPlusAbilDef;
    }

    public void TotalStatSpeedUpdate()
    {
        totalstat.Move_Speed = mystat.Move_Speed + itemstat.movement_speed + skillPlusSpeed;
        if (TheAIPath != null)
            TheAIPath.maxSpeed = totalstat.Move_Speed;
    }

    /// <summary>
    /// 킬냈을때 cs 골드 경험치 올려주는 함수임
    /// </summary>
    /// <param name="name">오브젝트 이름 넣어라</param>
    /// <param name="type">0 챔피언 / 1 미니언 / 2 타워 / 3 정글몹</param>
    /// <param name="pos"> cs 텍스트 띄울 위치</param>
    public void Kill_CS_Gold_Exp(string name, int type, Vector3 pos = default(Vector3))
    {
        // 일단 골드증가
        int csGold = PlayerData.Instance.KillGold(name, type);

        // 챔피언이면
        if (type == 0)
        {
            // 원래는 킬낸 챔피언의 레벨 ~ 다음 레벨의 경험치의 50%를 준다고함. 그냥 200줌
            mystat.Exp += 200;
            kill++;
            csTextPool.getCsText(pos, "+ " + csGold.ToString());
            // 오른쪽 위 UI 갱신
            UIRightTop.AllUpdate();
            return;
        }
        // 타워면
        else if (type == 2)
        {
            // 억제기부수면 글로벌 경험치
            if (name.Contains("Suppressor"))
            {
                this.photonView.RPC("GlobalExp", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 100);
            }
            csTextPool.getCsText(pos, "+ " + csGold.ToString());
            return;
        }
        // 미니언이면
        else if (type == 1)
        {
            cs++;
            // 미니언은 골드와 cs만 먹고
            // 경험치는 미니언이 죽을때 근처에 있는 애들한테 줌
            csTextPool.getCsText(pos, "+ " + csGold.ToString());
        }
        // 정글이면
        else if (type == 3)
        {
            if (name.Contains("Raptor_Big")) // 칼날부리
            {
                mystat.Exp += 25;
                cs += 2;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Raptor_Small")) // 칼날부리 작은애
            {
                mystat.Exp += 23;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Wolf")) // 늑대
            {
                mystat.Exp += 86;
                cs += 2;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Wolf_Small")) // 늑대 작은애
            {
                mystat.Exp += 33;
                cs += 1;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Krug_Big")) // 작골
            {
                mystat.Exp += 133;
                cs += 1;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Krug_Small")) // 작골 작은애
            {
                mystat.Exp += 47;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Gromp")) // 두꺼비
            {
                mystat.Exp += 153;
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            // 블루 레드
            else if (name.Contains("B_Sentinel") || name.Contains("R_Sentinel") || name.Contains("Dragon") || name.Contains("Rift_Herald"))
            {
                mystat.Exp += 148;
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            // 용
            else if (name.Contains("Dragon"))
            {
                mystat.Exp += 200;
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            // 전령
            else if (name.Contains("Rift_Herald"))
            {
                this.photonView.RPC("GlobalExp", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 300);
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
            else if (name.Contains("Baron")) // 내셔남작
            {
                this.photonView.RPC("GlobalExp", PhotonTargets.All, PhotonNetwork.player.GetTeam().ToString(), 700);
                cs += 4;
                csTextPool.getCsText(pos, "+ " + csGold.ToString());
            }
        }

        // 오른쪽 위 UI cs 갱신
        UIRightTop.CSUpdate();
    }

    [PunRPC]
    public void MinionExp(int exp)
    {
        mystat.Exp += exp;
    }

    [PunRPC]
    public void GlobalExp(string team, int exp)
    {
        // 같은팀일 경우 같이 경험치 냠냠
        if (PhotonNetwork.player.GetTeam().ToString().Equals(team))
        {
            mystat.Exp += exp;
        }
    }

    [PunRPC]
    public void GlobalGold(string team, int gold)
    {
        // 같은팀일 경우 같이 경험치 냠냠
        if (PhotonNetwork.player.GetTeam().ToString().Equals(team))
        {
            PlayerData.Instance.gold += gold;
        }
    }

    [PunRPC]
    public void AssistUP()
    {
        assist++;
        // 오른쪽 위 UI 갱신
        UIRightTop.AllUpdate();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            // We own mystat player: send the others our data
            // 기본정보

            stream.SendNext(ChampionName);
            stream.SendNext(spell_D);
            stream.SendNext(spell_F);
            stream.SendNext(kill);
            stream.SendNext(death);
            stream.SendNext(assist);
            stream.SendNext(cs);
            stream.SendNext(skill_Q);
            stream.SendNext(skill_W);
            stream.SendNext(skill_E);
            stream.SendNext(skill_R);

            // 스탯
            TotalStatSend(stream);
            stream.SendNext(mystat.Attack_Speed);

            // 아이템
            for (int i = 0; i < item.Length; i++)
            {
                stream.SendNext(item[i]);
            }
            stream.SendNext(accessoryItem);
        }
        else
        {
            // Network player, receive data
            // 기본정보
            ChampionName = (string)stream.ReceiveNext();
            spell_D = (int)stream.ReceiveNext();
            spell_F = (int)stream.ReceiveNext();
            kill = (int)stream.ReceiveNext();
            death = (int)stream.ReceiveNext();
            assist = (int)stream.ReceiveNext();
            cs = (int)stream.ReceiveNext();
            skill_Q = (int)stream.ReceiveNext();
            skill_W = (int)stream.ReceiveNext();
            skill_E = (int)stream.ReceiveNext();
            skill_R = (int)stream.ReceiveNext();

            // 스탯
            TotalStatReceive(stream);
            mystat.Attack_Speed = (float)stream.ReceiveNext();

            // 아이템
            for (int i = 0; i < item.Length; i++)
            {
                item[i] = (int)stream.ReceiveNext();
            }
            accessoryItem = (int)stream.ReceiveNext();
        }
    }

    public void TotalStatSend(PhotonStream stream)
    {
        stream.SendNext(totalstat.Ability_Def);
        stream.SendNext(totalstat.Ability_Power);
        stream.SendNext(totalstat.Attack_Damage);
        stream.SendNext(totalstat.Attack_Def);
        stream.SendNext(totalstat.Attack_Range);
        stream.SendNext(totalstat.Attack_Speed);
        stream.SendNext(totalstat.CoolTime_Decrease);
        stream.SendNext(totalstat.Critical_Percentage);
        stream.SendNext(totalstat.Exp);
        stream.SendNext(totalstat.Gold);
        stream.SendNext(totalstat.Health_Regen);
        stream.SendNext(totalstat.Hp);
        stream.SendNext(totalstat.Level);
        stream.SendNext(totalstat.Mana_Regen);
        stream.SendNext(totalstat.MaxHp);
        stream.SendNext(totalstat.MaxMp);
        stream.SendNext(totalstat.Move_Speed);
        stream.SendNext(totalstat.Mp);
        stream.SendNext(totalstat.RequireExp);
        stream.SendNext(totalstat.Respawn_Time);
        stream.SendNext(totalstat.UP_AttackSpeed);
    }

    public void TotalStatReceive(PhotonStream stream)
    {
        totalstat.Ability_Def = (float)stream.ReceiveNext();
        totalstat.Ability_Power = (float)stream.ReceiveNext();
        totalstat.Attack_Damage = (float)stream.ReceiveNext();
        totalstat.Attack_Def = (float)stream.ReceiveNext();
        totalstat.Attack_Range = (float)stream.ReceiveNext();
        totalstat.Attack_Speed = (float)stream.ReceiveNext();
        totalstat.CoolTime_Decrease = (float)stream.ReceiveNext();
        totalstat.Critical_Percentage = (float)stream.ReceiveNext();
        totalstat.Exp = (int)stream.ReceiveNext();
        totalstat.Gold = (int)stream.ReceiveNext();
        totalstat.Health_Regen = (float)stream.ReceiveNext();
        totalstat.Hp = (float)stream.ReceiveNext();
        totalstat.Level = (int)stream.ReceiveNext();
        totalstat.Mana_Regen = (float)stream.ReceiveNext();
        totalstat.MaxHp = (float)stream.ReceiveNext();
        totalstat.MaxMp = (float)stream.ReceiveNext();
        totalstat.Move_Speed = (float)stream.ReceiveNext();
        totalstat.Mp = (float)stream.ReceiveNext();
        totalstat.RequireExp = (int)stream.ReceiveNext();
        totalstat.Respawn_Time = (float)stream.ReceiveNext();
        totalstat.UP_AttackSpeed = (float)stream.ReceiveNext();
    }
}