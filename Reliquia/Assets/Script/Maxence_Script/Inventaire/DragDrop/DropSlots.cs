using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlots : MonoBehaviour, IDropHandler
{
    private GameObject sacochePanel;
    private GameObject consommablePanel;
    private GameObject objetQuetePanel;

    [SerializeField] private InventaireManager thisManager;
    [SerializeField] private PlayerInventory playerInventory;

    private void Awake()
    {
        sacochePanel = thisManager.sacochePanel;
        consommablePanel = thisManager.consommablePanel;
        objetQuetePanel = thisManager.objetQuetePanel;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ItemInventaire item = eventData.pointerDrag.GetComponent<InventaireSlot>().thisItem;
        InventaireSlot typeItem = eventData.pointerDrag.GetComponent<InventaireSlot>();

        if (gameObject.name == "FondSacoche")
        {
            if (typeItem.TypeItem == "Sacoche")
            {
                //Retour à la position de base
                eventData.pointerDrag.transform.SetParent(sacochePanel.transform);
            }
            else
            {
                if (playerInventory.sacochesInventory.Contains(item))
                {
                    item.numberHeld++;
                    Destroy(eventData.pointerDrag.gameObject);
                    thisManager.ClearInventorySlots();
                    thisManager.MakeSacocheSlots();
                }
                else
                {
                    eventData.pointerDrag.transform.SetParent(sacochePanel.transform);
                    if (typeItem.TypeItem == "Consommable") playerInventory.consommablesInventory.Remove(item);
                    if (typeItem.TypeItem == "ObjetQuete") playerInventory.objetsQuetesInventory.Remove(item);

                    playerInventory.sacochesInventory.Add(item);
                    typeItem.TypeItem = "Sacoche";
                    item.typeItem = "Sacoche";
                }
            }
        }
        else if (gameObject.name == "FondConsommables")
        {
            if (typeItem.TypeItem == "Consommable") eventData.pointerDrag.transform.SetParent(consommablePanel.transform);
            else if (typeItem.TypeItem == "ObjetQuete") eventData.pointerDrag.transform.SetParent(objetQuetePanel.transform);
            else if (typeItem.TypeItem == "Sacoche" && typeItem.TypeItemBase != "Consommable") eventData.pointerDrag.transform.SetParent(sacochePanel.transform);
            else
            {
                if (playerInventory.consommablesInventory.Contains(item))
                {
                    item.numberHeld++;
                    Destroy(eventData.pointerDrag.gameObject);
                    thisManager.ClearConsommableSlots();
                    thisManager.MakeConsommableSlot();
                }
                else
                {
                    eventData.pointerDrag.transform.SetParent(consommablePanel.transform);
                    if (typeItem.TypeItem == "Sacoche") playerInventory.sacochesInventory.Remove(item);

                    playerInventory.consommablesInventory.Add(item);
                    typeItem.TypeItem = "Consommable";
                    item.typeItem = "Consommable";
                }
            }
        }
        else if (gameObject.name == "FondObjetQuetes") 
        {
            if (typeItem.TypeItem == "ObjetQuete") eventData.pointerDrag.transform.SetParent(objetQuetePanel.transform);
            //else if (typeItem.TypeItemBase == "ObjetQuete") eventData.pointerDrag.transform.SetParent(objetQuetePanel.transform);
            else if(typeItem.TypeItem == "Consommable") eventData.pointerDrag.transform.SetParent(consommablePanel.transform);
            else if (typeItem.TypeItem == "Sacoche" && typeItem.TypeItemBase != "ObjetQuete") eventData.pointerDrag.transform.SetParent(sacochePanel.transform); 
            else
            {
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

                    //playerInventory.objetsQuetesInventory.Add(item);
                    playerInventory.AddItem("ObjetQuete", item);
                    typeItem.TypeItem = "ObjetQuete";
                    item.typeItem = "ObjetQuete";
                }
            }
        }
        else if (gameObject.name == "FondPuzzles")
        {
            if (typeItem.TypeItem == "Consommable") eventData.pointerDrag.transform.SetParent(consommablePanel.transform);
            else if (typeItem.TypeItem == "ObjetQuete") eventData.pointerDrag.transform.SetParent(objetQuetePanel.transform);
            else if (typeItem.TypeItem == "Sacoche") eventData.pointerDrag.transform.SetParent(sacochePanel.transform);
        }
    }
}
