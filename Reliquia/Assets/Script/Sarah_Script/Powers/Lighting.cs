using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lighting : MonoBehaviour
{
    public Animator _animator;
    public GameObject lighting;
    public Transform tlighting;
    public bool isCreated;
    public GameObject g;
    public GameObject ybot;
    public bool isLighting;
    public int manaPool;

    // Var (Alexis)

    public GameObject targetImage;
    public Transform cameraStandardAngle;
    public Transform cameraAimingAngle;

    public Camera gameCamera;

    public float cameraSpeed = 5f;


    public bool shot;
    public float force = 1000f;
    public float duration = 3f;
    public float cooldownBase = 0.5f;
    public float cooldown;



    void Start() {
        
        manaPool = ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam;
        isCreated = false;
        isLighting = false;
        _animator = GetComponent<Animator>();
        g = Instantiate(lighting, tlighting);
        g.SetActive(false);

        cameraStandardAngle.transform.position = gameCamera.transform.position;
    }

    public void InstantiateSpell() {

        Instantiate(lighting, transform);
    }

    IEnumerator ManaSubstract() {

        ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().EnleverMana((manaPool / 100) * 2 );
        Debug.Log(ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam);
        yield return new WaitForSeconds(1);

        if (isCreated == true) {
            isLighting = true;
        }
    }
   
    public void Update() { 

        if (ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().manaWilliam <= 0) {

            _animator.SetBool("Lighting", false);
            g.SetActive(false);
            isCreated = false;
            isLighting = false;
        }
    
        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.E)) {

            targetImage.gameObject.SetActive(true);
            // gameCamera.transform.position = cameraAimingAngle.transform.position;

            float step =  cameraSpeed * Time.deltaTime; // calculate distance to move
            gameCamera.transform.position = Vector3.MoveTowards(gameCamera.transform.position, cameraAimingAngle.position, step);

            if(!isCreated) {
                _animator.SetBool("Lighting", true);
                g.SetActive(true);
                isCreated = true;
                isLighting = true;
            }
        }
        

        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.R)) {

            targetImage.gameObject.SetActive(false);
            //gameCamera.GetComponent<Transform>().transform.position = new Vector3(cameraStandardAngle.x, cameraStandardAngle.y, cameraStandardAngle.z);

            float step =  cameraSpeed * Time.deltaTime; // calculate distance to move
            gameCamera.transform.position = Vector3.MoveTowards(gameCamera.transform.position, cameraStandardAngle.position, step);

            if(isCreated) {
                _animator.SetBool("Lighting", false);
                g.SetActive(false);
                isCreated = false;
                isLighting = false;
            }
        }

        // Comportement Offensif (Alexis)

       if (Input.GetMouseButtonDown(0) && isCreated == true && shot == false){
            shot = true;

            ybot.GetComponent<RessourcesVitalesWilliam_Scrip>().EnleverMana((manaPool / 100) * 10);

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Rigidbody sphereRg = sphere.AddComponent(typeof(Rigidbody)) as Rigidbody;
            sphere.transform.position = new Vector3(tlighting.transform.position.x, tlighting.transform.position.y, tlighting.transform.position.z);
            sphere.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
            sphereRg.useGravity = false;
            sphereRg.AddForce(transform.forward * force);

            Destroy(sphere, duration);
        }


        if (shot == true){
            cooldown -= Time.deltaTime * 1f;
        }

        if (cooldown <= 0){
            shot = false;
            cooldown = cooldownBase;
        }



        if (isLighting == true) {
            StartCoroutine(ManaSubstract());
            isLighting = false;
        }
    }
}
