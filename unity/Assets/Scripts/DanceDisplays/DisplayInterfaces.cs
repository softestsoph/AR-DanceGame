using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher {
    public interface IAvatarDisplay {
        public void SetPose(PoseData pose);

        public void FadeOut();

        public GameObject gameObject { get; }

    }

    public interface IGoalDisplay {
        public void showGoal(PoseData pose, float alpha);

        public void showDanceData(DanceData data, float goalStartTime);

        public void showNothing();

        public GameObject gameObject { get; }
    }

    public interface IScoreDisplay {
        public void showScore(Scores score);

        public void addScore(int scoreToAdd);

        public int Score { get; }
    }
}
