using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommodityText : MonoBehaviour
{
    public enum OBJTYPE
    {
        OBJ_WOOD,
        OBJ_STONE,
        OBJ_END
    };

    public OBJTYPE m_eType;
    private Text m_Text;

    private PlayerProperty m_PlayerProperty;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_Text) m_Text = gameObject.GetComponent<Text>();
        if (!m_PlayerProperty) m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();
    }

    // Update is called once per frame
    void Update()
    {
        m_Text.text = "X" + m_PlayerProperty.GetPropertyAmount((PlayerProperty.OBJTYPE)(m_eType));
    }
}
