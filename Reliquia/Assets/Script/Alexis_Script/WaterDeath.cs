using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDeath : MonoBehaviour{


    // Ce script gère les crevasses dy Labyrinthe, il doit être placé sur le Player

    void OnTriggerEnter(Collider c) {
        if (c.gameObject.name == "WaterDeathZone"){
         this.gameObject.transform.position = new Vector3 (0, 0, 0);  
        }
    }

}
