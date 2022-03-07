using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KatarinaFire : MonoBehaviour{

    public Transform player;

    public GameObject flamesPrefab;
    
    public Vector3 targetLastPos;

    public bool stop = false;
    public float lifetime = 3f;
    public float speed = 3f;
    public float speedPhase2 = 30f;

    

    void Start(){
        player = GameObject.Find("ybot").GetComponent<Transform>();
        DestroyObjectDelayed();
        targetLastPos = player.position;
    }

    void Update(){
        if(transform.position != targetLastPos && stop == false){
            transform.position = Vector3.MoveTowards(transform.position, targetLastPos, speed * Time.deltaTime);
        }

        if(transform.position == targetLastPos && stop == false){
            stop = true;
            Rigidbody fireRigidBody = this.gameObject.AddComponent<Rigidbody>();
        }
    }

    void OnCollisionEnter(Collision c){
        Destroy(this.gameObject);

        if(c.gameObject.name == "Inflammable"){
            //Transform inflammablePos = c;
            Debug.Log("Touching");
            Instantiate(flamesPrefab, new Vector3(c.transform.position.x, c.transform.position.y, c.transform.position.z), Quaternion.identity);
            Destroy(c.gameObject, lifetime);
        }
    }
    

    void DestroyObjectDelayed(){
        Destroy(this.gameObject, lifetime);
    }
}
