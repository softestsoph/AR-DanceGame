using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class HighScoreData
{
    private static HighScoreData _highScoreData;
    private static string HighScorePath = Path.Combine(Application.persistentDataPath, "highScores");

    public static HighScoreData Instance
    {
        get
        {
            if (_highScoreData == null) LoadHighScoreData();
            return _highScoreData;
        }
    }

    private Dictionary<string, int> highScores;

    public HighScoreData ()
    {
        highScores = new Dictionary<string, int>();
    }

    public void UpdateHighScore(string songID, int score)
    {
        if (!highScores.ContainsKey(songID))
        {
            highScores.Add(songID, score);
        }
        else if (highScores[songID] < score)
        {
            highScores[songID] = score;
        }
        SaveHighScoreData();
    }

    public int GetHighScore(string songID)
    {
        if (highScores.ContainsKey(songID)) return highScores[songID];
        return 0;
    }

    public static void LoadHighScoreData()
    {
        if (File.Exists(HighScorePath)){
            using (var reader = new StreamReader(HighScorePath))
                _highScoreData = JsonUtility.FromJson<HighScoreData>(reader.ReadToEnd());
        } else
        {
            _highScoreData = new HighScoreData();
        }
    }

    public static void SaveHighScoreData()
    {
        var stream = new FileStream(HighScorePath, FileMode.Create);

        using (var writer = new StreamWriter(stream))
            writer.Write(JsonUtility.ToJson(_highScoreData));
    }

}
