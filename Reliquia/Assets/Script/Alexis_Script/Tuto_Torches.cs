using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto_Torches : MonoBehaviour{

    [Header("Torches")]

    public GameObject[] torchesSparks;
    public GameObject torcheFinale;

    public string[] torches;
    public bool[] activated;

    [Header("Portails")]

    public Animator[] portals;
    public GameObject[] portalsTriggers;
    public Camera[] portalsCameras;

    [Header("Right Path")]

    public GameObject[] cubes;



    public void OnTriggerEnter(Collider c){

        if(c.gameObject.name == torches[0]){
            torchesSparks[0].gameObject.SetActive(true);
            torchesSparks[1].gameObject.SetActive(true);
            portalsTriggers[0].gameObject.SetActive(true);
            activated[0] = true;
        }

        if(c.gameObject.name == torches[1]){
            torchesSparks[2].gameObject.SetActive(true);
            torchesSparks[3].gameObject.SetActive(true);
            portalsTriggers[1].gameObject.SetActive(true);
            activated[1] = true;
        }

        if(c.gameObject.name == torches[2]){
            torchesSparks[4].gameObject.SetActive(true);
            torchesSparks[5].gameObject.SetActive(true);
            portalsTriggers[2].gameObject.SetActive(true);
            cubes[0].transform.localScale = new Vector3(this.transform.localScale.x, 1f, this.transform.localScale.z);
            cubes[1].transform.localScale = new Vector3(this.transform.localScale.x, 3f, this.transform.localScale.z);
            activated[2] = true;
        }

        if(c.gameObject.name == portalsTriggers[0].name && activated[0] == true){
            portals[0].SetTrigger("Close");
            portals[1].SetTrigger("Close");
        }

        if(c.gameObject.name == portalsTriggers[1].name && activated[1] == true){
            portals[2].SetTrigger("Close");
            portals[3].SetTrigger("Close");
        }

        if(c.gameObject.name == portalsTriggers[2].name && activated[2] == true){
            portals[4].SetTrigger("Close");
            portals[5].SetTrigger("Close");
        }

        // Debug

        /*for (int i = 0; i <= torches.Length; i++) {
          if(c.gameObject.name == torches[i]){
                activated[i] = true;
                Debug.Log(i);
            }
        }*/

        /*for (int i = 0; i <= activated.Length; i++) {
          if(activated[i] == true){
                Debug.Log("Torche activée !");
            }
        }

        if(c.gameObject.name == torcheFinale.name){
            Debug.Log("Tuto Terminé !");
        }*/

    }

}

