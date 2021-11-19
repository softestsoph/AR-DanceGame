using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;

        private IScoringFunction scoringFunction = new ScoringQuaternionDistance();
        private List<IDanceGoal> goals;
        private int currentGoal = 0;
        private List<float> currentScores = new List<float>();
        private float lastScoredTimestamp = 0;

        private void Awake()
        {
            // make singleton
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        public void init(List<IDanceGoal> _goals)
        {
            goals = _goals;
            currentGoal = 0;
            currentScores = new List<float>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (DanceManager.IsRunning)
            {

                // Todo outsource this to a scoring manager
                // check if goal is over and calculate score
                while (currentGoal < goals.Count)
                {
                    IDanceGoal goal = goals[currentGoal];
                    if (goal.GetGoalType() == GoalType.POSE)
                    {
                        DanceGoalPose goalPose = (DanceGoalPose)goal;
                        if (DanceManager.SongTime > danceData.poses[goalPose.id].timestamp + poseGoalTestTime)
                        {
                            updateScore();
                            currentGoal++;
                            Debug.Log("new goal: " + currentGoal);
                        }
                        else
                        {
                            break; // the loop
                        }
                    }
                    else if (goal.GetGoalType() == GoalType.MOTION)
                    {
                        DanceGoalMotion goalMotion = (DanceGoalMotion)goal;
                        if (SongTime > danceData.poses[goalMotion.endId].timestamp)
                        {
                            updateScore();
                            currentGoal++;
                        }
                        else
                        {
                            break; // the loop
                        }
                    }
                }

                // Display Goal and maybe do a scoring run
                if (currentGoal < goals.Count)
                {
                    // Update GoalDisplay
                    //goalDisplay.showGoal(danceData, goals[currentGoal], SongTime);

                    // Calculate Score (Todo maybe do this on a timer for framerate independance)
                    IDanceGoal goal = goals[currentGoal];
                    if (goal.GetGoalType() == GoalType.POSE)
                    {
                        DanceGoalPose goalPose = (DanceGoalPose)goal;
                        if (SongTime > danceData.poses[goalPose.id].timestamp - poseGoalTestTime)
                        {
                            runningScores.Add(scoringFunction.GetScore(danceData.poses[goalPose.id], dancePoseSource.GetDancePose()));
                        }
                    }
                    else if (goal.GetGoalType() == GoalType.MOTION)
                    {
                        DanceGoalMotion goalMotion = (DanceGoalMotion)goal;
                        if (SongTime > danceData.poses[goalMotion.startId].timestamp)
                        {
                            runningScores.Add(scoringFunction.GetScore(interpolatedPose, dancePoseSource.GetDancePose()));
                        }
                    }
                }
                else
                {
                    //goalDisplay.showNothing();
                }
            }
        }

        // goal has ended, add scores and display TM
        private void FinishGoal()
        {
            float scoreToAdd = 0;
            IDanceGoal goal = goals[currentGoal];
            if (goal.GetGoalType() == GoalType.POSE)
            {
                if (runningScores.Count > 0)
                {
                    scoreToAdd = runningScores.Max();
                }
            }
            else if (goal.GetGoalType() == GoalType.MOTION)
            {
                if (runningScores.Count > 0)
                {
                    scoreToAdd = runningScores.Max();
                }
            }

            if (runningScores.Count == 0)
            {
                Debug.Log("Is this correct?");
            }

            runningScores = new List<float>();
            displayScore(scoreToAdd);
            score += scoreToAdd;
            Debug.Log("new score: " + score);
        }

        private void displayScore(float score)
        {
            if (score > 0.85)
            {
                ScoreDisplay.instance.addScore(Scores.GREAT);
            }
            else if (score > 0.6)
            {
                ScoreDisplay.instance.addScore(Scores.GOOD);
            }
            else
            {
                ScoreDisplay.instance.addScore(Scores.BAD);
            }
        }
    }
}

