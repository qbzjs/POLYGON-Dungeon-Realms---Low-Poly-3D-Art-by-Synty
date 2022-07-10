using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointCheck : MonoBehaviour
{
    private bool isCheckpointReached = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isCheckpointReached = true)
        {
            Debug.Log("Checkpoint reached");

        }
    }

    void OnTriggerEnter(Collider other)
    {
        isCheckpointReached = true;
     
    }
}
