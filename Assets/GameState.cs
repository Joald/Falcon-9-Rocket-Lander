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
        StreamReader sr = new StreamReader("score.json");
        string s = sr.ReadToEnd();
        sr.Close();
        score = new Score();
        JsonUtility.FromJsonOverwrite(s, score);
        updateScoreText();
	}

    void updateScoreText()
    {
        scoreText.text = "Score: " + score.wins + "/" + score.tries;
    }

    public void lose()
    {
        score.tries++;
        StreamWriter sw = new StreamWriter("score.json");
        string s = JsonUtility.ToJson(score);
        print(s);
        sw.Write(s);
        sw.Close();
        updateScoreText();
    }

    public void win()
    {
        score.tries++;
        score.wins++;
        updateScoreText();
    }

	// Update is called once per frame
	void Update()
    {
		
	}
}
