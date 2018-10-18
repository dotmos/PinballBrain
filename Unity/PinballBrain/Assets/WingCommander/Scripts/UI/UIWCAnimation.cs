using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWCAnimation : MonoBehaviour {

    public WCAnimationManager.AnimationEnum animationID;
    public int fps = 12;
    public bool setNativeSize = false;
    public float nativeSizeRescale = 1;
    public Image image;

    float fpsTarget;
    float fpsCounter = 0;
    int animationFrameIndex = 1;

    WCAnimation wcAnimation;
   
    // Use this for initialization
    void Start () {
        if(image == null) image = GetComponent<Image>();

        wcAnimation = WCAnimationManager.GetAnimation(animationID);

        fpsTarget = 1.0f / (float)fps;

        image.sprite = wcAnimation.GetAnimationSprites()[0];
        image.preserveAspect = true;
        SetNativeSize();
    }

    void SetNativeSize() {
        if (setNativeSize) {
            image.SetNativeSize();
            image.transform.localScale = Vector3.one * nativeSizeRescale;
        }
    }
	
	// Update is called once per frame
	void Update () {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= fpsTarget) {
            fpsCounter = 0;

            image.sprite = wcAnimation.GetAnimationSprites()[animationFrameIndex];
            SetNativeSize();
            animationFrameIndex++;
            if (animationFrameIndex >= wcAnimation.Count) {
                animationFrameIndex = 0;
            }
        }
    }
}
