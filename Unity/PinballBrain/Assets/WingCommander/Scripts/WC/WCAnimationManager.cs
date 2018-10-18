using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WCAnimationManager {

    public enum AnimationEnum {
        WC2ConcordiaAndAsteroids,
        WC2PilotExplode,
        WC2Facepalm,
        WC2JazzRescue,
        WC2KissMinx,
        WC2KissAngel,
        WC2Asteroid,
        WC2Asteroid2,
        WC2Explosion,
        WC2Explosion2,
        WC2BlueExplosion,
        WC2EngineExhaust
    }

    const string wc2AnimationsPath = "WC2/Sprites/Animation/";

    class WCAnimationData {
        /// <summary>
        /// Path to animation
        /// </summary>
        public string path;
        /// <summary>
        /// Animation has additive comporession?
        /// </summary>
        public bool additiveComporession;
        /// <summary>
        /// Animation should use a specific first frame
        /// </summary>
        public string optionalFirstFramePath;
        /// <summary>
        /// All frames are based on first frame
        /// </summary>
        public bool basedOnFirstFrame;
    }

    static Dictionary<AnimationEnum, WCAnimation> animations = new Dictionary<AnimationEnum, WCAnimation>();

    static Dictionary<AnimationEnum, WCAnimationData> wcAnimationData = new Dictionary<AnimationEnum, WCAnimationData>() {
        { AnimationEnum.WC2ConcordiaAndAsteroids, new WCAnimationData(){
                path = wc2AnimationsPath + "powerdn.v00",
                additiveComporession = true,
                basedOnFirstFrame = false
            } },
        { AnimationEnum.WC2PilotExplode, new WCAnimationData(){
            path = wc2AnimationsPath + "pilotExplode",
            additiveComporession = false,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2Facepalm, new WCAnimationData(){
            path = wc2AnimationsPath + "despair.v00",
            additiveComporession = true,
            basedOnFirstFrame = true
        } },
        { AnimationEnum.WC2JazzRescue, new WCAnimationData(){
            path = wc2AnimationsPath + "jazzresc.v00",
            additiveComporession = true,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2KissMinx, new WCAnimationData(){
            path = wc2AnimationsPath + "kiss.v20",
            additiveComporession = true,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2KissAngel, new WCAnimationData(){
            path = wc2AnimationsPath + "love.v00",
            additiveComporession = false,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2Asteroid, new WCAnimationData(){
            path = wc2AnimationsPath + "Asteroid",
            additiveComporession = false,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2Asteroid2, new WCAnimationData(){
            path = wc2AnimationsPath + "Asteroid2",
            additiveComporession = false,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2Explosion, new WCAnimationData(){
            path = wc2AnimationsPath + "Explosion",
            additiveComporession = false,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2Explosion2, new WCAnimationData(){
            path = wc2AnimationsPath + "Explosion2",
            additiveComporession = false,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2BlueExplosion, new WCAnimationData(){
            path = wc2AnimationsPath + "BlueExplosion",
            additiveComporession = false,
            basedOnFirstFrame = false
        } },
        { AnimationEnum.WC2EngineExhaust, new WCAnimationData(){
            path = wc2AnimationsPath + "EngineExhaust",
            additiveComporession = false,
            basedOnFirstFrame = false
        } }
    };

    public static WCAnimation GetAnimation(AnimationEnum animationID) {
        if (animations.ContainsKey(animationID)) {
            return animations[animationID];
        } else {
            //Load data for animation
            WCAnimationData data = wcAnimationData[animationID];
            Texture2D[] wcAnimation = Resources.LoadAll<Texture2D>(data.path);
            Texture2D firstFrame = null;
            if (!string.IsNullOrEmpty(data.optionalFirstFramePath)) firstFrame = Resources.Load<Texture2D>(data.optionalFirstFramePath);

            //Create animation
            WCAnimation animation = new WCAnimation(wcAnimation, data.additiveComporession, data.basedOnFirstFrame, firstFrame);
            animations.Add(animationID, animation);
            return animation;
        }
    }
}
