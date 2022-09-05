using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour{

    //public Health healthScript;

    Vector3 startPosition;
    Renderer rend;
    Rigidbody rigid;
    ParticleSystem sparks;
    
    public GameObject impactEffect;
    
    public float damage = 10f;
    public float speed = 1f;
    public float cooldown = 2f;
    public float lifeTime;

    void Start(){
        startPosition = this.transform.position;
        rigid = this.GetComponent<Rigidbody>();
        sparks = impactEffect.GetComponent<ParticleSystem>();
        lifeTime = cooldown;
        Sparks();
    }

    void Update(){
        rigid.velocity = transform.forward * speed;

        lifeTime -= 1 * Time.deltaTime;

        if(lifeTime <= 0){
            lifeTime = cooldown;
            this.transform.position = startPosition;
            this.GetComponent<Renderer>().enabled = true;
            this.GetComponent<Collider>().enabled = true;
        }
    }

    void OnCollisionEnter(Collision c){
        this.GetComponent<Renderer>().enabled = false;
        this.GetComponent<Collider>().enabled = false;
        Sparks();

        if(c.gameObject.name == "William_Player"){
            //healthScript.m_CurrentHealth -= damage;
        }
    }

    void Sparks(){
        impactEffect.transform.position = startPosition;
        sparks.Play();
    }
}
