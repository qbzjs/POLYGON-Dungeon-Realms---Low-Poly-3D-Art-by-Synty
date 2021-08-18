using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBrasier : MonoBehaviour
{
    public GameObject areaLight;

    private bool flagBrasierAlreadyOn;
    private GameManager gameManager;
    private float delay;

    private void OnEnable()
    {
        William_Script.INTERACT_ACTIONS += LightBrasier;
    }

    private void OnDisable()
    {
        William_Script.INTERACT_ACTIONS -= LightBrasier;
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    private void LightBrasier()
    {

        bool isLightPowerCreated = false;

        bool isOnlyOnceInteract = false;

        if (!flagBrasierAlreadyOn)
        {

            flagBrasierAlreadyOn = true;
            // light brasero

            ParticleSystem[] areaLights = areaLight.GetComponentsInChildren<ParticleSystem>();

            areaLight.SetActive(true);

            StartCoroutine(StartLight(areaLights));
            GetComponent<Outline>().OutlineWidth = 0;


        }
        else
        {

            gameManager.AfficherMessageInteraction("Active Light");
        }

    }


    public IEnumerator StartLight(ParticleSystem[] areaLights)
    {
        delay = 0.4f;
        for (int i = 0; i < areaLights.Length; i++)
        {
            yield return new WaitForSeconds(delay);
            areaLights[i].Play();
            delay -= 0.2f;
        }
    }
}
