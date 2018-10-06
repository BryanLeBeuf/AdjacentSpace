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
		if(m_VideoStarted){
			m_TimeElapsed = 0;

			if(AnyInputPressed()){
				EndAttractMode();
			}
			if(GameManager.Instance.AttractModeVideo.ScriptableFinished()){
				EndAttractMode();
			}else{
				return;
			}
		}

		if(AnyInputPressed()){
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

	private bool AnyInputPressed(){
		return Input.anyKey || !Mathf.Approximately(Input.GetAxis("Mouse X"), 0) || !Mathf.Approximately(Input.GetAxis("Mouse Y"), 0);
	}

	private void EndAttractMode(){
		m_TimeElapsed = 0;
		m_VideoStarted = false;
		m_AudioToPause.UnPause();
		GameManager.Instance.AttractModeVideo.ScriptableWrapUp();
	}
}
