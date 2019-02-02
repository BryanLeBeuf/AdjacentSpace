using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelThree : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player"){
            GameManager.Instance.LoadLevel("LevelThree");
        }
    }
}