using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayVideo : MonoBehaviour {

    public GameObject videoPlayer;
    public int timeToStop;

    [SerializeField]
    bool m_AutoPlay=true;


    // Use this for initialization
    void Start () {
        videoPlayer.SetActive(false);
    }
    
    // Update is called once per frame
    void OnTriggerEnter (Collider player) {

        if (m_AutoPlay && player.gameObject.tag == "Player")
        {
            TriggerVideo();
        }
    }

    public void TriggerVideo(){
        videoPlayer.SetActive(true);
        Destroy(videoPlayer, timeToStop);
    }
}