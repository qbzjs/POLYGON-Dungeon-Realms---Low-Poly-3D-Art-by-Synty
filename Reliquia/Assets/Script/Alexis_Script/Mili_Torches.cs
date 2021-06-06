using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mili_Torches : MonoBehaviour{

    [Header("Essentials")]

    public Lighting lightingScript;

    public Transform[] torches;

    public Transform torcheFinale;

    public Transform flammeFinale;
    public Transform lumièreFinale;

    [Header("Variables")]

    public float minimumDistance = 2f;


    // Update is called once per frame
    void Update(){

        // Calcul Distance

        foreach (var torche in torches){

            float dist = Vector3.Distance(torche.position, transform.position);
            //print("Distance to other: " + dist);

        }

        // Distance d'une torche

        for (int i = 0; i < torches.Length; ++i){

            if (Vector3.Distance(transform.position, torches[i].transform.position) <= minimumDistance /*&& lightingScript.isLighting == true*/){
                Debug.Log("Torch nearby !");
                flammeFinale = torches[i].transform.GetChild(0);
                lumièreFinale = torches[i].transform.GetChild(1);
                flammeFinale.gameObject.SetActive(true);
                lumièreFinale.gameObject.SetActive(true);
            }

        }

        

    }
    

} 

