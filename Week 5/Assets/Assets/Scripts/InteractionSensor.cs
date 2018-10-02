using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Narrate;

public class InteractionSensor : MonoBehaviour {

	[SerializeField]
	int RequiredKeyId = -1;
	[SerializeField]
	int RequiredKeyBId = -1;
	[SerializeField]
	string VignetteToPlay;
	[SerializeField]
	KeyPickup KeyToGivePlayer;
	[SerializeField]
	PlayVideo VideoToPlay;
	[SerializeField]
	string m_OptionalLevelToJumpTo;
	[SerializeField]
	bool m_CanOnlyBeTriggeredOnce = true;
	[SerializeField]
	Collider OptionalRequiredLookPoint;
	[SerializeField]
	bool m_AutoTriggerWhenWithin = false;
	[SerializeField]
	NarrationTrigger m_NarationToTriggerIfDontHavekeys;

	private int m_TriggeredTimes = 0;

	private Collider[] m_Colliders;

	GameObject m_Player;
	Camera m_PlayerCamera;

	void Start(){
		m_Colliders = GetComponentsInChildren<Collider>();
	}

	void Update(){
		GameManager.Instance.InteractionSensorReport(this);

		if(PlayerWithin() && (GameManager.Instance.ActivationKeyPressed() || m_AutoTriggerWhenWithin)){
			if(HasRequiredKeys()){
				Trigger();
			}
			else if(m_NarationToTriggerIfDontHavekeys!=null && m_NarationToTriggerIfDontHavekeys.enabled)
			{
				m_NarationToTriggerIfDontHavekeys.Trigger();
			}
		}
	}

	public bool CanDisplayUITrigger(){
		return !m_AutoTriggerWhenWithin && HasRequiredKeys();
	}

	private bool HasRequiredKeys(){
		if(RequiredKeyId>0 && !GameManager.Instance.HasKey(RequiredKeyId)){
			return false;
		}
		if(RequiredKeyBId>0 && !GameManager.Instance.HasKey(RequiredKeyBId)){
			return false;
		}
		return true;
	}

	public bool CanTrigger(){
		if(m_TriggeredTimes > 0 && m_CanOnlyBeTriggeredOnce){
			return false;
		}
		return true;
	}

	public bool PlayerWithin(){
		if(!CanTrigger()){
			return false;
		}
		// try to cache our player (FindWithTag is really expensive on the CPU)
		if(m_Player==null){
			m_Player = GameObject.FindWithTag("Player");
		}
		// if we still haven't found player, bail.
		if(m_Player==null){
			return false;
		}

		if(m_PlayerCamera==null){
			m_PlayerCamera = m_Player.GetComponentInChildren<Camera>();
		}
		if(m_PlayerCamera==null){
			return false;
		}

		if(OptionalRequiredLookPoint!=null){
            Ray ray = new Ray(m_PlayerCamera.transform.position, m_PlayerCamera.transform.forward);
            RaycastHit hit;
            if(!OptionalRequiredLookPoint.Raycast(ray, out hit, 100.0F)){
            	return false;
            }
		}

		foreach(Collider collider in m_Colliders){
			if(collider.bounds.Contains(m_Player.transform.position)){
				return true;
			}
		}
		return false;
	}

	private void Trigger(){
		m_TriggeredTimes++;

		// do we have a key? give it.
		if(KeyToGivePlayer!=null){
			KeyToGivePlayer.GiveToPlayer();
		}

		if(VideoToPlay!=null){
			VideoToPlay.TriggerVideo();
		}

		// do we have a vignette? play it.
		if(!string.IsNullOrEmpty(VignetteToPlay)){
			GameManager.Instance.DisplayVignette(VignetteToPlay, JumpToLevel);
		}else{
			JumpToLevel();
		}

		if(m_CanOnlyBeTriggeredOnce){
			Destroy(gameObject);
		}
	}

	private void JumpToLevel(){
		if(!string.IsNullOrEmpty(m_OptionalLevelToJumpTo)){
        	Application.LoadLevel(m_OptionalLevelToJumpTo);
		}
	}
}
