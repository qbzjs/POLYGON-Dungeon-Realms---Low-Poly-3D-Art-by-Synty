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

    public UnityEvent Effect;

    public enum Type { Sacoche, Quete, Puzzle, Consommable }

    public bool Use()
    {
        Effect.Invoke();
        bool isUsed = ItemEffect.used;

        ItemEffect.used = false;
        return isUsed;
    }

}