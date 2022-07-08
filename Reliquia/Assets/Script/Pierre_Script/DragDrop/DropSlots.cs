using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlots : MonoBehaviour, IDropHandler
{
    private GameObject consommablePanel;
    private GameObject objetQuetePanel;

    [SerializeField] private InventaireManager thisManager;
    
    private Inventory playerInventory;

    private void Awake()
    {
        consommablePanel = thisManager.consommablePanel;
        objetQuetePanel = thisManager.objetQuetePanel;
        playerInventory = William_Script.instance.Inventory;
    }


    public void OnDrop(PointerEventData eventData)
    {
        ItemInventory item = eventData.pointerDrag.gameObject.GetComponent<InventaireSlot>().Item;

        // Si le drop se trouve sur l'objet FondConsommables
        if (gameObject.name == "FondConsommables")
        {
            if (!item.typeItem.Equals(ItemAsset.Type.Consommable) && item.asset.typeItemBase.Equals(ItemAsset.Type.Consommable))
            {
                playerInventory.sacoche.Remove(item);
                item.typeItem = ItemAsset.Type.Consommable;
                playerInventory.AddItem(item);
                Destroy(eventData.pointerDrag.gameObject);

                item.isDropped = true;
            }
        }

        // Si le drop se trouve sur l'objet FondObjetQuetes
        else if (gameObject.name == "FondObjetQuetes")
        {
            if (item.asset.typeItemBase.Equals(ItemAsset.Type.Quete))
            {
                item.isDropped = true;
                if (playerInventory.objetsQuetes.Contains(item))
                {
                    //item.numberHeld++;
                    Destroy(eventData.pointerDrag.gameObject);
                    thisManager.ClearSlots(ItemAsset.Type.Quete);
                    thisManager.MakeSlots(ItemAsset.Type.Quete);
                }
                else
                {
                    eventData.pointerDrag.transform.SetParent(objetQuetePanel.transform);
                    if (item.typeItem.Equals(ItemAsset.Type.Sacoche)) playerInventory.sacoche.Remove(item);

                    //typeItem.TypeItem = "ObjetQuete";
                    item.typeItem = ItemAsset.Type.Quete;
                    playerInventory.AddItem(item);
                }
            }
        }

        // Si le drop se trouve sur l'objet FondPuzzles
        else if (gameObject.name == "FondPuzzles")
        {
            if (item.asset.typeItemBase.Equals(ItemAsset.Type.Puzzle)) { }
        }
    }
}
