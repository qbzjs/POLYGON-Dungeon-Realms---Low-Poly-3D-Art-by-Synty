using DiasGames.ThirdPersonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PouvoirPraesidium : ThirdPersonAbility,IPouvoir
{
    [SerializeField]
    private PouvoirDonnees _donneesPraesidium;
    [SerializeField]
    private GameObject _particulesPouvoir;
    public bool PouvoirInput = false;
    public bool PraesidiumActif = false;
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
        if (m_System.IsGrounded && PouvoirInput && _estDisponible && William_Script.instance.Mana.ManaValue >= _donneesPraesidium.CoutMana)
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
        GlobalEvents.ExecuteEvent("ManaDamage", gameObject, _donneesPraesidium.CoutMana);
        StartCoroutine(TempsRechargeDemarrer());
        StartCoroutine(VerifierBouclierActif());
        SoundManager.instance.Play("SfxPouvoir2");
    }
    /// <summary>
    /// A la sortie du pouvoir.
    /// </summary>
    public override void OnExitAbility()
    {
        base.OnExitAbility();
    }
    /// <summary>
    /// Pour l'AnimEvent de l'animation de Pulsate.
    /// </summary>
    public void PraesidiumAnimEvent()
    {
        _particulesPouvoir.SetActive(true);
    }
    /// <summary>
    /// Lancer le temps de recharge de Pulsate.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TempsRechargeDemarrer()
    {
        _estDisponible = false;
        _tempsRechargeActuel = 0;
        while (_tempsRechargeActuel <= _donneesPraesidium.TempsRecharge)
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
    private IEnumerator VerifierBouclierActif()
    {
        float dureeBouclier = 0;
        PraesidiumActif = true;
        while (PraesidiumActif)
        {
            dureeBouclier += Time.deltaTime;
            if (dureeBouclier >= _donneesPraesidium.DureePouvoir)
            {
                PraesidiumActif = false;
            }
            yield return null;
        }
        _particulesPouvoir.SetActive(false);
    }
    public UnityEvent GetOnEnterAbilityEvent()
    {
        return OnEnterAbilityEvent;
    }

    public PouvoirDonnees GetPouvoirDonnees()
    {
        return _donneesPraesidium;
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
