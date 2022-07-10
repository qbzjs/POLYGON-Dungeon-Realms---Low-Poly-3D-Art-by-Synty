using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpColorTest : MonoBehaviour
{
    public Color blankColor, grayColor;
    Color currentColor;
    MeshRenderer myRenderer;
    // Start is called before the first frame update
    void Start()
    {
        myRenderer = GetComponent<MeshRenderer>();
        myRenderer.material.color = blankColor;
        currentColor = blankColor;
    }


    void OnTriggerEnter(Collider other)
    {
        currentColor = grayColor;
        myRenderer.material.color = Color.Lerp (myRenderer.material.color, currentColor, Time.deltaTime);
    }

    //  myRenderer.material.color = Color.lerp(myRenderer.material.color, currentColor, 0,f);
}
