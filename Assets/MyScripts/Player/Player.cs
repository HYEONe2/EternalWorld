using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // My Components
    private CharacterController m_Controller;

    // Child Components
    private GameObject m_Mesh;
    private Transform m_MeshTrans;
    private Animator m_Animator;

    private Transform m_CamArmTrans;

    // Other Components

    // Values
    private Vector3 m_Pos;

    private float m_MoveSpeed;
    private float m_RotateSpeed;

    private float m_Gravity;
    private bool m_bJump;
    private float m_JumpPower;
    private bool m_bSprint;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, true);

        InitializeComponents();
        InitializeValues();
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateKeyInput();
        UpdateMovement();
    }

    private void FixedUpdate()
    {

    }

    void InitializeComponents()
    {
        // My Components
        if (!m_Controller) m_Controller = gameObject.GetComponent<CharacterController>();

        // Child Components
        if (!m_Mesh) m_Mesh = transform.Find("PlayerMesh").gameObject;

        if (m_Mesh)
        {
            if (!m_MeshTrans) m_MeshTrans = m_Mesh.GetComponent<Transform>();
            if (!m_Animator) m_Animator = m_Mesh.GetComponent<Animator>();
        }

        if (!m_CamArmTrans) m_CamArmTrans = transform.Find("CameraArm");
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
    }

    void UpdateMovement()
    {
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
}
