using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tuto_Torches : MonoBehaviour{

    [Header("Torches")]

    public GameObject[] torches;
    public bool[] activated;

    public GameObject[] torchesSparks;
    public GameObject portalEnd;

    [Header("Portals")]

    public Animator[] portals;
    public GameObject[] portalsTriggers;
    public Camera[] portalsCameras;
    public AudioSource[] portalsSounds;

    public bool[] camerasActivated;

    public float cameraTime = 2f;
    float cameraTimer = 0f;

    [Header("Right Path")]

    public GameObject[] cubes;
    public Camera cameraCubes;
    public bool cameraCubesEnabled = false;
    public float[] cubesScaleY = new float[3];

    void Update(){
        if(cameraTimer > 0){
            cameraTimer -= 1 * Time.deltaTime;
        }

        if(cameraTimer <= 0){
            cameraTimer = cameraTime;
            for (int i = 0; i < portalsCameras.Length; i++){
                portalsCameras[i].GetComponent<Camera>().enabled = false;
            }
            
        }

        //for (int i = 0; i < torchesSparks.Length; i++){
        if(torchesSparks[0].activeSelf && torchesSparks[1].activeSelf && torchesSparks[2].activeSelf && torchesSparks[3].activeSelf && torchesSparks[4].activeSelf && 
            torchesSparks[5].activeSelf){
            portalEnd.gameObject.SetActive(true);
        }
        //}

        if(portalEnd.gameObject.activeSelf){
            portalEnd.transform.LookAt(this.transform.position);
            portalEnd.transform.eulerAngles = new Vector3 (0, portalEnd.transform.eulerAngles.y, 0);

        }

        if(torches[0].gameObject.activeSelf){
            torchesSparks[0].gameObject.SetActive(true);
            torchesSparks[1].gameObject.SetActive(true);
            portalsTriggers[0].gameObject.SetActive(true);
            activated[0] = true;
        }

        if(torches[1].gameObject.activeSelf){
            torchesSparks[2].gameObject.SetActive(true);
            torchesSparks[3].gameObject.SetActive(true);
            portalsTriggers[1].gameObject.SetActive(true);
            activated[1] = true;
        }

        if(torches[2].gameObject.activeSelf){
            torchesSparks[4].gameObject.SetActive(true);
            torchesSparks[5].gameObject.SetActive(true);
            portalsTriggers[2].gameObject.SetActive(true);
            activated[2] = true;
            
            cameraCubes.GetComponent<Camera>().enabled = true;

            if(cubes[0].transform.localScale.y < cubesScaleY[0]){
                cubes[0].transform.localScale += new Vector3(0, 0.5f, 0) * Time.deltaTime;
            }
            if(cubes[1].transform.localScale.y < cubesScaleY[1]){
                cubes[1].transform.localScale += new Vector3(0, 0.5f, 0) * Time.deltaTime;
            }
            if(cubes[2].transform.localScale.y < cubesScaleY[2]){
                cubes[2].transform.localScale += new Vector3(0, 0.5f, 0) * Time.deltaTime;
            }

            if(cubes[2].transform.localScale.y >= cubesScaleY[2]){
                cameraCubes.GetComponent<Camera>().enabled = false;
            }
        }
    }

    public void OnTriggerEnter(Collider c){

        if(c.gameObject.name == portalsTriggers[0].name && activated[0] == true && camerasActivated[0] == false){
            camerasActivated[0] = true;
            cameraTimer = cameraTime;
            portalsCameras[0].GetComponent<Camera>().enabled = true;
            portals[0].SetTrigger("Close");
            portals[1].SetTrigger("Close");
            portalsSounds[0].Play();
            portalsSounds[1].Play();
        }

        if(c.gameObject.name == portalsTriggers[1].name && activated[1] == true && camerasActivated[1] == false){
            camerasActivated[1] = true;
            cameraTimer = cameraTime;
            portalsCameras[1].GetComponent<Camera>().enabled = true;
            portals[2].SetTrigger("Close");
            portals[3].SetTrigger("Close");
            portalsSounds[2].Play();
            portalsSounds[3].Play();
        }

        if(c.gameObject.name == portalsTriggers[2].name && activated[2] == true && camerasActivated[2] == false){
            camerasActivated[2] = true;
            cameraTimer = cameraTime;
            portalsCameras[2].GetComponent<Camera>().enabled = true;
            portals[4].SetTrigger("Close");
            portals[5].SetTrigger("Close");
            portalsSounds[4].Play();
            portalsSounds[5].Play();
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

