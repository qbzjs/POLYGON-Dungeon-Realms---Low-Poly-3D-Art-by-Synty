using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour{

    Vector3 startPosition;
    Rigidbody rigid;

    public ParticleSystem impactEffect;

    public float speed = 1f; 

    void Start(){
        startPosition = this.transform.position;
        rigid = this.GetComponent<Rigidbody>();
        impactEffect.transform.position = startPosition;
        impactEffect.Play();
    }

    void Update(){
        rigid.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(){
        this.transform.position = startPosition;
        impactEffect.transform.position = startPosition;
        impactEffect.Play();
    }
}
