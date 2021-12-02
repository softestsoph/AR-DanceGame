using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher {
    public class AvatarDisplay : MonoBehaviour, IAvatarDisplay {
        public AvatarType avatarType = AvatarType.ROBOT;
        private AvatarContainer avatarContainer;

        private void Awake() {
            avatarContainer = new AvatarContainer(gameObject);
            avatarContainer.ChangeActiveType(avatarType);
        }

        public void SetPose(PoseData pose) {
            avatarContainer.MovePerson(pose);
        }
    }
}
