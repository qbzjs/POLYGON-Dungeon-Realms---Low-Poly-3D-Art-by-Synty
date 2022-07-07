using clavier;
using System;
using AlexandreDialogues;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Image))]
public class PhysicaltemInventaire : MonoBehaviour,IInteractable
{
    private Inventory playerInventory; 
    public GameObject inGameDialogueGameObject;
    private Outline _outline = null;

    public ItemAsset itemAsset;
    
    public int amount = 1;
    private ItemInventory Item;

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
        itemImage.sprite  = itemAsset.itemImage;
        if (_outline == null)
        {
            _outline = GetComponent<Outline>();
        }
        _outline.enabled = false;

        Item = new ItemInventory(itemAsset, amount);
    }

    private void Start()
    {
        playerInventory = William_Script.instance.Inventory;
    }
    public void Interaction()
    {
        if (inGameDialogueGameObject != null)
        {
            InGameDialogue inGameDialogue = inGameDialogueGameObject.GetComponent<DialogueAttached>().inGameDialogue;
            InGameDialogueManager.Instance.StartDialogue(inGameDialogue);
        }
        playerInventory.AddItem(Item);
        Destroy(gameObject);
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
