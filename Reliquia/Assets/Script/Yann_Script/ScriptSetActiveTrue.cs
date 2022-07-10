using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptSetActiveTrue : MonoBehaviour
{
    public GameObject Script;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        Script.SetActive(true);

    }

}
