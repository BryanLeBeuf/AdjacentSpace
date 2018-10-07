using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteAuthoring : GameSingleton<VignetteAuthoring> {
	public Dictionary<string, VignetteVideoConfig> m_ConfigurationsById = new Dictionary<string, VignetteVideoConfig>();

	void Start(){
		DontDestroyOnLoad(gameObject);
		foreach(VignetteConfiguration vigConfig in GetComponentsInChildren<VignetteConfiguration>()){
			VignetteVideoConfig config = vigConfig.Config;

			//initialize some variables
			if(config.AudioSourceToPlay!=null){
				config.InitialMusicVolume = config.AudioSourceToPlay.volume;
			}

			config.UniqueId = vigConfig.gameObject.name;

			if(m_ConfigurationsById.ContainsKey(config.UniqueId)){
				Debug.LogError("Cannot have two vignette configurations with the same name: " + config.UniqueId);
			}
			if(String.IsNullOrEmpty(config.UniqueId)){
				continue;
			}
			m_ConfigurationsById[config.UniqueId] = config;

		}
	}

	public VignetteVideoConfig GetVideoConfig(string id){
		return m_ConfigurationsById[id];
	}
}

[Serializable]
public struct VignetteVideoConfig {
	[HideInInspector]
	public string UniqueId;
	public string DisplayName;
	public VignetteVideoStepConfig[] IndividualSteps;
	public Vector2 TextOffset;
	public float TextBoxWidth;
	public float TextSpeed;
	public Font Font;
	public int FontSize;
	public float LineSpacing;
	public Vector2 BackgroundImageOffset;
	public Color BackgroundColor;
	public Sprite Background;
	public float BackgroundScale;
	public bool PlayAudioSource;
	public AudioSource AudioSourceToPlay;
	public bool FadeIn;	
	public Color FadeInColor;
	public AnimationCurve FadeInCurve;
	public bool FadeOut;	
	public Color FadeOutColor;
	public AnimationCurve FadeOutCurve;
	public bool ClearTextOnFadeOut;	
	public bool HideAdvanceIndicator;
	[HideInInspector]
	public float InitialMusicVolume;
}

[Serializable]
public struct VignetteVideoStepConfig {
	public string Text;
	public float DelayInSecsBeforeTextShows;
	public float TextFadeInSecs;
	public float TextFadeOutSecs;
	public VignetteVideoStepExtraConfig[] Extras;
}

[Serializable]
public struct VignetteVideoStepExtraConfig {
	public bool AdjustMusicVolume;
	public float MusicVolume;
	public bool AdjustTextSpeed;
	public float TextSpeed;
	public Vector2 TextOffset;
	public bool ChangeBackground;
	public Vector2 BackgroundImageOffset;
	public Color BackgroundColor;
	public Sprite Background;
	public float BackgroundScale;
	public VignetteScriptable ScriptToRun;
}