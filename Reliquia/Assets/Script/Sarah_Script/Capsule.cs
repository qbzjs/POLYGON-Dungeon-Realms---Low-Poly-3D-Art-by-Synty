using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    public GameObject capsule;
    public Transform pos1;
    public Transform pos2;
    public bool isNear;
    public bool isArrived;
    public bool isMoving;

    void Update() {

        if (transform.localPosition.x == pos1.position.x) {
            isArrived = true;
        }

        else if (transform.localPosition.x == pos2.position.x) {
            isArrived = false;
        }

        if (isArrived == true && isMoving == false) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pos2.position, Time.deltaTime * 1); 
        }

        else if (isArrived == false && isMoving == false) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pos1.position, Time.deltaTime * 1);
        }
    }

    public void OnTriggerEnter (Collider other) {
        isNear = true;
    }

    public void OnTriggerExit (Collider other) {
        isNear = false;
    }
}
