using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katarina : MonoBehaviour{

    public GameObject fireBall;
    public Transform player;

    public bool active = false;
    public float distanceActiv = 5f;

    public float cooldownTimer;
    public float cooldown = 3f;

    void Start(){
        player = GameObject.Find("ybot").GetComponent<Transform>();
    }

    void Update(){

        float dist = Vector3.Distance(player.position, transform.position);

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
}
