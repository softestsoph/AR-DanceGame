using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace PoseTeacher {
    public class GoalGhostDisplay : MonoBehaviour, IGoalDisplay {
        public Material material;

        private AvatarContainer avatarContainer;
        private float goalStartTime;
        private DanceData goalDanceData;
        private bool showingList = false;
        private int currentId = 0;

        void Start() {
            avatarContainer = new AvatarContainer(gameObject);
            avatarContainer.ChangeActiveType(AvatarType.ROBOT);
            material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 0f);
        }

        void Update() {
            if (showingList) {
                float currentTime = DanceManager.Instance.songTime;

                if (currentTime > goalStartTime) {
                    material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 1f);
                    float timeOffset = currentTime - (goalDanceData.poses[currentId].timestamp + goalStartTime);
                    avatarContainer.MovePerson(goalDanceData.GetInterpolatedPose(currentId, out currentId, timeOffset).toPoseData());
                }

                if (currentId == goalDanceData.poses.Count) {
                    showingList = false;
                    material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 0f);
                }
            }
        }

        public void showGoal(PoseData pose, float alpha) {
            avatarContainer.MovePerson(pose);
            showingList = false;
            material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", alpha);
        }

        public void showNothing() {
            material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 0f);
            showingList = false;
        }

        private void OnApplicationQuit() {
            material.SetFloat("Vector1_f3692b551e1149e99f89c979f8f7364e", 1f);
            showingList = false;
        }

        public void showDanceData(DanceData goalDanceData, float goalStartTime) {
            this.goalStartTime = goalStartTime;
            this.goalDanceData = goalDanceData;
            showingList = true;
            currentId = 0;
        }
    }
}
