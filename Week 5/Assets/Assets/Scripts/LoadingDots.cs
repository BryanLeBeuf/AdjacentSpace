using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingDots : MonoBehaviour {

	[SerializeField]
	string m_LoadingText;

	[SerializeField]
	double m_Speed = 1;

	[SerializeField]
	Text m_textField;

	private double progress;

	void OnEnable(){
		m_textField.text = m_LoadingText;
		progress = 0;
	}

	// Update is called once per frame
	void Update () {
		progress+= (m_Speed * Time.deltaTime)/60.0;

		while(progress>3){
			progress-=3;
		}

		m_textField.text = m_LoadingText;
		for (int i = 0; i < progress - 1; i++){
			m_textField.text += ".";
		}
	}
}
