using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class VignetteScriptable : MonoBehaviour {

	public abstract void StartScriptable();
	public abstract bool ScriptableFinished();
	public abstract void ScriptableWrapUp();
}
