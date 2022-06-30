using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/NewItems")]
public class ItemAsset : ScriptableObject
{
    public string itemNom;
    public string itemDescription;
    public Sprite itemImage;
    public Mesh itemMesh;
    public Material itemMaterial;

    public bool usable;
    public bool unique;

    public Type typeItemBase;

    public UnityEvent thisEvent;

    public enum Type { Sacoche, Quete, Puzzle, Consommable }

    public void Use()
    {
        Debug.Log("test item : " + itemNom);
        thisEvent.Invoke();
    }

}