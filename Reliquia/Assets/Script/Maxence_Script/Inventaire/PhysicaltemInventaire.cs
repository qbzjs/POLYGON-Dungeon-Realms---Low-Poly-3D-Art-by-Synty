using clavier;
using System;
using AlexandreDialogues;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Image))]
public class PhysicaltemInventaire : MonoBehaviour,IInteractable
{
    [SerializeField] private PlayerInventory playerInventory; // private ? 
    public GameObject inGameDialogueGameObject;
    private Outline _outline = null;
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
        if (_outline == null)
        {
            _outline = GetComponent<Outline>();
        }
        _outline.enabled = false;
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
                //item.numberHeld++;
                item.numberHeld = 1;
            }

            //playerInventory.objetsQuetesInventory.Add(item);
            playerInventory.AddItem("ObjetQuete", item);

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
    public void Interaction()
    {
        if (inGameDialogueGameObject != null)
        {
            InGameDialogue inGameDialogue = inGameDialogueGameObject.GetComponent<DialogueAttached>().inGameDialogue;
            InGameDialogueManager.Instance.StartDialogue(inGameDialogue);
        }
        AddItem(thisItem);
    }

    public void MontrerOutline(bool affichage)
    {
        _outline.enabled = affichage;
        if (_outline.enabled)
        {
            AfficherMessageInteraction();
        }
    }
    private void AfficherMessageInteraction()
    {
        GameManager.instance.AfficherMessageInteraction($"Appuyer sur {William_Script.instance.PlayerInput.actions["Interaction"].GetBindingDisplayString()} pour ramasser.");
    }
}
