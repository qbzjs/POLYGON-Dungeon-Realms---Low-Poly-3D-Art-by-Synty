using DiasGames.ThirdPersonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionTorche : MonoBehaviour,IInteractable
{
    private Outline _outline;
    [SerializeField]
    private GameObject[] _gameObjectsTorches;
    [SerializeField]
    private bool _estAllume = false;
    [SerializeField]
    private float _coutAllumerTorche = 3.0f;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    private void AllumerTorche()
    {
        if (!_estAllume)
        {
            _estAllume = true;
            for (int indexGameObjects = 0; indexGameObjects < _gameObjectsTorches.Length; indexGameObjects++)
            {
                _gameObjectsTorches[indexGameObjects].SetActive(true);
            }
        }
    }

    public void Interaction()
    {
        if (PouvoirEstActif() && !_estAllume && VerifierMana())
        {
            AllumerTorche();
            GlobalEvents.ExecuteEvent("ManaDamage", GameObject.FindGameObjectWithTag("Player"), 3.0f);
            GameManager.instance.FermerMessageInteraction();
        }      
    }
    private bool PouvoirEstActif()
    {
        return William_Script.instance.ThirdPersonSystem.ActiveAbility is PouvoirLighting;
    }

    private bool VerifierMana()
    {
        return William_Script.instance.Mana.ManaValue >= _coutAllumerTorche;
    }

    public void MontrerOutline(bool affichage)
    {
        if (!_estAllume)
        {
            _outline.enabled = affichage;
        }
        else
        {
            _outline.enabled = false;
        }

        AfficherMessageInteraction();
    }

    private void AfficherMessageInteraction()
    {
        if (_outline.enabled)
        {
            if (William_Script.instance.ThirdPersonSystem.ActiveAbility is PouvoirLighting)
            {
                GameManager.instance.AfficherMessageInteraction($"Appuyer sur {William_Script.instance.PlayerInput.actions["Interaction"].GetBindingDisplayString()} pour allumer.");
            }
            else
            {
                GameManager.instance.AfficherMessageInteraction("Utiliser Lighting");
            }
        }
    }
}
