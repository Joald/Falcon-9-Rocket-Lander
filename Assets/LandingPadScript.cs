using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPadScript : MonoBehaviour {
    System.Random random;

    public void randomize()
    {
        random = new System.Random();
        Vector3 v = Camera.main.ScreenToWorldPoint(new Vector3(random.Next(Screen.width * 3 / 4) + Screen.width / 8, 0, 0));
        transform.position = new Vector3(v.x, transform.position.y, transform.position.z);
    }

	// Use this for initialization
	void Start () 
    {
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
