using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeManager : MonoBehaviour
{
    private GameObject m_MazePlane;
    private Vector3 m_GeneratePos = new Vector3(50, 0, 50);

    void Start()
    {
        m_MazePlane = Resources.Load<GameObject>("Environment/Maze/MazePlane");
    }

    public void GenerateNavmesh()
    {
        GameObject NewMazePlane = Instantiate(m_MazePlane, m_GeneratePos, Quaternion.identity, transform);
        NewMazePlane.GetComponent<MeshRenderer>().enabled = false;
        m_GeneratePos += new Vector3(50, 0, 50);

        NavMeshSurface[] surfaces = gameObject.GetComponentsInChildren<NavMeshSurface>();
        foreach (var s in surfaces)
        {
            s.RemoveData();
            s.BuildNavMesh();
        }
    }
}
