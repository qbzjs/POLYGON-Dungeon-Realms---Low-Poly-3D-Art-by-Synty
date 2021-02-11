using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour{

    // Ce script gère les pièges des Catacombes, il doit être placé sur le Player

    public Camera cameraTrappe;
    public Camera cameraEboulement;

    string piègeTrappe = "PiègeTrappe"; 
    string piègeEboulement = "PiègeEboulement"; 

    void OnTriggerEnter(Collider other){

        if (other.gameObject.name == piègeTrappe){
            cameraTrappe.enabled = true;
        }

        if (other.gameObject.name == piègeEboulement){
            cameraEboulement.enabled = true;
        }

    }

        void OnTriggerExit(Collider other){

        if (other.gameObject.name == piègeTrappe){
            cameraTrappe.enabled = false;
        }

        if (other.gameObject.name == piègeEboulement){
            cameraEboulement.enabled = false;
        }
    }

}
