using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powers : MonoBehaviour
{
    public Animator _animator;
    public GameObject lighting;
    public Transform tlighting;
    public bool isCreated;
    public GameObject g;

    void Start()
    {
        _animator = GetComponent<Animator>();
        g = Instantiate(lighting, tlighting);
        g.SetActive(false);
    }

    public void InstantiateSpell() {

        Instantiate(lighting, transform);
    }
   
    public void Update()
    {   

        if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.E)) {

            if(!isCreated) {
                _animator.SetBool("Lighting", true);
                g.SetActive(true);
                isCreated = true;
            }
        }

        if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.R)) {

            if(isCreated) {
                _animator.SetBool("Lighting", false);
                g.SetActive(false);
                isCreated = false;
            }
        }
    }
}
