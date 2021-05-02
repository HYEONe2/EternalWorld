using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private GameObject m_FirstMagicSkill;
    private GameObject m_SecondMagicSkill;

    private GameObject[] m_SkillObject = new GameObject[3];
    private bool[] m_bSkillOn = new bool[2];
    private float[] m_CoolTime = new float[2];
    private float[] m_EndCoolTime = new float[2];
    private float m_CoolPercent;

    // Values
    private ObjectManager.ABILITY m_eAbility;

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

    // Animator values
    private readonly int m_bHashGrounded = Animator.StringToHash("IsGrounded");
    private readonly int m_bHashShift = Animator.StringToHash("UseShift");
    private readonly int m_HashMoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly int m_bHashSpace = Animator.StringToHash("UseSpace");
    private readonly int m_bHashLButton = Animator.StringToHash("UseLButton");
    private readonly int m_bHashDamaged = Animator.StringToHash("IsDamaged");

    // Function
    public void SetNearObject(GameObject nearObj) { m_NearObject = nearObj; }
    public void SetSkillObject(int index, GameObject skillObj) { m_SkillObject[index] = skillObj; }
    public void SetSwing(bool bSwing) { m_bSwing = bSwing; }
    public void SetUsePhone(bool use) { m_bUsePhone = use; }
    public void SetPosition(Vector3 pos) { m_Pos = pos; }
    public void SetCoolPercent(float percent) { m_CoolPercent = percent; }
    //public void SetSceneName(string name) { m_SceneName = name; }

    public GameObject GetNearObject() { return m_NearObject; }
    public GameObject GetSkillObject(int index) { return m_SkillObject[index]; }
    public GameObject GetTarget() { return m_Target; }
    public bool GetSkillOn(int index) { return m_bSkillOn[index]; }
    public float GetCoolTime(int index) { return m_CoolTime[index] / m_EndCoolTime[index]; }

    public ObjectManager.ABILITY GetAbility() { return m_eAbility; }
    public bool GetAttack() { return m_Animator.GetBool(m_bHashLButton); }
    public bool GetDamaged() { return m_Animator.GetBool(m_bHashDamaged); }
    public void SetAniDamaged() { m_Animator.SetBool(m_bHashDamaged, true); }

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
        InitializeValues();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < 3; ++i)
        {
            Destroy(m_SkillObject[i]);
            m_SkillObject[i] = null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Cursor.visible)
            ResetAnimator();

        UpdateMovement();

        if (CheckHP())
            return;

        UpdateKeyInput();
        UpdateAction();
        UpdateInteraction();
    }

    //private void FixedUpdate()
    //{

    //}

    private void OnTriggerEnter(Collider other)
    {
        if (m_Animator.GetBool(m_bHashDamaged)
            || (m_eAbility == ObjectManager.ABILITY.ABIL_WATER && m_SkillObject[2]))
            return;

        if (other.CompareTag("Monster"))
        {
            PlayerProperty playerProperty = GetComponent<PlayerProperty>();
            Monster monster = other.GetComponent<Monster>();

            if (monster)
                playerProperty.SetDamaged(monster.GetAbility(), playerProperty.GetLevel());
            else
                playerProperty.SetDamaged(m_eAbility, playerProperty.GetLevel());
        }
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
        if (!m_NearObject) m_NearObject = null;
    }

    private void InitializeValues()
    {
        m_eAbility = ObjectManager.ABILITY.ABIL_END;

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

        for (int i = 0; i < 2; ++i)
            m_CoolTime[i] = 0;

        m_EndCoolTime[0] = 5f;
        m_EndCoolTime[1] = 10f;
    }

    private bool CheckHP()
    {
        int HP = GetComponent<PlayerProperty>().GetHP();

        if (HP <= 0)
            return true;
        else
            return false;
    }

    private void UpdateMovement()
    {
        if (m_bSwing)
            return;

        if (!m_Controller.isGrounded)
        {
            m_Pos.y += m_Gravity * Time.deltaTime * 0.8f;
            m_Animator.SetBool(m_bHashGrounded, false);
        }
        else
        {
            Jump();
            m_Animator.SetBool(m_bHashGrounded, true);
        }

        Move();

        m_Controller.Move(m_Pos * m_MoveSpeed * Time.deltaTime);
    }

    private void UpdateKeyInput()
    {
        switch (Input.inputString)
        {
            case "C":
            case "c":
                SetAbility(ObjectManager.ABILITY.ABIL_FIRE);
                break;
            case "B":
            case "b":
                SetAbility(ObjectManager.ABILITY.ABIL_WATER);
                break;
            case "G":
            case "g":
                SetAbility(ObjectManager.ABILITY.ABIL_GRASS);
                break;
            default:
                break;
        }
    }

    private void Move()
    {
        if (m_Animator.GetBool(m_bHashDamaged) || GetComponent<PlayerProperty>().GetHP()<= 0)
            return;

        // Animation Initialize
        m_Animator.SetBool(m_bHashShift, false);
        m_Animator.SetFloat(m_HashMoveSpeed, 0);

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
        m_Animator.SetBool(m_bHashShift, false);
        m_Animator.SetFloat(m_HashMoveSpeed, m_MoveSpeed);

        m_Controller.Move(moveDir * m_MoveSpeed * Time.deltaTime);
        m_JumpPower = 3f;
    }

    private void Sprint(Vector3 moveDir)
    {
        m_Animator.SetBool(m_bHashShift, true);
        m_Animator.SetFloat(m_HashMoveSpeed, m_MoveSpeed * 1.8f);

        m_Controller.Move(moveDir * (m_MoveSpeed * 1.8f) * Time.deltaTime);
        m_JumpPower = 3.5f;
    }

    private void Jump()
    {
        m_Animator.SetBool(m_bHashSpace, false);
        m_bJump = false;

        if (m_Animator.GetBool(m_bHashDamaged))
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Animator.SetBool(m_bHashSpace, true);
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
        UpdateCoolTime();

        if (m_Animator.GetBool(m_bHashSpace) || Cursor.visible || m_bUsePhone || m_bSwing)
            return;

        Targeting();
        Swing();

        if (!m_Weapon.activeSelf)
            return;
        UseMagicBall();
        UseFirstOwnMagic();
        UseSecondOwnMagic();
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

        Vector3 TargetPos = m_Target.transform.position;
        TargetPos.y = transform.position.y + 0.3f;

        Vector3 vDir = TargetPos - transform.position;
        Quaternion newRotation = Quaternion.LookRotation(vDir * m_MoveSpeed * Time.deltaTime);

        m_Mesh.transform.rotation = Quaternion.Slerp(m_Mesh.transform.rotation, newRotation, m_RotateSpeed * 3f * Time.deltaTime);
        m_CamArmTrans.rotation = Quaternion.Slerp(m_Mesh.transform.rotation, newRotation, m_RotateSpeed * 3f * Time.deltaTime);
    }

    private void Swing()
    {
        if (m_Equipment.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                m_bSwing = true;
                m_Animator.SetBool(m_bHashLButton, true);
            }
        }
        else if (m_Weapon.activeSelf)
        {
            if (Input.GetMouseButton(0))
            {
                m_bSwing = true;
                m_Animator.SetBool(m_bHashLButton, true);
            }
        }
    }

    private void UpdateCoolTime()
    {
        if (m_bSkillOn[0])
        {
            if (m_CoolTime[0] > m_EndCoolTime[0] * (100-m_CoolPercent)*0.01)
            {
                m_bSkillOn[0] = false;
                m_CoolTime[0] = 0;
            }
            else
                m_CoolTime[0] += Time.deltaTime;
        }

        if (m_bSkillOn[1])
        {
            if (m_CoolTime[1] > m_EndCoolTime[1] * (100 - m_CoolPercent) * 0.01)
            {
                m_bSkillOn[1] = false;
                m_CoolTime[1] = 0;
            }
            else
                m_CoolTime[1] += Time.deltaTime;
        }
    }

    private void UseMagicBall()
    {
        if (m_SkillObject[0])
            return;

        if (Input.GetKey(KeyCode.Q))
        {
            m_bSwing = true;
            m_Animator.SetBool(m_bHashLButton, true);

            Vector3 PlayerPos = transform.position;
            Vector3 PlayerLook = m_Mesh.transform.forward;

            Vector3 NewPos = PlayerPos + PlayerLook * 3f;
            NewPos.y = PlayerPos.y + 1f;

            m_SkillObject[0] = Instantiate(m_MagicBall, NewPos, Quaternion.identity);
            m_SkillObject[0].GetComponent<MagicBall>().SetLookVector(PlayerLook);
        }
    }

    private void UseFirstOwnMagic()
    {
        if (m_bSkillOn[0] || m_SkillObject[1])
            return;

        if (Input.GetKey(KeyCode.E))
        {
            m_bSkillOn[0] = true;
            m_bSwing = true;
            m_Animator.SetBool(m_bHashLButton, true);

            Vector3 PlayerPos = transform.position;
            Vector3 NewPos;
            Vector3 PlayerLook;

            switch (m_eAbility)
            {
                case ObjectManager.ABILITY.ABIL_FIRE:
                    {
                        if (m_Target)
                        {
                            NewPos = m_Target.transform.position;
                            NewPos.y = PlayerPos.y + 5f;
                        }
                        else
                        {
                            PlayerLook = m_Mesh.transform.forward;

                            NewPos = PlayerPos + PlayerLook * 3f;
                            NewPos.y += 5f;
                        }
                        m_SkillObject[1] = Instantiate(m_FirstMagicSkill, NewPos, Quaternion.Euler(new Vector3(90, 180, 180)));
                    }
                    break;
                case ObjectManager.ABILITY.ABIL_WATER:
                    {
                        PlayerLook = m_Mesh.transform.forward;

                        NewPos = PlayerPos + PlayerLook * 3f;
                        NewPos.y = PlayerPos.y + 1f;

                        Instantiate(m_FirstMagicSkill, NewPos, Quaternion.Euler(new Vector3(90, 180, 180)));
                    }
                    break;
                case ObjectManager.ABILITY.ABIL_GRASS:
                    {
                        PlayerLook = m_Mesh.transform.forward;
                        NewPos = PlayerPos + PlayerLook * 3f;
                        NewPos.y -= 0.5f;

                        m_SkillObject[1] = Instantiate(m_FirstMagicSkill, NewPos, Quaternion.identity);
                        m_SkillObject[1].transform.rotation = Quaternion.LookRotation(PlayerLook);
                    }
                    break;
            }
        }
    }

    private void UseSecondOwnMagic()
    {
        if (m_bSkillOn[1] || m_SkillObject[2])
            return;

        if (Input.GetKey(KeyCode.R))
        {
            m_bSkillOn[1] = true;
            m_bSwing = true;
            m_Animator.SetBool(m_bHashLButton, true);

            Vector3 PlayerPos = transform.position;
            Vector3 PlayerLook;
            Vector3 NewPos;

            switch (m_eAbility)
            {
                case ObjectManager.ABILITY.ABIL_FIRE:
                    {
                        PlayerLook = m_Mesh.transform.forward;
                        NewPos = PlayerPos + PlayerLook;
                        NewPos.y += 1f;

                        m_SkillObject[2] = Instantiate(m_SecondMagicSkill, NewPos, Quaternion.identity);
                        m_SkillObject[2].transform.rotation = Quaternion.LookRotation(PlayerLook);
                    }
                    break;
                case ObjectManager.ABILITY.ABIL_WATER:
                    {
                        NewPos = new Vector3(PlayerPos.x, PlayerPos.y + 0.5f, PlayerPos.z);

                        m_SkillObject[2] = Instantiate(m_SecondMagicSkill, NewPos, Quaternion.identity);
                    }
                    break;
                case ObjectManager.ABILITY.ABIL_GRASS:
                    {
                        PlayerLook = m_Mesh.transform.forward;
                        NewPos = PlayerPos + PlayerLook * 3f;
                        NewPos.y -= 0.2f;

                        m_SkillObject[2] = Instantiate(m_SecondMagicSkill, NewPos, Quaternion.identity);
                        m_SkillObject[2].transform.rotation = Quaternion.LookRotation(PlayerLook);
                        m_SkillObject[2].GetComponent<MagicSeedBoundary>().RespawnSeeds(3);
                    }
                    break;
            }
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (m_NearObject.tag == "Equipment")
            {
                Destroy(m_NearObject);

                if (m_Equipment)
                {
                    if (!m_Weapon.activeSelf)
                        m_Equipment.SetActive(true);
                    m_bInitialEquip[0] = true;
                }
            }
            else if (m_NearObject.tag == "Weapon")
            {
                m_Weapon.GetComponent<MeshFilter>().sharedMesh = m_NearObject.GetComponent<Weapon>().GetMesh();
                Destroy(m_NearObject);

                if (m_Weapon)
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

    public void SetAbility(ObjectManager.ABILITY ability)
    {
        m_eAbility = ability;

        switch (m_eAbility)
        {
            case ObjectManager.ABILITY.ABIL_FIRE:
                {
                    m_MagicBall = Resources.Load<GameObject>("Particle/Player/Fire/ErekiBall2");
                    m_FirstMagicSkill = Resources.Load<GameObject>("Particle/Player/Fire/fireShot");
                    m_SecondMagicSkill = Resources.Load<GameObject>("Particle/Player/Fire/FlameThrower");
                }
                break;
            case ObjectManager.ABILITY.ABIL_WATER:
                {
                    m_MagicBall = Resources.Load<GameObject>("Particle/Player/Water/ErekiBall2");
                    m_FirstMagicSkill = Resources.Load<GameObject>("Particle/Player/Water/SteamBomb");
                    m_SecondMagicSkill = Resources.Load<GameObject>("Particle/Player/Water/MagicShield");
                }
                break;
            case ObjectManager.ABILITY.ABIL_GRASS:
                {
                    m_MagicBall = Resources.Load<GameObject>("Particle/Player/Grass/ErekiBall2");
                    m_FirstMagicSkill = Resources.Load<GameObject>("Particle/Player/Grass/GrassWall");
                    m_SecondMagicSkill = Resources.Load<GameObject>("Particle/Player/Grass/MagicSeedBoundary");
                }
                break;
        }
    }

    public void ResetAnimator()
    {
        m_bSwing = false;

        m_Animator.SetFloat(m_HashMoveSpeed, 0);
        m_Animator.SetBool(m_bHashShift, false);
        m_Animator.SetBool(m_bHashSpace, false);
        m_Animator.SetBool(m_bHashGrounded, true);
        m_Animator.SetBool(m_bHashLButton, false);
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

    public void ResetCoolTime()
    {
        for (int i = 0; i < 2; ++i)
            m_CoolTime[i] = 0;
    }
}
