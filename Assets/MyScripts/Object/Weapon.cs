using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Player m_Player;
    private MeshFilter m_MeshFilter;

    public Mesh GetMesh() { return m_MeshFilter.sharedMesh; }

    // Start is called before the first frame update
    void Start()
    {
        m_Player = GameObject.Find("Player").GetComponent<Player>();

        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshFilter.sharedMesh = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_MeshFilter.sharedMesh)
            return;

        ObjectManager.ABILITY PlayerAbility = m_Player.GetAbility();

        if(PlayerAbility == ObjectManager.ABILITY.ABIL_FIRE)
        {
            MeshFilter tempFilter= Resources.Load<MeshFilter>("Equipment/Weapon/Staff_04");
            m_MeshFilter.sharedMesh = tempFilter.sharedMesh;
        }
        else if(PlayerAbility == ObjectManager.ABILITY.ABIL_WATER)
        {
            MeshFilter tempFilter = Resources.Load<MeshFilter>("Equipment/Weapon/Staff_05");
            m_MeshFilter.sharedMesh = tempFilter.sharedMesh;
        }
        else if (PlayerAbility == ObjectManager.ABILITY.ABIL_GRASS)
        {
            MeshFilter tempFilter = Resources.Load<MeshFilter>("Equipment/Weapon/Staff_06");
            m_MeshFilter.sharedMesh = tempFilter.sharedMesh;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!m_MeshFilter.sharedMesh)
            return;

        if (other.CompareTag("Player"))
            m_Player.SetNearObject(this.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!m_MeshFilter.sharedMesh)
            return;

        if (other.CompareTag("Player"))
            m_Player.SetNearObject(null);
    }
}
