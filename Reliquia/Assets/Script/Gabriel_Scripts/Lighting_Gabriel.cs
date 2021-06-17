using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Lighting_Gabriel : MonoBehaviour
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

    [Header("Essentials")]

    public GameObject lightProjectilePrefab;
    public GameObject targetImage;
    public Transform cameraStandardAngle;
    public Transform cameraAimingAngle;

    public Camera gameCamera;

    [Header("Values")]

    public float cameraSpeed = 5f;

    public bool shot;
    public float force = 1000f;
    public float duration = 3f;
    public float cooldownBase = 0.5f;
    public float cooldown;

    // variables (Gabriel)

    [SerializeField]
    private CinemachineFreeLook aimCam;
    [SerializeField]
    private CinemachineFreeLook normalCam;
    [SerializeField]
    private Transform followTarget;
    public bool aim = false;
    private Transform position_test;



    void Start() {
        
        manaPool = ybot.GetComponent<RessourcesVitalesWilliam_Script>().manaWilliam;
        isCreated = false;
        isLighting = false;
        _animator = GetComponent<Animator>();
        g = Instantiate(lighting, tlighting);
        g.SetActive(false);

        cameraStandardAngle.transform.position = gameCamera.transform.position;

        
        

    }

    public void InstantiateSpell() {

        Instantiate(lighting, position_test);
    }

    IEnumerator ManaSubstract() {

        ybot.GetComponent<RessourcesVitalesWilliam_Script>().EnleverMana((manaPool / 100) * 2 );
        Debug.Log(ybot.GetComponent<RessourcesVitalesWilliam_Script>().manaWilliam);
        yield return new WaitForSeconds(1);

        if (isCreated == true) {
            isLighting = true;
        }
    }
   
    public void Update() { 

        if (ybot.GetComponent<RessourcesVitalesWilliam_Script>().manaWilliam <= 0) {

            _animator.SetBool("Lighting", false);
            g.SetActive(false);
            isCreated = false;
            isLighting = false;
            aim = false;
            SwitchAimCamera();
        }
    
        else if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.E)) {

            //targetImage.gameObject.SetActive(true);
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

       if (Input.GetMouseButtonDown(0) && isCreated == true && shot == false && aim == true){
            shot = true;

            ybot.GetComponent<RessourcesVitalesWilliam_Script>().EnleverMana((manaPool / 100) * 10);

            //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            Instantiate(lightProjectilePrefab, new Vector3 (tlighting.transform.position.x, tlighting.transform.position.y, tlighting.transform.position.z), Quaternion.identity);
            //Rigidbody lightProjectilePrefabRg = lightProjectilePrefab.AddComponent(typeof(Rigidbody)) as Rigidbody;
            //gRg.transform.position = new Vector3(tlighting.transform.position.x, tlighting.transform.position.y, tlighting.transform.position.z);
            //sphere.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
            //lightProjectilePrefabRg.useGravity = false;
            
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
        // touche pour switcher de camera (entre visée et normal)
        if (Input.GetKeyDown(KeyCode.T) && isCreated == true)
        {
            if (aim)
            {
                aim = false;
                SwitchAimCamera();
            }
            else if (aim == false)
            {
                aim = true;
                SwitchAimCamera();
            }
        }
        // Raycast pour savoir si quelque chose est devant la lighting et est donc touchable et donc changer la couleur du viseur
        if (Physics.Raycast(tlighting.position, tlighting.forward , 100f, 1 << 10))
        {
            print("There is something in front of the object!");
            targetImage.GetComponent<Image>().color = new Color32(255, 0, 0, 100);
        }
        else
        {
            targetImage.GetComponent<Image>().color = new Color32(255, 255, 225, 100);
        }

        tlighting.localRotation = gameCamera.transform.localRotation;
        Debug.DrawRay(tlighting.position, tlighting.forward, Color.green);


    }

    
    private void SwitchAimCamera()  // script pour placer la camera en position de visée ou non.
    {
        if (aim)
        {
            aimCam.Priority = 1;
            normalCam.Priority = 0;
            targetImage.gameObject.SetActive(true);
        }
        else
        {
            normalCam.Priority = 1;
            aimCam.Priority = 0;
            targetImage.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other) //Si un objet avec lequel le joueur peut interragir est dans un collider précis devant william, on peux l'allumer
    {
        if (Input.GetKeyDown(KeyCode.Y) && other.gameObject.CompareTag("lightingCheck") && isCreated == true)
        {
            other.SendMessage("lightObject", this.transform);
        }

    }

    
}
