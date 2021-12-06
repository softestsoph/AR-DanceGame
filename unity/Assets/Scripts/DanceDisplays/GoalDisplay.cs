using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace PoseTeacher {
    public class GoalDisplay : MonoBehaviour, IGoalDisplay {
        public VisualEffect vfx;

        private AvatarContainer avatarContainer;

        void Start() {
            avatarContainer = new AvatarContainer(gameObject);
            avatarContainer.ChangeActiveType(AvatarType.ROBOT);
            vfx.SetFloat("alpha", 0);
        }

        public void showGoal(PoseData pose, float alpha) {
            avatarContainer.MovePerson(pose);
            
            vfx.SetFloat("alpha", alpha);
        }

        public void showNothing() {
            vfx.SetFloat("alpha", 0f);
        }
    }
}
