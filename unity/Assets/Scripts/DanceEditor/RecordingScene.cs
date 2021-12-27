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

        KinectPoseGetter selfPoseInputGetter;

        public GameObject videoCube;

        public AvatarDisplay self;

        PoseData currentSelfPose;

        public AudioClip song;
        private AudioSource audioSource;

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
           
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = song;

            selfPoseInputGetter = new KinectPoseGetter() { VideoCube = videoCube };

            Debug.Log("Successfull start initialization.");
        }


        public void FixedUpdate()
        {
            currentSelfPose = selfPoseInputGetter.GetNextPose();
            AnimateSelf(currentSelfPose);
        }

        public void OnApplicationQuit()
        {
            selfPoseInputGetter.Dispose();

        }

        void AnimateSelf(PoseData live_data)
        {
            self.SetPose(live_data);
        }

        public void ButtonRecord()
        {
            Debug.Log("Recording Toggled.");
            if (!selfPoseInputGetter.Recording)
            {
                Debug.Log("Now: Recording.");
                selfPoseInputGetter.Recording = true;
                audioSource.PlayDelayed(3);
            }
            else
            {
                Debug.Log("Now: Stopped and Saved Recording.");
                selfPoseInputGetter.Recording = false;
                audioSource.Stop();
            }
        }
    }
}
