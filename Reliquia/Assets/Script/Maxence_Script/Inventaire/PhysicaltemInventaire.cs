using clavier;
using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PhysicaltemInventaire : MonoBehaviour
{
    [SerializeField] private PlayerInventory playerInventory;
    public ItemInventaire thisItem;
    public Image itemImage;
    [SerializeField] private InventaireManager thisManager;

    public enum TypeItem
    {
        None,
        Quetes,
        Consommable,
        Puzzle
    }

    public TypeItem typeItemValue;

    private void Awake()
    {
        itemImage = gameObject.GetComponent<Image>();
        itemImage.sprite = thisItem.itemImage;
        
    }

    public void AddItem(ItemInventaire item)
    {
        MarqeurQuete_Script marqueur = gameObject.GetComponent<MarqeurQuete_Script>();
        if (marqueur != null)
        {
            Compas_Script.instance.RemoveMarqueurQuete(marqueur);
        }

        if (typeItemValue == TypeItem.Quetes)
        {
            if (playerInventory.consommablesInventory.Contains(thisItem) && thisManager.maxItemQuete < 6) AddItemToQuetes(item);
            else if (playerInventory.sacochesInventory.Contains(thisItem) && thisManager.maxItemSacoche < 12) AddItemToSacoche(item);
            else AddItemToQuetes(item);
        }
        else if (typeItemValue == TypeItem.Consommable)
        {
            if (playerInventory.consommablesInventory.Contains(thisItem) && thisManager.maxItemConsommable < 12) AddItemToConsommable(item);
            else if (playerInventory.sacochesInventory.Contains(thisItem) && thisManager.maxItemSacoche < 12) AddItemToSacoche(item);
            else AddItemToConsommable(item);
        }
        else if (typeItemValue == TypeItem.Puzzle && thisManager.maxItemPuzzles < 6) AddItemToPuzzles(item);

        Destroy(gameObject);
    }

    void AddItemToSacoche(ItemInventaire item)
    {
        if (playerInventory && item)
        {
            thisManager.ClearInventorySlots();

            if (playerInventory.sacochesInventory.Contains(item))
            {
                item.numberHeld++;
            }
            else
            {
                thisManager.maxItemSacoche++;
                item.numberHeld++;
                playerInventory.sacochesInventory.Add(item);
            }

            thisManager.MakeSacocheSlots();
        }
    }

    void AddItemToConsommable(ItemInventaire item)
    {
        if (playerInventory && item)
        {
            thisManager.ClearConsommableSlots();

            if (playerInventory.consommablesInventory.Contains(item))
            {
                item.numberHeld++;
            }
            else
            {
                thisManager.maxItemConsommable++;
                item.numberHeld++;
                playerInventory.consommablesInventory.Add(item);
            }

            thisManager.MakeConsommableSlot();
        }
    }

    void AddItemToQuetes(ItemInventaire item)
    {
        if (playerInventory && item)
        {
            thisManager.maxItemQuete++;

            thisManager.ClearObjetQuetesSlots();
            if (!playerInventory.objetsQuetesInventory.Contains(item))
            {
                item.numberHeld++;
            }

            playerInventory.objetsQuetesInventory.Add(item);

            thisManager.MakeObjetQueteSlot();
        }
    }

    void AddItemToPuzzles(ItemInventaire item)
    {
        if (playerInventory && item)
        {
            thisManager.maxItemPuzzles++;

            thisManager.ClearPuzzlesSlots();
            if (!playerInventory.puzzlesInventory.Contains(item))
            {
                item.numberHeld++;
            }

            playerInventory.puzzlesInventory.Add(item);

            thisManager.MakePuzzlesSlot();
        }
    }
}
