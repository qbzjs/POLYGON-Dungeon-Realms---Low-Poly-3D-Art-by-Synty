using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class Demo : MonoBehaviour
{
    private void Update() {
        //if(Input.GetKeyDown(KeyCode.W))
            TransitionScene.Out();
    }
}
