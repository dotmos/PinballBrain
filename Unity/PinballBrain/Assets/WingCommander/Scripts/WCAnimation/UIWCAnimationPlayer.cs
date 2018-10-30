using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWCAnimationPlayer : WCAnimationPlayerBase {
    public Image image;
    public bool setNativeSize = false;
    public float nativeSizeRescale = 1;

    protected override void OnStart() {
        base.OnStart();

        if (image == null) image = GetComponent<Image>();
    }

    void SetNativeSize() {
        if (setNativeSize) {
            image.SetNativeSize();
            image.transform.localScale = Vector3.one * nativeSizeRescale;
        }
    }

    protected override void UpdateFrame(Sprite frame) {
        image.preserveAspect = true;
        image.sprite = frame;
        SetNativeSize();
    }
}
