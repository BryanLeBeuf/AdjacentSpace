using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VignetteConfiguration : MonoBehaviour {
	[SerializeField]
	VignetteVideoConfig m_Configuration;

	public VignetteVideoConfig Config { 
		get { return m_Configuration; }}
}
