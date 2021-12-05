using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Katarina_Fire : MonoBehaviour{

    public Transform player;
    public float lifetime = 3f;

    void Start(){
        player = GameObject.Find("ybot").GetComponent<Transform>();
        DestroyObjectDelayed();
    }

    void Update(){
        transform.position = Vector3.MoveTowards(transform.position, player.position, 0.5f);
    }

    void OnCollisionEnter(Collision c){
        Destroy(this.gameObject);
    }
    

    void DestroyObjectDelayed(){
        Destroy(this.gameObject, lifetime);
    }
}
