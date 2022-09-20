using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TestNiveau : MonoBehaviour
{
    public string sceneName;

    public void AllerAuNiveau(){
        SceneManager.LoadScene(sceneName);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.transform.gameObject.tag == "Player"){
        AllerAuNiveau();
        }
    }

}

