using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Katarina_Boss : MonoBehaviour{

    [Header("Base")]
    public GameObject fireBall;
    public GameObject shield;
    public Transform player;

    public Image lifeBar;

    public bool active = false;
    public float distanceActiv = 5f;
    public float dist = 0f;

    public float cooldownTimer;
    public float cooldown = 3f;

    public float katarinaBaseLife = 100f; // Variable de restauration de vie
    public float katarinaLife = 100f;

    [Header("Boss Phases")]
    public bool onCutscene = false;
    public bool phase1 = true;
    public bool phase2 = false;
    public bool phase3 = false;

    [Header("Phase 3")]
    public GameObject flamesPrefab;

    public bool usedShield = false;
    public float flameCooldown = 7f;


    void Start(){
        player = GameObject.Find("ybot").GetComponent<Transform>();
        shield.gameObject.SetActive(false);
    }

    void Update(){

        dist = Vector3.Distance(player.position, transform.position);
        transform.LookAt(player);

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

        katarinaLife -= 10 * Time.deltaTime;
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
        
        if(onCutscene == true){
            onCutscene = false;
            this.transform.position = new Vector3 (0, 5, 20);
        }

        if(dist < distanceActiv){
            phase2 = false;
            phase3 = true;
            onCutscene = true;
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

        cooldownTimer -= 1 * Time.deltaTime;

        if(onCutscene == true){
            onCutscene = false;
            lifeBar.fillAmount = katarinaBaseLife / 100;
            shield.gameObject.SetActive(true);
            cooldownTimer = flameCooldown;
            Debug.Log("Reached");
        }

        if(onCutscene == false){
            ShieldFunction();
            FlamesPatterns();
        }

    }

    void ShieldFunction(){
        if(katarinaLife <= 66 || katarinaLife <= 33 && usedShield == false){
            usedShield = true;
            shield.gameObject.SetActive(true);
        }
    }

    void FlamesPatterns(){
        if(cooldownTimer <= 0){
            cooldownTimer = flameCooldown;
            Instantiate(flamesPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }

}
