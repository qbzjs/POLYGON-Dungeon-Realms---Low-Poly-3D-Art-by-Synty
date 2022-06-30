using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Transform startParent;
    private ItemInventory item;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        canvas = GameObject.FindGameObjectWithTag("HUD").GetComponent<Canvas>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startParent = transform.parent;
        item = eventData.pointerDrag.gameObject.GetComponent<InventaireSlot>().Item;
        item.isDropped = false;

        canvasGroup.DOFade(0.6f, 0.5f);
        canvasGroup.blocksRaycasts = false;
        eventData.pointerDrag.transform.SetParent(canvas.transform);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Si l'item est drop en dehors d'une zone autorisée, il revient à sa position de base
        if(item != null && !item.isDropped) transform.SetParent(startParent);

        canvasGroup.DOFade(1f, 0.5f);
        canvasGroup.blocksRaycasts = true;
    }
}
