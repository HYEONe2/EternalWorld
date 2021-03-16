using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private enum BUILD{ BUILDING, BUILT, BUILD_END};

    private BUILD m_eBuild;
    private Material m_MeshMaterial;
    private Color m_MeshOriginColor;

    private UIManager m_UIManager;
    private Transform m_BuildingArmTrans;
    private Transform m_CamArmTrans;

    // Start is called before the first frame update
    void Start()
    {
        m_eBuild = BUILD.BUILDING;

        m_MeshMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        m_MeshOriginColor = m_MeshMaterial.color;

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        m_BuildingArmTrans = GameObject.Find("Player").transform.Find("BuildingArm");
        m_CamArmTrans = GameObject.Find("Player").transform.Find("CameraArm");
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_eBuild)
        {
            case BUILD.BUILDING:
                UpdateBuilding();
                break;
            case BUILD.BUILT:
                break;
        }
    }

    private void UpdateBuilding()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_UIManager.SetPhoneCanvasActive(true);
            Destroy(gameObject);
            return;
        }

        SetAlpha(0.7f);
        BuildingMovement();
        BuildingRotation();
    }

    private void BuildingMovement()
    {
        Vector3 PlayerPos = m_BuildingArmTrans.transform.position;
        Vector3 CamLook = m_CamArmTrans.forward;
        Vector3 camAngle = m_CamArmTrans.rotation.eulerAngles;

        Vector3 NewPos = PlayerPos + CamLook * 7f;
        NewPos.y = PlayerPos.y + 1f;

        transform.position = NewPos;
        m_BuildingArmTrans.rotation = Quaternion.Euler(0f, camAngle.y, camAngle.z);
    }

    private void BuildingRotation()
    {
        float Scroll = Input.GetAxis("Mouse ScrollWheel") * 30f;
        Quaternion Rotation = transform.rotation;

        if(Scroll != 0f)
            transform.Rotate(new Vector3(0f, Scroll, 0f));
    }

    private void SetAlpha(float alpha)
    {
        if (alpha == m_MeshMaterial.color.a)
            return;

        if (alpha < 1f)
            SetTransparent(alpha);
        else
            SetOriginOpaque();
    }

    private void SetOriginOpaque()
    {
        m_MeshMaterial.SetFloat("_Mode", 0f);
        m_MeshMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        m_MeshMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        m_MeshMaterial.SetInt("_ZWrite", 1);
        m_MeshMaterial.DisableKeyword("_ALPHATEST_ON");
        m_MeshMaterial.DisableKeyword("_ALPHABLEND_ON");
        m_MeshMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m_MeshMaterial.renderQueue = -1;

        m_MeshMaterial.color = m_MeshOriginColor;
    }

    private void SetTransparent(float alpha)
    {
        m_MeshMaterial.SetFloat("_Mode", 3f);
        m_MeshMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        m_MeshMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m_MeshMaterial.SetInt("_ZWrite", 0);
        m_MeshMaterial.DisableKeyword("_ALPHATEST_ON");
        m_MeshMaterial.DisableKeyword("_ALPHABLEND_ON");
        m_MeshMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        m_MeshMaterial.renderQueue = 3000;

        m_MeshMaterial.color = new Color(m_MeshOriginColor.r, m_MeshOriginColor.g, m_MeshOriginColor.b, alpha);
    }
}
