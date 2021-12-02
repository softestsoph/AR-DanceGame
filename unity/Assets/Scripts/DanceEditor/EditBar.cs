using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PoseTeacher
{
    public class EditBar : MonoBehaviour
    {

        public int totalFrames;
        public float progress;
        public List<(int, int)> selectors;
        public int activeSelector;
        public List<int> activeFrames;


        float framesPerMeter;

        // visual elements
        // mby additional bar for song
        public GameObject barBackground;
        // frames of the dance
        public GameObject frameDisplay;
        public GameObject frame;
        float frameSize;
        float frameSpacing;
        float borderSpace;
        // progress of current dance
        public GameObject progressIndicator;
        // TODO: make selectors value between 0, 1 and then set position depending on parent.
        //       also draw bar between handles according to handle positions.
        // selectors of active frames
        public GameObject selectorContainer;
        public GameObject selectorObject;  // should have begin and end which are moveable
        public List<GameObject> selectorList;

        public EditBar(int totalFrames)
        {
            this.totalFrames = totalFrames;
            this.progress = 0;
            this.selectors = new List<(int, int)> {(0, totalFrames)};
            this.activeSelector = 0;

            this.framesPerMeter = 50;
            this.frameSize = 0.015f;
            this.frameSpacing = (1f / framesPerMeter) - frameSize;
            this.borderSpace = frameSpacing * 10;

            //this.selectorsIndicator
            //this.progressIndicator

            // TODO: how does this work exactly? > for now in Dance Editor itself
            // instantiate visual elements
            this.barBackground = Instantiate(barBackground);

            this.Start();
        }


        public void Start()
        {
            Debug.Log("edit bar start creating.");
            // maybe create the backwall 
            barBackground.transform.localScale = new Vector3(totalFrames / framesPerMeter, 1, 1);
            // and place the progress indicator on first frame

            // create Frame Display
            activeFrames = new List<int>();

            Debug.Log(String.Format("EditBar: Number of poses: {0}", totalFrames));

            for(int i = 0; i < totalFrames; ++i)
            {
                // create each frame
                float x_pos = borderSpace + frameSpacing / 2 + i * (frameSize + frameSpacing);
                GameObject indicator = Instantiate(frame, frameDisplay.transform);
                indicator.transform.localPosition += new Vector3(x_pos, 0, 0);
                indicator.transform.localScale = Vector3.Scale(indicator.transform.localScale, new Vector3(frameSize, 1, 1));
                // set all frames initally to active
                activeFrames.Add(i);
            }
            Debug.Log("placed all frames");

            // and also make the selectors

        }


        public void SelectSelector(int index)
        {
            activeSelector = index;
        }

        public void MoveSelector(int index, int newStart, int newEnd)
        {
            selectors[index] = (newStart, newEnd);
        }

        public void Split(int index, int position)
        {
            (int oldStart, int oldEnd) = selectors[index];
            this.MoveSelector(index, oldStart, position);
            selectors.Insert(index + 1, (position, oldEnd));
        }

        public void Update()
        {
            // constraint the selectors to not move beyond the max length (given by nr frames and frame sizes...)
            // looping behaviour
            // set pose according to progress? > rather do this in the dance editor script
        }


    }
}
