using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto_Torches : MonoBehaviour{

    public GameObject[] torchesSparks;
    public GameObject torcheFinale;

    public string[] torches;
    public bool[] activated;


    public void OnTriggerEnter(Collider c){

        /*for (int i = 0; i <= torches.Length; i++) {
          if(c.gameObject.name == torches[i]){
                activated[i] = true;
                Debug.Log(i);
            }
        }*/

        if(c.gameObject.name == torches[0]){
            torchesSparks[0].gameObject.SetActive(true);
            activated[0] = true;
        }

        if(c.gameObject.name == torches[1]){
            torchesSparks[1].gameObject.SetActive(true);
            activated[1] = true;
        }

        if(c.gameObject.name == torches[2]){
            torchesSparks[2].gameObject.SetActive(true);
            activated[2] = true;
        }


        for (int i = 0; i <= activated.Length; i++) {
          if(activated[i] == true){
                Debug.Log("Torche activée !");
            }

        }

        if(c.gameObject.name == torcheFinale.name){
            Debug.Log("Tuto Terminé !");
        }


    }

}

