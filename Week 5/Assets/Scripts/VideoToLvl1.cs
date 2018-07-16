using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoToLvl1 : MonoBehaviour {

private void Start(){
        StartCoroutine(WaitAndLoad(63f, "Ply_test"));
		}

		private IEnumerator WaitAndLoad(float value, string scene) {
			yield return new WaitForSeconds(value);
			Application.LoadLevel(scene);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
