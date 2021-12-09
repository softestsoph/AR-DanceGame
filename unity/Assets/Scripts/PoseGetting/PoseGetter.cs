using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PoseTeacher
{ 
    public enum InputSource { KINECT, FILE }
    public abstract class PoseGetter
    {
        public abstract bool Recording { get; set; }
        public float LastTimeStamp = 0;
        public float CurrentTimeStamp;
        public long RecordingStartTicks;
        public long CurrentTicks;
        public float GetTimeStamp()
        {
            CurrentTimeStamp = LastTimeStamp + (RecordingStartTicks - CurrentTicks) / 10_000_000;
            return CurrentTimeStamp;
        }
        public PoseData CurrentPose { get; protected set; }
        public DancePose CurrentDancePose { get; protected set; }
        public DanceData recordedDanceData = new DanceData();
        public abstract PoseData GetNextPose();
        public abstract DancePose GetNextDancePose();
        public abstract void SaveDanceData();
        public abstract void Dispose(); 
    }


}

