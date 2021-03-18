using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneCanvas : MonoBehaviour
{
    private GameObject m_Phone;
    private Transform m_PhoneTrans;

    private bool m_bUsePhone;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, false);

        if(!m_Phone) m_Phone = GameObject.Find("PhoneUI");
        if (!m_PhoneTrans) m_PhoneTrans = m_Phone.transform;

        m_bUsePhone = false;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        if(Input.GetKeyDown(KeyCode.I))
            m_bUsePhone = !m_bUsePhone;

        if (m_bUsePhone)
            GoingUp();
        else
            GoingDown();
    }

    private void GoingUp()
    {
        if (m_PhoneTrans.position.y >= 275f)
        {
            Cursor.visible = true;
            return;
        }

        m_PhoneTrans.position += new Vector3(0f, 500f * Time.deltaTime, 0f);
    }

    private void GoingDown()
    {
        if (m_PhoneTrans.position.y <= -300f)
        {
            Cursor.visible = false;
            return;
        }

        m_PhoneTrans.position -= new Vector3(0f, 500f* Time.deltaTime, 0f);
    }
}
