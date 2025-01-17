﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityStandardAssets.ImageEffects;

public class GameManager : GameSingleton<GameManager> {

	[SerializeField]
	bool CheatsMenuEnabled = true;

	private SaveData m_SaveData = new SaveData();

	private Dictionary<string, VignetteConfig> m_Vignettes = new Dictionary<string, VignetteConfig>();
	private List<string> m_VignetteIds = new List<string>();

	private Dictionary<InteractionSensor, bool> m_InteractionSensorReporting = new Dictionary<InteractionSensor, bool>();

	private AudioSource m_PlayingAudioSource;

	[HideInInspector]
	public bool GameplayScene = false;
	[HideInInspector]
	public string DefaultMapConfig = null;
	[HideInInspector]
	public bool FastRun = false;
	[SerializeField]
	public VignetteScriptableVideoPlayer AttractModeVideo;

	[SerializeField]
	float SecondsTillBootBackToTitleScreen = 60 * 4;
	private float m_SecondsSinceInput;

	HashSet<Camera> m_ActiveCameras = new HashSet<Camera>();
	HashSet<Menu> m_AllSubMenus = new HashSet<Menu>();

	Menu m_CurrentMenu;
	[SerializeField]
	Menu MainMenu;
	[SerializeField]
	Menu CheatMenu;
	[SerializeField]
	VignetteOverlay VignetteOverlay;
	[SerializeField]
	Menu InteractionMenu;
	[SerializeField]
	GameObject VideoPlayerOverlay;
	[SerializeField]
	float m_BlurAmountWhenPaused = 1.85f;
	[SerializeField]
	float m_SubtitleManagerAlphaWhenPaused = 0.3f;

	[SerializeField]
	GameObject LoadingHider;

	[SerializeField]
	VignetteConfig[] VignettesConfig;

	private float m_AudioListenerVolume = 1.0f;

	void Start () {
		DontDestroyOnLoad(gameObject);

		// load and init menus.
		m_AllSubMenus = new HashSet<Menu>(GetComponentsInChildren<Menu>(true));
		foreach(Menu menu in m_AllSubMenus){
			menu.gameObject.SetActive(false);
		}
		SetBlur(0);

		// cache data
		foreach(VignetteConfig config in VignettesConfig){
			if(String.IsNullOrEmpty(config.Name)){
				continue;
			}
			m_Vignettes[config.Name] = config;
			m_VignetteIds.Add(config.Name);
		}
		VignetteOverlay.gameObject.SetActive(false);
		VideoPlayerOverlay.SetActive(false);

		LoadingHider.SetActive(false);

		m_AudioListenerVolume = UnityEngine.AudioListener.volume;
	}

	public void ForceOpenLoadingHider(){
		LoadingHider.SetActive(true);
	}

	public bool LoadingHiderIsVisible(){
		return LoadingHider.activeSelf;
	}

	public delegate void LoadCompleted();
	private IEnumerator WaitAndLoadLevel(float wait, string scene, LoadCompleted callback) {
		if(wait>0){
			yield return new WaitForSeconds(wait);
		}
		LoadingHider.SetActive(true);
		// wait 3frames
		yield return new WaitForSeconds(1.01f);
		yield return null;
		Application.LoadLevel(scene);
		yield return new WaitForSeconds(.01f);
		yield return null;
		LoadingHider.SetActive(false);
		callback();
	}

	public void LoadLevel(string level, float wait = 0){
		StartCoroutine(WaitAndLoadLevel(wait, level, () => {}));
	}


	public void NotifySceneLoaded(){
		SetBlur(0);
		m_InteractionSensorReporting.Clear();
	}

	public bool IsPaused(){
		return m_CurrentMenu != null;
	}
	
	public bool IsVideoPlaying(){
		return VideoPlayerOverlay.activeSelf;
	}

	void Update(){
		ProcessLoadingAudio();
		ProcessMenus();

		InteractionSensor sensor = PlayerWithinInteractionSensor();
		bool showOverlay = (sensor!= null && sensor.CanDisplayUITrigger());
		if(showOverlay!=ShowingInteractionMenu()){
			ShowInteractionMenu(showOverlay);
		}

		ProcessAutoReturnToTitleScreen();
	}

	private void ProcessLoadingAudio(){
		UnityEngine.AudioListener.volume = (float)(LoadingHiderIsVisible() ? 0 : m_AudioListenerVolume);
	}

	void ProcessMenus(){

		if(!IsVignettePlaying() && Input.GetKeyDown(KeyCode.Escape)){
			ShowGameMainMenu(GameplayScene && m_CurrentMenu == null);
		}

		// testing;
		if(Input.GetKeyDown(KeyCode.BackQuote)){
			SetCheatsMenuActive(!GetCheatsMenuActive());
		}
	}


	void ProcessAutoReturnToTitleScreen(){
		// reset if input pressed or video playing.
		if(AnyInputPressed() || VideoPlayerOverlay.activeSelf){
			m_SecondsSinceInput = 0;
			return;
		}else if (m_SecondsSinceInput < SecondsTillBootBackToTitleScreen){
			m_SecondsSinceInput += Time.deltaTime;
		}

		if(m_SecondsSinceInput >= SecondsTillBootBackToTitleScreen){
			bool onTitleScreen = TitleScreenManager.HasInstance;
			if(!onTitleScreen || IsVignettePlaying()){
				PlayerClickedReturnToMainMenu();
				m_SecondsSinceInput = 0;
			}
		}
	}

	public void PlayerClickedResumeGameFromMenu(){
		ShowGameMainMenu(GameplayScene && m_CurrentMenu == null);
	}

