using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDeath : MonoBehaviour{

    public PouvoirPraesidium pouvoirPraesidiumScript;

    void Start(){
        pouvoirPraesidiumScript = GameObject.Find("William_Player").GetComponent<PouvoirPraesidium>();
    }

    // Ce script gère le piège de Feu du Tutoriel, il doit être placé sur le Player

    void OnTriggerEnter(Collider c) {
        if (c.gameObject.name == "FeuPiège" && pouvoirPraesidiumScript.PraesidiumActif == false){
            this.gameObject.transform.position = new Vector3 (34, -9, 30);  
        }
    }

}
