using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TestNiveau : MonoBehaviour
{
    public string NomDeScene;

    public void AllerAuNiveau(){
        SceneManager.LoadScene(NomDeScene);
    }

    private void OnTriggerEnter(Collider other) {
        AllerAuNiveau();
    }
}
