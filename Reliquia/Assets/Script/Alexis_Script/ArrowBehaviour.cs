using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour{

    //Transform transform;
    Vector3 startPosition;
    Rigidbody rigid;

    public float speed = 1f; 

    void Start(){
        startPosition = this.transform.position;
        rigid = this.GetComponent<Rigidbody>();
    }

    void Update(){
        rigid.velocity = transform.forward * speed;
    }

    void OnCollisionEnter(){
        this.transform.position = startPosition;
    }
}
