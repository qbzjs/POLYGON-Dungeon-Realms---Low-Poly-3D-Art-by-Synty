using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GestionDropItem_Script : MonoBehaviour, IDropHandler
{
    public Inventaire_Script inventaire;
    public IInventaireItem item { get; set; }

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;

        if(!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            Debug.Log("Item Drop");
        }
    }
}
