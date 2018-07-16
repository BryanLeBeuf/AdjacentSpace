using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyRaw : MonoBehaviour {

    void Start() {
        
    }

    void Update(){
        
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
            Destroy(this.gameObject, 5.0f);{

        }

    }

}