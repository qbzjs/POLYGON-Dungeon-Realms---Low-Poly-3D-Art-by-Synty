using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public int maxStack = 5;

    public int MaxConsommableSlot = 12;
    public int MaxQueteSlot = 6;

    //[System.NonSerialized] 
    public List<ItemInventory> sacoche = new List<ItemInventory>();
    //[System.NonSerialized]
    public List<ItemInventory> consommables = new List<ItemInventory>();
    //[System.NonSerialized]
    public List<ItemInventory> objetsQuetes = new List<ItemInventory>();
    //[System.NonSerialized]
    public List<ItemInventory> puzzles = new List<ItemInventory>();

    public bool AddItem(ItemInventory item)
    {
        InventaireManager.instance.ClearSlots(item.typeItem);
        List<ItemInventory> stackableItems = new List<ItemInventory>();
        bool full = false;

        switch (item.typeItem)
        {
            case ItemAsset.Type.Sacoche:
                sacoche.Add(item);
                break;

            case ItemAsset.Type.Quete:
                // Récupération des items de l'inventaire qui peuvent être stack : même objet et nombre inférieur à 5
                stackableItems = objetsQuetes.FindAll(delegate (ItemInventory mItem) { return mItem.asset == item.asset && mItem.amount < 5; });

                // Pour ces items, on va remplir les stack déjà entammés
                // On arrête si le reste d'item à stack est nul
                foreach(ItemInventory mItem in stackableItems)
                {
                    item.amount = mItem.StackItem(item.amount, maxStack);
                    if (item.amount == 0) break;
                }

                // Si tous les stacks ont été rempli, mais qu'il nous reste des items à stack, on va créer de nouveaux emplacements d'inventaire
                if(item.amount != 0)
                {
                    int loop = item.amount / maxStack;
                    // Création de stacks plein
                    if (loop > 0)
                        for (int i = 0; i < loop; i++)
                            if (objetsQuetes.Count < MaxQueteSlot)
                            {
                                objetsQuetes.Add(new ItemInventory(item.asset, maxStack));
                                item.amount -= maxStack;
                            }
                            else full = true;

                    // Création d'un dernier stack, prenant le reste
                    item.amount %= maxStack;    
                    if (item.amount > 0)
                        if (objetsQuetes.Count < MaxQueteSlot) objetsQuetes.Add(item);
                        else full = true;
                }
                break;

            case ItemAsset.Type.Consommable:
                // Récupération des items de l'inventaire qui peuvent être stack : même objet et nombre inférieur à 5
                stackableItems = consommables.FindAll(delegate (ItemInventory mItem) { return mItem.asset == item.asset && mItem.amount < 5; });

                // Pour ces items, on va remplir les stack déjà entammés
                // On arrête si le reste d'item à stack est nul
                foreach (ItemInventory mItem in stackableItems)
                {
                    item.amount = mItem.StackItem(item.amount, maxStack);
                    if (item.amount == 0) break;
                }

                // Si tous les stacks ont été rempli, mais qu'il nous reste des items à stack, on va créer de nouveaux emplacements d'inventaire
                if (item.amount != 0)
                {
                    int loop = item.amount / maxStack;
                    // Création de stacks plein
                    if (loop > 0)
                        for (int i = 0; i < loop; i++)
                            if (consommables.Count < MaxConsommableSlot)
                            {
                                consommables.Add(new ItemInventory(item.asset, maxStack));
                                item.amount -= maxStack;
                            }
                            else full = true;

                    // Création d'un dernier stack si des emplacements sont disponibles, prenant le reste
                    int reste = item.amount % maxStack;
                    if (reste > 0)
                        if (consommables.Count < MaxConsommableSlot) consommables.Add(new ItemInventory(item.asset, reste));
                        else full = true;
                }
                break;

            case ItemAsset.Type.Puzzle:
                puzzles.Add(item);
                break;

            default:
                Debug.LogError("Inventory::Add not implemented yet : " + item.asset.itemNom);
                break;

        }

        InventaireManager.instance.MakeSlots(item.typeItem);
        return !full;
    }

    public void RemoveItem(ItemInventory item, int amount=-1)
    {
        switch (item.typeItem)
        {
            case ItemAsset.Type.Sacoche:
                if (amount == -1) sacoche.Remove(item);
                break;

            case ItemAsset.Type.Quete:
                if (amount == -1) objetsQuetes.Remove(item);

                break;

            case ItemAsset.Type.Consommable:
                if (amount == -1) consommables.Remove(item);
                break;

            case ItemAsset.Type.Puzzle:
                if (amount == -1) puzzles.Remove(item);
                break;

            default:
                Debug.LogError("Inventory::Remove not implemented yet : " + item.asset.itemNom);
                break;
        }
    }

    // Permet de retirer un item de la sacoche en renseignant son asset, utile pour les objets de quête.
    public void RemoveItemInSacoche(ItemAsset item, int amount = -1)
    {
        List<ItemInventory> Items = sacoche.FindAll(i => i.asset.Equals(item));

        if (amount == -1)
        {
            sacoche.Remove(Items[0]);
            InventaireManager.instance.RemoveSacocheSlot(Items[0]);
        }
        else
        {
            foreach (ItemInventory i in Items)
            {
                if (amount >= 0)
                {
                    int tmp = amount;
                    amount -= (maxStack - i.amount);
                    i.amount -= tmp;
                    if (i.amount == 0) sacoche.Remove(i);
                }
            }
        }
    }
}
