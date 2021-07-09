using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZoneController : MonoBehaviour
{
    private Animator playerAnim;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Animator>() != null)
        {
            playerAnim = other.GetComponent<Animator>();

            PlayerDeath();



        }
    }

    private void PlayerDeath()
    {
        if (playerAnim == null)
        {
            return;
        }

        playerAnim.SetBool("Trappe", true);
        StartCoroutine(ReLoad());

        
    }

    private IEnumerator ReLoad()
    {
        
        yield return new WaitForSeconds(3f);
        playerAnim.SetBool("Trappe", false);
        ReLoadCheckpoint();
    }

    private void ReLoadCheckpoint()
    {
        string nom = GameManager.instance.nomSauvegarde;
        if (nom != null && SaveManager.instance != null)
        {
            bool retrunToCheckPoint = true;
            SaveManager.instance.LoadInGame(nom, retrunToCheckPoint);
        }
        
    }


}
