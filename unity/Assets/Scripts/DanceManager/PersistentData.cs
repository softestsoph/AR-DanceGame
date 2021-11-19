using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    //Singleton Instance
    public static PersistentData instance;

    public DancePerformanceScriptableObject[] tracks;
    public int startingTrack = 0;
    public bool playOnStart = false;

    private void Awake()
    {
        if(instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }
}
