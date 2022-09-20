using System.Collections;
using System.Collections.Generic;
using DiasGames.ThirdPersonSystem;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
    //Variable pour stocker PlayerSpawn
    private Transform playerSpawn;
    private Animator fadeSystem;

    private void Awake(){
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
        fadeSystem = GameObject.FindGameObjectWithTag("FadeSystem").GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collision) {
        if(collision.CompareTag("Player")){
            collision.transform.position = playerSpawn.position;
            StartCoroutine(ReplacePlayer(collision));
        }
    }

    private IEnumerator ReplacePlayer(Collider collision){
        fadeSystem.SetTrigger("Out");
        yield return new WaitForSeconds(0.5f);
        collision.transform.position = playerSpawn.position;
    }
}
