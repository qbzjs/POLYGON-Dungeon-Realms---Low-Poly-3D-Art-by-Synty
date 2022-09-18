using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TransitionScene : MonoBehaviour
{
    Animator animator;
    public float TransitionTime = 0.5f;
    [SerializeField] int NextScene;

    //J'appel mon animation Fade In&Out
    private void Awake(){
        animator = GetComponent<Animator>();
    }

    //J'appel SceneManager et j'incrémente de 1 le buildIndex
    public void LoadNextScene(){
        NextScene = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(LoadScene(NextScene));
    }

    IEnumerator LoadScene(int buil_index){
        animator.SetTrigger("Out");
        yield return new WaitForSeconds(TransitionTime);
        SceneManager.LoadScene(buil_index);
    }

    //Création du variable static afin que le prefab s'applique dans toutes les scènes
    public static void Out(){
        GameObject.Find("ItemTransition").GetComponent<TransitionScene>().LoadNextScene();
    }
}
