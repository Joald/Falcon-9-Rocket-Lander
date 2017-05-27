using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine;

[System.Serializable]
public class GameState : MonoBehaviour {

    private class Score
    {
        public int wins;
        public int tries;
    }

    private Score score;

    public Text scoreText;
	// Use this for initialization
	void Start()
    {
        score = new Score();
        if (System.IO.File.Exists(Application.persistentDataPath + "/score.json"))
        {
            StreamReader sr = new StreamReader(Application.persistentDataPath + "/score.json");
            string s = sr.ReadToEnd();
            sr.Close();
            JsonUtility.FromJsonOverwrite(s, score);
        }
        else
        {
            score.tries = 0;
            score.wins = 0;
        }

        updateScoreText();
        Screen.orientation = ScreenOrientation.Portrait;
    }

    void updateScoreText()
    {
        scoreText.text = "Score: " + score.wins + "/" + score.tries;
    }

    void writeScore()
    {
        StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/score.json");
        string s = JsonUtility.ToJson(score);
        sw.Write(s);
        sw.Close();
    }

    public void lose()
    {
        score.tries++;
        writeScore();
        updateScoreText();
    }

    public void win()
    {
        score.wins++;
        lose();
    }

	// Update is called once per frame
	void Update()
    {
		
	}
}
