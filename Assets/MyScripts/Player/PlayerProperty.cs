using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProperty : MonoBehaviour
{
    public enum OBJTYPE
    {
        OBJ_WOOD,
        OBJ_STONE,
        OBJ_END
    };

    private List<int> m_Property = new List<int>();

    public void AddProperty(OBJTYPE eType, int amount) { m_Property[(int)eType] += amount; }
    public void ReduceProperty(OBJTYPE eType, int amount) { m_Property[(int)eType] -= amount; }
    public int GetPropertyAmount(OBJTYPE eType) { return m_Property[(int)eType]; }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < (int)OBJTYPE.OBJ_END; ++i)
            m_Property.Add(0);
    }
}
