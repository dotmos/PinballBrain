using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCAnimationPlayer : WCAnimationPlayerBase {
    public SpriteRenderer sprite;

    protected override void OnStart() {
        base.OnStart();

        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
    }

    protected override void UpdateFrame(Sprite frame) {
        sprite.sprite = frame;
    }
}
