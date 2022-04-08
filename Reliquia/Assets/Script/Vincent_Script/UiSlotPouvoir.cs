using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSlotPouvoir : MonoBehaviour
{
    public int slotIndex = 0;
    public IPouvoir Pouvoir;
    public Image pouvoirBackgroundImage;
    public Image pouvoirDisponibleImage;
    private void Start()
    {
        InitialiserSlotPouvoirUI();
    }
    private void InitialiserSlotPouvoirUI()
    {
        Pouvoir = William_Script.instance.ListePouvoirs[slotIndex];
        pouvoirBackgroundImage.sprite = Pouvoir.GetPouvoirDonnees().ImageVide;
        pouvoirDisponibleImage.sprite = Pouvoir.GetPouvoirDonnees().ImagePlein;
        Pouvoir.GetOnEnterAbilityEvent().AddListener(CouroutineRefreshSlot);
    }
    private void CouroutineRefreshSlot()
    {
        StartCoroutine(RefreshSlot());
    }
    private IEnumerator RefreshSlot()
    {
        while (pouvoirDisponibleImage.fillAmount <= 1)
        {
            pouvoirDisponibleImage.fillAmount = Pouvoir.GetTempsRechargeActuel() / Pouvoir.GetPouvoirDonnees().TempsRecharge;
            yield return null;
        }
    }
}
