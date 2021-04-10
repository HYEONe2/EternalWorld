using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MazeManager : MonoBehaviour
{
    private Maze m_Maze;
    private GameObject m_MazePlane;
    private Vector3 m_GeneratePos = new Vector3(0, 100, 0);

    public void SetMaze(GameObject maze) { m_Maze = maze.GetComponent<Maze>(); } 
    public Vector3 GetPatrolPos() { return m_Maze.GetPatrolPos(); }

    void Start()
    {
        m_MazePlane = Resources.Load<GameObject>("Environment/Maze/MazePlane");
    }

    public void GenerateNavmesh()
    {
        GameObject MazePlane = Instantiate(m_MazePlane, m_GeneratePos, Quaternion.identity, transform);
        MazePlane.GetComponent<MeshRenderer>().enabled = false;

        NavMeshSurface[] surfaces = gameObject.GetComponentsInChildren<NavMeshSurface>();
        foreach (var s in surfaces)
        {
            s.RemoveData();
            s.BuildNavMesh();
        }
    }
}
