using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEndGame : MonoBehaviour {

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player"){

            Application.LoadLevel("EndGame");
        }
    }
}