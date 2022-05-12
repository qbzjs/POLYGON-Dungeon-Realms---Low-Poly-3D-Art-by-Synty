using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mili_Traces : MonoBehaviour{

    public Lighting_Vincent_Test lightingScript;
   
    void Start(){
        lightingScript = GameObject.Find("William_Player").GetComponent<Lighting_Vincent_Test>();
    }

    void Update(){
        if(lightingScript.isCreated == false){
            this.gameObject.SetActive(false);
            Debug.Log("Inactive");
        }

        if(lightingScript.isCreated == true){
            this.gameObject.SetActive(true);
            Debug.Log("Active");
        }

        Debug.Log(lightingScript.isCreated);
    }
}
