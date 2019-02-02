using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KeyPickup : MonoBehaviour
{

    [SerializeField]
    bool m_AutoPickup=true;


    // This ID corresponds to the keyState array of a LockedDoor script.
    public int keyID = 0;


    void OnTriggerEnter(Collider c)
    {
        if (m_AutoPickup && c.CompareTag("Player"))
        {
            GiveToPlayer();
        }
    }

    public void GiveToPlayer(){
        if(gameObject.activeSelf){
            GameManager.Instance.GivePlayerKey(keyID);
            gameObject.SetActive(false);
        }
    }
}