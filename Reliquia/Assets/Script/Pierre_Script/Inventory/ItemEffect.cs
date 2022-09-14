using DiasGames.ThirdPersonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ItemEffect : MonoBehaviour
{
    private Health Health;
    private William_Script William;

    public static bool used = false;

    public void Bandage()
    {
        William = William_Script.instance;
        Health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();

        if (Health.HealthValue < Health.MaximumHealth)
        {
            GlobalEvents.ExecuteEvent("RestoreHealth", William_Script.instance.gameObject, 10.0f);
            used = true;
        }
    }

    public void Trousse()
    {
        William = William_Script.instance;
        Health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();

        if (Health.HealthValue < Health.MaximumHealth)
        {
            GlobalEvents.ExecuteEvent("RestoreHealth", William_Script.instance.gameObject, 20.0f);
            used = true;
        }
    }

}
