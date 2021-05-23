using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mili_Torches : MonoBehaviour{

    //public lightingScript;

    public Transform[] torches;

    public float distanceTolerance = 2f;

    // Update is called once per frame
    void Update(){

        
        foreach (var torche in torches){

            // Calcul Distance

            float dist = Vector3.Distance(torche.position, transform.position);
            print("Distance to other: " + dist);


            // Distance d'une torche

            if(dist > distanceTolerance){
  
            }
        }

        

        

        
        
    }

}
