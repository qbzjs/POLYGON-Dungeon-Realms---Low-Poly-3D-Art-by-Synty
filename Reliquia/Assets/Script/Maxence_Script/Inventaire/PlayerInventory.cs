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

    public ItemInventaire GetItemByTypeFromSacoche(ItemInventaire item)
    {
        ItemInventaire itemResult = null;
        switch (item.typeItem)
        {
            case "Consommable":
                break;
            case "ObjetQuete":

                if (objetsQuetesInventory.Contains(item))
                {
                    itemResult = item;
                }
                break;
            default:
                break;
        }

        return itemResult;
    }

    public bool UseItem(ItemInventaire item)
    {
        bool response = false;

        if (item == null)
        {
            return response;
        }
        item.Use();
        if (item.typeItem == "Consommable" || item.typeItem == "ObjetQuete")
        {
            item.DecreaseAmount(1);
        }
        
        switch (item.typeItem)
        {
            case "Consommable":
                break;
            case "ObjetQuete":
                objetsQuetesInventory.Remove(item);
                response = true;
                break;
            default:
                break;
        }
        return response;

    }
}
