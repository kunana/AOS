using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEnemy : MonoBehaviour
{
    public StatClass.Stat stat;
    public StatClass.Stat originstat;
    private StatClass.Stat towerstat = new StatClass.Stat();
    public Text attackdamage;
    public Text abilitypower;
    public Text defence;
    public Text magicresist;
    public Text attackspeed;
    public Text cooldown_reduce;
    public Text critical;
    public Text move_speed;

    [Space]
    public Image icon;
    public Image[] itemicon;
    public Text level_text;
    public Text cs_text;
    public Text kda_text;

    [Space]
    public ProgressBar healthBar;
    public ProgressBar manaBar;

    [HideInInspector]
    public GameObject selectedObject;

    private ChampionData cd;
    private MinionBehavior mb;
    private TowerBehaviour tb;
    private MonsterBehaviour monB;

    enum SelectType
    {
        player = 1,
        tower = 2,
        minion = 3,
        monster = 4
    }
    private SelectType selectType;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (selectedObject)
        {
            StatUpdate();

            // 플레이어면 kda, cs도 갱신해주기
            if (selectType == SelectType.player)
            {
                ItemUpdate();
            }

            HealthBarUpdate();
        }
    }

    public void ApplyObject(GameObject go)
    {
        selectedObject = go;

        // 챔피언을 눌렀을때
        if (go.layer.Equals(LayerMask.NameToLayer("Champion")))
        {
            selectType = SelectType.player;
            cd = go.GetComponent<ChampionData>();

            // 챔피언의 스탯을 가져와서 스탯업데이트
            stat = cd.totalstat;
            originstat = cd.mystat;
            StatUpdate();

            // 챔피언의 아이콘을 가져와서 아이콘 업데이트
            icon.sprite = Resources.Load<Sprite>("Champion/ChampionIcon/" + cd.ChampionName);

            // 챔피언의 아이템을 가져와서 아이템 업데이트
            ItemUpdate();
        }

        else if (go.layer.Equals(LayerMask.NameToLayer("Monster")))
        {
            selectType = SelectType.monster;
            monB = go.GetComponent<MonsterBehaviour>();
            stat = monB.stat;
            StatUpdate();
            icon.sprite = null;
        }

        // 미니언을 눌렀을때
        else if (go.CompareTag("Minion"))
        {
            selectType = SelectType.minion;
            mb = go.GetComponent<MinionBehavior>();

            stat = mb.stat;
            StatUpdate();

            if (mb.name.Contains("Red"))
            {
                if (mb.name.Contains("Magician"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_caster_red");
                else if (mb.name.Contains("Melee"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_melee_red");
                else if (mb.name.Contains("Siege"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_siege_red");
            }
            else if (mb.name.Contains("Blue"))
            {
                if (mb.name.Contains("Magician"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_caster_blue");
                else if (mb.name.Contains("Melee"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_melee_blue");
                else if (mb.name.Contains("Siege"))
                    icon.sprite = Resources.Load<Sprite>("Icon/Minion_siege_blue");
            }
            else
                icon.sprite = null;

            for (int i = 0; i < itemicon.Length; i++)
            {
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                itemicon[i].sprite = null;
                itemicon[i].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }

        // 타워를 눌렀을때
        else if (go.CompareTag("Tower"))
        {
            selectType = SelectType.tower;
            tb = go.GetComponent<TowerBehaviour>();

            stat = tb.towerstat;
            StatUpdate();

            if (tb.Team.Equals("Red"))
                icon.sprite = Resources.Load<Sprite>("Icon/Tower_Icon_Red");
            else if (tb.Team.Equals("Blue"))
                icon.sprite = Resources.Load<Sprite>("Icon/Tower_Icon_Blue");
            else
                icon.sprite = null;

            for (int i = 0; i < itemicon.Length; i++)
            {
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                itemicon[i].sprite = null;
                itemicon[i].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }
    }

    public void StatUpdate()
    {
        if (stat == null)
            return;

        attackdamage.text = Mathf.RoundToInt(stat.Attack_Damage).ToString();
        abilitypower.text = Mathf.RoundToInt(stat.Ability_Power).ToString();
        defence.text = Mathf.RoundToInt(stat.Attack_Def).ToString();
        magicresist.text = Mathf.RoundToInt(stat.Ability_Def).ToString();

        if (selectType == SelectType.player)
        {
            float AS = originstat.Attack_Speed * (1 + (stat.UP_AttackSpeed * (stat.Level - 1) + (stat.Attack_Speed - originstat.Attack_Speed)) / 100);
            attackspeed.text = System.Math.Round(AS, 2).ToString();
        }
        else
            attackspeed.text = System.Math.Round(stat.Attack_Speed, 2).ToString();

        cooldown_reduce.text = Mathf.RoundToInt(stat.CoolTime_Decrease).ToString();
        critical.text = Mathf.RoundToInt(stat.Critical_Percentage).ToString();
        move_speed.text = Mathf.RoundToInt(stat.Move_Speed * 50f).ToString();

        level_text.text = stat.Level.ToString();
    }

    public void ItemUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            if (cd.item[i] != 0)
            {
                ShopItem.Item it = ShopItem.Instance.itemlist[cd.item[i]];
                // 원본의 주소를 가져오므로 변경해서는 myItem을 변경해서는 안됨.
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = it;
                itemicon[i].sprite = Resources.Load<Sprite>("Item_Image/" + it.icon_name);
                itemicon[i].color = Color.white;
            }
            else
            {
                itemicon[i].gameObject.GetComponent<ItemInfo>().myItem = null;
                itemicon[i].sprite = null;
                itemicon[i].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
            }
        }

        if (cd.accessoryItem != 0)
        {
            ShopItem.Item it = ShopItem.Instance.itemlist[cd.accessoryItem];
            itemicon[6].gameObject.GetComponent<ItemInfo>().myItem = it;
            itemicon[6].sprite = Resources.Load<Sprite>("Item_Image/" + it.icon_name);
            itemicon[6].color = Color.white;
        }
        else
        {
            itemicon[6].gameObject.GetComponent<ItemInfo>().myItem = null;
            itemicon[6].sprite = null;
            itemicon[6].color = new Color(14f / 255f, 26f / 255f, 23f / 255f, 1);
        }
    }

    public void HealthBarUpdate()
    {
        healthBar.value = stat.Hp / stat.MaxHp;
        healthBar.text = Mathf.FloorToInt(stat.Hp).ToString() + " / " + Mathf.FloorToInt(stat.MaxHp).ToString();

        manaBar.value = stat.Mp / stat.MaxMp;
        manaBar.text = Mathf.FloorToInt(stat.Mp).ToString() + " / " + Mathf.FloorToInt(stat.MaxMp).ToString();
    }
}
