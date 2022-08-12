using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnOnCollider : MonoBehaviour
{
    public GameObject flamesPrefab;

    public float lifetime = 3f;


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
        if (other.gameObject.CompareTag("Inflammable"))
        {
            //Transform inflammablePos = c;
            Debug.Log("Touching");
            Instantiate(flamesPrefab, new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z), Quaternion.identity);
        }

        if (other.gameObject.CompareTag("Phase2"))
        {
            //Transform inflammablePos = c;
            Debug.Log("Touching");
            Instantiate(flamesPrefab, new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z), Quaternion.identity);
        }

    }

}