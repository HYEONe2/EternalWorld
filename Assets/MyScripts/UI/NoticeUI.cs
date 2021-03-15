using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
    public enum OBJTYPE
    {
        OBJ_WOOD,
        OBJ_STONE,
        OBJ_END
    };

    private Image m_NoticeIcon;
    private Text m_NoticeText;

    // Function
    public void SetNoticeUI(OBJTYPE eType, int amount)
    {
        switch(eType)
        {
            case OBJTYPE.OBJ_WOOD:
                {
                    m_NoticeIcon.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Wooden Plank");
                }
                break;
            case OBJTYPE.OBJ_STONE:
                {
                    m_NoticeIcon.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Coal");
                }
                break;
        }

        m_NoticeText.text = "+" + amount;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!m_NoticeIcon) m_NoticeIcon = GameObject.Find("NoticeIcon").GetComponent<Image>();
        if (!m_NoticeText) m_NoticeText = GameObject.Find("NoticeText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
