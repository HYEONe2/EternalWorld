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
        OBJ_REDGEMSTONE,
        OBJ_BLUEGEMSTONE,
        OBJ_END
    };

    // Values
    private int m_Level;
    private int m_Exp;
    private int m_MaxExp;
    private int m_HP;
    private int m_Str;
    private int m_CoolTime;

    private List<int> m_Property = new List<int>();
    private int m_Coin;

    public void AddProperty(OBJTYPE eType, int amount) { m_Property[(int)eType] += amount; }
    public void ReduceProperty(OBJTYPE eType, int amount) { m_Property[(int)eType] -= amount; }
    public int GetPropertyAmount(OBJTYPE eType) { return m_Property[(int)eType]; }

    public int GetLevel() { return m_Level; }
    public int GetExp() { return m_Exp; }
    public int GetMaxExp() { return m_MaxExp; }
    public int GetHP() { return m_HP; }
    public int GetCoin() { return m_Coin; }
    public int GetStr() { return m_Str; }
    public int GetCoolTime() { return m_CoolTime; }

    public void SetCoin(int coin) { m_Coin = coin; }

    // Start is called before the first frame update
    void Start()
    {
        m_Level = 1;
        m_Exp = 0;
        m_MaxExp = 100;
        m_HP = 10;

        m_Str = 1;
        m_CoolTime = 0;

        for (int i = 0; i < (int)OBJTYPE.OBJ_END; ++i)
            m_Property.Add(0);
        m_Coin = 500;
        m_Property[1] = 10;
    }

    private void OnDestroy()
    {
        m_Property.Clear();
    }

    void Update()
    {
        if (Input.GetKeyDown("9"))
            m_Level += 1;
    }

    public void AddExperience(int exp)
    {
        if (SceneManager.GetActiveScene().name == "TutorialScene")
            return;

        m_Exp += exp;

        if(m_Exp >= m_MaxExp * m_Level)
        {
            m_Exp = m_MaxExp * m_Level - m_Exp;
            ++m_Level;

            ++m_Str;
            m_CoolTime = 10 * (m_Level - 1);
            GetComponent<Player>().SetCoolPercent((100 - m_CoolTime) / 100);
        }

        Debug.Log(m_Level + ": " + m_Exp);
    }

    public void SetDamaged(int damage)
    {
        Player player = GetComponent<Player>();
        if (player.GetDamaged())
            return;

        m_HP -= damage;
        player.SetAniDamaged();
    }
}
