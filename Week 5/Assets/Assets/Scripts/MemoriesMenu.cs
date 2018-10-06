using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemoriesMenu : MonoBehaviour {

	[SerializeField]
	GameObject VignetteSelectionContainer;

	[SerializeField]
	VignetteSelection m_Prefab;

	[SerializeField]
	MemoriesMap m_Map;

	[SerializeField]
	GameObject MenuRoot;

	[SerializeField]
	GameObject SystemMenu;

	private VignetteSelection m_PreviouslySelected;

	private List<VignetteSelection> m_Vignettes = new List<VignetteSelection>();

	void Start(){
		MenuRoot.SetActive(true);
		SystemMenu.SetActive(false);

		Utilities.DeleteAllChildren(VignetteSelectionContainer);

		List<string> vignetteIds = GameManager.Instance.AllVignetteIds();
		foreach(string id in vignetteIds){
			VignetteConfig? config = GameManager.Instance.GetVignetteConfig(id);
			if(config == null || ((VignetteConfig)config).HideInList){
				continue;
			}

			VignetteSelection entry = (VignetteSelection) Instantiate(m_Prefab);
			entry.transform.SetParent(VignetteSelectionContainer.transform);
			entry.transform.localScale = Vector3.one;
			entry.Init(id, this);
			m_Vignettes.Add(entry);
		}

		UpdateVignettes();
	}

	public void OpenSystemMenu(){
		MenuRoot.SetActive(false);
		SystemMenu.SetActive(true);
	}

	public void CloseSystemMenu(){
		MenuRoot.SetActive(true);
		SystemMenu.SetActive(false);
	}

	public void ClickedResumeGame(){
		GameManager.Instance.PlayerClickedResumeGameFromMenu();
	}

	public void ClickedMainMenu(){
		GameManager.Instance.PlayerClickedReturnToMainMenu();
	}

	void OnDisable(){
		CloseSystemMenu();
	}

	void OnEnable(){
		UpdateVignettes();

		m_Map.SetMapLocation(GameManager.Instance.DefaultMapConfig);

		if(m_PreviouslySelected != null){
			m_PreviouslySelected.GetComponentInChildren<Button>().interactable = true;
			m_PreviouslySelected = null;
		}
	}

	private void UpdateVignettes(){
		foreach(VignetteSelection entry in m_Vignettes){
			entry.UpdateLockedState();
		}
	}

	public void OnClicked(string id, VignetteSelection selectedVignette){
		m_Map.SetMapLocation(id);

		if(m_PreviouslySelected != null){
			m_PreviouslySelected.GetComponentInChildren<Button>().interactable = true;
			m_PreviouslySelected = null;
		}

		m_PreviouslySelected = selectedVignette;
		m_PreviouslySelected.GetComponentInChildren<Button>().interactable = false;
	}
}
