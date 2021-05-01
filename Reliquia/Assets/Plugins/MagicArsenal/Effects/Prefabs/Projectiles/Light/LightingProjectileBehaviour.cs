using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingProjectileBehaviour : MonoBehaviour{

    public GameObject tlighting;
    public GameObject fxImpact;
    private Transform projectilePos;
    public float force = 1000f;
    public float duration = 3f;
    

    // Start is called before the first frame update
    void Start(){

        tlighting = GameObject.Find("Position Lighting");

        this.gameObject.GetComponent<Rigidbody>().AddForce(tlighting.transform.forward * force);
        Destroy(this.gameObject, duration);
        projectilePos = this.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Destroy(this.gameObject);
            Instantiate(fxImpact, projectilePos.position, projectilePos.rotation);
        }
    }
}
