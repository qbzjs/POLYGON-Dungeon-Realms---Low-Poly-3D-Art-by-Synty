using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    private Animator anim;
    
    private CharacterController charController;

    public float speed = 3.0f;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Avancer", false);
        anim.SetBool("Reculer", false);
        anim.SetBool("Droite", false);
        anim.SetBool("Gauche", false);

        if (Input.GetKey(KeyCode.Z))
        {
            charController.Move(transform.forward * Time.deltaTime * speed);
            anim.SetBool("Avancer", true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            charController.Move(-transform.forward * Time.deltaTime * speed);
            anim.SetBool("Reculer", true);
        }
        else if (Input.GetKey(KeyCode.Q))
        {
            charController.Move(-transform.right * Time.deltaTime * speed);
            anim.SetBool("Gauche", true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            charController.Move(transform.right * Time.deltaTime * speed);
            anim.SetBool("Droite", true);
        }

    }

}
