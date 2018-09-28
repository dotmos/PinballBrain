using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerScore : MonoBehaviour {

    public Text score;

    public void SetScore(int score) {
        this.score.text = score.ToString();
    }
}
