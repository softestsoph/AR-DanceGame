using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace PoseTeacher {
    public class ScoreDisplayJD : MonoBehaviour, IScoreDisplay {
        public Color GreatTextColor;
        public Color GreatOutlineColor;
        public Color GreatVFXColor;

        public Color GoodTextColor;
        public Color GoodOutlineColor;
        public Color GoodVFXColor;

        public Color BadTextColor;
        public Color BadOutlineColor;
        public Color BadVFXColor;

        public Animator baseAnimator;
        public Animator scoreCounterAnimator;
        public TextMeshPro ScoreDisplayText;
        public TextMeshPro ScoreCounterText;
        public VisualEffect vfx;

        public bool debugShow = false;
        public bool debugCounter = false;
        public int Score => currentScore;

        private int currentScore = 0;
        private float animatedScore = 0f;
        private MeshRenderer textMRenderer;
        private int nextDebug = 0;

        public void showScore(Scores score) {
            if(score == Scores.GREAT) {
                ScoreDisplayText.text = "GREAT";
                textMRenderer.material.SetColor("_FaceColor", GreatTextColor);
                textMRenderer.material.SetColor("_OutlineColor", GreatOutlineColor);
                vfx.SetVector4("Color", GreatVFXColor);
                vfx.SendEvent("Great");
            } else if(score == Scores.GOOD) {
                ScoreDisplayText.text = "GOOD";
                textMRenderer.material.SetColor("_FaceColor", GoodTextColor);
                textMRenderer.material.SetColor("_OutlineColor", GoodOutlineColor);
                vfx.SetVector4("Color", GoodVFXColor);
                vfx.SendEvent("Good");
            } else {
                ScoreDisplayText.text = "BAD";
                textMRenderer.material.SetColor("_FaceColor", BadTextColor);
                textMRenderer.material.SetColor("_OutlineColor", BadOutlineColor);
                vfx.SetVector4("Color", BadVFXColor);
                vfx.SendEvent("Bad");
            }

            baseAnimator.Play("ScoreText", -1, 0f);
        }

        // Start is called before the first frame update
        void Awake() {
            textMRenderer = ScoreDisplayText.gameObject.GetComponent<MeshRenderer>();
        }

        // Update is called once per frame
        void Update() {
            if(debugShow) {
                debugShow = false;
                showScore((Scores)(nextDebug++ %3));
            }
            if(debugCounter) {
                debugCounter = false;
                addScore(1000);
            }

            float speed = Mathf.Clamp01((currentScore - animatedScore) / 10) / 2 + 1.1f;
            animatedScore = Mathf.Lerp(animatedScore, currentScore, speed * Time.deltaTime);
            ScoreCounterText.text = "<mspace=3.5>" + Mathf.CeilToInt(animatedScore).ToString() + "</mspace>";
        }

        public void addScore(int scoreToAdd) {
            scoreCounterAnimator.Play("ScoreNumberAdding", -1, 0f);
            currentScore += scoreToAdd;
        }
    }


}
