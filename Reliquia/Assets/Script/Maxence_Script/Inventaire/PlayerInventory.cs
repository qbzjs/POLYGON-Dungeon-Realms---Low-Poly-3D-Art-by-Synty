using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Inventory", menuName = "Iventory/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public List<ItemInventaire> sacochesInventory = new List<ItemInventaire>();
    public List<ItemInventaire> consommablesInventory = new List<ItemInventaire>();
    public List<ItemInventaire> objetsQuetesInventory = new List<ItemInventaire>();
    public List<ItemInventaire> puzzlesInventory = new List<ItemInventaire>();
}
