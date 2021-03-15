using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // My Components
    private CharacterController m_Controller;
    public GameObject m_Axe;
    //private Transform m_EquipPoint;

    // Child Components
    private GameObject m_Mesh;
    private Transform m_MeshTrans;
    private Animator m_Animator;

    private Transform m_CamArmTrans;

    // Other Components
    private GameObject m_NearObject;

    // Values
    private Vector3 m_Pos;

    private float m_MoveSpeed;
    private float m_RotateSpeed;

    private float m_Gravity;
    private bool m_bJump;
    private float m_JumpPower;
    private bool m_bSprint;

    private bool[] m_bInitialEquip = new bool[2];
    private bool m_bSwingAxe;

    // Function
    public void SetNearObject(GameObject nearObj) { m_NearObject = nearObj; }
    public void SetSwingAxe(bool bSwing) { m_bSwingAxe = bSwing; }

    public GameObject GetNearObject() { return m_NearObject; }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeValues();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        //UpdateKeyInput();
        UpdateAction();
        UpdateInteraction();
    }

    private void FixedUpdate()
    {

    }

    void InitializeComponents()
    {
        // My Components
        if (!m_Controller) m_Controller = gameObject.GetComponent<CharacterController>();
        if(!m_Axe) m_Axe = transform.Find("Axe").gameObject;
        //if (!m_EquipPoint) m_EquipPoint = transform.Find("EquipPoint");

        // Child Components
        if (!m_Mesh) m_Mesh = transform.Find("PlayerMesh").gameObject;

        if (m_Mesh)
        {
            if (!m_MeshTrans) m_MeshTrans = m_Mesh.GetComponent<Transform>();
            if (!m_Animator) m_Animator = m_Mesh.GetComponent<Animator>();
        }

        if (!m_CamArmTrans) m_CamArmTrans = transform.Find("CameraArm");

        // Other Components
        if(!m_NearObject) m_NearObject = null;
    }

    void InitializeValues()
    {
        // Position
        m_Pos = Vector3.zero;

        // Speed
        m_MoveSpeed = 4.5f;
        m_RotateSpeed = 5f;

        // Jump
        m_Gravity = -9.8f;
        m_bJump = false;
        m_JumpPower = 3f;
        m_bSprint = false;

        // Equip
        m_bInitialEquip[0] = false;
        m_bInitialEquip[1] = false;
        m_bSwingAxe = false;
    }

    void UpdateMovement()
    {
        if (m_bSwingAxe)
            return;

        if (!m_Controller.isGrounded)
        {
            m_Pos.y += m_Gravity * Time.deltaTime * 0.8f;

            m_Animator.SetBool("IsGrounded", false);
        }
        else
        {
            Jump();

            m_Animator.SetBool("IsGrounded", true);
        }

        Move();

        m_Controller.Move(m_Pos * m_MoveSpeed * Time.deltaTime);
    }

    void UpdateKeyInput()
    {
        switch (Input.inputString)
        {
            default:
                break;
        }
    }

    void Move()
    {
        // Animation Initialize
        m_Animator.SetBool("UseShift", false);
        m_Animator.SetFloat("MoveSpeed", 0);

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;

        if (isMove)
        {
            Vector3 lookForward = new Vector3(m_CamArmTrans.forward.x, 0f, m_CamArmTrans.forward.z).normalized;
            Vector3 lookRight = new Vector3(m_CamArmTrans.right.x, 0f, m_CamArmTrans.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            if (moveInput.x == 0 && moveInput.y == 0)
                return;

            // Move
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (m_bSprint || !m_bJump)
                    Sprint(moveDir);
                else if (m_bSprint && m_bJump)
                    Sprint(moveDir);
                else
                    Walk(moveDir);
            }
            else
            {
                if (!m_bSprint || !m_bJump)
                    Walk(moveDir);
                else if (!m_bSprint && m_bJump)
                    Walk(moveDir);
                else
                    Sprint(moveDir);
            }

            // Rotate
            Quaternion newRotation = Quaternion.LookRotation(moveDir * m_MoveSpeed * Time.deltaTime);
            m_Mesh.transform.rotation = Quaternion.Slerp(m_Mesh.transform.rotation, newRotation, m_RotateSpeed * Time.deltaTime);
        }
    }

    void Walk(Vector3 moveDir)
    {
        m_Animator.SetBool("UseShift", false);
        m_Animator.SetFloat("MoveSpeed", m_MoveSpeed);

        m_Controller.Move(moveDir * m_MoveSpeed * Time.deltaTime);
        m_JumpPower = 3f;
    }

    void Sprint(Vector3 moveDir)
    {
        m_Animator.SetBool("UseShift", true);
        m_Animator.SetFloat("MoveSpeed", m_MoveSpeed * 1.8f);

        m_Controller.Move(moveDir * (m_MoveSpeed * 1.8f) * Time.deltaTime);
        m_JumpPower = 3.5f;
    }

    void Jump()
    {
        m_Animator.SetBool("UseSpace", false);
        m_bJump = false;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetBool("UseSpace", true);
            m_bJump = true;

            m_Pos.y = m_JumpPower;

            if (Input.GetKey(KeyCode.LeftShift))
                m_bSprint = true;
            else
                m_bSprint = false;
        }
    }
    
    void UpdateAction()
    {
        SwingAxe();
    }

    void SwingAxe()
    {
        if (m_Animator.GetBool("UseSpace") || Cursor.visible)
            return;

        if (m_bInitialEquip[0] && m_Axe.activeSelf)
        {
            if(Input.GetMouseButtonDown(0))
            {
                m_bSwingAxe = true;
                m_Animator.SetBool("UseLButton", true);
            }
        }
    }

    void UpdateInteraction()
    {
        Equip();
        InitialEquip();
    }

    void InitialEquip()
    {
        if (!m_NearObject || m_bJump || !m_Controller.isGrounded || (m_bInitialEquip[0] && m_bInitialEquip[1]))
            return;

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(m_NearObject.tag == "Equipment")
            {
                Destroy(m_NearObject);

                if (m_Axe)
                {
                    m_Axe.SetActive(true);
                    m_bInitialEquip[0] = true;
                }
            }
        }
    }

    void Equip()
    {
        if (m_bInitialEquip[0])
        {
            if (m_Axe)
            {
                if (Input.GetKeyDown(KeyCode.X))
                {
                    if (m_Axe.activeSelf)
                        m_Axe.SetActive(false);
                    else
                        m_Axe.SetActive(true);
                }
            }
        }
    }
}
