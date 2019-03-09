using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MemoriesMap : MonoBehaviour {

	[SerializeField]
	RawImage m_MapOverlayImage;

	[SerializeField]
	GameObject m_Locations;

	[SerializeField]
	GameObject m_ReplayButton;

	private string m_CurrentVignetteId;

	public void Start(){
	    foreach(Transform child in m_Locations.transform)
	    {
	        child.gameObject.SetActive(false);
	    }
	}

	void OnEnable(){
	    m_ReplayButton.gameObject.SetActive(false);
	}

	public void SetMapLocation(string vignettedId){
		if(vignettedId!=null){
			VignetteConfig? config = GameManager.Instance.GetVignetteConfig(vignettedId);
			if(config!=null){
				m_MapOverlayImage.texture = ((VignetteConfig)config).MapImage;
			}
		}

		m_CurrentVignetteId = vignettedId;
	    m_ReplayButton.gameObject.SetActive(vignettedId!=null);

		Color color = m_MapOverlayImage.color;
		color.a = vignettedId == null ? 0 : 1;
		m_MapOverlayImage.color = color;

	    foreach(Transform child in m_Locations.transform)
	    {
	        child.gameObject.SetActive(child.gameObject.name == vignettedId);
	    }
	}

	public void ReplayVignette(){
		if(m_CurrentVignetteId!=null){
			GameManager.Instance.DisplayVignette(m_CurrentVignetteId);
		EventSystem.current.SetSelectedGameObject(null);
		}
	}
}
