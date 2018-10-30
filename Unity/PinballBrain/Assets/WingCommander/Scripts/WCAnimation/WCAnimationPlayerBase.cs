using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WCAnimationPlayerBase : MonoBehaviour {

    public WCAnimationManager.AnimationEnum animationID;
    public int fps = 12;
        
    [Tooltip("Object to destroy when animation is finished")]
    public GameObject destroyOnAnimationFinish;

    public enum Mode {
        Loop,
        PlayOnce
    }
    public Mode mode = Mode.Loop;


    float fpsTarget;
    float fpsCounter = 0;
    int animationFrameIndex = 1;

    WCAnimation wcAnimation;

    // Use this for initialization
    void Start() {
        wcAnimation = WCAnimationManager.GetAnimation(animationID);

        fpsTarget = 1.0f / (float)fps;

        OnStart();

        UpdateFrame(wcAnimation.GetAnimationSprites()[0]);
    }

    protected virtual void OnStart() {

    }

    
    protected abstract void UpdateFrame(Sprite frame);

    // Update is called once per frame
    void Update() {
        fpsCounter += Time.deltaTime;
        if (fpsCounter >= fpsTarget) {
            fpsCounter = 0;

            UpdateFrame(wcAnimation.GetAnimationSprites()[animationFrameIndex]);
           
            animationFrameIndex++;
            if (animationFrameIndex >= wcAnimation.Count) {
                if (mode == Mode.Loop) {
                    animationFrameIndex = 0;
                } else if (mode == Mode.PlayOnce) {
                    if (destroyOnAnimationFinish != null) Destroy(destroyOnAnimationFinish);
                    if (this != null) Destroy(this);
                }

            }
        }
    }
}
