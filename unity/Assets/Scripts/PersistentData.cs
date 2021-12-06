using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public DancePerformanceScriptableObject performance;

    public Vector3 playerPosition = Vector3.zero;
    public Vector3 kinectPosition = Vector3.zero;
    public List<Vector3> teacherPositions = new List<Vector3>();

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        teacherPositions.Add(Vector3.zero);
    }
}
