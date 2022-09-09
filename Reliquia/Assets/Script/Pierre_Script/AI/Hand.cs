using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Script pour faire fonctionner les coups de poing à l'aide de collider
public class Hand : MonoBehaviour
{
    IFighter Fighter;
    private void Awake()
    {
        Fighter = GetComponentInParent<IFighter>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Fighter.Equals(William_Script.instance))
        {

        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("enemy touch player");
        }
    }
}
