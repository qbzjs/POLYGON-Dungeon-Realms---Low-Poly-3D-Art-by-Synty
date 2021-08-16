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
    private List<ItemInventaire> sessionAddedItem = new List<ItemInventaire>();

    public bool GetItemFromSacoche(ItemInventaire item)
    {
        bool itemResult = false;

        if (sacochesInventory.Contains(item))
        {
            itemResult = true;
        }

        return itemResult;
    }

    public bool UseItemFromSacoche(ItemInventaire item)
    {
        bool response = false;

        if (item == null)
        {
            return response;
        }
        if (sacochesInventory.Contains(item))
        {
            item.Use();
            // A tester : Inutile, déjà fait dans le item.Use() : 
            item.DecreaseAmount(1);
            sacochesInventory.Remove(item);
            sessionAddedItem.Remove(item);
            response = true;
        }
        

        

        return response;

    }

    // Lorsque j'ajoute un item à l'inventaire (DropSlots.cs) je remlace List.Add(item) par cette fonction
    // pour enregistrer les items de la session et pouvoir les supprimer si le player meurt
    public void AddItem(string typeItem, ItemInventaire item)
    {
        switch (typeItem)
        {
            case "ObjetQuete":
                //SaveItemSession(item);
                objetsQuetesInventory.Add(item);
                break;
            default:
                break;
        }
    }

    // Si le player meurt, je supprime tous les items de la session.
    // Appelé par InventaireSauvegarde.RemoveItemSession()au moment de DeathZoneController.ReLoadCheckpoint()
    public void RemoveItemSession()
    {
        foreach (var itemInventaire in sessionAddedItem)
        {
            itemInventaire.DecreaseAmount(1);
            if (objetsQuetesInventory.Contains(itemInventaire) && itemInventaire.numberHeld == 0) objetsQuetesInventory.Remove(itemInventaire);

        }
        sessionAddedItem.Clear();
    }

    // J'enregistre les items récupérée pdt le level sur la session
    // TODO : à la fin du level vider la liste sessionAddedItem
    private void SaveItemSession(ItemInventaire item)
    {
        if (item.typeItem == "ObjetQuete" && sessionAddedItem.Contains(item))
        {
            return;
        }
        sessionAddedItem.Add(item);
    }

    
}