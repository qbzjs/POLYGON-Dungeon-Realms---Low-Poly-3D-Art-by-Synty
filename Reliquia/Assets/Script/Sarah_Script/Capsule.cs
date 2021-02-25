using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : MonoBehaviour
{
    public Transform pos1;
    public Transform pos2;
    public bool isArrived;

    void Update() {

        if (transform.localPosition.x == pos1.position.x) {
            isArrived = true;
        }

        else if (transform.localPosition.x == pos2.position.x) {
            isArrived = false;
        }

        if (isArrived == true) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pos2.position, Time.deltaTime * 1); 
        }

        else if (isArrived == false) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, pos1.position, Time.deltaTime * 1);
        }
    }
}
