using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour {

    public GameObject rocket;
    private bool isTimerSet;
    private float timer;
    private const int maxTimer = 3;
    // Use this for initialization
	void Start()
    {
        resetTimer();
	}

    void startTimer()
    {
        isTimerSet = true;
    }

    void resetTimer()
    {
        isTimerSet = false;
        timer = 0;
    }
    void updateTimer()
    {
        timer += Time.deltaTime;
    }

	// Update is called once per frame
	void Update() 
    {
        if (rocket.GetComponent<Rocket>().finished && !isTimerSet)
        {
            startTimer();
        }
        else if (rocket.GetComponent<Rocket>().finished && isTimerSet)
        {
            updateTimer();
            if (timer > maxTimer)
            {
                print(timer);
                rocket.GetComponent<Rocket>().reset();
                resetTimer();
            }
        }
	}
}
