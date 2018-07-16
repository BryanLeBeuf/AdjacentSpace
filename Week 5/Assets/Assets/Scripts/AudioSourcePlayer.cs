using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePlayer : MonoBehaviour {

	[SerializeField]
	AudioSource m_AudioSource;

	// Use this for initialization
	void Start () {
		StartCoroutine("PlayMusic");
	}

	private IEnumerator PlayMusic(){
		while(!GameManager.HasInstance){
			yield return null;
		}
		GameManager.Instance.PlayAudioSource(m_AudioSource);
	}
}
