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
    public List<int> selectedFrames;
    public int activeSelector;
    public List<int> activeFrames;
    public List<int> goalFrames;

    public float framesPerMeter; 
    public float frameSize; 
    public float frameSpacing; 
    public float borderSpace;

    // visuals
    float startPos, endPos;
    public GameObject backgroundBar;
    public GameObject frameDisplay;
    public GameObject frame;
    public GameObject frameIndicator;
    public bool grabbed;
    Renderer[] frameRenderes;
    // Select frames by activating button and hovering over them.
    // Change colour to indicate selected frames.
    public PressableButton selectorToggle;
    public PressableButton deselectorToggle;
    public bool selecting;
    public bool deselecting;

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

        SliderMinMax();
        Debug.Log(String.Format("Start end positions: {0}  {1}", startPos, endPos));

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
            // set all frames initially as active
            activeFrames.Add(i);
        }

         this.frameRenderes = frameDisplay.GetComponentsInChildren<Renderer>();

        avatar = new AvatarContainer(AvatarContainerObject);
        avatar.ChangeActiveType(AvatarType.ROBOT);

        if (playing)
        {
            audioSource.Play();
        }
    }
    
    // Update is called once per frame
    void Update() {

        if (playing)
        { 
            float timeOffset = audioSource.time - danceData.poses[currentId].timestamp;
            //Debug.Log(String.Format("time: {0}    stamp: {1}    offset: {2}", audioSource.time, danceData.poses[currentId].timestamp, timeOffset));
            avatar.MovePerson(danceData.GetInterpolatedPose(currentId, out currentId, timeOffset).toPoseData());
            SetSliderPosition(currentId, timeOffset);
        }

        // show the current active frame: now for 3D boxes..
        RenderFrames();

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

    public void SaveActiveFrames(float td = 0)
    {
        Debug.Log(String.Format("Saving all {0} active frames", activeFrames.Count));
        // TODO: change timestamps to be in order
        // save active frames into dance data
        DanceData activeData = new DanceData();
        activeData.poses = new List<DancePose>();
        DancePose copyPose = new DancePose();

        // if unsure what the timestamp difference was in the old dance
        // calculate it in average 
        if(td == 0)
        {
            float stampSum = 0;
            float lastTs = 0;
            foreach(DancePose p in danceData.poses)
            {
                float currentTs = p.timestamp;
                stampSum += (currentTs - lastTs);
                lastTs = currentTs;
            }
            td = stampSum / totalFrames;
        }
        //Debug.Log("Time difference: " + td);

        float initialTs = 0;
        for (int i = 0; i < totalFrames; ++i)
        {
            if (activeFrames.Contains(i) || goalFrames.Contains(i))
            {
                copyPose = danceData.poses[i];
                copyPose.timestamp = initialTs;
                initialTs += td;
                activeData.poses.Add(copyPose);
            }
        }

        activeData.SaveToJSON();

        Debug.Log("Saved all active Frames.");
    }


    public (List<float>, List<float>) SaveGoalFrames(float td = 0)
    {
        List<float> goalStartTimestamps = new List<float>();
        List<float> goalDurations = new List<float>();

        DancePose copyPose = new DancePose();

        if (td == 0)
        {
            float stampSum = 0;
            float lastTs = 0;
            foreach (DancePose p in danceData.poses)
            {
                float currentTs = p.timestamp;
                stampSum += (currentTs - lastTs);
                lastTs = currentTs;
            }
            td = stampSum / totalFrames;
        }

        float initialTs = 0;
        bool firstGoalFrame = true;
        float goalDuration = 0;
        for (int i = 0; i < totalFrames + 1; ++i)
        {
            if (activeFrames.Contains(i) || goalFrames.Contains(i))
            {
                copyPose.timestamp = initialTs;

                if (goalFrames.Contains(i))
                {
                    if (firstGoalFrame)
                    {
                        goalStartTimestamps.Add(initialTs);
                        firstGoalFrame = false;
                    }
                    else
                    {
                        goalDuration += td;
                    }
                }
                else
                {
                    if(firstGoalFrame == false)
                    {
                        goalDurations.Add(goalDuration);
                        firstGoalFrame = true;
                    }
                }

                initialTs += td;

            }

            // check if a goal last until the very end
            if(i == totalFrames)
            {
                if (firstGoalFrame == false)
                {
                    goalDurations.Add(goalDuration);
                    firstGoalFrame = true;
                }
            }

        }

        return (goalStartTimestamps, goalDurations);
    }

    public (float, float) SliderMinMax()
    {
        // Call this function to get the min and max positions of the Slider
        // Important if Editor was moved
        Transform barT = backgroundBar.transform;
        float sPos = barT.position.x - (barT.localScale.x / 2) + borderSpace;
        float ePos = barT.position.x + (barT.localScale.x / 2) - borderSpace;
        startPos = sPos;
        endPos = ePos;
        return (startPos, endPos);
    }

    public float SliderValue()
    {
        SliderMinMax();
        Transform indicatorT = frameIndicator.transform;
        if(indicatorT.position.x < startPos)
        {
            indicatorT.position = new Vector3(startPos, indicatorT.position.y, indicatorT.position.z);
        }
        if (indicatorT.position.x > endPos)
        {
            indicatorT.position = new Vector3(endPos, indicatorT.position.y, indicatorT.position.z);
        }
        progress = (indicatorT.position.x - startPos) / (endPos - startPos);
        //Debug.Log(String.Format("start: {0}    pos: {1}    end: {2}    -> progress: {3}", startPos, indicatorT.position.x, endPos, progress));
        return progress;
    }

    public void UpdateSlider()
    {
        // On movement set take the new position and change the currently active frame
        float progress = SliderValue();
        audioSource.time = song.length * progress;
        currentId = (int)(totalFrames * progress);
        RenderFrames();
    }

    public void SetSliderPosition(int frame, float offset)
    {
        SliderMinMax();
        float barDist = (float)frame / (float)totalFrames;
        float frameDist = offset;

        float posx = startPos + (endPos - startPos) * barDist + (frameSpacing + frameSize) * frameDist;
        frameIndicator.transform.position = new Vector3(posx, frameIndicator.transform.position.y, frameIndicator.transform.position.z);
        SliderValue();
    }

    public void RenderFrames()
    {
        for (int i = 0; i < totalFrames; ++i)
        {
            if (i == this.currentId)
            {
                //Call SetColor using the shader property name "_Color" and setting the color to red
                frameRenderes[i].material.SetColor("_Color", Color.blue);
            }
            else if (selectedFrames.Contains(i))
            {
                frameRenderes[i].material.SetColor("_Color", Color.yellow);
            }
            else if (goalFrames.Contains(i))
            {
                frameRenderes[i].material.SetColor("_Color", Color.green);
            }
            else if (activeFrames.Contains(i))
            {
                frameRenderes[i].material.SetColor("_Color", Color.red);
            }
            else
            {
                frameRenderes[i].material.SetColor("_Color", Color.gray);
            }
        }
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

    public void ButtonSelector(int type)
    {
        // select
        if(type == 0)
        {
            Debug.Log("selecting.");
            selecting = true;
            deselecting = false;
        }
        // deselect
        else if(type == 1)
        {
            Debug.Log("de-selecting.");
            selecting = false;
            deselecting = true;
        }
        else // nothing
        {
            Debug.Log("nothing");
            selecting = false;
            deselecting = false;
        }
    }

    public void HoverSelect(GameObject selfObj)
    {
        int frameIndex = GetFrameIndex(selfObj.transform.position.x);
        if (selecting)
        {
            if (!selectedFrames.Contains(frameIndex))
            {
                selectedFrames.Add(frameIndex);
            }
        }
        if (deselecting)
        {
            if (selectedFrames.Contains(frameIndex))
            {
                selectedFrames.Remove(frameIndex);
            }
        }
    }

    public void ChangeFrameType(int type)
    {
        switch (type)
        {
            // in-active
            case 0:
                foreach(int f in selectedFrames)
                {
                    if (activeFrames.Contains(f))
                    {
                        activeFrames.Remove(f);
                    }
                    if (goalFrames.Contains(f))
                    {
                        goalFrames.Remove(f);
                    }
                }
                selectedFrames.Clear();
                break;

            // active
            case 1:
                foreach (int f in selectedFrames)
                {
                    if (!activeFrames.Contains(f))
                    {
                        activeFrames.Add(f);
                    }
                    if (goalFrames.Contains(f))
                    {
                        goalFrames.Remove(f);
                    }
                }
                selectedFrames.Clear();
                break;

            // goal
            case 2:
                foreach (int f in selectedFrames)
                {
                    if (!activeFrames.Contains(f))
                    {
                        activeFrames.Add(f);
                    }
                    if (!goalFrames.Contains(f))
                    {
                        goalFrames.Add(f);
                    }
                }
                selectedFrames.Clear();
                break;
                        
            // default
            default:
                Debug.Log("Changing to unexpected frame type.");
                break;

        }

    }

    public int GetFrameIndex(float posx)
    {
        SliderMinMax();
        // calculate the frame index given through parameters set in start.
        float fIdx = (posx - startPos + borderSpace/2 - (frameSpacing+frameSize)) / (frameSize + frameSpacing);
        //Debug.Log("Frame idx: " + fIdx);
        return (int)Math.Round(fIdx);
        
    }


}
