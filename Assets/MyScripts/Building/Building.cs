using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private enum BUILD{ BUILDING, BUILT, BUILD_END};

    private BUILD m_eBuild;
    private Material m_MeshMaterial;
    private Color m_MeshOriginColor;

    // Start is called before the first frame update
    void Start()
    {
        m_eBuild = BUILD.BUILDING;

        //m_MeshMaterial = GetComponent<Material>();
        //m_MeshOriginColor = m_MeshMaterial.color;
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_eBuild)
        {
            case BUILD.BUILDING:
                break;
            case BUILD.BUILT:
                break;
        }
    }

    private void SetAlpha()
    {
        //m_MeshMaterial.color = new Color(m_MeshOriginColor.r, m_MeshOriginColor.g, m_MeshOriginColor.b, 50f);
    }
}
