using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatsMenu : MonoBehaviour {

	public void PlayVignette(string name){
		if(GameManager.Instance.IsVignettePlaying()){
			GameManager.Instance.ForceFinishCurrentVignette();
		}
		GameManager.Instance.DisplayVignette(name);
		GameManager.Instance.SetCheatsMenuActive(false);
	}

	public void SkipVideo(){
		GameManager.Instance.SkipCurrentVignetteVideo();
		GameManager.Instance.SetCheatsMenuActive(false);
	}

	public void FinishCurrentVignette(){
		if(GameManager.Instance.IsVignettePlaying()){
			GameManager.Instance.SetCheatsMenuActive(false);
			GameManager.Instance.ForceFinishCurrentVignette();
		}
	}

	public void GiveKey(int keyId){
		GameManager.Instance.SetCheatsMenuActive(false);
		GameManager.Instance.GivePlayerKey(keyId);
	}

	public void UnlockAllVignettes(){
		foreach(string id in GameManager.Instance.AllVignetteIds()){
			GameManager.Instance.SetVignetteFound(id);
		}
		GameManager.Instance.SetCheatsMenuActive(false);
	}

	public void ForceOpenMainMenu(){
		if(GameManager.Instance.IsVignettePlaying()){
			GameManager.Instance.HideVignette();
		}
		GameManager.Instance.ShowGameMainMenu(true);
		GameManager.Instance.SetCheatsMenuActive(false);
	}

	public void ToggleFastRun(){
		GameManager.Instance.FastRun = !GameManager.Instance.FastRun;
	}

	public void SkipToIvanhoe(){	
		Application.LoadLevel("Ply_test");
		GameManager.Instance.SetCheatsMenuActive(false);
	}

	public void SkipToHouse(){	
		Application.LoadLevel("LevelThree");
		GameManager.Instance.SetCheatsMenuActive(false);
	}
}
