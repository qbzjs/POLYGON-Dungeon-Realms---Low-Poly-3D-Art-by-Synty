using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laby_Levers : MonoBehaviour{

    public Animator[] doors;
    public bool[] activated;

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

        if(c.gameObject.name == "SM_Bld_Dwarf_Building_02_Door_L" && activated[0] == true && activated[1] == true && activated[2] == true  && activated[3] == true){
            foreach (Animator door in doors){
            Debug.Log("In Door");
            door.SetTrigger("Open");
        }

        }

    }

}
