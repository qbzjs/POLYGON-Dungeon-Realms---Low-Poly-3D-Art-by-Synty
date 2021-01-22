using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Items")]
public class Item : ScriptableObject
{
    public new string name;
    public string description;

    public Mesh forme;
    public Material material;
    public Sprite imageInventaireCompas;
}
