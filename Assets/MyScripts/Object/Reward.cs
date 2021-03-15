using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reward : MonoBehaviour
{
    public enum OBJTYPE
    {
        OBJ_TREE,
        OBJ_ROCK,
        OBJ_END
    };

    private OBJTYPE m_eType;

    private PlayerProperty m_PlayerProperty;
    private PlayerCanvas m_PlayerCanvas;
    private NoticeUI m_NoticeUI;

    private bool m_bOnce;

    // Function
    public void SetObjType(OBJTYPE eType) { m_eType = eType; }

    // Start is called before the first frame update
    void Start()
    {
        if (!m_PlayerProperty) m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();
        if (!m_PlayerCanvas) m_PlayerCanvas = GameObject.Find("PlayerCanvas").GetComponent<PlayerCanvas>();
        if (!m_NoticeUI) m_NoticeUI = GameObject.Find("NoticeUI").GetComponent<NoticeUI>();

        m_bOnce = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!m_bOnce && Input.GetKeyDown(KeyCode.F))
            {
                // Player 재화 저장하는 내뇬 필요
                m_bOnce = true;

                int Amount = Random.Range(1, 10);
                m_PlayerProperty.AddProperty((global::PlayerProperty.OBJTYPE)(m_eType), Amount);
                m_NoticeUI.SetNoticeUI((global::NoticeUI.OBJTYPE)m_eType, Amount);
                m_PlayerCanvas.SetUseNoticeUI(true);

                Destroy(gameObject);
            }
        }
    }
}
