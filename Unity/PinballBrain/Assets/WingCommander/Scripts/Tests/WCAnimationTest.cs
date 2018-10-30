using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WCAnimationTest : MonoBehaviour {

    public Image animationOutput;
    public int fps = 8;
    public WCAnimationManager.AnimationEnum animationID;

    float fpsTarget;
    float fpsCounter = 0;
    int animationFrameIndex = 1;

    WCAnimation wcAnimation;

    // Use this for initialization
    void Start () {
        wcAnimation = WCAnimationManager.GetAnimation(animationID);

        fpsTarget = 1.0f / (float)fps;

        animationOutput.sprite = wcAnimation.GetAnimationSprites()[0];
        animationOutput.preserveAspect = true;
	}
	
	// Update is called once per frame
	void Update () {
        fpsCounter += Time.deltaTime;
        if(fpsCounter >= fpsTarget) {
            fpsCounter = 0;

            animationOutput.sprite = wcAnimation.GetAnimationSprites()[animationFrameIndex];
            animationOutput.SetNativeSize();
            animationFrameIndex++;
            if(animationFrameIndex >= wcAnimation.Count) {
                animationFrameIndex = 0;
            }
        }
	}
}
