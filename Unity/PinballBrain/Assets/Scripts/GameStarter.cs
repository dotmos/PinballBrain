using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour {

    GameBrain brain;

	// Use this for initialization
	void Start () {
        brain = new GameBrain();
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI() {
        if(GUI.Button(new Rect(0, 0, 100, 32), "Disconnect")) {
            brain.Dispose();
        }
    }
}
