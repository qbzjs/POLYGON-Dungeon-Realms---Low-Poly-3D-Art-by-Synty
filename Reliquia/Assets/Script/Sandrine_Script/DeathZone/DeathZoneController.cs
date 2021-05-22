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

            StartCoroutine(PlayerDeath());
            

        }
    }

    private IEnumerator PlayerDeath()
    {
        if (playerAnim == null)
        {
            yield return null;
        }

        playerAnim.SetBool("Accroupissement", true);
        yield return new WaitForSeconds(0.5f);

        ReLoadCheckpoint();
    }

    private void ReLoadCheckpoint()
    {
        string nom = GameManager.instance.nomSauvegarde;
        SaveManager.instance.LoadInGame(nom);
    }


}
