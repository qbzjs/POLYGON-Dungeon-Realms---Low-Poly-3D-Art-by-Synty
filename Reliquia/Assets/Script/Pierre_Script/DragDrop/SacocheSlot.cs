using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SacocheSlot : InventaireSlot, IDropHandler
{
    // Canvas qui contiendra l'item déposé dans la sacoche
    [SerializeField] private GameObject canvasSlot;
    // Inventaire du joueur
    private Inventory playerInventory;

    [System.NonSerialized] public InventaireSlot slot;

    private void Start()
    {
        playerInventory = William_Script.instance.Inventory;
    }

    /*** 
     *** Fonction appelée lorsque l'utilisateur dépose un item dans la sacoche 
     ***/
    public void OnDrop(PointerEventData eventData)
    {
        slot = eventData.pointerDrag.GetComponent<InventaireSlot>();
        Item = slot.Item;
        Manager = slot.Manager;

        // Si on déplace un objet qui était présent dans la sacoche sur un nouvel emplacement
        if (slot.TypeItem.Equals(ItemAsset.Type.Sacoche))
        {
            slot.Item.isDropped = true;
            eventData.pointerDrag.transform.SetParent(canvasSlot.transform);
        }
        // Si il s'agit d'un objet venant du grimoire
        else
        {
            if (playerInventory.sacoche.Contains(slot.Item))
            {
                slot.Item.amount++;
                Destroy(eventData.pointerDrag.gameObject);
            }
            else
            {
                // UI Setup
                slot.Item.isDropped = true;
                eventData.pointerDrag.transform.SetParent(canvasSlot.transform);
                Button btn = eventData.pointerDrag.GetComponent<Button>();
                btn.enabled = true;
                btn.onClick.AddListener(ClickedOn);


                if (slot.TypeItem.Equals(ItemAsset.Type.Consommable)) playerInventory.consommables.Remove(slot.Item);
                else if (slot.TypeItem.Equals(ItemAsset.Type.Quete)) playerInventory.objetsQuetes.Remove(slot.Item);

                slot.Item.typeItem = slot.TypeItem = ItemAsset.Type.Sacoche;
                playerInventory.AddItem(slot.Item);
            }
        }
    }

    /*** 
     *** Fonction appelée lorsque l'utilisateur utilise un item dans la sacoche 
     ***/
    public void ClickedOn()
    {
        if (Item != null && Item.asset.usable && TypeItem.Equals(ItemAsset.Type.Sacoche))
            if (Item.asset.Use())
            {
                Item.amount--;
                slot.Setup(Item, Manager);
                if (Item.amount <= 0)
                {
                    playerInventory.sacoche.Remove(Item);
                    Destroy(slot.gameObject);
                }
            }
    }
}
