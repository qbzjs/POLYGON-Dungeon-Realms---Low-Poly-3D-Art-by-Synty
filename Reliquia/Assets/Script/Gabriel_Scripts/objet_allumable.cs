using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objet_allumable : MonoBehaviour
{

    [SerializeField]
    private GameObject feu;

    public bool lightable = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("lightingCheck"))
        {
            lightable = true;
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("lightingCheck"))
        {
            lightable = false;
        }
    }

    public void lightObject()
    {
        Debug.Log("feu allumé");
        feu.SetActive(true);
    }




}
