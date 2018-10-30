using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerScore : MonoBehaviour {

    public Text score;
    public Text ball;

    public void SetScore(int score) {
        this.score.text = score.ToString();
    }

    public void SetBall(int ball) {
        this.ball.text = "BALL " + ball.ToString();
    }

    public void Show(bool show) {
        gameObject.SetActive(show);
    }
}
