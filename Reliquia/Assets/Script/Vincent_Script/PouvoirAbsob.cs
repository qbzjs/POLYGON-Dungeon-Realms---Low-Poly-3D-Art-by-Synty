using DiasGames.ThirdPersonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PouvoirAbsob : ThirdPersonAbility,IPouvoir
{
    [SerializeField]
    private PouvoirDonnees _donneesAbsob;
    [SerializeField]
    private GameObject _particulesPouvoir;
    public bool PouvoirInput = false;
    public bool AbsobActif = false;
    private bool _estDisponible = true;
    private float _tempsRechargeActuel;
    /// <summary>
    /// Initialisation de l'habilité en rajoutant l'initialisation des variables spécifiques a cette habilité.
    /// </summary>
    /// <param name="mainSystem"></param>
    /// <param name="animatorManager"></param>
    /// <param name="inputManager"></param>
    public override void Initialize(ThirdPersonSystem mainSystem, AnimatorManager animatorManager, UnityInputManager inputManager)
    {
        base.Initialize(mainSystem, animatorManager, inputManager);
        _particulesPouvoir.SetActive(false);
    }
    /// <summary>
    /// Condition pour lancer le pouvoir.
    /// </summary>
    /// <returns></returns>
    public override bool TryEnterAbility()
    {
        if (PouvoirInput && _estDisponible && William_Script.instance.Mana.ManaValue >= _donneesAbsob.CoutMana)
        {
            PouvoirInput = false;
            return true;
        }
        return false;
    }
    /// <summary>
    /// Condition pour sortir du pouvoir.
    /// </summary>
    /// <returns></returns>
    public override bool TryExitAbility()
    {
        return true;
    }
    /// <summary>
    /// Quand le pouvoir est lancer.
    /// </summary>
    public override void OnEnterAbility()
    {
        base.OnEnterAbility();
        GlobalEvents.ExecuteEvent("ManaDamage", gameObject, _donneesAbsob.CoutMana);
        StartCoroutine(TempsRechargeDemarrer());
        StartCoroutine(VerifierAbsobActif());
    }
    /// <summary>
    /// A la sortie du pouvoir.
    /// </summary>
    public override void OnExitAbility()
    {
        base.OnExitAbility();
    }
    /// <summary>
    /// Lancer le temps de recharge de Absob.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TempsRechargeDemarrer()
    {
        _estDisponible = false;
        _tempsRechargeActuel = 0;
        while (_tempsRechargeActuel <= _donneesAbsob.TempsRecharge)
        {
            _tempsRechargeActuel += Time.deltaTime;
            yield return null;
        }
        _estDisponible = true;
    }
    /// <summary>
    /// Désaciver les particules quand le pouvoir n'est plus actif.
    /// </summary>
    /// <returns></returns>
    private IEnumerator VerifierAbsobActif()
    {
        float dureeAbsobActuelle = 0;
        AbsobActif = true;
        _particulesPouvoir.SetActive(true);
        while (AbsobActif)
        {
            dureeAbsobActuelle += Time.deltaTime;
            if (dureeAbsobActuelle >= _donneesAbsob.DureePouvoir)
            {
                AbsobActif = false;
            }
            yield return null;
        }
        _particulesPouvoir.SetActive(false);
    }

    public float GetTempsRechargeActuel()
    {
        return _tempsRechargeActuel;
    }

    public void SetInputPouvoir(bool input)
    {
        PouvoirInput = input;
    }

    public PouvoirDonnees GetPouvoirDonnees()
    {
        return _donneesAbsob;
    }

    public UnityEvent GetOnEnterAbilityEvent()
    {
        return OnEnterAbilityEvent;
    }
}
