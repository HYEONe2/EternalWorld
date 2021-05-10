using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingCreditText : MonoBehaviour
{
    [SerializeField] float m_StartPoint = 0;
    [SerializeField] float m_EndPoint = 0;

    private Image m_Image;
    private Color m_OriginColor;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(960, m_StartPoint, 0);

        m_Image = transform.parent.GetChild(0).GetComponent<Image>();
        m_OriginColor = m_Image.color;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Pos = transform.position;

        if (Pos.y > m_EndPoint)
        {
            LoadingSceneManager.LoadScene("TitleScene");
            return;
        }

        m_OriginColor.a += 8f * Time.deltaTime;
        m_Image.color = m_OriginColor;

        Pos.y += 50f * Time.deltaTime;
        transform.position = Pos;
    }
}
