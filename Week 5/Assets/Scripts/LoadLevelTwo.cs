using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadLevelTwo : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player"){

            Application.LoadLevel("LevelTwo");
        }
    }
}