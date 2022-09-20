using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DiasGames.ThirdPersonSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

public class Checkpoint : MonoBehaviour
{
    private Transform playerSpawn;

    private void Awake() {
        playerSpawn = GameObject.FindGameObjectWithTag("PlayerSpawn").transform;
    }

    private void OnTriggerEnter(Collider collision) {
        if(collision.CompareTag("Player")){
            playerSpawn.position = transform.position;
            Destroy(gameObject);
        }
        
    }

    //créer script qui prend en compte la variable Die de ThirdPerson System
    

    
}
