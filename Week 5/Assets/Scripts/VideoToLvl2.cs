using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoToLvl2 : MonoBehaviour {

	private void Start(){
		GameManager.Instance.LoadLevel("TitleScreen",  241f);
	}
}
