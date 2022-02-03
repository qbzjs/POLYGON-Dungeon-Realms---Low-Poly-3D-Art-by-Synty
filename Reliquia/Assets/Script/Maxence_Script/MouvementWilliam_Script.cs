using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using clavier;
using UnityEngine.SceneManagement;

public class MouvementWilliam_Script : MonoBehaviour
{
    RaccourciClavier_Script raccourciClavier;
    GameManager gameManager;
    private SoundManager _SoundManager;
    private Animator _animator;
    private CharacterController _characterController;
    public float vitesse = 5.0f;
    public float vitesseRotation = 240.0f;
    private float gravite = 20.0f;
    private Vector3 _mouvementDir = Vector3.zero;
    private Transform relativeTransform;
    public bool enMouvement;
    public bool enCourse;
    public bool accroupi;
    public bool auSol;
    void Start()
    {
        raccourciClavier = FindObjectOfType<RaccourciClavier_Script>();
        gameManager = FindObjectOfType<GameManager>();
        auSol = true;
        _SoundManager = GameObject.FindObjectOfType<SoundManager>();
        _SoundManager.Play("pas_tunnel");
        _SoundManager.Play("course_terre");
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
    }
    void Update()
    {
        if (GameManager.instance.voirMenu == false)
        {
            Avancer();
            Reculer();
            Gauche();
            Droite();
            Accroupir();
            Attaque();
            Course();
            Saut();
        }

        if (Input.GetKeyUp(raccourciClavier.toucheClavier["MenuPause"]) && GameManager.instance.menuInventaireOuvert == false && GameManager.instance.menuOptionOuvert == false)
        {
            GameManager.instance.menuPause();
        }

        if (Input.GetKeyUp(raccourciClavier.toucheClavier["MenuInventaire"]) && GameManager.instance.menuPauseOuvert == false)
        {
            GameManager.instance.menuInventaire();
        }
        
        if(enMouvement)
        {
            _SoundManager.UnPause("pas_tunnel");
        }
        else
        {
            _SoundManager.Pause("pas_tunnel");
        }
        if(enCourse)
        {
            _SoundManager.UnPause("course_terre");
        }
        else
        {
            _SoundManager.Pause("course_terre");
        }
        
    }

    public void Avancer()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Avancer"]))
        {
            enMouvement = true;

            _animator.SetBool("Reculer", false);
            _animator.SetBool("Droite", false);
            _animator.SetBool("Gauche", false);
            _animator.SetBool("Course", false);
            _animator.SetBool("Avancer", enMouvement);

            transform.position += transform.forward * vitesse * Time.deltaTime;
        }
        else if (Input.GetKeyUp(raccourciClavier.toucheClavier["Avancer"]))
        {
            enMouvement = false;
            _animator.SetBool("Avancer", enMouvement);
        }
    }

    public void Reculer()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Reculer"]))
        {
            enMouvement = true;


            _animator.SetBool("Avancer", false);
            _animator.SetBool("Droite", false);
            _animator.SetBool("Gauche", false);
            _animator.SetBool("Reculer", enMouvement);

            transform.position += -transform.forward * vitesse * Time.deltaTime;
        }
        else if (Input.GetKeyUp(raccourciClavier.toucheClavier["Reculer"]))
        {
            enMouvement = false;
            _animator.SetBool("Reculer", enMouvement);
        }
    }

    public void Gauche()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Gauche"]))
        {
            enMouvement = true;


            _animator.SetBool("Avancer", false);
            _animator.SetBool("Reculer", false);
            _animator.SetBool("Droite", false);
            _animator.SetBool("Gauche", enMouvement);

            transform.position += -transform.right * 2 * Time.deltaTime;
        }
        else if (Input.GetKeyUp(raccourciClavier.toucheClavier["Gauche"]))
        {
            enMouvement = false;
            _animator.SetBool("Gauche", enMouvement);
        }
    }

    public void Droite()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Droite"]))
        {
            enMouvement = true;


            _animator.SetBool("Avancer", false);
            _animator.SetBool("Reculer", false);
            _animator.SetBool("Gauche", false);
            _animator.SetBool("Droite", enMouvement);

            transform.position += transform.right * 2 * Time.deltaTime;
        }
        else if (Input.GetKeyUp(raccourciClavier.toucheClavier["Droite"]))
        {
            enMouvement = false;
            _animator.SetBool("Droite", enMouvement);
        }
    }

    public void Accroupir()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Accroupir"]))
        {
            enMouvement = false;
            accroupi = true;

            _animator.SetBool("Avancer", enMouvement);
            _animator.SetBool("Reculer", enMouvement);
            _animator.SetBool("Gauche", enMouvement);
            _animator.SetBool("Droite", enMouvement);
            _animator.SetBool("Accroupissement", accroupi);

            transform.position += transform.right * 2 * Time.deltaTime;
        }
        else if (Input.GetKeyUp(raccourciClavier.toucheClavier["Accroupir"]))
        {
            accroupi = false;
            _animator.SetBool("Accroupissement", accroupi);
        }
    }

    public void Attaque()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Attaque"]))
        {
            enMouvement = false;
            accroupi = false;
            _SoundManager.Play("punch");

            _animator.SetBool("Avancer", enMouvement);
            _animator.SetBool("Reculer", enMouvement);
            _animator.SetBool("Gauche", enMouvement);
            _animator.SetBool("Droite", enMouvement);
            _animator.SetBool("Accroupissement", accroupi);
            _animator.SetBool("Attaque", true);
        } else if (Input.GetKeyUp(raccourciClavier.toucheClavier["Attaque"])) _animator.SetBool("Attaque", false);
    }

    public void Course()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Courir"])) enCourse = true;
        else if (Input.GetKeyUp(raccourciClavier.toucheClavier["Courir"]))
        {
            enCourse = false;
            Debug.Log(enCourse);
            Debug.Log(enMouvement);
        }

        if (enCourse && enMouvement)
        {
            _animator.SetBool("Reculer", false);
            _animator.SetBool("Droite", false);
            _animator.SetBool("Gauche", false);
            _animator.SetBool("Avancer", false);
            _animator.SetBool("Accroupissement", false);
            _animator.SetBool("Attaque", false);
            _animator.SetBool("Course", true);

            transform.position += transform.forward * 10.0f * Time.deltaTime;
        }
        else if (enCourse == false && enMouvement == true) Avancer();
        else if (enCourse == false && enMouvement == false)
        {
            enMouvement = false;
            _animator.SetBool("Course", false);
            _animator.SetBool("Avancer", false);
        }
    }

    public void Saut()
    {
        if (Input.GetKey(raccourciClavier.toucheClavier["Saut"]) && auSol)
        {
            _animator.SetBool("Saut", true);
            if (enCourse == false) StartCoroutine(ForceSautNormal());
            else StartCoroutine(ForceSautCourse());
            auSol = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Au sol");
        if (collision.gameObject.CompareTag("Sol"))  auSol = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("En l'air");
        if (collision.gameObject.CompareTag("Sol")) auSol = false;
    }

    IEnumerator ForceSautNormal()
    {
        yield return new WaitForSeconds(0.7f);
        GetComponent<Rigidbody>().AddForce(Vector3.up * 150);
        _animator.SetBool("Saut", false);
    }

    IEnumerator ForceSautCourse()
    {
        GetComponent<Rigidbody>().AddForce(Vector3.up * 150);
        yield return new WaitForSeconds(0.5f);
        _animator.SetBool("Saut", false);
    }
}
