using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionTorche : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        EtatTorche(other, true);

    }
    private void OnTriggerExit(Collider other)
    {
        EtatTorche(other, false);

    }
    /// <summary>
    /// Allume ou éteint la torche.
    /// </summary>
    /// <param name="other">Le collider entrant</param>
    /// <param name="etat">Etat de la torche</param>
    private void EtatTorche(Collider other, bool etat)
    {
        if (other.gameObject.tag == "Player")
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(etat);
            gameObject.transform.GetChild(1).gameObject.SetActive(etat);
        }
    }
}
