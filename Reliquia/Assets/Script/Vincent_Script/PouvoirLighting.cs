using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.ThirdPersonSystem;
using UnityEngine.Events;

public class PouvoirLighting : ThirdPersonAbility,IPouvoir
{
    [SerializeField] 
    private PhysicMaterial m_IdlePhysicMaterial = null;
    [SerializeField]
    private PouvoirDonnees _donneesLighting;
    [SerializeField]
    private PouvoirDonnees _donneesLightingOffensif;
    [SerializeField]
    private string _animationLightingOffensif = "Lighting_Offensif";
    [SerializeField]
    private GameObject _lightingOrbe;
    [SerializeField]
    private GameObject _viseur;
    [SerializeField]
    private GameObject _lightingPrefabProjectile;
    public bool PouvoirInput = false;
    public bool _estDisponible = true;
    public bool _tirDisponible = true;
    public bool ModeOffensif = false;
    private float _tempsRechargeActuel;
    private Coroutine _coroutineDrainMana;
    public LayerMask layerMask;
    public RaycastHit cibleLighting;
    /// <summary>
    /// Initialisation de l'habilité en rajoutant l'initialisation des variables spécifiques a cette habilité.
    /// </summary>
    /// <param name="mainSystem"></param>
    /// <param name="animatorManager"></param>
    /// <param name="inputManager"></param>
    public override void Initialize(ThirdPersonSystem mainSystem, AnimatorManager animatorManager, UnityInputManager inputManager)
    {
        base.Initialize(mainSystem, animatorManager, inputManager);
        _viseur = GameObject.FindGameObjectWithTag("Viseur");
        _lightingOrbe.SetActive(false);
        _viseur.SetActive(false);
    }
    /// <summary>
    /// Condition pour lancer le pouvoir.
    /// </summary>
    /// <returns></returns>
    public override bool TryEnterAbility()
    {
        if (m_System.IsGrounded && PouvoirInput && _estDisponible && William_Script.instance.Mana.ManaValue >= _donneesLighting.DrainManaParSeconde)
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
        if (PouvoirInput | !m_System.IsGrounded | William_Script.instance.Mana.ManaValue < _donneesLighting.DrainManaParSeconde)
        {
            PouvoirInput = false;
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
        _lightingOrbe.SetActive(true);
        StartCoroutine(TempsRechargeDemarrer());
        _coroutineDrainMana = StartCoroutine(DrainMana());
    }
    /// <summary>
    /// A la sortie du pouvoir.
    /// </summary>
    public override void OnExitAbility()
    {
        base.OnExitAbility();
        _lightingOrbe.SetActive(false);
        if (_viseur.activeSelf)
        {
            _viseur.SetActive(false);
        }
        StopCoroutine(_coroutineDrainMana);
    }
    /// <summary>
    /// FixedUpdate du pouvoir pour le mode offensif.
    /// </summary>
    public override void FixedUpdateAbility()
    {
        //UpdatePhysicMaterial();
        m_System.CalculateMoveVars();
        m_System.UpdateMovementAnimator(0.1f);

        if (William_Script.instance.BoutonGarde)
        {
            ModeOffensif = true;

            // Sert a forcer la direction du personnage dans le même sens que la caméra.
            m_System.RotateByCamera();

            _viseur.SetActive(true);

            // Sert a changer l'animation en cours et donc changer la caméra et la mettre en mode "Zoom".
            SetState(_animationLightingOffensif);

            if (William_Script.instance.BoutonAttaquer && _tirDisponible && William_Script.instance.Mana.ManaValue >= _donneesLightingOffensif.CoutMana)
            {
                William_Script.instance.BoutonAttaquer = false;
                GlobalEvents.ExecuteEvent("ManaDamage", gameObject, _donneesLightingOffensif.CoutMana);
                StartCoroutine(TempsRechargeTirDemarrer());

                //Pour que le projectile aille dans la direction visée, si le projectile ne touche pas de cible il va dans la direction du centre de la caméra.
                Vector2 centreEcran = new Vector2(Screen.width / 2.0f, Screen.height / 2.0f);
                Ray ray = Camera.main.ScreenPointToRay(centreEcran);

                GameObject projectile;
                if (Physics.Raycast(ray, out cibleLighting, 999f, layerMask, QueryTriggerInteraction.Ignore))
                {
                    Vector3 directionVisee = (cibleLighting.point - _lightingOrbe.transform.position).normalized;
                    projectile = Instantiate(_lightingPrefabProjectile, _lightingOrbe.transform.position, Quaternion.LookRotation(directionVisee, Vector3.up));
                }
                else
                {
                    projectile = Instantiate(_lightingPrefabProjectile, _lightingOrbe.transform.position, Quaternion.LookRotation(ray.direction, Vector3.up));
                }
                projectile.GetComponent<LightingProjectileBehaviour>().owner = William_Script.instance;
            }
        }
        else
        {
            ModeOffensif = false;
            _viseur.SetActive(false);

            // Sert a remettre l'animation d'origine et donc le déplacement a 360°.
            SetState(m_EnterState);
            m_System.RotateToDirection();
        }
    }
    /// <summary>
    /// Lancer le temps de recharge de Lighting.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TempsRechargeDemarrer()
    {
        _estDisponible = false;
        _tempsRechargeActuel = 0;
        while (_tempsRechargeActuel <= _donneesLighting.TempsRecharge)
        {
            _tempsRechargeActuel += Time.deltaTime;
            yield return null;
        }
        _estDisponible = true;
    }
    /// <summary>
    /// Drainer la mana toutes les secondes.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DrainMana()
    {
        while (true)
        {
            GlobalEvents.ExecuteEvent("ManaDamage", gameObject, _donneesLighting.DrainManaParSeconde);
            yield return new WaitForSeconds(1.0f);
        }
    }
    /// <summary>
    /// Récupérer de FreeLocomotion.
    /// </summary>
    private void UpdatePhysicMaterial()
    {
        if (Mathf.Approximately(m_System.FreeMoveDirection.magnitude, 0))
            m_System.m_Capsule.sharedMaterial = m_IdlePhysicMaterial;
        else
            m_System.m_Capsule.sharedMaterial = m_AbilityPhysicMaterial;
    }
    /// <summary>
    /// Lancer de temps de recharge du tir.
    /// </summary>
    /// <returns></returns>
    public IEnumerator TempsRechargeTirDemarrer()
    {
        _tirDisponible = false;
        yield return new WaitForSeconds(_donneesLightingOffensif.TempsRecharge);
        _tirDisponible = true;
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
        return _donneesLighting;
    }

    public UnityEvent GetOnEnterAbilityEvent()
    {
        return OnEnterAbilityEvent;
    }
}
