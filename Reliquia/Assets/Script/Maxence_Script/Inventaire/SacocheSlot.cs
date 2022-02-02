using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SacocheSlot : InventaireSlot, IDropHandler
{
    // Canvas qui contiendra l'item déposé dans la sacoche
    [SerializeField] private GameObject canvasSlot;
    // Inventaire du joueur
    [SerializeField] private PlayerInventory playerInventory;

    /*** 
     *** Fonction appelée lorsque l'utilisateur dépose un item dans la sacoche 
     ***/
    public void OnDrop(PointerEventData eventData)
    {
        InventaireSlot slot = eventData.pointerDrag.GetComponent<InventaireSlot>();

        // Si on déplace un objet qui était présent dans la sacoche sur un nouvel emplacement
        if (slot.TypeItem == "Sacoche")
        {
            slot.thisItem.isDropped = true;
            eventData.pointerDrag.transform.SetParent(canvasSlot.transform);
        }
        // Si il s'agit d'un objet venant du grimoire
        else
        {
            if (playerInventory.sacochesInventory.Contains(slot.thisItem))
            {
                slot.thisItem.numberHeld++;
                Destroy(eventData.pointerDrag.gameObject);
                thisManager.ClearInventorySlots();
                thisManager.MakeSacocheSlots();
            }
            else
            {
                slot.thisItem.isDropped = true;
                eventData.pointerDrag.transform.SetParent(canvasSlot.transform);
                if (slot.TypeItem == "Consommable") playerInventory.consommablesInventory.Remove(slot.thisItem);
                else if (slot.TypeItem == "ObjetQuete") playerInventory.objetsQuetesInventory.Remove(slot.thisItem);

                slot.TypeItem = "Sacoche";
                slot.thisItem.typeItem = "Sacoche";
                playerInventory.sacochesInventory.Add(slot.thisItem);
            }
        }
    }

    /*** 
     *** Fonction appelée lorsque l'utilisateur utilise un item dans la sacoche 
     ***/
    public void ClickedOn()
    {
        if (thisItem)
        {
            if (thisItem.usable && TypeItem == "Sacoche")
            {
                thisManager.SetupDescriptionAndButton(thisItem.itemDescription, thisItem.usable, thisItem);
                thisItem.Use();
                thisManager.ClearInventorySlots();
                thisManager.MakeSacocheSlots();
                thisManager.SetupDescriptionAndButton("", thisItem.usable, thisItem);

                if (thisItem.numberHeld <= 0)
                {
                    thisManager.playerInventory.sacochesInventory.Remove(thisItem);
                }
            }
        }
    }
}
