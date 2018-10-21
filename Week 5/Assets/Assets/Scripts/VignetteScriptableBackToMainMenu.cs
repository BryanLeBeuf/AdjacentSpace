using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VignetteScriptableBackToMainMenu : VignetteScriptable {

    public override void StartScriptable(){
        GameManager.Instance.PlayerClickedReturnToMainMenu();
    }

    public override bool ScriptableFinished(){
	    return true;
    }

    public override void ScriptableWrapUp(){
    }
}
