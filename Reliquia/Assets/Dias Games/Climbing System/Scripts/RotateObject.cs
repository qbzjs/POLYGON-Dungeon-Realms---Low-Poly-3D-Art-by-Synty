using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float angularSpeed = 15f;

    private void FixedUpdate()
    {
        transform.Rotate(0, angularSpeed * Time.deltaTime, 0);
    }
}
