using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laby_Levers : MonoBehaviour{

    public Animator[] doors;
    public Camera cameraDoors;
    public AudioSource[] doorsSounds;

    public bool[] activated;
    public bool doorOpened;
    bool doorLock;
    public float camTime = 3f;
    float camTimer;

    public void Start(){
        camTimer = camTime;
    }

    public void Update(){
        Debug.Log(camTimer);
        if(doorOpened == true && doorLock == false && camTimer > 0){
            camTimer -= 1 * Time.deltaTime;
            cameraDoors.GetComponent<Camera>().enabled = true;
        } 

        if(camTimer <= 0){
            doorLock = true;
            cameraDoors.GetComponent<Camera>().enabled = false;
        }

        if(activated[0] == true && activated[1] == true && activated[2] == true && activated[3] == true && doorLock == false && doorOpened == false){
            doorOpened = true;
            foreach (Animator door in doors){
                door.SetTrigger("Open");
            }
            foreach (AudioSource sound in doorsSounds){
                sound.Play();
            }
        }
    }

    public void OnTriggerEnter(Collider c){

        if(c.gameObject.name == "Lever1"){
            activated[0] =  true;
        }

        if(c.gameObject.name == "Lever2"){
            activated[1] =  true;
        }

        if(c.gameObject.name == "Lever3"){
            activated[2] =  true;
        }

        if(c.gameObject.name == "Lever4"){
            activated[3] =  true;
        }

    }

}
