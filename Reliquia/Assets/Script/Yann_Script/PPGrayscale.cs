using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PPGrayscale : MonoBehaviour
{
    public Material _material;
    public Shader _shader;
    public Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        // _material = new Material(_shader);
       // rend = GetComponent<Renderer>();

        // Use the Specular shader on the material
        rend.material.shader = Shader.Find("Blend");
        _material.SetFloat("_Blend", 0f);

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material);
    }

    void OnTriggerEnter(Collider other)
    {
        _material.SetFloat("_Blend", 0f);

    }

    void Update()
    {
        // Animate the Shininess value
        float blend = Mathf.PingPong(Time.time/3, 1f);
        _material.SetFloat("_Blend", blend);
    }

}

