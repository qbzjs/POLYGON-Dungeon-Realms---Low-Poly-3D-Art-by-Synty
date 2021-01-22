using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventaire_Script : MonoBehaviour
{
    public List<ItemInventaire> items = new List<ItemInventaire>();

    public static Inventaire_Script instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
