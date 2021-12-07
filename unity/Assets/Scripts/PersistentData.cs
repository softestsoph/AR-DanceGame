using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance;

    public DancePerformanceScriptableObject performance;

    public Vector3 playerPosition;
    public Vector3 kinectPosition;
    public List<Vector3> teacherPositions;
    public bool calibrated = false;

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
    }
}
