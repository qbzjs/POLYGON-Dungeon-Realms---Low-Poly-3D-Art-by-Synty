using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Item", menuName = "Iventory/Items")]
[System.Serializable]
public class ItemInventaire : ScriptableObject
{
    public string itemNom;
    public string itemDescription;
    public Sprite itemImage;
    public Mesh itemMesh;
    public Material itemMaterial;
    public int numberHeld;
    public bool usable;
    public bool unique;
    public bool isDropped = true;

    public string typeItemBase;
    public string typeItem;

    public UnityEvent thisEvent;
    [SerializeField] private PlayerInventory playerInventory;

    public void Use()
    {
        Debug.Log("test item : " + itemNom);
        thisEvent.Invoke();
    }

    public void DecreaseAmount(int valueToDecrease)
    {
        numberHeld -= valueToDecrease;
        if (numberHeld < 0) numberHeld = 0;
    }
}
