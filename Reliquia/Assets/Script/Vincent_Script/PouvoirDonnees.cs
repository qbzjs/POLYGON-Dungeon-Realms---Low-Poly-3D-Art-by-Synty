using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pouvoir_", menuName = "ScriptableObjects/Pouvoir")]
public class PouvoirDonnees : ScriptableObject
{
    public Sprite ImageVide;
    public Sprite ImagePlein;
    public float TempsRecharge;
    public float CoutMana;
    public float DureePouvoir;
    public float DrainManaParSeconde;
}
