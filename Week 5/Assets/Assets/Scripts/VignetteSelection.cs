using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VignetteSelection : MonoBehaviour {
	[SerializeField]
	Text m_TextField;

	[SerializeField]
	string m_EmptyString = "???";

	[SerializeField]
	Button m_Button;

	private MemoriesMenu m_MemoriesMenu;
	private string m_VignetteId;

	public void Init(string vignetteName, MemoriesMenu menu)
	{
		m_VignetteId = vignetteName;
		m_MemoriesMenu = menu;
		UpdateLockedState();
	}

	public void OnClick(){
		m_MemoriesMenu.OnClicked(m_VignetteId);
	}

	public void UpdateLockedState(){
		bool discovered = GameManager.Instance.HasSeenVignette(m_VignetteId);
		
		m_Button.enabled = discovered;
		if(discovered){
			m_TextField.text = m_VignetteId;
		}else{
			m_TextField.text = m_EmptyString;
		}
	}
}
