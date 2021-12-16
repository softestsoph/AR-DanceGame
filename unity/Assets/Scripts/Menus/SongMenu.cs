using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SongMenu : MonoBehaviour
{
    public GameObject SongDisplay;
    public TextMeshPro SongName;
    public TextMeshPro HighScore;

    List<DancePerformanceScriptableObject> dances;
    string? selectedID;

    private void Awake()
    {
        dances = new List<DancePerformanceScriptableObject>(Resources.LoadAll<DancePerformanceScriptableObject>("DancePerformances"));
        SongDisplay.SetActive(false);
    }

    public void selectSong(string songID)
    {
        DancePerformanceScriptableObject dance = dances.Find(element => element.songId == songID);
        if (dance != null)
        {
            SongDisplay.SetActive(true);
            selectedID = songID;
            SongName.text = dance.songTitle;
            HighScore.text = HighScoreData.Instance.GetHighScore(songID).ToString();
            gameObject.GetComponent<AudioSource>().clip = dance.SongObject.SongClip;
            gameObject.GetComponent<AudioSource>().Play();
        }
       
    }

    public void cancelSelection()
    {
        gameObject.GetComponent<AudioSource>().Stop();
        SongDisplay.SetActive(false);
        selectedID = null;
    }

    public void playSelectedSong()
    {
        if (selectedID == null) return;
        PersistentData.Instance.performance = dances.Find(element => element.songId == selectedID);
        SceneManager.LoadScene("Dance Scene", LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        cancelSelection();
    }
}
