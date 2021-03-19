using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeUI : MonoBehaviour
{
    private Image m_NoticeIcon;
    private Text m_NoticeText;

    // Function
    public void SetNoticeUI(PlayerProperty.OBJTYPE eType, int amount)
    {
        switch(eType)
        {
            case PlayerProperty.OBJTYPE.OBJ_WOOD:
                {
                    m_NoticeIcon.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Wooden Plank");
                }
                break;
            case PlayerProperty.OBJTYPE.OBJ_STONE:
                {
                    m_NoticeIcon.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Coal");
                }
                break;
            case PlayerProperty.OBJTYPE.OBJ_REDGEMSTONE:
                {
                    m_NoticeIcon.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Cut Ruby");
                }
                break;
            case PlayerProperty.OBJTYPE.OBJ_BLUEGEMSTONE:
                {
                    m_NoticeIcon.sprite = Resources.Load<Sprite>("UI/PlayerCanvas/Cut Sapphire");
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
