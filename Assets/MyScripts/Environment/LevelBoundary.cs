using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBoundary : MonoBehaviour
{
    [SerializeField] public int m_Level;
    private string m_SceneName;
    private Transform m_Trans;

    private bool m_bCloseEnough;
    List<Transform> m_LevelChildTrans = new List<Transform>();
    List<Material> m_LevelChildMat = new List<Material>();

    private Transform m_PlayerTrans;

    public bool GetCloseEnough() { return m_bCloseEnough; }

    // Start is called before the first frame update
    void Start()
    {
        m_Trans = transform;

        for (int i = 0; i < m_Trans.childCount; ++i)
        {
            m_LevelChildTrans.Add(m_Trans.GetChild(i));
            m_LevelChildMat.Add(m_Trans.GetChild(i).GetComponent<MeshRenderer>().material);
        }

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

    private bool CheckBoundaryDistance()
    {
        Vector3 PlayerPos = m_PlayerTrans.position;

        for (int i = 0; i < m_LevelChildTrans.Count; ++i)
        {
            Vector3 ChildTrans = m_LevelChildTrans[i].position;
            float Dist = (PlayerPos - ChildTrans).magnitude;

            //if(i == 0)
            //    Debug.Log(Dist);

            if (Dist < 4f)
            {
                SetTransparent(m_LevelChildMat[i], 1f - (Dist - 3f));
                return true;
            }
            else
            {
                SetTransparent(m_LevelChildMat[i], 0f);
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
        m_LevelChildTrans.Clear();
        m_LevelChildMat.Clear();
        Destroy(gameObject);
    }
}
