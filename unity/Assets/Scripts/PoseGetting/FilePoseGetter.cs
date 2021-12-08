using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace PoseTeacher
{
    public class FilePoseGetter : PoseGetter
    {

        private bool recording;
        public override bool Recording
        {
            get
            {
                return recording;
            }
            set
            {
                if (value)
                {
                    RecordingStartTicks = DateTime.Now.Ticks;
                }
                else
                {
                    LastTimeStamp = CurrentTimeStamp;
                }
                recording = value;
            }
        }

        bool loop = false;

        IEnumerator<string> SequenceEnum;
        private string _ReadDataPath;
        public string ReadDataPath
        {
            get { return _ReadDataPath; }
            set { _ReadDataPath = value; LoadData(); }
        }

        public FilePoseGetter(bool _loop)
        {
            loop = _loop;
        }

        public override PoseData GetNextPose()
        {
            if (!SequenceEnum.MoveNext())
            {
                // Quick and dirty way to loop (by reloading file)
                if (SequenceEnum == null || loop)
                {
                    LoadData();
                    SequenceEnum.MoveNext();
                }
            }

            string frame_json = SequenceEnum.Current;
            PoseData fake_live_data = PoseDataUtils.JSONstring2PoseData(frame_json);
            CurrentPose = fake_live_data;

            // also save as dance pose
            CurrentTicks = System.DateTime.Now.Ticks;
            CurrentDancePose = DancePose.fromPoseData(CurrentPose, GetTimeStamp());
            if (Recording)
            {
                recordedDanceData.poses.Add(CurrentDancePose);
            }

            return CurrentPose;
        }

        public override DancePose GetNextDancePose()
        {
            if (!SequenceEnum.MoveNext())
            {
                // Quick and dirty way to loop (by reloading file)
                if (SequenceEnum == null || loop)
                {
                    LoadData();
                    SequenceEnum.MoveNext();
                }
            }

            string frame_json = SequenceEnum.Current;
            PoseData fake_live_data = PoseDataUtils.JSONstring2PoseData(frame_json);
            CurrentTicks = System.DateTime.Now.Ticks;
            CurrentDancePose = DancePose.fromPoseData(CurrentPose, GetTimeStamp());
            if (Recording)
            {
                recordedDanceData.poses.Add(CurrentDancePose);
            }
            return CurrentDancePose;
        }

        public override void Dispose(){}

        public void RestartFile()
        {
            LoadData();
        }

        void LoadData()
        {
            SequenceEnum = File.ReadLines(ReadDataPath).GetEnumerator();
        }

        public override void SaveDanceData()
        {
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            string recordingName = "Recordings/recording-" + timestamp;
            DanceDataScriptableObject.SaveDanceDataToScriptableObject(recordedDanceData, recordingName, true);

            // After Saving reset recorded data to have space for a new one
            recordedDanceData = new DanceData();
        }


    }


}

