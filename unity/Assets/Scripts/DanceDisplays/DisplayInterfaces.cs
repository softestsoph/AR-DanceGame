using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher {
    public interface IAvatarDisplay {
        public void SetPose(PoseData pose);

        public GameObject gameObject { get; }

    }

    public interface IGoalDisplay {
        public void showGoal(PoseData pose, float alpha);
        public void showNothing();

        public GameObject gameObject { get; }
    }

    public interface IDancePoseSource {
        public DancePose GetDancePose();

        public GameObject gameObject { get; }
    }

}
