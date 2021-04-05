using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    [SerializeField] public int m_Level;
    private string m_SceneName;

    private bool m_bCloseEnough;
    List<Transform> m_LevelChildTrans = new List<Transform>();
    List<Material> m_LevelChildMat = new List<Material>();

    private Transform m_PlayerTrans;

    public bool GetCloseEnough() { return m_bCloseEnough; }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            m_LevelChildTrans.Add(transform.GetChild(i));
            m_LevelChildMat.Add(transform.GetChild(i).GetComponent<MeshRenderer>().material);
        }

        for (int i = 0; i < m_LevelChildTrans.Count; ++i)
            m_LevelChildTrans[i].GetComponent<MeshRenderer>().enabled = false;

        m_PlayerTrans = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckBoundaryDistance())
            m_bCloseEnough = true;
        else
            m_bCloseEnough = false;
    }

    private void OnDestroy()
    {
        DestroyLevelBoundary();
    }

    private bool CheckBoundaryDistance()
    {
        if (m_Level == 0)
        {
            Vector3 PlayerPos = m_PlayerTrans.position;

            for (int i = 0; i < m_LevelChildTrans.Count; ++i)
            {
                Vector3 ChildTrans = m_LevelChildTrans[i].position;
                float Dist = (PlayerPos - ChildTrans).magnitude;

                //if(i == 0)
                //    Debug.Log(Dist);
                //Debug.Log(Dist);

                if (Dist < 4f)
                {
                    m_LevelChildTrans[i].GetComponent<MeshRenderer>().enabled = true;
                    //SetTransparent(m_LevelChildMat[i], 1f - (Dist - 3f));
                    SetTransparent(m_LevelChildMat[i], 0.2f);
                    return true;
                }
                else
                {
                    m_LevelChildTrans[i].GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
        else
        {
            if (m_Level != m_PlayerTrans.gameObject.GetComponent<PlayerProperty>().GetLevel() + 1)
                return false;

            Vector3 PlayerPos = m_PlayerTrans.position;
            Vector3 Pos = transform.position;
            Pos.y = PlayerPos.y;

            float Dist = (PlayerPos - Pos).magnitude;
            if (Dist > 15f * (m_Level - 1) - 1f)
            {
                for (int i = 0; i < m_LevelChildTrans.Count; ++i)
                {
                    m_LevelChildTrans[i].GetComponent<MeshRenderer>().enabled = true;
                    //SetTransparent(m_LevelChildMat[i], (1f - (15f - Dist)) * 0.5f);
                    SetTransparent(m_LevelChildMat[i], 0.2f);
                }
                return true;
            }
            else
            {
                for (int i = 0; i < m_LevelChildTrans.Count; ++i)
                    m_LevelChildTrans[i].GetComponent<MeshRenderer>().enabled = false;
            }
        }

        return false;
    }

    private void SetTransparent(Material material, float alpha)
    {
        material.SetFloat("_Mode", 3f);
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;

        material.color = new Color(0f, 0f, 0f, alpha);
    }

    public void DestroyLevelBoundary()
    {
        foreach (Transform trans in m_LevelChildTrans)
            Destroy(trans.gameObject);
        m_LevelChildTrans.Clear();
        m_LevelChildMat.Clear();
        Destroy(gameObject);
    }
}
