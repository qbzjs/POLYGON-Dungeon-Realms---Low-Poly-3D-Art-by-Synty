using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Inventory", menuName = "Iventory/PlayerInventory")]
public class PlayerInventory : ScriptableObject
{
    public List<ItemInventaire> sacochesInventory = new List<ItemInventaire>();
    public List<ItemInventaire> consommablesInventory = new List<ItemInventaire>();
    public List<ItemInventaire> objetsQuetesInventory = new List<ItemInventaire>();
    public List<ItemInventaire> puzzlesInventory = new List<ItemInventaire>();

    public ItemInventaire GetItemByTypeFromSacoche(PhysicaltemInventaire.TypeItem typeItem, string name)
    {
        ItemInventaire itemResult = null;
        switch (typeItem)
        {
            case PhysicaltemInventaire.TypeItem.None:
                break;
            case PhysicaltemInventaire.TypeItem.Quetes:

                itemResult = (ItemInventaire)objetsQuetesInventory.Where( s=> s.itemNom == name);
                break;
            case PhysicaltemInventaire.TypeItem.Consommable:
                break;
            case PhysicaltemInventaire.TypeItem.Puzzle:
                break;
            default:
                break;
        }

        return itemResult;
    }

    public bool UseItem(PhysicaltemInventaire.TypeItem typeItem, ItemInventaire item)
    {
        bool response = false;

        if (item == null)
        {
            return response;
        }
        item.Use();
        switch (typeItem)
        {
            case PhysicaltemInventaire.TypeItem.None:
                break;
            case PhysicaltemInventaire.TypeItem.Quetes:
                objetsQuetesInventory.Remove(item);
                response = true;
                break;
            case PhysicaltemInventaire.TypeItem.Consommable:
                break;
            case PhysicaltemInventaire.TypeItem.Puzzle:
                break;
            default:
                break;
        }
        return response;

    }
}
