using DiasGames.ThirdPersonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PouvoirPulsate : ThirdPersonAbility,IPouvoir
{
    [SerializeField]
    private PouvoirDonnees _donneesPulsate;
    [SerializeField]
    private GameObject _particulesBras;
    [SerializeField]
    private GameObject _particulesPouvoir;
    public bool PouvoirInput = false;
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
        _particulesBras.SetActive(false);
        _particulesPouvoir.SetActive(false);
    }
    /// <summary>
    /// Condition pour lancer le pouvoir.
    /// </summary>
    /// <returns></returns>
    public override bool TryEnterAbility()
    {
        if (m_System.IsGrounded && PouvoirInput && _estDisponible && William_Script.instance.Mana.ManaValue >= _donneesPulsate.CoutMana)
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
        if (!m_System.IsGrounded)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// Quand le pouvoir est lancer.
    /// </summary>
    public override void OnEnterAbility()
    {
        base.OnEnterAbility();
        GlobalEvents.ExecuteEvent("ManaDamage", gameObject,_donneesPulsate.CoutMana);
        _particulesBras.SetActive(true);
        StartCoroutine(TempsRechargeDemarrer());
        SoundManager.instance.Play("SfxPouvoir1");
    }
    /// <summary>
    /// A la sortie du pouvoir.
    /// </summary>
    public override void OnExitAbility()
    {
        base.OnExitAbility();
        _particulesBras.SetActive(false);
        _particulesPouvoir.SetActive(false);
    }
    /// <summary>
    /// Pour l'AnimEvent de l'animation de Pulsate.
    /// </summary>
    public void PulsateAnimEvent()
    {
        _particulesPouvoir.SetActive(true);
        if (William_Script.instance.InteractableObject.TryGetComponent(out InteractionDestructible interactionDestructible))
        {
            interactionDestructible.Interaction();
        }
    }
    /// <summary>
    /// Lancer le temps de recharge de Pulsate.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TempsRechargeDemarrer()
    {
        _estDisponible = false;
        _tempsRechargeActuel = 0;
        while (_tempsRechargeActuel <= _donneesPulsate.TempsRecharge)
        {
            _tempsRechargeActuel += Time.deltaTime;
            yield return null;
        }
        _estDisponible = true;
    }
    public UnityEvent GetOnEnterAbilityEvent()
    {
        return OnEnterAbilityEvent;
    }

    public PouvoirDonnees GetPouvoirDonnees()
    {
        return _donneesPulsate;
    }

    public float GetTempsRechargeActuel()
    {
        return _tempsRechargeActuel;
    }

    public void SetInputPouvoir(bool input)
    {
        PouvoirInput = input;
    }

}
