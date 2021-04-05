using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    // My Components
    private CharacterController m_Controller;
    public GameObject m_Equipment;
    public GameObject m_Weapon;
    private GameObject m_Target;

    // Child Components
    private GameObject m_Mesh;
    private Transform m_MeshTrans;
    private Animator m_Animator;

    private Transform m_CamArmTrans;

    // Other Components
    private GameObject m_NearObject;
    private GameObject m_MagicBall;
    private GameObject m_SkillObject;

    // Values
    public enum ABILITY { ABIL_FIRE, ABIL_WATER, ABIL_GRASS, ABIL_END};
    private ABILITY m_eAbility;
    private string m_SceneName;

    private Vector3 m_Pos;
    private float m_MoveSpeed;
    private float m_RotateSpeed;

    private float m_Gravity;
    private bool m_bJump;
    private float m_JumpPower;
    private bool m_bSprint;

    private bool[] m_bInitialEquip = new bool[2];
    private bool m_bSwing;
    private bool m_bUsePhone;

    // Function
    public void SetNearObject(GameObject nearObj) { m_NearObject = nearObj; }
    public void SetSkillObject(GameObject skillObj) { m_SkillObject = skillObj; }
    public void SetSwing(bool bSwing) { m_bSwing = bSwing; }
    public void SetUsePhone(bool use) { m_bUsePhone = use; }
    public void SetPosition(Vector3 pos) { m_Pos = pos; }
    public void SetSceneName(string name) { m_SceneName = name; }

    public GameObject GetNearObject() { return m_NearObject; }
    public GameObject GetSkillObject() { return m_SkillObject; }
    public GameObject GetTarget() { return m_Target; }

    public ABILITY GetAbility() { return m_eAbility; }
    public bool GetAttack() { return m_bSwing; }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeValues();
    }

    // Update is called once per frame
    void Update()
    {
        LateInit();
        if (Cursor.visible)
            ResetAnimator();

        UpdateMovement();
        UpdateKeyInput();
        UpdateAction();
        UpdateInteraction();

        //Debug.Log("Player: " +transform.position.x + "\t" + transform.position.y + '\t' + transform.position.z);
    }

    private void FixedUpdate()
    {

    }

    private void InitializeComponents()
    {
        // My Components
        if (!m_Controller) m_Controller = gameObject.GetComponent<CharacterController>();
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

    private void InitializeValues()
    {
        m_eAbility = ABILITY.ABIL_END;

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
        m_bSwing = false;
    }

    private void LateInit()
    {
        if (m_SceneName == SceneManager.GetActiveScene().name)
            return;

        if (m_Pos.y <= -10f)
        {
            m_SceneName = SceneManager.GetActiveScene().name;
            return;
        }

        switch (SceneManager.GetActiveScene().name)
        {
            case "MainScene":
                m_Pos.x = 0;
                m_Pos.z = 0;
                m_Pos.y += m_Gravity * Time.deltaTime * 0.8f;
                transform.position = new Vector3(0, m_Pos.y, 0);
                break;
        }
    }

    private void UpdateMovement()
    {
        if (m_bSwing)
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
            m_SceneName = SceneManager.GetActiveScene().name;
        }

        Move();

        if (m_SceneName == SceneManager.GetActiveScene().name)
            m_Controller.Move(m_Pos * m_MoveSpeed * Time.deltaTime);
        //else
        //    m_Controller.Move(new Vector3(0, -1f, 0) * m_MoveSpeed * Time.deltaTime);
    }

    private void UpdateKeyInput()
    {
        switch (Input.inputString)
        {
            //case "R":
            //case "r":
            //    SetAbility(ABILITY.ABIL_FIRE);
            //    break;
            //case "B":
            //case "b":
            //    SetAbility(ABILITY.ABIL_WATER);
            //    break;
            //case "G":
            //case "g":
            //    SetAbility(ABILITY.ABIL_GRASS);
            //    break;
            default:
                break;
        }
    }

    private void Move()
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
            if (!m_Target)
            {
                Quaternion newRotation = Quaternion.LookRotation(moveDir * m_MoveSpeed * Time.deltaTime);
                m_Mesh.transform.rotation = Quaternion.Slerp(m_Mesh.transform.rotation, newRotation, m_RotateSpeed * Time.deltaTime);
            }
        }
    }

    private void Walk(Vector3 moveDir)
    {
        m_Animator.SetBool("UseShift", false);
        m_Animator.SetFloat("MoveSpeed", m_MoveSpeed);

        m_Controller.Move(moveDir * m_MoveSpeed * Time.deltaTime);
        m_JumpPower = 3f;
    }

    private void Sprint(Vector3 moveDir)
    {
        m_Animator.SetBool("UseShift", true);
        m_Animator.SetFloat("MoveSpeed", m_MoveSpeed * 1.8f);

        m_Controller.Move(moveDir * (m_MoveSpeed * 1.8f) * Time.deltaTime);
        m_JumpPower = 3.5f;
    }

    private void Jump()
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
        if (m_Animator.GetBool("UseSpace") || Cursor.visible || m_bUsePhone || m_bSwing)
            return;

        Targeting();
        Swing();
        UseMagicSpell();
    }

    public void SetTargetMonster(GameObject target)
    {
        if (Input.GetMouseButton(1))
            m_Target = target;
    }

    private void Targeting()
    {
        if (!Input.GetMouseButton(1))
            m_Target = null;

        if (!m_Target)
        {
            m_MoveSpeed = 4.5f;
            return;
        }

        m_MoveSpeed = 4.5f * 2f;
        Vector3 vDir = m_Target.transform.position - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(vDir * m_MoveSpeed * Time.deltaTime);

        m_Mesh.transform.rotation = Quaternion.Slerp(m_Mesh.transform.rotation, newRotation, m_RotateSpeed * 3f * Time.deltaTime);
        m_CamArmTrans.rotation = Quaternion.Slerp(m_Mesh.transform.rotation, newRotation, m_RotateSpeed * 3f * Time.deltaTime);
    }

    private void Swing()
    {
        if (m_Equipment.activeSelf)
        {
            if(Input.GetMouseButtonDown(0))
            {
                m_bSwing = true;
                m_Animator.SetBool("UseLButton", true);
            }
        }
        else if(m_Weapon.activeSelf)
        {
            if(Input.GetMouseButton(0))
            {
                m_bSwing = true;
                m_Animator.SetBool("UseLButton", true);
            }
        }
    }

    private void UseMagicSpell()
    {
        if (!m_Weapon.activeSelf || m_SkillObject)
            return;

        if (Input.GetKey(KeyCode.Q))
        {
            m_bSwing = true;
            m_Animator.SetBool("UseLButton", true);

            Vector3 PlayerPos = transform.position;
            Vector3 PlayerLook = m_Mesh.transform.forward;
            
            Vector3 NewPos = PlayerPos + PlayerLook * 3f;
            NewPos.y = PlayerPos.y + 1f;

            m_SkillObject = Instantiate(m_MagicBall, NewPos, new Quaternion(0, 0, 0, 0));
            m_SkillObject.GetComponent<MagicBall>().SetLookVector(PlayerLook);
        }
    }

    private void UpdateInteraction()
    {
        Equip();
        InitialEquip();
    }

    private void InitialEquip()
    {
        if (!m_NearObject || m_bJump || !m_Controller.isGrounded || (m_bInitialEquip[0] && m_bInitialEquip[1]))
            return;

        if(Input.GetKeyDown(KeyCode.F))
        {
            if(m_NearObject.tag == "Equipment")
            {
                Destroy(m_NearObject);

                if (m_Equipment)
                {
                    if(!m_Weapon.activeSelf)
                        m_Equipment.SetActive(true);
                    m_bInitialEquip[0] = true;
                }
            }
            else if(m_NearObject.tag == "Weapon")
            {
                m_Weapon.GetComponent<MeshFilter>().sharedMesh = m_NearObject.GetComponent<Weapon>().GetMesh();
                Destroy(m_NearObject);

                if(m_Weapon)
                {
                    if (!m_Equipment.activeSelf)
                        m_Weapon.SetActive(true);
                    m_bInitialEquip[1] = true;
                }
            }
        }
    }

    private void Equip()
    {
        if (m_bSwing)
            return;

        if (m_bInitialEquip[0] && !m_Weapon.activeSelf)
        {
            if (m_Equipment)
            {
                if (Input.GetKeyDown("1"))
                {
                    if (m_Equipment.activeSelf)
                        m_Equipment.SetActive(false);
                    else
                        m_Equipment.SetActive(true);
                }
            }
        }

        if (m_bInitialEquip[1] && !m_Equipment.activeSelf)
        {
            if (m_Weapon)
            {
                if (Input.GetKeyDown("2"))
                {
                    if (m_Weapon.activeSelf)
                        m_Weapon.SetActive(false);
                    else
                        m_Weapon.SetActive(true);
                }
            }
        }
    }

    public void SetAbility(ABILITY eAbility)
    {
        m_eAbility = eAbility;

        switch(m_eAbility)
        {
            case ABILITY.ABIL_FIRE:
                {
                    m_MagicBall = Resources.Load<GameObject>("Particle/Player/Fire/ErekiBall2");
                }
                break;
            case ABILITY.ABIL_WATER:
                {
                    m_MagicBall = Resources.Load<GameObject>("Particle/Player/Water/ErekiBall2");
                }
                break;
            case ABILITY.ABIL_GRASS:
                {
                    m_MagicBall = Resources.Load<GameObject>("Particle/Player/Grass/ErekiBall2");
                }
                break;
        }
    }

    private void ResetAnimator()
    {
        m_bSwing = false;

        m_Animator.SetFloat("MoveSpeed", 0);
        m_Animator.SetBool("UseShift", false);
        m_Animator.SetBool("UseSpace", false);
        m_Animator.SetBool("IsGrounded", true);
        m_Animator.SetBool("UseLButton", false);
    }

    public bool GetActive(string objectName)
    {
        if (objectName == "Equipment")
            return m_Equipment.activeSelf;
        else if (objectName == "Weapon")
            return m_Weapon.activeSelf;
        else
            return false;
    }
}
