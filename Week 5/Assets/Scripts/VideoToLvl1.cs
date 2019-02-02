using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoToLvl1 : MonoBehaviour {

	private void Start(){
        GameManager.Instance.LoadLevel("Ply_test", 63f);
	}
}
