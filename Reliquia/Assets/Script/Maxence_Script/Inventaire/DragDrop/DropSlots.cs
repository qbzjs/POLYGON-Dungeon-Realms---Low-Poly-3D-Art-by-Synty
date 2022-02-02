using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlots : MonoBehaviour, IDropHandler
{
    private GameObject consommablePanel;
    private GameObject objetQuetePanel;

    [SerializeField] private InventaireManager thisManager;
    [SerializeField] private PlayerInventory playerInventory;

    private void Awake()
    {
        consommablePanel = thisManager.consommablePanel;
        objetQuetePanel = thisManager.objetQuetePanel;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemInventaire item = eventData.pointerDrag.GetComponent<InventaireSlot>().thisItem;
        InventaireSlot typeItem = eventData.pointerDrag.GetComponent<InventaireSlot>();

        // Si le drop se trouve sur l'objet FondConsommables
        if (gameObject.name == "FondConsommables")
        {
            if (typeItem.TypeItem != "Consommable" && typeItem.TypeItemBase == "Consommable")
            {
                // Si l'objet est déjà présent dans le grimoire 
                if (playerInventory.consommablesInventory.Exists(i => i.name == item.name && i.isDropped))
                {
                    item.numberHeld++;
                    Destroy(eventData.pointerDrag.gameObject);
                    thisManager.ClearConsommableSlots();
                    thisManager.MakeConsommableSlot();
                }
                // Si l'objet n'est pas encore présent dans le grimoire
                else
                {
                    eventData.pointerDrag.transform.SetParent(consommablePanel.transform);
                    if (typeItem.TypeItem == "Sacoche") playerInventory.sacochesInventory.Remove(item);

                    typeItem.TypeItem = "Consommable";
                    item.typeItem = "Consommable";
                    playerInventory.consommablesInventory.Add(item);
                }
                item.isDropped = true;
            }
        }

        // Si le drop se trouve sur l'objet FondObjetQuetes
        else if (gameObject.name == "FondObjetQuetes")
        {
            if (typeItem.TypeItemBase == "ObjetQuete")
            {
                item.isDropped = true;
                if (playerInventory.objetsQuetesInventory.Contains(item))
                {
                    //item.numberHeld++;
                    Destroy(eventData.pointerDrag.gameObject);
                    thisManager.ClearObjetQuetesSlots();
                    thisManager.MakeObjetQueteSlot();
                }
                else
                {
                    eventData.pointerDrag.transform.SetParent(objetQuetePanel.transform);
                    if (typeItem.TypeItem == "Sacoche") playerInventory.sacochesInventory.Remove(item);

                    typeItem.TypeItem = "ObjetQuete";
                    item.typeItem = "ObjetQuete";
                    playerInventory.AddItem("ObjetQuete", item);
                }
            }
        }

        // Si le drop se trouve sur l'objet FondPuzzles
        else if (gameObject.name == "FondPuzzles")
        {
            if (typeItem.TypeItemBase == "Puzzle") { }
        }
    }
}
