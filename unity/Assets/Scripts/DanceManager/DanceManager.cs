using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PoseTeacher {
    public class DanceManager : MonoBehaviour {
        public static DanceManager instance;
        public static float SongTime => instance.audioSource.time;
        public static DanceData DanceData => instance.danceData;
        public static bool IsRunning => instance.isRunning;

        // would love to make this into a prefab but it does not work
        //public GameObject GoalDisplayObject;

        public GameObject AvatarDisplayPrefab;
        public GameObject dancePoseSourcePrefab;

        public ScoreDisplay scoreDisplay;

        private DanceData danceData;
        private int currentTrack = 0;
        private bool isRunning = false;
        private bool isReady = false;
        private AudioSource audioSource;
        private int currentPose = 0;
        private List<float> runningScores = new List<float>();

        private IAvatarDisplay avatarDisplay;
        //private IGoalDisplay goalDisplay;
        private IDancePoseSource dancePoseSource;


        private void Awake() {
            // make singleton
            if (instance == null) {
                instance = this;
                currentTrack = PersistentData.instance.startingTrack;
                audioSource = GetComponent<AudioSource>();
                //goalDisplay = GoalDisplayObject.GetComponent<IGoalDisplay>();

                if (PersistentData.instance.playOnStart) {
                    Invoke("Play", 0.5f);
                }
            } else {
                Destroy(this);
            }
        }

        private void init() {
            audioSource.clip = PersistentData.instance.tracks[currentTrack].SongObject.SongClip;
            audioSource.time = 0f;
            currentPose = 0;
            runningScores = new List<float>();

            // Restore the gameobjects
            if (avatarDisplay != null) {
                Destroy(avatarDisplay.gameObject);
            }
            avatarDisplay = Instantiate(AvatarDisplayPrefab).GetComponent<IAvatarDisplay>();

            //goalDisplay.showNothing();

            if (dancePoseSource != null) {
                Destroy(dancePoseSource.gameObject);
            }
            // todo maybe need to do this differently for kinect source, needs time to find player
            dancePoseSource = Instantiate(dancePoseSourcePrefab).GetComponent<IDancePoseSource>();

            isReady = true;
            Debug.Log("Init done");
        }

        public void SwitchTrack(int newTrack) {
            currentTrack = newTrack;
            isReady = false;
            Debug.Log("Switching Tracks");
            init();
        }

        public void Pause() {
            if (!isRunning) {
                return;
            }

            Debug.Log("Pause");
            isRunning = false;
            audioSource.Pause();
        }

        public void Play() {
            if (isRunning) {
                return;
            }

            // if not ready yet, init and call in .5 seconds again
            if (!isReady) {
                init();
                Invoke("Play", 0.5f);
            }
            Debug.Log("Play");
            isRunning = true;
            audioSource.Play();
        }

        void Update() {
            // Check if song has ended
            if (!audioSource.isPlaying && isRunning) {
                isReady = false;
                isRunning = false;
                //goalDisplay.showNothing();
                Destroy(avatarDisplay.gameObject);

                // Todo display finish screen
            }
            if (isRunning)
            {
                // Update avatar with interpolated DancePose
                float timeOffset = audioSource.time - danceData.poses[currentPose].timestamp;
                DancePose interpolatedPose = danceData.GetInterpolatedPose(currentPose, out currentPose, timeOffset);
                avatarDisplay.SetPose(interpolatedPose);
            }
        }
    }
}
