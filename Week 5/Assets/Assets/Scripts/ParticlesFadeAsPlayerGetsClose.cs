using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesFadeAsPlayerGetsClose : MonoBehaviour {

	[SerializeField]
	ParticleSystem m_ParticleSystem;

	[SerializeField]
	float m_MinAlpha = 0.002f;
	[SerializeField]
	float m_MaxAlpha = 0.141f;

	[SerializeField]
	float m_MinDistance = 5;
	[SerializeField]
	float m_MaxDistance = 50;

	[SerializeField]
	float m_CurrentLerp;

	[SerializeField]
	float m_CurrentDistance;

	[SerializeField]
	float m_CurrentAlpha;

	private Color m_Color;
	GameObject m_Player;

	void Start(){
		m_Color = m_ParticleSystem.startColor;
	}
	
	// Update is called once per frame
	void Update () {

		// try to cache our player (FindWithTag is really expensive on the CPU)
		if(m_Player==null){
			m_Player = GameObject.FindWithTag("Player");
		}
		// if we still haven't found player, bail.
		if(m_Player==null){
			return;
		}

		m_CurrentDistance = Vector3.Distance(transform.position, m_Player.transform.position) - m_MinDistance;
		m_CurrentLerp = Mathf.Clamp(m_CurrentDistance/m_MaxDistance, 0, 1);
		m_CurrentAlpha = Mathf.Clamp(m_CurrentLerp * m_MaxAlpha, m_MinAlpha, m_MaxAlpha);
		m_Color.a = m_CurrentAlpha;
		m_ParticleSystem.startColor = m_Color;
	}
}
