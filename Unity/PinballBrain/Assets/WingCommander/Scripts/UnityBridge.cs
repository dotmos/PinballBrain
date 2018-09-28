using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;


public class UnityBridge : MonoBehaviour {

    GameBrain brain;

    public List<UIPlayerScore> playerScore;

    // Use this for initialization
    void Start() {
        brain = new GameBrain();

        //Connect player score UI
        brain.OnPlayerScoreChanged(0).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
        brain.OnPlayerScoreChanged(1).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
        brain.OnPlayerScoreChanged(2).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
        brain.OnPlayerScoreChanged(3).Subscribe(e => playerScore[e.playerID].SetScore(e.currentScore)).AddTo(this);
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnGUI() {
        if (GUI.Button(new Rect(0, 0, 100, 32), "Disconnect")) {
            brain.Dispose();
        }
    }
}