	public void PlayerClickedReturnToMainMenu(){
		m_SaveData = new SaveData();
		GameManager.Instance.LoadLevel("TitleScreen");
		if(m_CurrentMenu != null){
			ShowGameMainMenu(GameplayScene && m_CurrentMenu == null);
		}
		if(NarrationManager.instance != null){
			NarrationManager.instance.Clear();
		}
		HideVignette();
	}

	public void GivePlayerKey(int keyId){
		m_SaveData.m_ObtainedKeys.Add(keyId);
	}

	public bool HasKey(int keyId){
		return m_SaveData.m_ObtainedKeys.Contains(keyId);
	}

	public void DisplayVignette(string name, VignetteOverlay.FinishedAction finishedAction = null){
		VignetteVideoConfig config = VignetteAuthoring.Instance.GetVideoConfig(name);
		VignetteOverlay.DisplayVignette(config, finishedAction);
		VignetteOverlay.gameObject.SetActive(true);
	}

	public void ForceFinishCurrentVignette(){
		VignetteOverlay.ForceFinishCurrentVignette();
	}

	public void SetVignetteFound(string name){
		m_SaveData.m_ViewedVignetts.Add(name);
	}

	public bool HasSeenVignette(string name){
		return m_SaveData.m_ViewedVignetts.Contains(name);
	}

	public void HideVignette(){
		VignetteOverlay.gameObject.SetActive(false);
		PlayAudioSource(null);
	}

	public void SkipCurrentVignetteVideo(){
		VignetteOverlay.ForceSkipCurrentVignetteVideo();
	}

	public void DisplayVideoPlayerOverlay(bool show){
		VideoPlayerOverlay.SetActive(show);
	}

	private void SetNarrationSystemAlpha(float amount){
		SubtitleManager[] subSystem = FindObjectsOfType<SubtitleManager>(); 
		foreach(SubtitleManager sub in subSystem){
			CanvasGroup group = sub.GetComponent<CanvasGroup>();
			if(group!=null){
				group.alpha = amount;
			}
		}
	}

	private void SetBlur(float amount){
		BlurOptimized[] blurs = FindObjectsOfType<BlurOptimized>(); 
		foreach(BlurOptimized blur in blurs){
			if(amount<=0){
				blur.enabled = false;
			}else{
				blur.enabled = true;
			}
			blur.blurSize = amount;
		}
	}

	public List<string> AllVignetteIds(){
		return m_VignetteIds;
	}

	public VignetteConfig? GetVignetteConfig(string id){
		if(!m_Vignettes.ContainsKey(id)){
			return null;
		}
		return m_Vignettes[id];
	}

	public bool IsVignettePlaying(){
		return VignetteOverlay.gameObject.activeSelf;
	}

	public void PlayAudioSource(AudioSource audioSource){
		if(m_PlayingAudioSource != null){
			m_PlayingAudioSource.Stop();
		}
		
		m_PlayingAudioSource = audioSource;
		if(m_PlayingAudioSource != null){
			m_PlayingAudioSource.Play();
		}
	}

	public void SetAudioSourceVolume(float volume){
		if(m_PlayingAudioSource != null){
			m_PlayingAudioSource.volume = volume;
		}
	}

	public float GetAudioSourceVolume(){
		if(m_PlayingAudioSource != null){
			return m_PlayingAudioSource.volume;
		}
		return 0;
	}

	public void SetCheatsMenuActive(bool setActive){
		if(!CheatsMenuEnabled){
			return;
		}
		CheatMenu.gameObject.SetActive(setActive);
	}

	public bool GetCheatsMenuActive(){
		return CheatMenu.gameObject.activeSelf;
	}

	public void InteractionSensorReport(InteractionSensor sensor){
		m_InteractionSensorReporting[sensor] = sensor.PlayerWithin();
	}

	public InteractionSensor PlayerWithinInteractionSensor(){
		foreach(KeyValuePair<InteractionSensor, bool> entry in m_InteractionSensorReporting){
			if(entry.Key==null){
				continue;
			}
			if (entry.Value){
				return entry.Key;
			}
		}

		return null;
	}

	public void ShowInteractionMenu(bool show){
		InteractionMenu.gameObject.SetActive(show);
	}

	public bool ShowingInteractionMenu(){
		return InteractionMenu.gameObject.activeSelf;
	}

	public void ShowGameMainMenu(bool show){
		// start menus
		if(show){
			MainMenu.gameObject.SetActive(true);
			m_CurrentMenu = MainMenu;
			SetBlur(m_BlurAmountWhenPaused);
			SetNarrationSystemAlpha(m_SubtitleManagerAlphaWhenPaused);
		// end menus
		}else{
			if(m_CurrentMenu!=null){
				m_CurrentMenu.gameObject.SetActive(false);
				m_CurrentMenu = null;
				SetBlur(0);
				SetNarrationSystemAlpha(1);
			}
		}
	}

	public bool ActivationKeyPressed(){
		return Input.GetKeyDown(KeyCode.Return)
			|| Input.GetKeyDown(KeyCode.Space)
			|| Input.GetMouseButtonDown(0);
	}

	public bool AnyInputPressed(){
		return Input.anyKey || !Mathf.Approximately(Input.GetAxis("Mouse X"), 0) || !Mathf.Approximately(Input.GetAxis("Mouse Y"), 0);
	}

	public void QuitGame(){
        Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}

[Serializable]
public struct VignetteConfig
{
	public string Name;
	public Texture MapImage;
	public bool HideInList;
}

[Serializable]
public struct AudioClipConfig
{
	public AudioClip AudioClip;
}

public class SaveData {
	public HashSet<string> m_ViewedVignetts = new HashSet<string>();
	public HashSet<int> m_ObtainedKeys = new HashSet<int>();
}
