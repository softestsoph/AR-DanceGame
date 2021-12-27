using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace PoseTeacher {
    public class GoalDisplay : MonoBehaviour, IGoalDisplay {
        public VisualEffect vfx;

        private AvatarContainer avatarContainer;
        private float goalStartTime;
        private DanceData goalDanceData;
        private bool showingList = false;
        private int currentId = 0;

        void Start() {
            avatarContainer = new AvatarContainer(gameObject);
            avatarContainer.ChangeActiveType(AvatarType.ROBOT);
            vfx.SetFloat("alpha", 0f);
            showingList = false;
        }

        void Update() {
            if (showingList) {
                float currentTime = DanceManager.Instance.songTime;

                if(currentTime > goalStartTime) {
                    vfx.SetFloat("alpha", 1f);
                    float timeOffset = currentTime - (goalDanceData.poses[currentId].timestamp + goalStartTime);
                    avatarContainer.MovePerson(goalDanceData.GetInterpolatedPose(currentId, out currentId, timeOffset).toPoseData());
                }

                if(currentId == goalDanceData.poses.Count) {
                    showingList = false;
                    vfx.SetFloat("alpha", 0f);
                }
            }
        }

        public void showGoal(PoseData pose, float alpha) {
            avatarContainer.MovePerson(pose);
            showingList = false;
            vfx.SetFloat("alpha", alpha);
        }

        public void showNothing() {
            vfx.SetFloat("alpha", 0f);
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
