using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldOnCollider : MonoBehaviour
{

    public GameObject Shield;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ShieldBreaker"))
        {
            //Transform inflammablePos = c;
            // Destroy(this.gameObject);

            Shield.SetActive(false);
        }

    }
}
