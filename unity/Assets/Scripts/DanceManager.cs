using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;

namespace PoseTeacher
{

    public class DanceManager : MonoBehaviour
    {
        public static DanceManager Instance;

        PoseGetter selfPoseInputGetter;

        public EndScoreScreen endScoreScreen;
        public GameObject videoCube;
        public DancePerformanceScriptableObject DancePerformanceObject;

        //public GameObject avatarContainerSelf, avatarContainerTeacher;
        //List<AvatarContainer> avatarListSelf, avatarListTeacher;
        public List<AvatarDisplay> teacherDisplays;
        public AvatarDisplay defaultTeacher;

        private readonly string fake_file = "jsondata/2020_05_27-00_01_59.txt";
        public InputSource selfPoseInputSource = InputSource.KINECT;

        public bool paused = false;

        public PoseData currentSelfPose;

        private DanceData danceData;
        private AudioClip song;
        private AudioSource audioSource;

        List<(float, DanceData)> goals = new List<(float,DanceData)>();

        public float songTime => audioSource?.time ?? 0;

        bool finished = true;
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

            // For checking if calibration worked, testing only
           /* GameObject calobjs = GameObject.Instantiate(Resources.Load<GameObject>("CalibrationObjects"));
            calobjs.transform.Find("Player").transform.position = PersistentData.Instance.playerPosition;
            calobjs.transform.Find("Kinect").transform.position = PersistentData.Instance.kinectPosition;
            calobjs.transform.Find("Teacher").transform.position = PersistentData.Instance.teacherPositions[0];
            */
        }

        // Start is called before the first frame update
        public void Start()
        {
            Setup();
            //RestartSong();
        }

        // Update is called once per frame
        public void Update()
        {
            currentSelfPose = selfPoseInputGetter.GetNextPose();
           // AnimateSelf(currentSelfPose);
            if (!finished)
            {
                float timeOffset = audioSource.time - danceData.poses[currentId].timestamp;
                if (goals.Count > 0 && audioSource.time >= goals[0].Item1)
                {
                    ScoringManager.Instance.StartNewGoal(goals[0].Item2.poses, 0f);
                    goals.RemoveAt(0);
                }
                AnimateTeacher(danceData.GetInterpolatedPose(currentId, out currentId, timeOffset).toPoseData());

                if (audioSource.time >= audioSource.clip.length)
                {
                    FinishSong();
                }
            }
            
        }

        public void OnApplicationQuit()
        {
            selfPoseInputGetter.Dispose();
            
        }

        /*void AnimateSelf(PoseData live_data)
        {
            // MovePerson() considers which container to move
            foreach (AvatarContainer avatar in avatarListSelf)
            {
                avatar.MovePerson(live_data);
            }
        }*/

        // Animates all teacher avatars based on the JointData provided
        void AnimateTeacher(PoseData recorded_data)
        {
            /*foreach (AvatarContainer avatar in avatarListTeacher)
            {
                avatar.MovePerson(recorded_data);
            }*/
            foreach (AvatarDisplay avatar in teacherDisplays)
            {
                avatar.SetPose(recorded_data);
            }
        }

        PoseGetter getPoseGetter(InputSource src) {
            switch (src)
            {
                case InputSource.KINECT:
                    return new KinectPoseGetter() { VideoCube = videoCube};
                case InputSource.FILE:
                    return new FilePoseGetter(true) { ReadDataPath = fake_file };
                default:
                    return new FilePoseGetter(true) { ReadDataPath = fake_file };
            }
        }

        void FinishSong()
        {
            finished = true;
            audioSource.Stop();
            int totalScore = ScoringManager.Instance.getFinalScores().Item1;
            List<Scores> finalScores = ScoringManager.Instance.getFinalScores().Item2;

            endScoreScreen.setValues(totalScore,
                finalScores.Where(element => element == Scores.GREAT).Count(),
                finalScores.Where(element => element == Scores.GOOD).Count(),
                finalScores.Where(element => element == Scores.BAD).Count(),
                totalScore > HighScoreData.Instance.GetHighScore(DancePerformanceObject.songId));
            endScoreScreen.gameObject.SetActive(true);
            HighScoreData.Instance.UpdateHighScore(DancePerformanceObject.songId, totalScore);
        } 

        void Setup()
        {
            if (PersistentData.Instance.performance != null)
            {
                DancePerformanceObject = PersistentData.Instance.performance;
            }
            /*
            avatarListSelf = new List<AvatarContainer>();
            avatarListTeacher = new List<AvatarContainer>();
            avatarListSelf.Add(new AvatarContainer(avatarContainerSelf));
            avatarListTeacher.Add(new AvatarContainer(avatarContainerTeacher));
            */

            if (PersistentData.Instance.calibrated)
            {
                defaultTeacher.gameObject.SetActive(false);
                foreach(Vector3 position in PersistentData.Instance.teacherPositions)
                {
                    GameObject newTeacher = Instantiate((GameObject) Resources.Load("Displays/HoloAvatarDisplay"));
                    newTeacher.transform.position = position;
                    newTeacher.transform.LookAt(PersistentData.Instance.playerPosition);
                    
                    newTeacher.transform.Rotate(new Vector3(0, 180, 0));
                    newTeacher.transform.eulerAngles = new Vector3(0, newTeacher.transform.eulerAngles.y, 0);
                    newTeacher.GetComponent<RobotTeacher>().resetOffsetMap();
                    teacherDisplays.Add(newTeacher.GetComponent<AvatarDisplay>());
                }

                /*
                avatarContainerTeacher.transform.position = PersistentData.Instance.teacherPositions[0];
                avatarContainerTeacher.transform.LookAt(PersistentData.Instance.playerPosition);
                avatarContainerTeacher.transform.Rotate(new Vector3(-avatarContainerTeacher.transform.rotation.eulerAngles.x, 180, -avatarContainerTeacher.transform.rotation.eulerAngles.z));
                */

                videoCube.transform.position = PersistentData.Instance.kinectPosition + 0.5f * Vector3.up;
                videoCube.transform.LookAt(PersistentData.Instance.playerPosition);
                videoCube.transform.Rotate(new Vector3(0, 180, 0));
                videoCube.transform.eulerAngles = new Vector3(0, videoCube.transform.eulerAngles.y, 0);
            }
            else
            {
                teacherDisplays.Add(defaultTeacher);
            }

            audioSource = GetComponent<AudioSource>();
            song = DancePerformanceObject.SongObject.SongClip;
            audioSource.clip = song;
            danceData = DancePerformanceObject.danceData.LoadDanceDataFromScriptableObject();

            selfPoseInputGetter = getPoseGetter(selfPoseInputSource);

        }

        public void RestartSong()
        {
            endScoreScreen.gameObject.SetActive(false);

            goals = new List<(float, DanceData)>();
            for (int i = 0; i < DancePerformanceObject.goals.Count; i++)
            {
                goals.Add((DancePerformanceObject.goalStartTimestamps[i], DancePerformanceObject.goals[i]));
            }
            audioSource.time = 0;
            currentId = 0;
            finished = false;
            audioSource.PlayDelayed(0.5f);
        }

        public void QuitToMenu()
        {
            SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
        }
    }
}
