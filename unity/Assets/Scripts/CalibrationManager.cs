using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CalibrationManager : MonoBehaviour
{
    public GameObject playerPosition;
    public GameObject kinectPosition;
    public List<GameObject> teacherPositions;

    public void ConfirmPositions()
    {
        PersistentData.Instance.playerPosition = playerPosition.transform.position;
        PersistentData.Instance.kinectPosition = kinectPosition.transform.position;
        PersistentData.Instance.teacherPositions = teacherPositions.Select(x => x.transform.position).ToList();
        PersistentData.Instance.calibrated = true;
    }
}
