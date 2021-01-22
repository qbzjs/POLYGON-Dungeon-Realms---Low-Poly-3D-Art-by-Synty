using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationsPlayer : MonoBehaviour //Permet de stocker les valeurs de la sauvegarde du joueur pour les utiliser après le changement de scène
{

    public int williamVie;
    public int maxWilliamVie;

    public int williamMana;
    public int maxWilliamMana;

    public Vector3 williamPosition;

    public static InformationsPlayer instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
