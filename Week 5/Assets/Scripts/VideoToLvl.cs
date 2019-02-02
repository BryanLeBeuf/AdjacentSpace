using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoToLvl : MonoBehaviour {

	private void Start(){
        GameManager.Instance.LoadLevel("Grand & Euclid", 72f);
	}
}
