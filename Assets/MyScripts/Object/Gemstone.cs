using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gemstone : MonoBehaviour
{
    public enum COLOR { COLOR_RED, COLOR_BLUE, COLOR_GREEN, COLOR_END };
    public COLOR m_eColor;

    private PlayerProperty m_PlayerProperty;
    private UIManager m_UIManager;
    private bool m_bOnce;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_PlayerProperty) m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();
        if (!m_UIManager) m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        m_bOnce = false;

        SetColor();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!m_bOnce && Input.GetKey(KeyCode.F))
            {
                m_bOnce = true;

                m_PlayerProperty.AddProperty((PlayerProperty.OBJTYPE)(m_eColor + 2), 1);
                m_UIManager.SetNoticeUI((PlayerProperty.OBJTYPE)(m_eColor + 2), 1);

                Destroy(gameObject);
            }
        }
    }

    private void SetColor()
    {
        Material MeshMaterial = transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>().material;

        MeshMaterial.SetFloat("_Mode", 0f);
        MeshMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        MeshMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        MeshMaterial.SetInt("_ZWrite", 1);
        MeshMaterial.DisableKeyword("_ALPHATEST_ON");
        MeshMaterial.DisableKeyword("_ALPHABLEND_ON");
        MeshMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        MeshMaterial.renderQueue = -1;

        switch (m_eColor)
        {
            case COLOR.COLOR_RED:
                MeshMaterial.color = new Color(1f, 0f, 0f, 1f);
                break;
            case COLOR.COLOR_GREEN:
                MeshMaterial.color = new Color(0f, 1f, 0f, 1f);
                break;
            case COLOR.COLOR_BLUE:
                MeshMaterial.color = new Color(0f, 0f, 1f, 1f);
                break;
        }
    }
}
