using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoseTeacher {
    public interface IScoringFunction  {
        public float GetScore(DancePose target, DancePose actual);
    }

    public class ScoringQuaternionDistance : IScoringFunction {
        public float GetScore(DancePose target, DancePose actual) {
            List<Quaternion> selfList = DancePoseToOrientation(actual);
            List<Quaternion> goalList = DancePoseToOrientation(target);
            int numberOfComparisons = 8;
            float result = 0;

            for (int i = 0; i < numberOfComparisons; i++) {
                float distance = quaternionDistance(selfList[i], goalList[i]);
                result += Mathf.Pow(distance,2) * scoringWeightsPrioritizeArms[i];
            }
            result = Mathf.Sqrt(result / TotalWeights(scoringWeightsPrioritizeArms));

            return Mathf.Clamp01(1f - result);
        }

        static float quaternionDistance(Quaternion a, Quaternion b) {
            return 1 - Mathf.Pow(a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z, 2);
        }


        static List<Quaternion> DancePoseToOrientation(DancePose pose) {
            List<Quaternion> list = new List<Quaternion>();
            Vector3 vector;

            //LeftUpperArm (SHOULDER_LEFT - ELBOW_LEFT)
            vector = pose.positions[5] - pose.positions[6];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            //RightUpperArm (SHOULDER_RIGHT - ELBOW_RIGHT)
            vector = pose.positions[12] - pose.positions[13];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            //TorsoLeft (SHOULDER_LEFT - HIP_LEFT
            vector = pose.positions[5] - pose.positions[18];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            //TorsoRight (SHOULDER_RIGHT - HIP_RIGHT
            vector = pose.positions[12] - pose.positions[22];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            //HipStick (HIP_LEFT - HIP_RIGHT)
            vector = pose.positions[18] - pose.positions[22];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            //LeftLowerArm (ELBOW_LEFT - WRIST_LEFT)
            vector = pose.positions[6] - pose.positions[7];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            //RightLowerArm (ELBOW_RIGHT - WRIST_RIGHT)
            vector = pose.positions[13] - pose.positions[14];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            //Shoulders (SHOULDER_LEFT - SHOULDER_RIGHT)
            vector = pose.positions[5] - pose.positions[12];
            list.Add(Quaternion.LookRotation(vector, Vector3.up));

            return list;
        }

        static List<int> scoringWeightsPrioritizeArms = new List<int> { 3, 3, 1, 1, 1, 3, 3, 1 };

        static int TotalWeights(List<int> weights) {
            int total = 0;
            foreach (int i in weights) {
                total += i;
            }
            return total;
        }
    }
}
