using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelativePosition : MonoBehaviour {

    public Transform target;
    private Vector3 offset = Vector3.zero;

    private void Awake()
    {
        offset = transform.position - target.position;
    }

    private void FixedUpdate()
    {
        transform.position = target.position + offset;
    }
}
