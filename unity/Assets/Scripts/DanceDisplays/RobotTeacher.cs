using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;

namespace PoseTeacher
{

    public class RobotTeacher : MonoBehaviour
    {
        public GameObject robot;
        Animator animator;
        Dictionary<JointId, Quaternion> absoluteOffsetMap;
        public float OffsetY = 0.5f;
        public float OffsetZ = 2.0f;

        public void Awake()
        {
            animator = robot.GetComponent<Animator>();
            absoluteOffsetMap = RiggingUtils.CreateOffsetMap(animator, robot.transform);
        }

        public void MovePerson(PoseData joint_data_list)
        {
            // Remove mirroring before applying pose and readd it afterwards
            // Necesary because MoveRiggedAvatar function works in global coordinates
            Vector3 prevScale = gameObject.transform.localScale;
            gameObject.transform.localScale = new Vector3(Mathf.Abs(prevScale.x), prevScale.y, prevScale.z);

            RiggingUtils.MoveRiggedAvatar(animator, absoluteOffsetMap, joint_data_list, robot.transform, OffsetY, OffsetZ);

            gameObject.transform.localScale = prevScale;
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}