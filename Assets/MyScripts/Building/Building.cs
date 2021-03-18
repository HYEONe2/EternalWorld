using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    private enum BUILD{ BUILDING, BUILT, BUILD_END};
    public enum BUILDING { BUILDING_WOOD, BUILDING_STONE, BUILDING_END };

    private BUILD m_eBuild;
    private BUILDING m_eBuildingType;

    private float m_BuildTime;
    private int m_BuildAmount;
    private int m_UpgradeAmount;

    private Material m_MeshMaterial;
    private Color m_MeshOriginColor;
    private Color m_PingpongColor;

    private GameObject m_BuildingGuide;
    private RaycastHit m_Hit;
    private float m_BoundaryY;

    private UIManager m_UIManager;
    private Transform m_BuildingArmTrans;
    private Transform m_CamArmTrans;
    private PlayerProperty m_PlayerProperty;

    private float m_CheckTime;
    private bool m_bCanTakeReward;

    public void SetBuildingType(BUILDING type) { m_eBuildingType = type; }
    public void SetBuildTime(float time) { m_BuildTime = time; }
    public void SetBuildAmount(int amount) { m_BuildAmount = amount; }
    public void SetUpgradeAmount(int amount) { m_UpgradeAmount = amount; }

    public BUILDING GetBuildingType() { return m_eBuildingType; }
    public float GetBuildTime() { return m_BuildTime; }

    // Start is called before the first frame update
    void Start()
    {
        m_eBuild = BUILD.BUILDING;

        m_MeshMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        m_MeshOriginColor = m_MeshMaterial.color;

        m_BuildingGuide = Instantiate(Resources.Load<GameObject>("Object/Building/BuildingGuide"), new Vector3(0,0,0), new Quaternion(0,0,0,0));
        m_BuildingGuide.transform.SetParent(this.transform);

        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        m_BuildingArmTrans = GameObject.Find("Player").transform.Find("BuildingArm");
        m_CamArmTrans = GameObject.Find("Player").transform.Find("CameraArm");
        m_PlayerProperty = GameObject.Find("Player").GetComponent<PlayerProperty>();

        Vector3 Look = GameObject.Find("Player").transform.position - m_CamArmTrans.transform.position;
        Look.y = 0;
        Look.Normalize();
        transform.localRotation = Quaternion.LookRotation(Look);
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
                UpdateBuilt();
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_bCanTakeReward)
            return;

        if(other.gameObject.CompareTag("Player"))
        {
            if(Input.GetKey(KeyCode.E))
            {
                m_PlayerProperty.AddProperty((PlayerProperty.OBJTYPE)m_eBuildingType, 1);
                m_UIManager.SetNoticeUI((NoticeUI.OBJTYPE)m_eBuildingType, 1);
                SetOriginOpaque();

                m_CheckTime = 0;
                m_bCanTakeReward = false;
            }
        }
    }

    private void UpdateBuilding()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetDestroy();
            return;
        }

        SetAlpha(0.7f);
        BuildingMovement();
        BuildingRotation();
    }

    private void UpdateBuilt()
    {
        SetTimer();
    }

    private void SetDestroy()
    {
        m_UIManager.SetPhoneCanvasActive(true);
        Destroy(m_BuildingGuide.gameObject);
        Destroy(gameObject);
    }

    public void SetBuilt()
    {
        m_UIManager.SetPhoneCanvasActive(true);

        transform.position = m_BuildingGuide.transform.position;
        SetAlpha(1f);
        transform.SetParent(null);
        m_BuildingGuide.SetActive(false);

        // 플레이어 프로퍼티 감소
        // 개수 BuildPanel에서 가져와서 줄여주기..!
        m_PlayerProperty.ReduceProperty((PlayerProperty.OBJTYPE)m_eBuildingType, m_BuildAmount);

        m_eBuild = BUILD.BUILT;
    }

    /// Update Buliding ///
    private void BuildingMovement()
    {
        Vector3 PlayerPos = m_BuildingArmTrans.transform.position;
        Vector3 CamLook = m_CamArmTrans.forward;
        Vector3 camAngle = m_CamArmTrans.rotation.eulerAngles;

        Vector3 NewPos = PlayerPos + CamLook * 7f;
        NewPos.y = PlayerPos.y + 1f;

        transform.position = NewPos;
        m_BuildingArmTrans.rotation = Quaternion.Euler(0f, camAngle.y, camAngle.z);

        if (Physics.Raycast(transform.position, -transform.up, out m_Hit))
        {
            //Debug.Log(m_Hit.collider.tag);

            if (!m_Hit.collider.CompareTag("Untagged"))
                m_BoundaryY = transform.position.y - m_Hit.distance;
            Debug.DrawRay(transform.position, -transform.up * m_Hit.distance, Color.red);
        }

        m_BuildingGuide.transform.position = new Vector3(NewPos.x, m_BoundaryY, NewPos.z);
    }

    private void BuildingRotation()
    {
        float Scroll = Input.GetAxis("Mouse ScrollWheel") * 30f;
        Quaternion Rotation = transform.rotation;

        if (Scroll != 0f)
        {
            transform.Rotate(new Vector3(0f, Scroll, 0f));
            m_BuildingGuide.transform.Rotate(new Vector3(0f, Scroll, 0f));
        }
    }

    /// UpdateBuilt ///
    private void SetTimer()
    {
        if (m_CheckTime >= m_BuildTime)
        {
            m_bCanTakeReward = true;
            SetPingPongTransparent();
            //m_CheckTime = 0;  재화습득후 초기화 시켜줘야함
        }
        else
            m_CheckTime += Time.deltaTime;
    }

    /// COLOR ///
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

    private void SetPingPongTransparent()
    {
        m_MeshMaterial.SetFloat("_Mode", 3f);
        m_MeshMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        m_MeshMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m_MeshMaterial.SetInt("_ZWrite", 0);
        m_MeshMaterial.DisableKeyword("_ALPHATEST_ON");
        m_MeshMaterial.DisableKeyword("_ALPHABLEND_ON");
        m_MeshMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        m_MeshMaterial.renderQueue = 3000;

        m_PingpongColor = Color.Lerp(new Color(0, 0, 1f, 0), new Color(0, 0, 1f, 1f), Mathf.PingPong(Time.time, 1f));
        m_MeshMaterial.color = m_PingpongColor;
    }
}

