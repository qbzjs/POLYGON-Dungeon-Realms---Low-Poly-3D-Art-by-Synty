using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting_Vincent_Test : MonoBehaviour
{
    public GameObject lightingPointDepart;
    public GameObject lightingPrefab;
    public GameObject ligthingInstance;
    public bool isCreated;
    // Start is called before the first frame update
    void Start()
    {
        isCreated = false;
        ligthingInstance = Instantiate(lightingPrefab, transform.position, Quaternion.identity, transform);
        ligthingInstance.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isCreated)
        {
            isCreated = true;

            ligthingInstance.SetActive(true);
            Debug.Log("Ligthing actif");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && isCreated)
        {
            isCreated = false;
            ligthingInstance.SetActive(false);
            Debug.Log("Ligthing non actif");
        }
        ligthingInstance.transform.position = lightingPointDepart.transform.position;
    }
}
