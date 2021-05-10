using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProperty : MonoBehaviour
{
    public enum OBJTYPE
    {
        OBJ_WOOD,
        OBJ_STONE,
        OBJ_BRICK,
        OBJ_COPPER,
        OBJ_BRONZE,
        OBJ_IRON,
        OBJ_REDGEMSTONE,
        OBJ_GREENGEMSTONE,
        OBJ_BLUEGEMSTONE,
        OBJ_END
    };

    // Values
    public struct PlayerStat
    {
        public int m_Level;
        public int m_Exp;
        public int m_MaxExp;
        public int m_HP;
        public int m_Str;
        public int m_CoolTime;
    }
    private int m_ExtraHP;
    private int m_ExtraCoolTime;

    // Quest
    private int m_ClearDungeon;
    private int m_UpgradeCount;

    private ObjectManager.ABILITY m_eAbility;
    private PlayerStat m_PlayerStat;

    private List<int> m_Property = new List<int>();
    private int m_Coin;

    public void AddProperty(OBJTYPE eType, int amount, bool bUseShop = false) {
        if(!bUseShop)
            Instantiate(Resources.Load<GameObject>("Particle/Player/magic_ring_03"), transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
        m_Property[(int)eType] += amount;
    }
    public void ReduceProperty(OBJTYPE eType, int amount) { m_Property[(int)eType] -= amount; }
    public int GetPropertyAmount(OBJTYPE eType) { return m_Property[(int)eType]; }

    public PlayerStat GetPlayerStat() { return m_PlayerStat; }
    public int GetCoin() { return m_Coin; }
    public int GetClearDungeon() { return m_ClearDungeon; }
    public int GetUpgradeCount() { return m_UpgradeCount; }

    public void SetCoin(int coin) { m_Coin = coin; }
    public void SetHP(int hp) { if(50 * m_PlayerStat.m_Level > m_PlayerStat.m_HP )m_PlayerStat.m_HP = hp; }

    public void AddStr(int str) { m_PlayerStat.m_Str += str; }
    public void AddExtraHP(int extra) { m_ExtraHP += extra; }
    public void AddExtraCooltime(int extra) { m_ExtraCoolTime += extra; }
    public void AddClearDungeon() { ++m_ClearDungeon; } 
    public void AddUpgradeCount() { ++m_UpgradeCount; }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void OnDestroy()
    {
        m_Property.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown("7"))
            m_PlayerStat.m_Level += 1;
        if(Input.GetKeyDown("6"))
        {
            for (int i = 0; i < (int)OBJTYPE.OBJ_END; ++i)
                m_Property[i] += 10;
        }
        if (Input.GetKeyDown(KeyCode.K))
            m_PlayerStat.m_HP = 1;

        if (m_eAbility == ObjectManager.ABILITY.ABIL_END)
            m_eAbility = GetComponent<Player>().GetAbility();
    }

    private void Initialize()
    {
        m_eAbility = GetComponent<Player>().GetAbility();

        m_PlayerStat.m_Level = 1;
        m_PlayerStat.m_Exp = 0;
        m_PlayerStat.m_MaxExp = 100;
        m_PlayerStat.m_HP = 1;
        m_PlayerStat.m_Str = 1;
        m_PlayerStat.m_CoolTime = 0;

        m_ExtraHP = 0;
        m_ExtraCoolTime = 0;

        m_ClearDungeon = 0;

        for (int i = 0; i < (int)OBJTYPE.OBJ_END; ++i)
            m_Property.Add(0);
        m_Coin = 500;
    }

    public void AddExperience(int exp)
    {
        if (SceneManager.GetActiveScene().name == "TutorialScene")
            return;

        m_PlayerStat.m_Exp += exp;

        if (m_PlayerStat.m_Exp >= m_PlayerStat.m_MaxExp * m_PlayerStat.m_Level)
        {
            ObjectManager.ABILITY eAbility = GetComponent<Player>().GetAbility();

            if (eAbility == ObjectManager.ABILITY.ABIL_FIRE)
                Instantiate(Resources.Load<GameObject>("Particle/Player/Fire/magic_ring_02"), transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            else if (eAbility == ObjectManager.ABILITY.ABIL_GRASS)
                Instantiate(Resources.Load<GameObject>("Particle/Player/Grass/magic_ring_04"), transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));
            else if (eAbility == ObjectManager.ABILITY.ABIL_WATER)
                Instantiate(Resources.Load<GameObject>("Particle/Player/Water/magic_ring_01"), transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

            m_PlayerStat.m_Exp = m_PlayerStat.m_Exp - m_PlayerStat.m_MaxExp * m_PlayerStat.m_Level;
            ++m_PlayerStat.m_Level;

            m_PlayerStat.m_HP = 50 * m_PlayerStat.m_Level + m_ExtraHP;
            ++m_PlayerStat.m_Str;

            m_PlayerStat.m_CoolTime = 10 * (m_PlayerStat.m_Level - 1) + m_ExtraCoolTime;
            GetComponent<Player>().SetCoolPercent((100 - m_PlayerStat.m_CoolTime) / 100);
        }
    }

    public void SetDamaged(ObjectManager.ABILITY eAbility, int damage)
    {
        Player player = GetComponent<Player>();
        if (player.GetDamaged())
            return;

        int damagePercent = 0;
        switch(m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_FIRE:
                {
                    if (eAbility == ObjectManager.ABILITY.ABIL_FIRE)
                        damagePercent = 2;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_GRASS)
                        damagePercent = 1;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_WATER)
                        damagePercent = 3;
                }
                break;
            case ObjectManager.ABILITY.ABIL_GRASS:
                {
                    if (eAbility == ObjectManager.ABILITY.ABIL_FIRE)
                        damagePercent = 3;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_GRASS)
                        damagePercent = 2;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_WATER)
                        damagePercent = 1;
                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {
                    if (eAbility == ObjectManager.ABILITY.ABIL_FIRE)
                        damagePercent = 1;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_GRASS)
                        damagePercent = 3;
                    else if (eAbility == ObjectManager.ABILITY.ABIL_WATER)
                        damagePercent = 2;
                }
                break;
            default:
                damagePercent = 10;
                break;
        }

        m_PlayerStat.m_HP -= damage * damagePercent;
        player.SetAniDamaged();
    }

    public void SetRebirth()
    {
        GetComponent<Player>().ResetAnimator();

        m_PlayerStat.m_Exp = 0;
        m_PlayerStat.m_HP = 50 * m_PlayerStat.m_Level + m_ExtraHP;
    }
}
