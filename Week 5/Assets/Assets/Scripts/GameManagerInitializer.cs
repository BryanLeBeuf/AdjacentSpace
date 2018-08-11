using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerInitializer : MonoBehaviour {

	[SerializeField]
	bool m_GameplayScene = false;
	[SerializeField]
	string m_DefaultMapConfig = "";

	// Use this for initialization
	void Start () {
		if(GameObject.FindGameObjectsWithTag("GameManager").Length==0)
		{
			SceneManager.LoadScene("GameManager", LoadSceneMode.Additive);
			StartCoroutine("NotifySceneLoaded");
		}else{
			GameManager.Instance.NotifySceneLoaded();
			GameManager.Instance.GameplayScene = m_GameplayScene;
			GameManager.Instance.DefaultMapConfig = m_DefaultMapConfig;
		}
	}

	private IEnumerator NotifySceneLoaded(){
		while(!GameManager.HasInstance){
			yield return null;
		}
		GameManager.Instance.GameplayScene = m_GameplayScene;
		GameManager.Instance.DefaultMapConfig = m_DefaultMapConfig;
	}
}
