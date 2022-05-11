using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPouvoir
{
    /// <summary>
    /// Récupérer la valeur du temps de recharge.
    /// </summary>
    /// <returns>Temps de recharge actuel</returns>
    float GetTempsRechargeActuel();
    /// <summary>
    /// Affecter la valeur bool pour lancer le pouvoir.
    /// </summary>
    /// <param name="input">Valeur de l'input</param>
    void SetInputPouvoir(bool input);
    /// <summary>
    /// Récupérer les données du pouvoir.
    /// </summary>
    /// <returns></returns>
    PouvoirDonnees GetPouvoirDonnees();
    /// <summary>
    /// Récupérer l'UnityEvent quand le pouvoir est lancé.
    /// </summary>
    /// <returns></returns>
    UnityEvent GetOnEnterAbilityEvent();
}
