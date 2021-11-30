using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndScoreScreen : MonoBehaviour
{
    public TextMeshPro TotalScore;
    public TextMeshPro Greats;
    public TextMeshPro Goods;
    public TextMeshPro Bads;
    public TextMeshPro NewHighScore;

    public void setValues(float totalScore, int greats, int goods, int bads, bool highScore)
    {
        TotalScore.text = totalScore.ToString();
        Greats.text = greats.ToString();
        Goods.text = goods.ToString();
        Bads.text = bads.ToString();
        NewHighScore.gameObject.SetActive(highScore);
    }
}
