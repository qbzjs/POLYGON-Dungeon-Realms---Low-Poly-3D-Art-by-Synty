using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemInventory
{
    public ItemAsset asset;
    public int amount;

    public ItemAsset.Type typeItem;
    public bool isDropped = true;

    public ItemInventory(ItemAsset asset, int amount)
    {
        this.asset = asset;
        this.amount = amount;
        this.typeItem = asset.typeItemBase;
    }

    // On essaye d'empiler un certain nombre d'item, si le nombre final dépasse 5, on rétablit à 5
    // et on renvoie le nombre restant, car il restera d'autres stack à créer ou empiler
    public int StackItem(int delta, int maxStack)
    {
        Debug.Log(asset.itemNom + " old amount : " + amount);
        amount += delta;
        if (amount > maxStack)
        {
            amount -= delta;
            delta -= maxStack - amount;
            amount = maxStack;
        }
        else delta = 0;

        Debug.Log(asset.itemNom + " new amount : " + amount);

        return delta;

    }

}
