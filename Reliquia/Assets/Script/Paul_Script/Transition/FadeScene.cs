using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScene : MonoBehaviour
{
    [SerializeField] AnimationFade fade;

    private void Update() {
        if(Input.anyKeyDown){
            fade.Out();
        }
    }
}
