using PoseTeacher;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using System.Threading.Tasks;
using System;

public class DanceEditor : MonoBehaviour {

    public PinchSlider slider;
    public ProgressIndicatorLoadingBar indicatorBar;
    public Task indicatorTask;

    public DancePerformanceScriptableObject DancePerformanceObject;
    public GameObject AvatarContainerObject;
    private AvatarContainer avatar;

    // Editor
    public int totalFrames;
    public float progress;
    public List<(int, int)> selectors;
    public int activeSelector;
    public List<int> activeFrames;

    public float framesPerMeter; 
    public float frameSize; 
    public float frameSpacing; 
    public float borderSpace;

    // visuals
    public GameObject backgroundBar;
    public GameObject frameDisplay;
    public GameObject frame;
    // TODO: make selectors value between 0, 1 and then set position depending on parent.
    //       also draw bar between handles according to handle positions.
    public GameObject selectorContainer;
    public GameObject selectorObject;
    public List<GameObject> selectorList;

    public PressableButtonHoloLens2 pauseToggle;
    public bool playing;

    // Data
    private DanceData danceData;

    private AudioClip song;
    private AudioSource audioSource;

    private int currentId = 0;

    // Start is called before the first frame update
    void Start() {
        audioSource = GetComponent<AudioSource>();
        song = DancePerformanceObject.SongObject.SongClip;
        audioSource.clip = song;


        this.playing = true;

        this.framesPerMeter = 500;
        this.frameSize = 0.0015f;
        this.frameSpacing = (1f / framesPerMeter) - frameSize;
        this.borderSpace = (frameSpacing + frameSize) * 10;

        this.danceData = DancePerformanceObject.danceData.LoadDanceDataFromScriptableObject();
        this.totalFrames = danceData.poses.Count;

        // set backgroundBar to the correct size and position
        this.backgroundBar.transform.localScale = Vector3.Scale(backgroundBar.transform.localScale,
            new Vector3(2 * borderSpace + frameSpacing + totalFrames * (frameSize + frameSpacing), 1, 1));

        float backgroundScaleX = backgroundBar.transform.localScale.x;
        float moveToNegOhFive = 0.5f - backgroundScaleX / 2f;
        this.backgroundBar.transform.localPosition -= new Vector3(moveToNegOhFive, 0, 0);

        // create frames and set active
        this.activeFrames = new List<int>();
        
        Debug.Log(String.Format("Number of poses: {0}", danceData.poses.Count));

        for (int i = 0; i < totalFrames; ++i)
        {
            // create each frame
            float x_pos = frameSpacing / 2 + i * (frameSize + frameSpacing);
            GameObject indicator = Instantiate(frame, frameDisplay.transform);
            indicator.transform.localPosition += new Vector3(x_pos, 0, 0);
            indicator.transform.localScale = Vector3.Scale(indicator.transform.localScale, new Vector3(frameSize, 1, 1));
            // set all frames initally to
            // TODO: all, now only part for debugging
            if (i > 335)
            {
                activeFrames.Add(i);
            }
        }

        this.selectors = new List<(int, int)> { (0, totalFrames) };
        GameObject initialSelector = Instantiate(selectorObject, selectorContainer.transform);
        this.selectorList.Add(initialSelector);

        avatar = new AvatarContainer(AvatarContainerObject);
        avatar.ChangeActiveType(AvatarType.ROBOT);

        if (playing)
        {
            audioSource.Play();
        }
    }
    
    // Update is called once per frame
    void Update() {
        // TODO: make FrameIndicator move according to time of song..
        //       handle movement of indicator to edit the current time

        //slider.SliderValue = audioSource.time / song.length;

        // TODO: selectors should mark the selected frames
        if (playing)
        { 

            float timeOffset = audioSource.time - danceData.poses[currentId].timestamp;
            avatar.MovePerson(danceData.GetInterpolatedPose(currentId, out currentId, timeOffset).toPoseData());

            // show the current active frame: now for 3D boxes..
            // TODO: mby optimize performace, i.e. have compontents saved initally
            Renderer[] frameRenderes = frameDisplay.GetComponentsInChildren<Renderer>();
            //Debug.Log("number of frame renderes: " + frameRenderes.Length);
            for (int i = 0; i < totalFrames; ++i)
            {
                if (i == currentId)
                {
                    //Call SetColor using the shader property name "_Color" and setting the color to red
                    frameRenderes[i].material.SetColor("_Color", Color.blue);
                }
                else
                {
                    frameRenderes[i].material.SetColor("_Color", Color.gray);

                    if (activeFrames.Contains(i))
                    {
                        frameRenderes[i].material.SetColor("_Color", Color.red);
                    }
                }
            }
        }

        // may also need to pause the song
        //things that need to be updated even if song is not playing

    }

    public void SliderChanged() {
        // also gets called when chaning via script, sight, so only do something if
        float time = slider.SliderValue;
        float songTime = audioSource.time / song.length;
        if (time < songTime + 0.01 && time > songTime - 0.01) {
            return;
        }
        Debug.Log("Reset");
        audioSource.time = time * song.length;
        currentId = 0;
    }

    public void ChangePitch(float pitch) {
        audioSource.pitch = pitch;
    }

    public void SaveActiveFrames(String filePath)
    {
        Debug.Log(String.Format("Saving all {0} active frames", activeFrames.Count));
        // TODO: change timestamps to be in order
        // save active frames into dance data
        DanceData activeData = new DanceData();
        activeData.poses = new List<DancePose>();
        for (int i = 0; i < activeFrames.Count; ++i)
        {
            int danceDataIndex = activeFrames[i];
            activeData.poses.Add(danceData.poses[danceDataIndex]);
        }
        activeData.SaveToJSON();
    }

    public void ButtonPauseToggle()
    {
        if(pauseToggle.GetComponent<Interactable>().IsToggled)
        {
            playing = false;
            audioSource.Pause();
        }
        else
        {
            playing = true;
            audioSource.Play();
        }
    }

}
