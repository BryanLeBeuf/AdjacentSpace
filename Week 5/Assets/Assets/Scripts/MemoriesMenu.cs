using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoriesMenu : MonoBehaviour {

	[SerializeField]
	GameObject VignetteSelectionContainer;

	[SerializeField]
	VignetteSelection m_Prefab;

	[SerializeField]
	MemoriesMap m_Map;

	private List<VignetteSelection> m_Vignettes = new List<VignetteSelection>();

	void Start(){

		Utilities.DeleteAllChildren(VignetteSelectionContainer);

		List<string> vignetteIds = GameManager.Instance.AllVignetteIds();
		foreach(string id in vignetteIds){
			VignetteSelection entry = (VignetteSelection) Instantiate(m_Prefab);
			entry.transform.SetParent(VignetteSelectionContainer.transform);
			entry.transform.localScale = Vector3.one;
			entry.Init(id, this);
			m_Vignettes.Add(entry);
		}

		UpdateVignettes();
	}

	void OnEnable(){
		UpdateVignettes();
		m_Map.SetMapLocation(null);
	}

	private void UpdateVignettes(){
		foreach(VignetteSelection entry in m_Vignettes){
			entry.UpdateLockedState();
		}
	}

	public void OnClicked(string id){
		m_Map.SetMapLocation(id);
	}
}
