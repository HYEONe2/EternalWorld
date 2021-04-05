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

    private int m_Level;
    private int m_Exp;
    private int m_MaxExp;

    private List<int> m_Property = new List<int>();
    private int m_Coin;

    public void AddProperty(OBJTYPE eType, int amount) { m_Property[(int)eType] += amount; }
    public void ReduceProperty(OBJTYPE eType, int amount) { m_Property[(int)eType] -= amount; }
    public int GetPropertyAmount(OBJTYPE eType) { return m_Property[(int)eType]; }

    public int GetCoin() { return m_Coin; }
    public void SetCoin(int coin) { m_Coin = coin; }
    public int GetLevel() { return m_Level; }
    public int GetExp() { return m_Exp; }
    public int GetMaxExp() { return m_MaxExp; }

    // Start is called before the first frame update
    void Start()
    {
        m_Level = 1;
        m_Exp = 0;
        m_MaxExp = 100;

        for (int i = 0; i < (int)OBJTYPE.OBJ_END; ++i)
            m_Property.Add(0);
        m_Coin = 500;
    }

    void Update()
    {
        if (Input.GetKeyDown("9"))
            m_Level += 1;
    }

    private void OnDestroy()
    {
        m_Property.Clear();
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
        }

        Debug.Log(m_Level + ": " + m_Exp);
    }
}
