using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatarinaFlame : MonoBehaviour{

    public Katarina_Boss katarinaBossScript;

    public Rigidbody rigid;

    public Vector3[] flamesPositions;

    public float randomNumber;

    public float lifetime = 5f;

    public float speedX = 3f;
    public float speedZ= 3f;

    void Start(){
        //katarinaBossScript = GameObject.Find("Katarina").GetComponent<Katarina_Boss>();
        rigid = this.GetComponent<Rigidbody>();
        DestroyObjectDelayed();
        randomNumber = Random.Range(0, 4);

        switch (randomNumber){
            case 0:
                this.transform.position = flamesPositions[0]; 
            break;
            case 1:
                this.transform.position = flamesPositions[1];
                this.transform.eulerAngles = new Vector3 (0, 90, 0);
            break;
            case 2:
                this.transform.position = flamesPositions[2];
            break;
            case 3:
                this.transform.position = flamesPositions[3];
                this.transform.eulerAngles =  new Vector3 (0, 90, 0);
            break;
        }
    }

    void Update(){
        switch (randomNumber){
            case 0:
                rigid.velocity = new Vector3(speedX, 0, 0);
            break;
            case 1:
                rigid.velocity = new Vector3(0, 0, -speedZ);
            break;
            case 2:
                rigid.velocity = new Vector3(-speedX, 0, 0);
            break;
            case 3:
                rigid.velocity = new Vector3(0, 0, speedZ);
            break;
        } 
    }

    void DestroyObjectDelayed(){
        Destroy(this.gameObject, lifetime);
    }

    void OnTriggerEnter(Collider c){
        if(c.gameObject.name == "ybot"){
            Debug.Log("Player hurt");
        }
    }

}
