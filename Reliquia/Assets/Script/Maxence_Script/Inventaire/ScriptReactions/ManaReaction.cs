using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaReaction : MonoBehaviour
{
    public void Use(int valueToAdd)
    {
        RessourcesVitalesWilliam_Scrip.instance.RajouterMana(valueToAdd);
    }
}
