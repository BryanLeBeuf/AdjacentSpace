using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VignetteOverlay : MonoBehaviour {

	public delegate void FinishedAction();

	private List<AudioSource> m_PreviouslyPlayingAudioSources = new List<AudioSource>();

	private const string kInivisibleColorStart = "<color=#00000000>";
	private const string kInivisibleColorEnd = "</color>";
	private const float kAudioVolumeAdjust = .006f;

	[SerializeField]
	Text m_Text;
	[SerializeField]
	Image m_BackgroundColor;
	[SerializeField]
	Image m_Background;
	[SerializeField]
	Image m_FadeOverlay;

	[SerializeField]
	CanvasGroup m_ReadyToAdvanceAnimation;

	double m_TextSpeed = 2;

	string text;
	double textIndex;
	float delayInSecsBeforeStarting;

	private int currentIndex = 0;
	private int furthestSeenIndex = 0;
	private VignetteVideoConfig m_Config;
	private float m_TargetAudioSourceVolume;

	private float m_TimeSinceStarted;
	private float m_FadeTime;
	private AnimationCurve m_FadeCurve;

	private float m_TextFadeInSecs;
	private float m_TextFadeInSecsTotal;
	private float m_TextFadeOutSecs;
	private float m_TextFadeOutSecsTotal;

	private FinishedAction m_FinishedAction;

	private bool m_Finished = true;
	private bool m_FiredFinishingAction = false;

	private Color m_TextColor = Color.white;

	private VignetteScriptable m_CurrentScriptable;

	public void DisplayVignette(VignetteVideoConfig config, FinishedAction finishedAction){
		m_FinishedAction = finishedAction;

		GatherAndPauseAllPlayingAudioSources();

		m_Config = config;
		currentIndex = 0;
		furthestSeenIndex = 0;
		m_Finished = false;
		m_FiredFinishingAction = false;

		m_TimeSinceStarted = 0;

		m_TextSpeed = config.TextSpeed;

		// visuals configs
		m_Background.sprite = m_Config.Background;
		m_BackgroundColor.color = m_Config.BackgroundColor;
		RectTransform bgRectTransform = ((RectTransform)m_Background.transform);
		bgRectTransform.anchoredPosition = m_Config.BackgroundImageOffset;

		m_Text.font = m_Config.Font;
		m_Text.fontSize = m_Config.FontSize;
		m_Text.lineSpacing = m_Config.LineSpacing;
		if(m_Config.PlayAudioSource){
			GameManager.Instance.PlayAudioSource(m_Config.AudioSourceToPlay);
			GameManager.Instance.SetAudioSourceVolume(config.InitialMusicVolume);
			m_TargetAudioSourceVolume = config.InitialMusicVolume;
		}

		RectTransform rectTransform = ((RectTransform)m_Text.transform);
		rectTransform.anchoredPosition = m_Config.TextOffset;
		rectTransform.sizeDelta = new Vector2(m_Config.TextBoxWidth, rectTransform.sizeDelta.y);

		m_ReadyToAdvanceAnimation.alpha = config.HideAdvanceIndicator ? 0 : 1;

		StartFade(m_Config.FadeIn, m_Config.FadeInCurve, m_Config.FadeInColor);
		DisplayStep(currentIndex);
	}

	private void DisplayStep(int index){
		m_ReadyToAdvanceAnimation.gameObject.SetActive(false);
		VignetteVideoStepConfig config = m_Config.IndividualSteps[index];
		
		// start text
		text = config.Text;
		m_Text.text = kInivisibleColorStart + text + kInivisibleColorEnd;
		textIndex = 0;
		delayInSecsBeforeStarting = config.DelayInSecsBeforeTextShows;

		if(config.Extras.Length > 0){
			if(config.Extras[0].AdjustMusicVolume){
				m_TargetAudioSourceVolume = config.Extras[0].MusicVolume;
			}
			if(config.Extras[0].AdjustTextSpeed){
				m_TextSpeed = config.Extras[0].TextSpeed;
			}

			if(config.Extras[0].ChangeBackground){
				m_Background.sprite = config.Extras[0].Background;
				m_BackgroundColor.color = config.Extras[0].BackgroundColor;
				RectTransform bgRectTransform = ((RectTransform)m_Background.transform);
				bgRectTransform.anchoredPosition = config.Extras[0].BackgroundImageOffset;
			}

			m_CurrentScriptable = config.Extras[0].ScriptToRun;
		}else{
			m_CurrentScriptable = null;
		}

		if(m_CurrentScriptable!=null){
			m_CurrentScriptable.StartScriptable();
		}

		m_TextFadeInSecsTotal = m_TextFadeInSecs = config.TextFadeInSecs;
		m_TextFadeOutSecsTotal = config.TextFadeOutSecs;
		m_TextFadeOutSecs = 0;

		if(m_TextFadeInSecsTotal<=0){
			m_TextColor.a = 1;
			m_Text.color = m_TextColor;
		}
	}

	void Update(){
		m_TimeSinceStarted += Time.deltaTime;
		m_FadeTime += Time.deltaTime;
		UpdateFadeAnimations();
		UpdateMusicVolume();

		// update the ready to advance animation
		bool showAdvanceIndicator = CanAdvance();
		if(showAdvanceIndicator != m_ReadyToAdvanceAnimation.gameObject.activeSelf){
			m_ReadyToAdvanceAnimation.gameObject.SetActive(showAdvanceIndicator);
		}

		// individual text fade out.
		if(m_TextFadeOutSecsTotal > 0 && m_TextFadeOutSecs>0){
			m_TextFadeOutSecs-=Time.deltaTime;

			m_TextColor.a = (m_TextFadeOutSecs/m_TextFadeOutSecsTotal);
			m_Text.color = m_TextColor;

			// let's stop the fade routine.
			if(m_TextFadeOutSecs<=0){
				m_TextFadeOutSecsTotal = 0;
				Advance();
			}
		}

		// process wrap up
		if(m_Finished){
			if(m_FadeCurve == null || m_FadeTime >= Utilities.Duration(m_FadeCurve)){
				if(!m_FiredFinishingAction){
					m_FiredFinishingAction = true;
					if(m_FinishedAction!=null){
						m_FinishedAction();
					}
					GameManager.Instance.SetVignetteFound(m_Config.UniqueId);
					GameManager.Instance.HideVignette();
					UnPauseAllPreviouslyPlayingAudioSources();
				}
			}
			return;
		}

		// navigate forward
		if(CanAdvance() && AdvancePressed()){
			TryAdvance();
		}
		// navigate backward
		if(CanGoBackward() && BackwardPressed()){
			GoBackward();
		}

		// if we have a special script, like a video player, process it and bail.
		if(m_CurrentScriptable!=null){
			if(m_CurrentScriptable.ScriptableFinished()){
				m_CurrentScriptable.ScriptableWrapUp();
				TryAdvance();
			}
			return;
		}

		// make sure we wait at least as long as we need to before starting display of text.
		if(textIndex < delayInSecsBeforeStarting){
			textIndex += Time.deltaTime;
			if(textIndex>=delayInSecsBeforeStarting){
				delayInSecsBeforeStarting = 0;
				textIndex = 0;
			}else{
				return;
			}
		}

		// text fade in.
		if(m_TextFadeInSecsTotal > 0 && m_TextFadeInSecs>0){
			m_TextFadeInSecs-=Time.deltaTime;

			m_TextColor.a = 1 - (m_TextFadeInSecs/m_TextFadeInSecsTotal);
			m_Text.color = m_TextColor;

			// let's stop the fade routine.
			if(m_TextFadeInSecs<=0){
				m_TextFadeInSecsTotal = 0;
			}
		}

		// text typewriter effect.
		textIndex += Time.deltaTime * m_TextSpeed;
		if(m_TextSpeed<=0){
			textIndex = text.Length;
		}

		int textDivider = (int) Mathf.Clamp(Mathf.Round((float)textIndex), 0, text.Length);
		string textVisible = text.Substring(0, textDivider);
		string textInvisible = text.Substring(textDivider, text.Length - textDivider);

		// doing it like this prevents text from jumping to the next line as its typed out.
		m_Text.text = textVisible + kInivisibleColorStart + textInvisible + kInivisibleColorEnd;
	}

	private bool CanGoBackward(){
		if(m_Finished){ 
			return false;
		}
		// if we're on first step, we cannot go backward.
		if(currentIndex<=0){
			return false;
		}

		// if we're in a text fade, we cannot advance.
		if(m_TextFadeOutSecs > 0 ||	m_TextFadeInSecs > 0){
			return false;
		}

		// if we want backwards in this one already, we can advance.
		if(currentIndex < furthestSeenIndex){
			return true;
		}

		// if we've seen this vignette already, we can advance.
		if(GameManager.Instance.HasSeenVignette(m_Config.UniqueId)){
			return true;
		}

		// if we're running a scriptable, for example a video, and it's not done, we cannot advance.
		if(m_CurrentScriptable!=null && !m_CurrentScriptable.ScriptableFinished()){
			return false;
		}

		// if we're running a scriptable, for example a video, and it's not done, we cannot advance.
		if(m_CurrentScriptable!=null && !m_CurrentScriptable.ScriptableFinished()){
			return false;
		}

		// if text is still advancing, we cannot advance.
		if(textIndex < text.Length){
			return false;
		}

		return true;
	}


	private bool CanAdvance(){
		if(m_Finished){ 
			return false;
		}

		// if we're in a text fade, we cannot advance.
		if(m_TextFadeOutSecs > 0 ||	m_TextFadeInSecs > 0){
			return false;
		}

		// if we want backwards in this one already, we can advance.
		if(currentIndex < furthestSeenIndex){
			return true;
		}

		// if we've seen this vignette already, we can advance.
		if(GameManager.Instance.HasSeenVignette(m_Config.UniqueId)){
			return true;
		}

		// if we're running a scriptable, for example a video, and it's not done, we cannot advance.
		if(m_CurrentScriptable!=null && !m_CurrentScriptable.ScriptableFinished()){
			return false;
		}

		// if text is still advancing, we cannot advance.
		if(textIndex < text.Length){
			return false;
		}

		return true;
	}

	private void UpdateMusicVolume(){
		float currentVolume = GameManager.Instance.GetAudioSourceVolume();
		if(currentVolume < m_TargetAudioSourceVolume){
			GameManager.Instance.SetAudioSourceVolume(
				(float)Mathf.Clamp(currentVolume + kAudioVolumeAdjust, currentVolume, m_TargetAudioSourceVolume)
			);
		}else if(currentVolume > m_TargetAudioSourceVolume){
			GameManager.Instance.SetAudioSourceVolume(
				(float)Mathf.Clamp(currentVolume - kAudioVolumeAdjust, m_TargetAudioSourceVolume, currentVolume)
			);
		}
	}

	private void GatherAndPauseAllPlayingAudioSources(){
		m_PreviouslyPlayingAudioSources = new List<AudioSource>(FindObjectsOfType<AudioSource>());
		foreach(AudioSource source in m_PreviouslyPlayingAudioSources){
			source.Pause();
		}
	}

	private void UnPauseAllPreviouslyPlayingAudioSources(){
		if(m_PreviouslyPlayingAudioSources==null){
			return;
		}

		foreach(AudioSource source in m_PreviouslyPlayingAudioSources){
			if(source==null){
				continue;
			}
			source.UnPause();
		}
	}

	public void ForceSkipCurrentVignetteVideo(){
		if(m_CurrentScriptable!=null && m_CurrentScriptable is VignetteScriptableVideoPlayer){
			m_CurrentScriptable.ScriptableWrapUp();
			m_CurrentScriptable = null;
			Advance();
		}
	}

	private void StartFade(bool enabled, AnimationCurve curve, Color color){
		if(enabled){
			m_FadeOverlay.gameObject.SetActive(true);
			m_FadeCurve = curve;
			m_FadeOverlay.color = color;
		}else{
			m_FadeOverlay.gameObject.SetActive(false);
			m_FadeCurve = null;
		}

		m_FadeTime = 0;
		UpdateFadeAnimations();
	}

	private void UpdateFadeAnimations(){
		if(m_FadeCurve != null){
			Color fadeColor = m_FadeOverlay.color;
			fadeColor.a = m_FadeCurve.Evaluate(m_FadeTime);
			m_FadeOverlay.color = fadeColor;
		}
	}

	private void TryAdvance(){
		if(m_TextFadeOutSecsTotal>0){
			m_TextFadeOutSecs = m_TextFadeOutSecsTotal;
			m_ReadyToAdvanceAnimation.gameObject.SetActive(false);
		}else{
			Advance();
		}
	}

	private void Advance(){
		if(m_TextFadeOutSecsTotal>0){
			m_TextFadeOutSecs = m_TextFadeOutSecsTotal;
		}

		if(m_CurrentScriptable!=null){
			m_CurrentScriptable.ScriptableWrapUp();
		}

		currentIndex++;
		furthestSeenIndex = Mathf.Max(furthestSeenIndex, currentIndex);
		if(currentIndex >= m_Config.IndividualSteps.Length){
			currentIndex = m_Config.IndividualSteps.Length;
			Finish();
			return;
		}
		DisplayStep(currentIndex);

		//if not latest step, remove delay.	
		if(currentIndex < furthestSeenIndex){
			delayInSecsBeforeStarting = 0;	
		}
	}

	private void GoBackward(){

		if(m_CurrentScriptable!=null){
			m_CurrentScriptable.ScriptableWrapUp();
		}

		furthestSeenIndex = Mathf.Max(furthestSeenIndex, currentIndex + 1);
		currentIndex--;
		if(currentIndex<0)
		{
			currentIndex = 0;
		}
		DisplayStep(currentIndex);
		delayInSecsBeforeStarting = 0;
	}

	private void Finish(){
		if(m_Finished) {
			return;
		}
		m_Finished = true;
		m_FiredFinishingAction = false;
		m_ReadyToAdvanceAnimation.gameObject.SetActive(false);

		if(m_Config.ClearTextOnFadeOut){
			text = string.Empty;
			m_Text.text = text;
		}
		StartFade(m_Config.FadeOut, m_Config.FadeOutCurve, m_Config.FadeOutColor);
	}

	public void ForceFinishCurrentVignette(){
		m_Finished = true;
		m_FiredFinishingAction = false;
		m_ReadyToAdvanceAnimation.gameObject.SetActive(false);
		m_FadeCurve = null;
		UnPauseAllPreviouslyPlayingAudioSources();
		GameManager.Instance.PlayAudioSource(null);

		if(m_CurrentScriptable!=null){
			m_CurrentScriptable.ScriptableWrapUp();
			m_CurrentScriptable = null;
		}
	}

	private bool AdvancePressed(){
		return Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return);
	}
	private bool BackwardPressed(){
		return Input.GetKeyDown(KeyCode.LeftArrow);
	}
}
