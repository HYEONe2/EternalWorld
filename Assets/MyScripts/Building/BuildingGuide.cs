using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGuide : MonoBehaviour
{
    private Material m_MeshMaterial;

    private bool m_bOtherTag;

    private Building m_Building;

    // Start is called before the first frame update
    void Start()
    {
        m_MeshMaterial = transform.GetComponent<MeshRenderer>().material;

        m_bOtherTag = false;

        m_Building = transform.parent.GetComponent<Building>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_bOtherTag)
        {
            SetColor(0f, 0f, 1f, 0.3f);

            if (Input.GetMouseButtonDown(0))
                m_Building.SetBuilt();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Floor"))
        {
            if (other.CompareTag("LevelBoundary")
                || other.CompareTag("TreeBoundary"))
                return;

            m_bOtherTag = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Floor"))
        {
            if (other.CompareTag("LevelBoundary")
                || other.CompareTag("TreeBoundary"))
                return;

            SetColor(1f, 0f, 0f, 0.3f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Floor"))
        {
            if (other.CompareTag("LevelBoundary")
                || other.CompareTag("TreeBoundary"))
                return;

            m_bOtherTag = false;
        }
    }

    private void SetColor(float r, float g, float b, float a)
    {
        m_MeshMaterial.SetFloat("_Mode", 3f);
        m_MeshMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        m_MeshMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m_MeshMaterial.SetInt("_ZWrite", 0);
        m_MeshMaterial.DisableKeyword("_ALPHATEST_ON");
        m_MeshMaterial.DisableKeyword("_ALPHABLEND_ON");
        m_MeshMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        m_MeshMaterial.renderQueue = 3000;

        m_MeshMaterial.color = new Color(r, g, b, a);
    }
}
