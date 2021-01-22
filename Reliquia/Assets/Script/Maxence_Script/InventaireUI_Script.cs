using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventaireUI_Script : MonoBehaviour
{
    public Inventaire_Script inventaire;

    // Start is called before the first frame update
    void Start()
    {
        /*inventaire.ItemAdded += InventaireScript_ItemAdded;
        inventaire.ItemRemoved += InventaireScript_ItemRemoved;*/
    }

    private void InventaireScript_ItemAdded(object sender, InventaireEventArgs e)
    {
        Transform inventairePanel = gameObject.transform;
        foreach(Transform slot in inventairePanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            GestionDragItem_Script gestionDragItem = imageTransform.GetComponent<GestionDragItem_Script>();
            
            if (!image.enabled)
            {
                image.enabled = true;
                image.sprite = e.Item.Image;

                gestionDragItem.item = e.Item;

                break;
            }
        }
    }

    private void InventaireScript_ItemRemoved(object sender, InventaireEventArgs e)
    {
        Transform inventairePanel = gameObject.transform;
        foreach (Transform slot in inventairePanel)
        {
            Transform imageTransform = slot.GetChild(0).GetChild(0);
            Image image = imageTransform.GetComponent<Image>();
            GestionDragItem_Script gestionDragItem = imageTransform.GetComponent<GestionDragItem_Script>();

            if (gestionDragItem.item.Equals(e.Item))
            {
                image.enabled = false;
                image.sprite = null;
                gestionDragItem.item = null;

                break;
            }
        }
    }
}
