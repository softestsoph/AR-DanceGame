using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher {
    public class AvatarDisplay : MonoBehaviour, IAvatarDisplay {
        public AvatarType avatarType = AvatarType.ROBOT;
        public Material material;
        private RobotTeacher robotTeacher;

        private bool fadeOut = false;
        private float alpha = 1f;

        private void Awake() {
            robotTeacher = gameObject.GetComponent<RobotTeacher>();
            material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 1f);
        }

        private void Update() {
            if (fadeOut) {
                // fade out within 2 seconds
                alpha -= Time.deltaTime / 2f;

                if (alpha < 0f) {
                    fadeOut = false;
                    alpha = 0f;
                }

                material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", alpha);
            }
        }

        public void SetPose(PoseData pose) {
            material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 1f);
            robotTeacher.MovePerson(pose);
        }

        public void FadeOut() {
            fadeOut = true;
        }

        private void OnApplicationQuit() {
            material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 1f);
        }
    }
}
