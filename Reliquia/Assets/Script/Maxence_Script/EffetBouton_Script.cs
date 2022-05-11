using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EffetBouton_Script : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private bool changementCouleurText = true;
    Scene scene;

    void Awake()
    {
        scene = SceneManager.GetActiveScene();
    }
    private void OnEnable()
    {
        gameObject.GetComponentInChildren<Text>().color = Color.white;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (changementCouleurText)
        {
            if(gameObject.tag == "BoutonMenu") gameObject.GetComponentInChildren<Text>().color = new Color32(105,105,105,255);
            else gameObject.GetComponentInChildren<Text>().color = new Color32(198, 150, 82, 255);
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (changementCouleurText) gameObject.GetComponentInChildren<Text>().color = Color.white;
    }
}
