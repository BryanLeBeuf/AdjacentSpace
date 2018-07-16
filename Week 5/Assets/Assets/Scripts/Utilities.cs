using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour {

	public static void DeleteAllChildren(GameObject obj){
		 foreach (Transform child in obj.transform) {
		     GameObject.Destroy(child.gameObject);
		 }
	}
	
	public static float Duration(AnimationCurve curve){
		return curve [curve.length - 1].time;
	}

	public static float Duration(AnimationCurve curveX, AnimationCurve curveY){
		return Mathf.Max (
			curveX [curveX.length - 1].time,
			curveY [curveY.length - 1].time
		);
	}

}
