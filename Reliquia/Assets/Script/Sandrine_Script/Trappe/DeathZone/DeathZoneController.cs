using System.Collections;
using System.Collections.Generic;
using DiasGames.ThirdPersonSystem;
using UnityEngine;

public class DeathZoneController : MonoBehaviour
{
    private Animator playerAnim;
    private Health _thirdPersonSystem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Animator>() != null)
        {
            playerAnim = other.GetComponent<Animator>();

            PlayerDeath();
            _thirdPersonSystem = other.GetComponent<Health>();
            _thirdPersonSystem.Die();


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

        float countWait = 5f;
        yield return new WaitForSeconds(countWait);
        playerAnim.SetBool("Trappe", false);
        ReLoadCheckpoint();
    }

    private void ReLoadCheckpoint()
    {
        string nom = GameManager.instance.nomSauvegarde;
        if (nom != null && SaveManager.instance != null)
        {
            bool retrunToCheckPoint = true;
            //InventaireSauvegarde.instance.ResetSessionInventaire();
            SaveManager.instance.LoadInGame(nom, retrunToCheckPoint);
        }
        
    }


}
