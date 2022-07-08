using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InventaireSlot : MonoBehaviour
{
    [Header("Affichage infos Item")]
    [SerializeField] private Text itemNumberText;
    [SerializeField] private Image itemImage;

    [Header("Variable infos Item")]
    public ItemInventory Item;
    public InventaireManager Manager;

    public ItemAsset.Type TypeItemBase;
    public ItemAsset.Type TypeItem;

    private void Start()
    {
        Manager = InventaireManager.instance;
    }

    public void Setup(ItemInventory Item, InventaireManager Manager)
    {
        this.Item = Item;
        this.Manager = Manager;

        if (this.Item != null)
        {
            itemImage.sprite = this.Item.asset.itemImage;
            itemNumberText.text = this.Item.amount.ToString();
            TypeItemBase = TypeItem = this.Item.asset.typeItemBase;
        }
    }
}
