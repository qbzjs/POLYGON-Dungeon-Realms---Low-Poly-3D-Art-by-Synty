using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class AnimationFade : MonoBehaviour
{
    public string LevelToLoad;
    Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    public void Out(){
        animator.SetTrigger("Out");
    }

    public void LoadScene(){
        SceneManager.LoadScene(LevelToLoad);
    }
}
