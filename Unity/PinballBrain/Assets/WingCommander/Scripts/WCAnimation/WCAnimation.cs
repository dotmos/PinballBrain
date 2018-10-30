using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCAnimation {

    /// <summary>
    /// First frame of the wing commander animation. Might be null.
    /// </summary>
    Texture2D wcFirstFrame;

    /// <summary>
    /// Difference only, wing commander style compressed animation
    /// </summary>
    Texture2D[] wcAnimation;

    /// <summary>
    /// Converted animation. Full frames.
    /// </summary>
    Texture2D[] unityAnimation;

    Sprite[] unitySpriteAnimation;

    /// <summary>
    /// Whether or not the wc2 animation has additive compression
    /// </summary>
    bool hasAdditiveCompression;

    /// <summary>
    /// Whether or not the wc2 animation frames are all based on the first frame
    /// </summary>
    bool basedOnFirstFrame;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Count { get { return unityAnimation.Length; } }

    public WCAnimation(Texture2D[] wcAnimation, bool hasAdditiveCompression = true, bool basedOnFirstFrame = false, Texture2D firstFrame = null) {
        this.wcAnimation = wcAnimation;
        this.wcFirstFrame = firstFrame;
        this.hasAdditiveCompression = hasAdditiveCompression;
        this.basedOnFirstFrame = basedOnFirstFrame;

        if (this.hasAdditiveCompression) ConvertAdditiveCompression();
        else Convert();
    }

    public Texture2D[] GetAnimation() {
        return unityAnimation;
    }

    public Sprite[] GetAnimationSprites() {
        return unitySpriteAnimation;
    }

    public Texture2D[] GetWCAnimation() {
        return wcAnimation;
    }

    Texture2D CloneTexture(Texture2D source) {
        Texture2D clone = new Texture2D(source.width, source.height, TextureFormat.RGBA32, source.mipmapCount > 1 ? true : false);
        clone.filterMode = FilterMode.Point;
        clone.SetPixels(source.GetPixels());
        clone.Apply();

        return clone;
    }

    Texture2D MergeByAlpha(Texture2D destination, Texture2D source) {
        Texture2D result = CloneTexture(destination);
        for (int x = 0; x < source.width; ++x) {
            for (int y = 0; y < source.height; ++y) {
                Color c = source.GetPixel(x, y);
                if (c.a > 0) {
                    result.SetPixel(x, y, c);
                }
            }
        }
        result.Apply();
        return result;
    }

    void Convert() {
        unityAnimation = new Texture2D[wcAnimation.Length];
        unitySpriteAnimation = new Sprite[wcAnimation.Length];

        for(int i=0; i<wcAnimation.Length; ++i) {
            Texture2D frame = null;
            if(wcFirstFrame != null) {
                frame = MergeByAlpha(wcFirstFrame, wcAnimation[i]);
            } else {
                frame = CloneTexture(wcAnimation[i]);
            }

            unityAnimation[i] = frame;
            unitySpriteAnimation[i] = Sprite.Create(frame, new Rect(0, 0, frame.width, frame.height), Vector2.zero);
        }
    }

    void ConvertAdditiveCompression() {
        //Create first frame
        Texture2D firstFrame = null;
        if (this.wcFirstFrame != null) {
            firstFrame = CloneTexture(this.wcFirstFrame);
            Texture2D _overlay = wcAnimation[0];
            for(int x=0; x< _overlay.width; ++x) {
                for(int y=0; y<_overlay.height; ++y) {
                    Color c = _overlay.GetPixel(x, y);
                    if(c.a > 0) {
                        firstFrame.SetPixel(x, y, c);
                    }
                }
            }
            firstFrame.Apply();
        } else {
            firstFrame = CloneTexture(wcAnimation[0]);
        }

        Width = firstFrame.width;
        Height = firstFrame.height;

        unityAnimation = new Texture2D[wcAnimation.Length];
        unityAnimation[0] = firstFrame;
        unitySpriteAnimation = new Sprite[wcAnimation.Length];
        unitySpriteAnimation[0] = Sprite.Create(firstFrame, new Rect(0, 0, firstFrame.width, firstFrame.height), Vector2.zero);
        Texture2D lastFrame = firstFrame;

        //Create other frames, using the first frame as base
        for (int i=1; i<unityAnimation.Length; ++i) {
            //Create a copy of the last created frame
            Texture2D unityFrame = null;
            if (this.basedOnFirstFrame) unityFrame = CloneTexture(firstFrame);
            else unityFrame = CloneTexture(lastFrame);

            //Copy relevant parts from wc frame to new frame
            Texture2D wcFrame = wcAnimation[i];
            for (int x = 0; x < wcFrame.width; ++x) {
                for (int y = 0; y < wcFrame.height; ++y) {
                    Color c = wcFrame.GetPixel(x, y);
                    if (c.a > 0) {
                        unityFrame.SetPixel(x, y, c);
                    }
                }
            }
            unityFrame.Apply();

            //Store frame
            unityAnimation[i] = unityFrame;            

            //Store sprite
            unitySpriteAnimation[i] = Sprite.Create(unityFrame, new Rect(0, 0, unityFrame.width, unityFrame.height), Vector2.zero);

            lastFrame = unityFrame;
        }
    }
}
