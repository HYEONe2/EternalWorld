﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public enum BUILD { BUILDING, BUILT, BUILD_END };
    public enum BUILDING { BUILDING_WOOD, BUILDING_STONE, BUILDING_END };

    [System.Serializable]
    public struct BuildingInfo
    {
        public BUILDING m_eBuildingType;
        public float m_BuildTime;
        public int m_BuildAmount;
        public int m_UpgradeAmount;
        public int m_Exp;
    };

    private BUILD m_eBuild;
    private BuildingInfo m_Info;

    private UIManager m_UIManager;
    private Transform m_BuildingArmTrans;
    private Transform m_CamArmTrans;
    private PlayerProperty m_PlayerProperty;
    private GameObject m_Mark;

    /*[SerializeField]*/ private Material m_MeshMaterial;
    private Color m_MeshOriginColor;
    //private Color m_PingpongColor;

    private GameObject m_BuildingGuide;
    private RaycastHit m_Hit;
    private float m_BoundaryY;

    private float m_CheckTime;
    private bool m_bCanTakeReward;
    private bool m_bIsClicked;

    public void SetBuild(BUILD build) { m_eBuild = build; if (m_eBuild == BUILD.BUILDING) m_BuildingGuide.SetActive(true); }
    public void SetBuildingInfo(BuildingInfo info) { m_Info = info; }

    public BUILD GetBuild() { return m_eBuild; }
    public BuildingInfo GetBuildingInfo() { return m_Info; }
    public void SetIsClicked(bool clicked) { m_bIsClicked = clicked; }

    // Start is called before the first frame update
    void Start()
    {
        m_eBuild = BUILD.BUILDING;

        GameObject Player = GameObject.Find("Player");
        m_UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        m_BuildingArmTrans = Player.transform.Find("BuildingArm");
        m_CamArmTrans = Player.transform.Find("CameraArm");
        m_PlayerProperty = Player.GetComponent<PlayerProperty>();

        m_Mark = transform.Find("Mark").gameObject;
        m_Mark.SetActive(false);

        m_MeshMaterial = transform.GetChild(0).GetComponent<MeshRenderer>().material;
        m_MeshOriginColor = m_MeshMaterial.color;

        m_BuildingGuide = transform.Find("BuildingGuide").gameObject;

        Vector3 Look = GameObject.Find("Player").transform.position - m_CamArmTrans.transform.position;
        Look.y = 0;
        Look.Normalize();
        transform.rotation = Quaternion.LookRotation(-m_BuildingArmTrans.forward);
        transform.Rotate(new Vector3(0, -90f, 0));
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
                m_PlayerProperty.AddProperty((PlayerProperty.OBJTYPE)m_Info.m_eBuildingType, 1);
                m_UIManager.SetNoticeUI((PlayerProperty.OBJTYPE)m_Info.m_eBuildingType, 1);
                m_PlayerProperty.AddExperience(m_Info.m_Exp * m_Info.m_UpgradeAmount);

                SetOriginOpaque();
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
        m_UIManager.SetPhoneCanvasActive(true);

        SetOriginOpaque();
        m_BuildingGuide.SetActive(false);

        transform.position = m_BuildingGuide.transform.position;
        transform.SetParent(GameObject.Find("BuildingManager").transform);

        m_UIManager.SetPlayerRebuild(false);
        m_UIManager.SetRebuild(true);

        m_PlayerProperty.AddExperience(m_Info.m_Exp * 10);

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
            //Debug.DrawRay(transform.position, -transform.up * m_Hit.distance, Color.red);
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
            //m_BuildingGuide.transform.Rotate(new Vector3(0f, Scroll, 0f));
        }
    }

    /// UpdateBuilt ///
    private void SetTimer()
    {
        if (m_CheckTime >= m_Info.m_BuildTime)
        {
            m_bCanTakeReward = true;
            m_Mark.SetActive(true);

            //if (!m_bIsClicked)
            //    SetPingPongTransparent(0, 0, 1f, 1f);
            //else
        }
        else
            m_CheckTime += Time.deltaTime;
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

    //private void SetPingPongTransparent(float r, float g, float b, float timeSpeed)
    //{
    //    m_MeshMaterial.SetFloat("_Mode", 3f);
    //    m_MeshMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
    //    m_MeshMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
    //    m_MeshMaterial.SetInt("_ZWrite", 0);
    //    m_MeshMaterial.DisableKeyword("_ALPHATEST_ON");
    //    m_MeshMaterial.DisableKeyword("_ALPHABLEND_ON");
    //    m_MeshMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
    //    m_MeshMaterial.renderQueue = 3000;

    //    m_PingpongColor = Color.Lerp(new Color(r, g, b, 0), new Color(r, g, b, 1f), Mathf.PingPong(Time.time, timeSpeed));
    //    m_MeshMaterial.color = m_PingpongColor;
    //}

    public void ClickedToUpdate()
    {
        SetTransparent(0.5f);
        m_bIsClicked = true;
    }

    public void ResetToOrigin()
    {
        m_bIsClicked = false;

        //if (m_bCanTakeReward)
        //    SetPingPongTransparent(0, 0, 1f, 1f);
        //else
        //if (!m_bCanTakeReward)
        SetOriginOpaque();
    }
}