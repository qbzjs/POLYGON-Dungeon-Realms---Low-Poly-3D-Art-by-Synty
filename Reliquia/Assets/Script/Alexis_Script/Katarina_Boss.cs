using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Katarina_Boss : MonoBehaviour{

    // Base
    
    public GameObject fireBall;
    public Transform player;

    public Image lifeBar;

    public bool active = false;
    public float distanceActiv = 5f;

    public float cooldownTimer;
    public float cooldown = 3f;

    public float dist = 0f;

    // Boss Phases

    public bool onCutscene = false;
    public bool phase1 = true;
    public bool phase2 = false;
    public bool phase3 = false;

    // Phase 1

    public float katarinaBaseLife = 100f; // Variable de restauration de vie
    public float katarinaLife = 100f;

    void Start(){
        player = GameObject.Find("ybot").GetComponent<Transform>();
    }

    void Update(){

        dist = Vector3.Distance(player.position, transform.position);

        if(phase1 == true){
            Phase1();
        }

        if(phase2 == true){
            Phase2();
        }

        
        if(phase3 == true){
            Phase3();
        }
        
    }

    void Phase1(){

        // Base

        katarinaLife -= 12 * Time.deltaTime;
        lifeBar.fillAmount = katarinaLife / 100;

        // Conditions 

        if(katarinaLife <= 0){
            onCutscene = true;
            phase1 = false;
            phase2 = true;
            katarinaLife = katarinaBaseLife;
        }

        // Comportement Général
        

        if(dist < distanceActiv){
            active = true;
        }

        if(active == true){

            transform.LookAt(player);

            if(cooldownTimer >= 0){
                cooldownTimer -= 1 * Time.deltaTime;
            }

            if(cooldownTimer <= 0){
                Instantiate(fireBall, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
                cooldownTimer = cooldown;
            }

        }
    }


    void Phase2(){

        if(dist < distanceActiv){
            cooldown = 0.5f;
            onCutscene = true;
            phase2 = false;
            phase3 = true;
        }

        if(onCutscene == true){
            onCutscene = false;
            this.transform.position = new Vector3 (0, 5, 20);
        }

        // Comportement

        transform.LookAt(player);

        if(cooldownTimer >= 0){
            cooldownTimer -= 1 * Time.deltaTime;
        }

        if(cooldownTimer <= 0){
            Instantiate(fireBall, new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y, this.gameObject.transform.position.z), Quaternion.identity);
            cooldownTimer = 0.5f;
        }


    }

    void Phase3(){
        if(onCutscene == true){
            onCutscene = false;
            lifeBar.fillAmount = katarinaLife / 100;
        }

    }

}
