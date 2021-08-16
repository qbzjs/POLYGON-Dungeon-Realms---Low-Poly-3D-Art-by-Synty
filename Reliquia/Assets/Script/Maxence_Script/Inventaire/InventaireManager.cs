using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventaireManager : MonoBehaviour
{
    public static InventaireManager instance;

    [Header("Inventaire informations")]
    public PlayerInventory playerInventory;
    [SerializeField] private GameObject blankInventtaireSlot;

    public GameObject sacochePanel;
    public GameObject consommablePanel;
    public GameObject objetQuetePanel;
    public GameObject PuzzlePanel;

    [SerializeField] private Text descriptionText;
    [SerializeField] private float scaleItemValeur;

    public int maxItemQuete;
    public int maxItemConsommable ;
    public int maxItemPuzzles;
    public int maxItemSacoche;

    public ItemInventaire currentItem;

    private void Awake()
    {
        //InventaireSauvegarde.instance.LoadInventory();
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        InventaireSauvegarde.instance.LoadInventory();

        MakeSacocheSlots();
        MakeConsommableSlot();
        MakeObjetQueteSlot();
        MakePuzzlesSlot();
    }

    #region MakeInventory
    public void MakeSacocheSlots()
    {
        if (playerInventory)
        {
            for(int i = 0; i < playerInventory.sacochesInventory.Count; i++)
            {
                if (playerInventory.sacochesInventory[i].numberHeld > 0 || playerInventory.sacochesInventory[i].itemNom == "Bottle")
                {
                    GameObject temp = Instantiate(blankInventtaireSlot, sacochePanel.transform.localPosition, Quaternion.identity);
                    temp.transform.SetParent(sacochePanel.transform);
                    temp.transform.localScale = new Vector3(scaleItemValeur, scaleItemValeur, scaleItemValeur);

                    InventaireSlot newSlot = temp.GetComponent<InventaireSlot>();

                    if (newSlot) newSlot.Setup(playerInventory.sacochesInventory[i], this);
                }
            }
        }
    }

    public void MakeConsommableSlot()
    {
        if (playerInventory)
        {
            for (int i = 0; i < playerInventory.consommablesInventory.Count; i++)
            {
                if (playerInventory.consommablesInventory[i].numberHeld > 0 || playerInventory.consommablesInventory[i].itemNom == "Bottle")
                {
                    GameObject temp = Instantiate(blankInventtaireSlot, consommablePanel.transform.localPosition, Quaternion.identity);
                    temp.transform.SetParent(consommablePanel.transform);
                    temp.transform.localScale = new Vector3(scaleItemValeur, scaleItemValeur, scaleItemValeur);

                    InventaireSlot newSlot = temp.GetComponent<InventaireSlot>();

                    if (newSlot) newSlot.Setup(playerInventory.consommablesInventory[i], this);
                }
            }
        }
    }
    public void MakeObjetQueteSlot()
    {
        if (playerInventory)
        {
            for (int i = 0; i < playerInventory.objetsQuetesInventory.Count; i++)
            {
                if (playerInventory.objetsQuetesInventory[i].numberHeld > 0 || playerInventory.objetsQuetesInventory[i].itemNom == "Bottle")
                {
                    GameObject temp = Instantiate(blankInventtaireSlot, objetQuetePanel.transform.localPosition, Quaternion.identity);
                    temp.transform.SetParent(objetQuetePanel.transform);
                    temp.transform.localScale = new Vector3(scaleItemValeur, scaleItemValeur, scaleItemValeur);

                    InventaireSlot newSlot = temp.GetComponent<InventaireSlot>();

                    if (newSlot) newSlot.Setup(playerInventory.objetsQuetesInventory[i], this);
                }
            }
        }
    }
    public void MakePuzzlesSlot()
    {
        if (playerInventory)
        {
            for (int i = 0; i < playerInventory.puzzlesInventory.Count; i++)
            {
                if (playerInventory.puzzlesInventory[i].numberHeld > 0 || playerInventory.puzzlesInventory[i].itemNom == "Bottle")
                {
                    GameObject temp = Instantiate(blankInventtaireSlot, PuzzlePanel.transform.localPosition, Quaternion.identity);
                    temp.transform.SetParent(PuzzlePanel.transform);
                    temp.transform.localScale = new Vector3(scaleItemValeur, scaleItemValeur, scaleItemValeur);

                    Destroy(temp.GetComponent<DragDrop>());

                    InventaireSlot newSlot = temp.GetComponent<InventaireSlot>();

                    if (newSlot) newSlot.Setup(playerInventory.puzzlesInventory[i], this);
                }
            }
        }
    }
    #endregion

    #region Setup
    public void SetupDescriptionAndButton(string newDescription, bool isButtonUsable, ItemInventaire newItem)
    {
        currentItem = newItem;
        descriptionText.text = newDescription;
    }

    public void SetTextAndButton(string description, bool buttonActive)
    {
        descriptionText.text = description;
    }
    #endregion

    #region ClearInventaires
    public void ClearInventorySlots()
    {
        for (int i = 0; i < playerInventory.sacochesInventory.Count; i++)
        {
            Destroy(sacochePanel.transform.GetChild(i).gameObject);
        }
    }
    public void ClearConsommableSlots()
    {
        for (int i = 0; i < playerInventory.consommablesInventory.Count; i++)
        {
            Destroy(consommablePanel.transform.GetChild(i).gameObject);
        }
    }
    public void ClearObjetQuetesSlots()
    {
        for (int i = 0; i < playerInventory.objetsQuetesInventory.Count; i++)
        {
            Destroy(objetQuetePanel.transform.GetChild(i).gameObject);
        }
    }
    public void ClearPuzzlesSlots()
    {
        for (int i = 0; i < playerInventory.puzzlesInventory.Count; i++)
        {
            Destroy(PuzzlePanel.transform.GetChild(i).gameObject);
        }
    }
    #endregion

    public void RemoveItemFromSacoche(ItemInventaire item)
    {
        if (item != null)
        {
            int count = sacochePanel.transform.GetChildCount();
            for (int i = 0; i < count; i++)
            {
                GameObject tempItem = sacochePanel.transform.GetChild(i).gameObject;
                if (tempItem.GetComponent<InventaireSlot>().thisItem == item )
                {
                    item.typeItem = item.typeItemBase;
                    Destroy(tempItem);
                }
            }
        }
    }
}
