using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UniRx;
using System;

public class UnityVideoPlayer {
    VideoPlayer videoPlayer = null;
    UnityEngine.UI.RawImage videoOutput;

    ReactiveCommand<VideoPlayer> onVideoStarted;
    ReactiveCommand<VideoPlayer> onVideoFinished;

    public UnityVideoPlayer(UnityEngine.UI.RawImage outputImage) {
        onVideoStarted = new ReactiveCommand<VideoPlayer>();
        onVideoFinished = new ReactiveCommand<VideoPlayer>();

        videoPlayer = new GameObject("VideoPlayer").AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.APIOnly;
        //videoPlayer.url = Application.streamingAssetsPath + "/WC2/Videos/Intro.mp4";
        //videoPlayer.Play();
        videoPlayer.started += _OnVideoStarted;
        videoPlayer.loopPointReached += _OnVideoFinished;

        if(outputImage != null) {
            videoOutput = outputImage;
            videoOutput.enabled = false;
        }
        
        
    }

    public UnityVideoPlayer() : this(null) {
    }

    public void Play(string pathToFile) {
        if (System.IO.File.Exists(pathToFile)) {
            videoPlayer.url = pathToFile;
            videoPlayer.Play();
        } else {
            Debug.LogError("Videofile " + pathToFile + " does not exist.");
        }
    }

    void _OnVideoStarted(VideoPlayer player) {
        //Set video output, if available
        if (videoOutput != null) {
            videoOutput.enabled = true;
            videoOutput.texture = player.texture;
        }
        onVideoStarted.Execute(player);
    }

    public IObservable<VideoPlayer> OnVideoStarted() {
        return onVideoStarted;
    }

    void _OnVideoFinished(VideoPlayer player) {
        //Set video output, if available
        if (videoOutput != null) {
            videoOutput.enabled = false;
        }
        onVideoFinished.Execute(player);
    }

    public IObservable<VideoPlayer> OnVideoFinished() {
        return onVideoFinished;
    }
}
