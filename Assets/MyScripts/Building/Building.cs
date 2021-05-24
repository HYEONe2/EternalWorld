using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BUILD { BUILDING, BUILT, BUILD_END };
    public enum BUILDING { BUILDING_WOOD, BUILDING_STONE, BUILDING_BRICK, BUILDING_COPPER, BUILDING_BRONZE, BUILDING_IRON,
        BUILDING_HEALHP, BUILDING_BUFFHP, BUILDING_BUFFCOOL, BUILDING_BUFFSTR, BUILDING_END };

    [System.Serializable]
    public struct BuildingInfo
    {
        public BUILDING m_eBuildingType;
        public PlayerProperty.OBJTYPE m_eMaterialType;
        public PlayerProperty.OBJTYPE m_eUpgradeType;
        public PlayerProperty.OBJTYPE m_eProductType;

        public float m_BuildTime;
        public int m_BuildAmount;
        public int m_UpgradeAmount;
        public float m_ReduceTime;
        public int m_CreateAmount;
        public int m_Exp;
    };

    [SerializeField] private float m_LocationY = 0;
    [SerializeField] private float m_RotationY = 0;

    private BUILD m_eBuild;
    private BuildingInfo m_Info;

    private UIManager m_UIManager;
    private Transform m_BuildingArmTrans;
    private Transform m_CamArmTrans;
    private PlayerProperty m_PlayerProperty;
    private GameObject m_Mark;

    /*[SerializeField]*/ private List<Material> m_MeshMaterialList = new List<Material>();
    private List<Color> m_MeshOriginColorList = new List<Color>();
    //private Color m_PingpongColor;

    private GameObject m_BuildingGuide;
    private RaycastHit m_Hit;
    private float m_BoundaryY;
    private bool m_bOnceBuilt;

    private bool m_bIsLandmark;
    private float m_CheckTime;
    private bool m_bCanTakeReward;
    private bool m_bIsClicked;

    public void SetBuild(BUILD build) { m_eBuild = build; if (m_eBuild == BUILD.BUILDING) m_BuildingGuide.SetActive(true); }
    public void SetBuildingInfo(BuildingInfo info) { m_Info = info; }
    public void SetCheckTime(float time) { m_CheckTime = time; }
    public void SetIsClicked(bool clicked) { m_bIsClicked = clicked; }

    public BUILD GetBuild() { return m_eBuild; }
    public BuildingInfo GetBuildingInfo() { return m_Info; }
    public float GetCheckTime() { return m_CheckTime; }

    // Start is called before the first frame update
    void Start()
    {
        m_eBuild = BUILD.BUILDING;

        GameObject Player = GameObject.Find("Player");
        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        m_BuildingArmTrans = Player.transform.Find("BuildingArm");
        m_CamArmTrans = Player.transform.Find("CameraArm");
        m_PlayerProperty = Player.GetComponent<PlayerProperty>();

        m_bOnceBuilt = false;

        if (m_Info.m_eBuildingType == BUILDING.BUILDING_HEALHP ||
            m_Info.m_eBuildingType == BUILDING.BUILDING_BUFFHP ||
            m_Info.m_eBuildingType == BUILDING.BUILDING_BUFFCOOL ||
            m_Info.m_eBuildingType == BUILDING.BUILDING_BUFFSTR)
            m_bIsLandmark = true;
        else
            m_bIsLandmark = false;

        if (!m_bIsLandmark)
        {
            m_Mark = transform.Find("Mark").gameObject;
            m_Mark.SetActive(false);
        }
        else
            GameObject.Find("ObjectManager").GetComponent<ObjectManager>().SetLandmark(this);

        foreach(Material material in transform.GetChild(0).GetComponent<MeshRenderer>().materials)
        {
            m_MeshMaterialList.Add(material);
            m_MeshOriginColorList.Add(material.color);
        }

        m_BuildingGuide = transform.Find("BuildingGuide").gameObject;

        Vector3 Look = GameObject.Find("Player").transform.position - m_CamArmTrans.transform.position;
        Look.y = 0;
        Look.Normalize();
        transform.rotation = Quaternion.LookRotation(-m_BuildingArmTrans.forward);
        transform.Rotate(new Vector3(0, m_RotationY, 0));
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_eBuild)
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
        if (!m_bCanTakeReward || m_bIsClicked)
            return;

        if (other.gameObject.CompareTag("Player"))
        {
            if (Input.GetKey(KeyCode.F))
            {
                m_PlayerProperty.AddProperty(m_Info.m_eProductType, m_Info.m_CreateAmount);
                m_UIManager.SetNoticeUI(m_Info.m_eProductType, m_Info.m_CreateAmount);
                m_PlayerProperty.AddExperience(m_Info.m_Exp * m_Info.m_UpgradeAmount);

                SetOriginOpaque();
                if(m_Mark)
                  m_Mark.SetActive(false);

                m_CheckTime = 0;
                m_bCanTakeReward = false;
            }
        }
    }

    private void UpdateBuilding()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_PlayerProperty.AddProperty(m_Info.m_eMaterialType, m_Info.m_BuildAmount, true);
            m_UIManager.SetPlayerRebuild(false);
            SetDestroy();
            return;
        }

        SetTransparent(0.7f);
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
        m_BuildingGuide.SetActive(false);

        Destroy(gameObject);
    }

    public void SetBuilt()
    {
        SoundManager.PlayEffectSound(SoundManager.TYPE.TYPE_UI, GetComponent<AudioSource>(), 2);
        m_UIManager.SetPhoneCanvasActive(true);

        SetOriginOpaque();
        m_BuildingGuide.SetActive(false);

        Vector3 position = m_BuildingGuide.transform.position;
        position.y += m_LocationY;
        transform.position = position;
        transform.SetParent(GameObject.Find("BuildingManager").transform);

        m_UIManager.SetPlayerRebuild(false);
        m_UIManager.SetRebuild(true);

        if(!m_bOnceBuilt)
            m_PlayerProperty.AddExperience(m_Info.m_Exp * 10);

        m_bOnceBuilt = true;
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
            if (!m_Hit.collider.CompareTag("Untagged"))
                m_BoundaryY = transform.position.y - m_Hit.distance;
        }

        m_BuildingGuide.transform.position = new Vector3(NewPos.x, m_BoundaryY + 0.1f, NewPos.z);
    }

    private void BuildingRotation()
    {
        float Scroll = Input.GetAxis("Mouse ScrollWheel") * 30f;
        Quaternion Rotation = transform.rotation;

        if (Scroll != 0f)
            transform.Rotate(new Vector3(0f, Scroll, 0f));
    }

    /// UpdateBuilt ///
    private void SetTimer()
    {
        if (m_CheckTime >= m_Info.m_BuildTime)
        {
            if(!m_bIsLandmark)
                m_bCanTakeReward = true;

            if (m_Mark)
                m_Mark.SetActive(true);
        }
        else
            m_CheckTime += Time.deltaTime;
    }
    
    private void SetOriginOpaque()
    {
        for (int i = 0; i < m_MeshMaterialList.Count; ++i)
        {
            Material tempMaterial = m_MeshMaterialList[i];

            tempMaterial.SetFloat("_Mode", 0f);
            tempMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            tempMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            tempMaterial.SetInt("_ZWrite", 1);
            tempMaterial.DisableKeyword("_ALPHATEST_ON");
            tempMaterial.DisableKeyword("_ALPHABLEND_ON");
            tempMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            tempMaterial.renderQueue = -1;

            tempMaterial.color = m_MeshOriginColorList[i];
        }
    }

    private void SetTransparent(float alpha)
    {
        for (int i = 0; i < m_MeshMaterialList.Count; ++i)
        {
            Material tempMaterial = m_MeshMaterialList[i];

            tempMaterial.SetFloat("_Mode", 3f);
            tempMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            tempMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            tempMaterial.SetInt("_ZWrite", 0);
            tempMaterial.DisableKeyword("_ALPHATEST_ON");
            tempMaterial.DisableKeyword("_ALPHABLEND_ON");
            tempMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
            tempMaterial.renderQueue = 3000;

            tempMaterial.color = new Color(m_MeshOriginColorList[i].r, m_MeshOriginColorList[i].g, m_MeshOriginColorList[i].b, alpha);
        }
    }

    public void ClickedToUpdate()
    {
        SetTransparent(0.5f);
        m_bIsClicked = true;
    }

    public void ResetToOrigin()
    {
        m_bIsClicked = false;

        SetOriginOpaque();
    }
}