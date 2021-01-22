using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject tutorialReference;
    private bool stays = false;
    private bool enterTrigger = false;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stays = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialReference.SetActive(true);
            enterTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialReference.SetActive(false);
            enterTrigger = false;
        }
    }

    private void FixedUpdate()
    {
        if (!stays && enterTrigger)
        {
            tutorialReference.SetActive(false);
            enterTrigger = false;
        }

        stays = false;
    }
}
