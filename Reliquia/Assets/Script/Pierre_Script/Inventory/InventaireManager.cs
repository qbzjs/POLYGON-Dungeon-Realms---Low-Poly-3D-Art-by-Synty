using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventaireManager : MonoBehaviour
{
    public static InventaireManager instance;

    [Header("Inventaire informations")]
    private Inventory playerInventory;
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

    public ItemInventory currentItem;

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
        // Chargement de l'inventaire du joueur
        playerInventory = William_Script.instance.Inventory;

        InventaireSauvegarde.instance.LoadInventory();

        MakeSlots(ItemAsset.Type.Sacoche);
        MakeSlots(ItemAsset.Type.Consommable);
        MakeSlots(ItemAsset.Type.Quete);
        MakeSlots(ItemAsset.Type.Puzzle);
    }

    #region MakeInventory

    public void MakeSlots(ItemAsset.Type TypeItem)
    {
        List<ItemInventory> Items = new List<ItemInventory>();
        GameObject temp, Panel = null;
        InventaireSlot newSlot;


        switch (TypeItem)
        {
            case ItemAsset.Type.Sacoche:
                Items = playerInventory.sacoche;
                Panel = sacochePanel;
                break;
            case ItemAsset.Type.Quete:
                Items = playerInventory.objetsQuetes;
                Panel = objetQuetePanel;
                break;
            case ItemAsset.Type.Consommable:
                Items = playerInventory.consommables;
                Panel = consommablePanel;
                break;
            case ItemAsset.Type.Puzzle:
                Items = playerInventory.puzzles;
                Panel = PuzzlePanel;
                break;
            default:
                Debug.Log(TypeItem + " not implemented");
                return;

        }

        foreach (ItemInventory Item in Items)
        {
            temp = Instantiate(blankInventtaireSlot, Panel.transform.localPosition, Quaternion.identity);
            if(!TypeItem.Equals(ItemAsset.Type.Sacoche))
                temp.transform.SetParent(Panel.transform);
            temp.transform.localScale = new Vector3(scaleItemValeur, scaleItemValeur, scaleItemValeur);

            if(Panel == PuzzlePanel) Destroy(temp.GetComponent<DragDrop>());
            newSlot = temp.GetComponent<InventaireSlot>();

            if (newSlot) newSlot.Setup(Item, this);
        }
    }

    public void ClearSlots(ItemAsset.Type TypeItem)
    {
        switch(TypeItem)
        {
            case ItemAsset.Type.Sacoche:
                for (int i = 0; i < playerInventory.sacoche.Count; i++)
                    Destroy(sacochePanel.transform.GetChild(i).gameObject);
                break;

            case ItemAsset.Type.Quete:
                for (int i = 0; i < playerInventory.objetsQuetes.Count; i++)
                    Destroy(objetQuetePanel.transform.GetChild(i).gameObject);
                break;

            case ItemAsset.Type.Consommable:
                for (int i = 0; i < playerInventory.consommables.Count; i++)
                    Destroy(consommablePanel.transform.GetChild(i).gameObject);
                break;

            case ItemAsset.Type.Puzzle:
                for (int i = 0; i < playerInventory.puzzles.Count; i++)
                    Destroy(PuzzlePanel.transform.GetChild(i).gameObject);
                break;

            default:
                Debug.Log(TypeItem + " not implemented");
                break;
        }
    }
    #endregion

    #region Setup
    public void SetupDescriptionAndButton(string newDescription, bool isButtonUsable, ItemInventory newItem)
    {
        currentItem = newItem;
        descriptionText.text = newDescription;
    }
    #endregion

    public void RemoveItemFromSacoche(ItemInventory item)
    {
        if (item != null)
        {
            int count = sacochePanel.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject tempItem = sacochePanel.transform.GetChild(i).gameObject;
                if (tempItem.GetComponent<InventaireSlot>().Item == item )
                {
                    item.typeItem = item.asset.typeItemBase;
                    Destroy(tempItem);
                }
            }
        }
    }
}
