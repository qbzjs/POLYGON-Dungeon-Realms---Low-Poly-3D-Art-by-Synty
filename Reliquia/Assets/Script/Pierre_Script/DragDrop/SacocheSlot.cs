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
        Manager = InventaireManager.instance;
    }

    private void SetInventaireSlot(InventaireSlot slot)
    {
        this.slot = slot;
        Item = slot.Item;
    }

    public void ResetSlot()
    {
        slot = null;
        Item = null;
    }

    /*** 
     *** Fonction appelée lorsque l'utilisateur dépose un item dans la sacoche 
     ***/
    public void OnDrop(PointerEventData eventData)
    {
        InventaireSlot DroppedSlot = eventData.pointerDrag.GetComponent<InventaireSlot>();

        // Si on déplace un objet qui était présent dans la sacoche sur un nouvel emplacement
        if (DroppedSlot.TypeItem.Equals(ItemAsset.Type.Sacoche))
        {
            if (slot != null && slot != DroppedSlot) SwapSlots(DroppedSlot, slot);
            else Manager.GetSacocheSlot(DroppedSlot.Item).ResetSlot();

            SetInventaireSlot(DroppedSlot);

            slot.Item.isDropped = true;
            eventData.pointerDrag.transform.SetParent(canvasSlot.transform);
        }
        // Si il s'agit d'un objet venant du grimoire
        else
        {
            if (slot != null) SwapSlots(DroppedSlot, slot);

            SetInventaireSlot(DroppedSlot);
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

    private void SwapSlots(InventaireSlot Dropped, InventaireSlot Swapped)
    {
        Debug.Log("swap");
        // Si l'objet déposé provient de la sacoche
        if(Dropped.TypeItem.Equals(ItemAsset.Type.Sacoche))
        {
            Debug.Log("in sacoche");
            SacocheSlot SwapSlot = Manager.GetSacocheSlot(Dropped.Item);
            SwapSlot.SetInventaireSlot(Swapped);
            Swapped.transform.SetParent(SwapSlot.canvasSlot.transform);
        }
        // Si l'objet déposé provient du grimoire
        else
        {
            Debug.Log("in grimoire");

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
