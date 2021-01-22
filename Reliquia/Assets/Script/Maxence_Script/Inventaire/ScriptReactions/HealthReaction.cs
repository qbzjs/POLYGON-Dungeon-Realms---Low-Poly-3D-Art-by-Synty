using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthReaction : MonoBehaviour
{
    public void Use(int valueToAdd)
    {
        RessourcesVitalesWilliam_Scrip.instance.RajouterVie(valueToAdd);
    }
}
