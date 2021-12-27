using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Microsoft.MixedReality.Toolkit.UI;

namespace PoseTeacher
{

    public class RecordingScene : MonoBehaviour
    {
        public static RecordingScene Instance;

        PoseGetter selfPoseInputGetter;

        public GameObject videoCube;
        public DancePerformanceScriptableObject DancePerformanceObject;

        public bool withTeacher = false;

        public GameObject avatarContainerSelf, avatarContainerTeacher;
        List<AvatarContainer> avatarListSelf, avatarListTeacher;

        private readonly string fake_file = "jsondata/2021_12_13-15_52_07.txt"; //(impossible ) // 2021_12_13-15_27_58.txt"; //(dance monkey) // 2020_05_27-00_01_59.txt"; //(irgendwas)
        //private readonly string dance_path = "Assets/Dances/Recordings/recording-2021_12_13-15_29_23.asset";
        public InputSource selfPoseInputSource = InputSource.KINECT;

        public bool paused = false;

        public bool recording = false;
        public PressableButtonHoloLens2 recordButton;
        public PressableButtonHoloLens2 saveButton;

        public PoseData currentSelfPose;

        private DanceData danceData;
        private AudioClip song;
        private AudioSource audioSource;

        readonly List<(float, DanceData)> goals = new List<(float, DanceData)>();

        public float songTime => audioSource?.time ?? 0;

        int currentId = 0;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        // Start is called before the first frame update
        public void Start()
        {
            avatarListSelf = new List<AvatarContainer>();
            avatarListSelf.Add(new AvatarContainer(avatarContainerSelf));


            avatarListTeacher = new List<AvatarContainer>();
            avatarListTeacher.Add(new AvatarContainer(avatarContainerTeacher));

            audioSource = GetComponent<AudioSource>();
            song = DancePerformanceObject.SongObject.SongClip;
            audioSource.clip = song;
            danceData = DancePerformanceObject.danceData.LoadDanceDataFromScriptableObject();
            /*
            for (int i = 0; i < DancePerformanceObject.goals.Count; i++)
            {
                goals.Add((DancePerformanceObject.goalStartTimestamps[i], DancePerformanceObject.goals[i]));
            }
            */
            selfPoseInputGetter = getPoseGetter(selfPoseInputSource);

            audioSource.Play();

            Debug.Log("Successfull start initialization.");
        }

        // TODO: decide wheter to recorde per frame, i.e Update();
        //                             or per fixed step (50fps), i.e. FixedUpdate()

        // Update is called once per frame
        public void Update()
        {
            float timeOffset = audioSource.time - danceData.poses[currentId].timestamp;
            currentSelfPose = selfPoseInputGetter.GetNextPose();
            // TODO: or GetNextDancePose(). Currently saving both in GetNextPose();
            AnimateSelf(currentSelfPose);
            if (withTeacher)
            {
                /*
                if (goals.Count > 0 && audioSource.time >= goals[0].Item1)
                {
                    ScoringManager.Instance.StartNewGoal(goals[0].Item2.poses, 0f);
                    goals.RemoveAt(0);
                }
                */
                AnimateTeacher(danceData.GetInterpolatedPose(currentId, out currentId, timeOffset).toPoseData());

            }


            if (audioSource.time > danceData.poses[danceData.poses.Count - 1].timestamp)
            {
                audioSource.Stop();
                if (withTeacher)
                {
                    List<Scores> finalScores = ScoringManager.Instance.getFinalScores().Item2;
                    Debug.Log("nr of final scores: " + finalScores.Count);
                    foreach (Scores s in finalScores)
                    {
                        Debug.Log(s);
                    }
                }

            }
            
        }

        public void OnApplicationQuit()
        {
            selfPoseInputGetter.Dispose();

        }

        void AnimateSelf(PoseData live_data)
        {
            // MovePerson() considers which container to move
            foreach (AvatarContainer avatar in avatarListSelf)
            {
                avatar.MovePerson(live_data);
            }
        }
        // Animates all teacher avatars based on the JointData provided
        void AnimateTeacher(PoseData recorded_data)
        {
            foreach (AvatarContainer avatar in avatarListTeacher)
            {
                avatar.MovePerson(recorded_data);
            }
        }

        PoseGetter getPoseGetter(InputSource src)
        {

            PoseGetter poseGetter;

            switch (src)
            {

                case InputSource.KINECT:
                    poseGetter = new KinectPoseGetter() { VideoCube = videoCube };
                    break;
                case InputSource.FILE:
                    poseGetter = new FilePoseGetter(true) { ReadDataPath = fake_file };
                    break;
                default:
                    poseGetter = new FilePoseGetter(true) { ReadDataPath = fake_file };
                    break;
            }

            if (poseGetter != null)
            {
                Debug.Log("created posegetter: " + poseGetter);
                return poseGetter;
            }
            else
            {
                Debug.Log("Could not create posegetter.");
                return null;
            }

        }

        public void ButtonSave()
        {
            Debug.Log("Saving current dance data. And stoping the recording.");
            selfPoseInputGetter.SaveDanceData();
            selfPoseInputGetter.Recording = false;
            recordButton.GetComponent<Interactable>().IsToggled = false;
        }

        public void ButtonRecord()
        {
            Debug.Log("Recording Toggled.");
            if (recordButton.GetComponent<Interactable>().IsToggled)
            {
                Debug.Log("Now: Recording.");
                selfPoseInputGetter.Recording = true;
            }
            else
            {
                Debug.Log("Now: Stoped Recording.");
                selfPoseInputGetter.Recording = false;
            }
        }


    }


}
