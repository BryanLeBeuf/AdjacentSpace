using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VignetteScriptableVideoPlayer : VignetteScriptable {

	[SerializeField]
	VideoPlayer m_VideoPlayer;

    private bool m_Finished = false;

    void Start(){
        m_VideoPlayer.loopPointReached += OnVideoFinishedPlaying;
    }

    private void OnVideoFinishedPlaying(UnityEngine.Video.VideoPlayer vp){
        m_Finished = true;
    }

    public override void StartScriptable(){
        m_VideoPlayer.Play();
        GameManager.Instance.DisplayVideoPlayerOverlay(true);
        m_Finished = false;
    }

    public override bool ScriptableFinished(){
	    return m_Finished;
    }

    void StopPlaying(){
        m_VideoPlayer.Stop();
        m_Finished = true;
    }

    public override void ScriptableWrapUp(){
        StopPlaying();
        GameManager.Instance.DisplayVideoPlayerOverlay(false);
    }
}
