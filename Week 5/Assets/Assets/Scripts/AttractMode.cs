using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractMode : MonoBehaviour {

	[SerializeField]
	float SecondsBeforePlaying = 30;
	[SerializeField]
	AudioSource m_AudioToPause;

	float m_TimeElapsed;

	bool m_VideoStarted = false;

	// Update is called once per frame
	void Update () {
		// wait for game manager to load
		if(!GameManager.HasInstance){
			return;
		}

		if(m_VideoStarted){
			m_TimeElapsed = 0;

			if(GameManager.Instance.AnyInputPressed()){
				EndAttractMode();
			}
			if(GameManager.Instance.AttractModeVideo.ScriptableFinished()){
				EndAttractMode();
			}else{
				return;
			}
		}

		if(GameManager.Instance.AnyInputPressed()
			|| GameManager.Instance.IsVignettePlaying()){
			m_TimeElapsed = 0;
		}

		m_TimeElapsed += Time.deltaTime;

		if(m_TimeElapsed >= SecondsBeforePlaying){
			PlayAttractMode();
		}
	}

	private void PlayAttractMode(){
		GameManager.Instance.AttractModeVideo.StartScriptable();
		m_TimeElapsed = 0;
		m_VideoStarted = true;
		m_AudioToPause.Pause();
	}

	private void EndAttractMode(){
		m_TimeElapsed = 0;
		m_VideoStarted = false;
		m_AudioToPause.UnPause();
		GameManager.Instance.AttractModeVideo.ScriptableWrapUp();
	}
}
